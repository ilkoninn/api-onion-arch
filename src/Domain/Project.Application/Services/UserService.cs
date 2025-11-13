namespace Project.Application.Services;

/// <summary>
/// User service implementation with MediatR orchestration
/// </summary>
public sealed class UserService(
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    IPasswordService passwordHasher) : IUserService
{
    #region Authentication

    public async Task<RegisterUserResponseDto> RegisterAsync(
        RegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        // Business validation
        if (await unitOfWork.UserReadRepository.IsEmailExistsAsync(dto.Email, cancellationToken))
            throw new InvalidOperationException("Email already exists");

        // Create user entity
        var user = new User
        {
            Email = dto.Email,
            NormalizedEmail = dto.Email.ToUpperInvariant(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PasswordHash = passwordHasher.HashPassword(dto.Password),
            SecurityStamp = Guid.NewGuid().ToString(),
            Status = EUserStatus.Active,
            EmailConfirmed = false
        };

        string token = string.Empty;
        string refreshToken = string.Empty;

        // Persist within transaction
        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            // Add user to database
            await unitOfWork.UserWriteRepository.AddAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // Generate tokens after user is created (to have user.Id)
            var roles = new List<string> { "User" }; // Default role
            token = tokenService.GenerateAccessToken(user, roles);
            refreshToken = tokenService.GenerateRefreshToken();

            // Store refresh token
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = "0.0.0.0" // TODO: Get from HTTP context
            };

            await unitOfWork.RefreshTokenWriteRepository.AddAsync(refreshTokenEntity, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);

        return new(user, token, refreshToken);
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default)
    {
        // Get user with roles
        var user = await unitOfWork.UserReadRepository
            .GetByEmailWithRolesAsync(dto.Email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        // Validate password
        if (!passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            // Increment failed attempts
            user.AccessFailedCount++;

            // Lock account after 5 failed attempts
            if (user.AccessFailedCount >= 5)
            {
                user.IsLocked = true;
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
            }

            unitOfWork.UserWriteRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Log failed attempt
            var failedLoginHistory = new UserLoginHistory
            {
                UserId = user.Id,
                AttemptedAt = DateTime.UtcNow,
                IpAddress = dto.IpAddress ?? "0.0.0.0",
                UserAgent = "Unknown", // TODO: Get from HTTP context
                IsSuccessful = false,
                FailureReason = "Invalid password"
            };

            await unitOfWork.UserLoginHistoryWriteRepository.AddAsync(failedLoginHistory, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Check account status
        if (user.IsLocked && user.LockoutEnd > DateTime.UtcNow)
            throw new UnauthorizedAccessException("Account is locked");

        string token = string.Empty;
        string refreshToken = string.Empty;

        // Update user and generate tokens within transaction
        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            // Reset failed attempts on successful login
            user.AccessFailedCount = 0;
            user.LastLoginAt = DateTime.UtcNow;
            user.LastLoginIp = dto.IpAddress ?? "0.0.0.0";

            unitOfWork.UserWriteRepository.Update(user);

            // Generate tokens
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            token = tokenService.GenerateAccessToken(user, roles);
            refreshToken = tokenService.GenerateRefreshToken();

            // Store refresh token
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = dto.IpAddress ?? "0.0.0.0"
            };

            await unitOfWork.RefreshTokenWriteRepository.AddAsync(refreshTokenEntity, ct);

            // Add successful login history
            var loginHistory = new UserLoginHistory
            {
                UserId = user.Id,
                AttemptedAt = DateTime.UtcNow,
                IpAddress = dto.IpAddress ?? "0.0.0.0",
                UserAgent = "Unknown", // TODO: Get from HTTP context
                IsSuccessful = true
            };

            await unitOfWork.UserLoginHistoryWriteRepository.AddAsync(loginHistory, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);

        return new(user, token, refreshToken);
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(
        RefreshTokenDto dto,
        CancellationToken cancellationToken = default)
    {
        // Get and validate refresh token from database
        var existingToken = await unitOfWork.RefreshTokenReadRepository
            .GetByTokenAsync(dto.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        // Check if token is active
        if (!existingToken.IsActive)
        {
            if (existingToken.IsRevoked)
                throw new UnauthorizedAccessException("Refresh token has been revoked");
            
            if (existingToken.IsExpired)
                throw new UnauthorizedAccessException("Refresh token has expired");
        }

        // Get user with roles
        var user = await unitOfWork.UserReadRepository
            .GetByIdWithRolesAsync(existingToken.UserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("User not found");

        // Check if user is active
        if (user.IsLocked)
            throw new UnauthorizedAccessException("User account is locked");

        if (user.Status != EUserStatus.Active)
            throw new UnauthorizedAccessException("User account is not active");

        string newToken = string.Empty;
        string newRefreshToken = string.Empty;

        // Implement token rotation within transaction
        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            // Generate new tokens
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            newToken = tokenService.GenerateAccessToken(user, roles);
            newRefreshToken = tokenService.GenerateRefreshToken();

            // Revoke old refresh token (token rotation for security)
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.RevokedByIp = dto.IpAddress ?? "0.0.0.0";
            existingToken.ReplacedByToken = newRefreshToken;

            unitOfWork.RefreshTokenWriteRepository.Update(existingToken);

            // Create new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = dto.IpAddress ?? "0.0.0.0"
            };

            await unitOfWork.RefreshTokenWriteRepository.AddAsync(newRefreshTokenEntity, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);

        return new(newToken, newRefreshToken);
    }

    public async Task RevokeTokenAsync(
        RevokeTokenDto dto,
        CancellationToken cancellationToken = default)
    {
        // Get refresh token from database
        var existingToken = await unitOfWork.RefreshTokenReadRepository
            .GetByTokenAsync(dto.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        // Check if token is already revoked
        if (existingToken.IsRevoked)
            throw new InvalidOperationException("Token is already revoked");

        // Revoke token within transaction
        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.RevokedByIp = dto.IpAddress ?? "0.0.0.0";

            unitOfWork.RefreshTokenWriteRepository.Update(existingToken);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);
    }

    #endregion

    #region User Management

    public async Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.UserReadRepository.GetByIdAsync(id, asNoTracking: true, cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID {id} not found");
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.UserReadRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new KeyNotFoundException($"User with email {email} not found");
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        unitOfWork.UserWriteRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserReadRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID {id} not found");

        unitOfWork.UserWriteRepository.SoftDelete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Password Management

    public async Task<bool> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserReadRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        // Verify current password
        if (!passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
            return false;

        // Update password within transaction
        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            user.PasswordHash = passwordHasher.HashPassword(newPassword);
            user.LastPasswordChangedAt = DateTime.UtcNow;
            user.SecurityStamp = Guid.NewGuid().ToString(); // Invalidate existing tokens

            unitOfWork.UserWriteRepository.Update(user);
            await unitOfWork.SaveChangesAsync(ct);

            // Revoke all existing refresh tokens for security
            var userTokens = await unitOfWork.RefreshTokenReadRepository
                .GetActiveTokensByUserIdAsync(userId, ct);

            foreach (var token in userTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = "System - Password Change";
                unitOfWork.RefreshTokenWriteRepository.Update(token);
            }

            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);

        return true;
    }

    public async Task ResetPasswordAsync(
        string email,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        // TODO: Validate reset token
        var user = await unitOfWork.UserReadRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            user.PasswordHash = passwordHasher.HashPassword(newPassword);
            user.LastPasswordChangedAt = DateTime.UtcNow;
            user.SecurityStamp = Guid.NewGuid().ToString(); // Invalidate existing tokens

            unitOfWork.UserWriteRepository.Update(user);
            await unitOfWork.SaveChangesAsync(ct);

            // Revoke all existing refresh tokens for security
            var userTokens = await unitOfWork.RefreshTokenReadRepository
                .GetActiveTokensByUserIdAsync(user.Id, ct);

            foreach (var token in userTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = "System - Password Reset";
                unitOfWork.RefreshTokenWriteRepository.Update(token);
            }

            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);
    }

    #endregion

    #region Role Management

    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        // Get user
        var user = await unitOfWork.UserReadRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        // TODO: Check if role exists
        // TODO: Check if user already has this role
        // TODO: Add UserRole entity and save

        await Task.CompletedTask;
    }

    public async Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        // Get user
        var user = await unitOfWork.UserReadRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        // TODO: Check if user has this role
        // TODO: Remove UserRole entity and save

        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserReadRepository.GetByIdWithRolesAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        return user.UserRoles.Select(ur => ur.Role).ToList();
    }

    #endregion

    #region Account Management

    public async Task LockUserAsync(Guid userId, DateTime? lockoutEnd, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserReadRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            user.IsLocked = true;
            user.LockoutEnd = lockoutEnd;

            unitOfWork.UserWriteRepository.Update(user);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);
    }

    public async Task UnlockUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserReadRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            user.IsLocked = false;
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;

            unitOfWork.UserWriteRepository.Update(user);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);
    }

    public async Task ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default)
    {
        // TODO: Validate confirmation token
        var user = await unitOfWork.UserReadRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            user.EmailConfirmed = true;

            unitOfWork.UserWriteRepository.Update(user);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);
    }

    #endregion
}
