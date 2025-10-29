namespace Project.Persistance.Repositories.Users;

public sealed class UserWriteRepository : WriteRepository<User>, IUserWriteRepository
{
    public UserWriteRepository(AppDbContext context) : base(context) { }

    public async Task<User> CreateUserWithRolesAsync(
        User user,
        IEnumerable<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(user, cancellationToken);
        
        foreach (var roleId in roleIds)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            });
        }

        return user;
    }
}