namespace Project.Persistance.Configurations;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(ur => ur.Id);

        // Composite index
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
    }
}