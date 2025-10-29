namespace Project.Application.Abstractions.Repositories.Users;

/// <summary>
/// User-specific write repository
/// </summary>
public interface IUserWriteRepository : IWriteRepository<User>
{
    Task<User> CreateUserWithRolesAsync(
        User user,
        IEnumerable<Guid> roleIds,
        CancellationToken cancellationToken = default);
}