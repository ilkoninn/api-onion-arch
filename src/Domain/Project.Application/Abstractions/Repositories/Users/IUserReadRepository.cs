namespace Project.Application.Abstractions.Repositories.Users;

/// <summary>
/// User-specific read repository with custom query methods
/// </summary>
public interface IUserReadRepository : IReadRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetUsersWithRolesAsync(
        Expression<Func<User, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}