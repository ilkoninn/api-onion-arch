namespace Project.Persistance.Configurations;

public sealed class UserLoginHistoryConfiguration : IEntityTypeConfiguration<UserLoginHistory>
{
    public void Configure(EntityTypeBuilder<UserLoginHistory> builder)
    {
        builder.ToTable("UserLoginHistories");

        builder.HasKey(lh => lh.Id);

        // Indexes
        builder.HasIndex(lh => lh.UserId);
        builder.HasIndex(lh => lh.AttemptedAt);
        builder.HasIndex(lh => lh.IsSuccessful);

        // Properties
        builder.Property(lh => lh.IpAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(lh => lh.UserAgent)
            .HasMaxLength(500);

        builder.Property(lh => lh.FailureReason)
            .HasMaxLength(500);
    }
}