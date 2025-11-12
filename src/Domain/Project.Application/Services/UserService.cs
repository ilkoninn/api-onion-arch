namespace Project.Application.Services;

/// <summary>
/// User service implementation with business logic orchestration
/// </summary>
public sealed class UserService(
          IUserReadRepository userReadRepository,
          IUserWriteRepository userWriteRepository,
          ITokenService tokenService,
          IPasswordHasher passwordHasher) : IUserService
{

    #region Authentication

    public async Task<(User User, string Token, string RefreshToken)> RegisterAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        CancellationToken cancellationToken = default)
    {
        // Business validation
        if (await userReadRepository.IsEmailExistsAsync(email, cancellationToken))
            throw new InvalidOperationException("Email already exists");

        // Create user entity
        var user = new User
        {
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHasher.HashPassword(password),
            SecurityStamp = Guid.NewGuid().ToString(),
            Status = EUserStatus.Active,
            EmailConfirmed = false
        };

        // Persist
        await userWriteRepository.AddAsync(user, cancellationToken);
        await userWriteRepository.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var roles = new List<string> { "User" }; // Default role
        var token = tokenService.GenerateAccessToken(user, roles);
        var refreshToken = tokenService.GenerateRefreshToken();

        // Store refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = "0.0.0.0" // TODO: Get from HTTP context
        };

        await userWriteRepository.SaveChangesAsync(cancellationToken);

        return (user, token, refreshToken);
    }

    public async Task<(User User, string Token, string RefreshToken)> LoginAsync(
        string email,
        string password,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        // Get user with roles
        var user = await userReadRepository.GetByEmailWithRolesAsync(email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        // Validate password
        if (!passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            // Increment failed attempts
            user.AccessFailedCount++;

            // Lock account after 5 failed attempts
            if (user.AccessFailedCount >= 5)
            {
                user.IsLocked = true;
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
            }

            userWriteRepository.Update(user);
            await userWriteRepository.SaveChangesAsync(cancellationToken);

            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Check account status
        if (user.IsLocked && user.LockoutEnd > DateTime.UtcNow)
            throw new UnauthorizedAccessException("Account is locked");

        // Reset failed attempts on successful login
        user.AccessFailedCount = 0;
        user.LastLoginAt = DateTime.UtcNow;
        user.LastLoginIp = ipAddress;

        userWriteRepository.Update(user);

        // Generate tokens
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var token = tokenService.GenerateAccessToken(user, roles);
        var refreshToken = tokenService.GenerateRefreshToken();

        // Store refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        // Add login history
        var loginHistory = new UserLoginHistory
        {
            UserId = user.Id,
            AttemptedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = "Unknown" // TODO: Get from HTTP context
        };

        await userWriteRepository.SaveChangesAsync(cancellationToken);

        return (user, token, refreshToken);
    }

    public async Task<(string Token, string RefreshToken)> RefreshTokenAsync(
        string refreshToken,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        // Validate and get user from refresh token
        var userId = await tokenService.ValidateRefreshTokenAsync(refreshToken, cancellationToken)
       ?? throw new UnauthorizedAccessException("Invalid refresh token");

        var user = await userReadRepository.GetByIdWithRolesAsync(userId, cancellationToken)
            ?? throw new UnauthorizedAccessException("User not found");

        // Generate new tokens
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var newToken = tokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        // Revoke old token and create new one
        // TODO: Implement token rotation

        return (newToken, newRefreshToken);
    }

    public async Task RevokeTokenAsync(
        string refreshToken,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        // TODO: Mark refresh token as revoked in database
        await Task.CompletedTask;
    }

    #endregion

    #region User Management

    public async Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await userReadRepository.GetByIdAsync(id, asNoTracking: true, cancellationToken)
        ?? throw new KeyNotFoundException($"User with ID {id} not found");
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await userReadRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new KeyNotFoundException($"User with email {email} not found");
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        userWriteRepository.Update(user);
        await userWriteRepository.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userReadRepository.GetByIdAsync(id, cancellationToken)
         ?? throw new KeyNotFoundException($"User with ID {id} not found");

        userWriteRepository.SoftDelete(user);
        await userWriteRepository.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Password Management

    public async Task<bool> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await userReadRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        // Verify current password
        if (!passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
            return false;

        // Update password
        user.PasswordHash = passwordHasher.HashPassword(newPassword);
        user.LastPasswordChangedAt = DateTime.UtcNow;
        user.SecurityStamp = Guid.NewGuid().ToString();

        userWriteRepository.Update(user);
        await userWriteRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task ResetPasswordAsync(
        string email,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        // TODO: Validate reset token
        var user = await userReadRepository.GetByEmailAsync(email, cancellationToken)
                 ?? throw new KeyNotFoundException("User not found");

        user.PasswordHash = passwordHasher.HashPassword(newPassword);
        user.LastPasswordChangedAt = DateTime.UtcNow;
        user.SecurityStamp = Guid.NewGuid().ToString();

        userWriteRepository.Update(user);
        await userWriteRepository.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Role Management

    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement role assignment
        await Task.CompletedTask;
    }

    public async Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement role removal
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userReadRepository.GetByIdWithRolesAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        return user.UserRoles.Select(ur => ur.Role).ToList();
    }

    #endregion

    #region Account Management

    public async Task LockUserAsync(Guid userId, DateTime? lockoutEnd, CancellationToken cancellationToken = default)
    {
        var user = await userReadRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        user.IsLocked = true;
        user.LockoutEnd = lockoutEnd;

        userWriteRepository.Update(user);
        await userWriteRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UnlockUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userReadRepository.GetByIdAsync(userId, cancellationToken)
               ?? throw new KeyNotFoundException("User not found");

        user.IsLocked = false;
        user.LockoutEnd = null;
        user.AccessFailedCount = 0;

        userWriteRepository.Update(user);
        await userWriteRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default)
    {
        // TODO: Validate confirmation token
        var user = await userReadRepository.GetByIdAsync(userId, cancellationToken)
        ?? throw new KeyNotFoundException("User not found");

        user.EmailConfirmed = true;

        userWriteRepository.Update(user);
        await userWriteRepository.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
