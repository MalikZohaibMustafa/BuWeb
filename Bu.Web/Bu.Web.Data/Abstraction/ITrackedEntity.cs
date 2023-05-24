namespace Bu.Web.Data.Abstraction;

public interface ITrackedEntity
{
    int CreatedByUserId { get; set; }

    DateTime CreatedOnUtc { get; set; }

    DateTime CreatedOnUser { get; set; }

    User? CreatedByUser { get; set; }

    int LastUpdatedByUserId { get; set; }

    DateTime LastUpdatedOnUtc { get; set; }

    DateTime LastUpdatedOnUser { get; set; }

    User? LastUpdatedByUser { get; set; }

    public void SetCreated(DateTime createdOnUser, int createdByUserId)
    {
        if (createdOnUser.Kind != DateTimeKind.Unspecified)
        {
            throw new InvalidOperationException();
        }

        this.CreatedByUserId = createdByUserId;
        this.CreatedOnUser = createdOnUser;
        this.LastUpdatedByUserId = createdByUserId;
        this.LastUpdatedOnUser = createdOnUser;
    }

    public void SetLastUpdated(DateTime lastUpdatedOnUser, int lastUpdatedByUserId)
    {
        if (lastUpdatedOnUser.Kind != DateTimeKind.Unspecified)
        {
            throw new InvalidOperationException();
        }

        this.LastUpdatedByUserId = lastUpdatedByUserId;
        this.LastUpdatedOnUser = lastUpdatedOnUser;
    }

    void PreSaveChanges(ClaimsPrincipal claimsPrincipal)
    {
        this.CreatedOnUtc = this.CreatedOnUser.ToUtcFromUserDateTime(claimsPrincipal);
        this.LastUpdatedOnUtc = this.LastUpdatedOnUser.ToUtcFromUserDateTime(claimsPrincipal);
    }

    void PostLoad(ClaimsPrincipal claimsPrincipal)
    {
        this.CreatedOnUtc = this.CreatedOnUtc.SpecifyKindUtc();
        this.CreatedOnUser = this.CreatedOnUtc.ToUserDateTimeFromUtc(claimsPrincipal);
        this.LastUpdatedOnUtc = this.LastUpdatedOnUtc.SpecifyKindUtc();
        this.LastUpdatedOnUser = this.LastUpdatedOnUtc.ToUserDateTimeFromUtc(claimsPrincipal);
    }

    public static void OnModelCreating<TEntity>(EntityTypeBuilder<TEntity> entity)
        where TEntity : class
    {
        int columnOrder = 10_000;
        _ = entity.Property(nameof(CreatedByUserId)).HasColumnOrder(columnOrder++);
        _ = entity.Property(nameof(CreatedOnUtc)).HasColumnOrder(columnOrder++);
        _ = entity.Property(nameof(LastUpdatedByUserId)).HasColumnOrder(columnOrder++);
        _ = entity.Property(nameof(LastUpdatedOnUtc)).HasColumnOrder(columnOrder);
        entity.Ignore(nameof(CreatedOnUser));
        entity.Ignore(nameof(LastUpdatedOnUser));

        _ = entity.HasOne(e => ((ITrackedEntity)e).CreatedByUser)
            .WithMany()
            .HasPrincipalKey(e => e.UserId)
            .HasForeignKey(e => ((ITrackedEntity)e).CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        _ = entity.HasOne(e => ((ITrackedEntity)e).LastUpdatedByUser)
            .WithMany()
            .HasPrincipalKey(e => e.UserId)
            .HasForeignKey(e => ((ITrackedEntity)e).LastUpdatedByUserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}