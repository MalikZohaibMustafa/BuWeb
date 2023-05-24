namespace Bu.Web.Data.Abstraction;

public interface IPublished : ITrackedEntity, IExpiringEntity
{
    public enum Statuses
    {
        [Description("Pending")]
        Pending = 0,

        [Description("Rejected")]
        Rejected = 1,

        [Description("Published")]
        Published,
    }

    ITrackedEntity TrackedEntity => this;

    IExpiringEntity ExpiringEntity => this;

    Statuses Status { get; set; }

    DateTime PublishedOnUtc { get; set; }

    DateTime PublishedOnUser { get; set; }

    int? PublishedByUserId { get; set; }

    User? PublishedByUser { get; set; }

    string Remarks { get; set; }

    void SetPublishedStatus(Statuses status, DateTime publishedOnUser, int publishedByUserId, string remarks)
    {
        if (this.Status.IsPublished())
        {
            throw new InvalidOperationException($"Already published. Type: {this.GetType().FullName}");
        }

        this.Status = status;
        this.Remarks = remarks;
        this.PublishedOnUser = publishedOnUser;
        this.PublishedByUserId = publishedByUserId;
    }

    string Version => this.Status switch
    {
        Statuses.Pending => $"{this.LastUpdatedOnUser:g} ({this.Status.GetDescription()})",
        Statuses.Rejected => $"{this.LastUpdatedOnUser:g} ({this.Status.GetDescription()})",
        Statuses.Published => $"{this.PublishedOnUser:g} ({this.Status.GetDescription()})",
        _ => throw new InvalidOperationException(this.Status.ToString()),
    };

    bool IsPending => this.Status.IsPending();

    bool IsRejected => this.Status.IsRejected();

    bool IsPublished => this.Status.IsPublished();

    new void PreSaveChanges(ClaimsPrincipal claimsPrincipal)
    {
        this.PublishedOnUtc = this.PublishedOnUser.ToUtcFromUserDateTime(claimsPrincipal);
        this.ExpiringEntity.PreSaveChanges(claimsPrincipal);
    }

    new void PostLoad(ClaimsPrincipal claimsPrincipal)
    {
        this.ExpiringEntity.PostLoad(claimsPrincipal);
        this.PublishedOnUtc = this.PublishedOnUtc.SpecifyKindUtc();
        this.PublishedOnUser = this.PublishedOnUtc.ToUserDateTimeFromUtc(claimsPrincipal);
    }

    static new void OnModelCreating<TEntity>(EntityTypeBuilder<TEntity> entity)
        where TEntity : class
    {
        ITrackedEntity.OnModelCreating(entity);
        int columnOrder = 30_000;
        _ = entity.Property(nameof(PublishedByUserId)).HasColumnOrder(columnOrder++);
        _ = entity.Property(nameof(PublishedOnUtc)).HasColumnOrder(columnOrder++);
        _ = entity.Property(nameof(ExpiryDateUtc)).HasColumnOrder(columnOrder);
        entity.Ignore(nameof(PublishedOnUser));
        entity.Ignore(nameof(ExpiryDateUser));

        _ = entity.HasOne(e => ((IPublished)e).PublishedByUser)
            .WithMany()
            .HasPrincipalKey(e => e.UserId)
            .HasForeignKey(e => ((IPublished)e).PublishedByUserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }
}