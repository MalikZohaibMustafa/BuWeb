namespace Bu.Web.Data.Entities;

public sealed class UserArea : BaseTimestampEntity, ITrackedEntity, IArea, IExpiringEntity
{
    public enum Statuses : byte
    {
        [Description("Active")]
        Active = 1,

        [Description("Inactive")]
        Inactive = 2,
    }

    public enum Roles : byte
    {
        [Description("Editor")]
        Editor = 3,

        [Description("Author")]
        Author = 4,

        [Description("Contributor")]
        Contributor = 5,
    }

    [Column(Order = 0)]
    public int UserAreaId { get; set; }

    [Column(Order = 2)]
    public int UserId { get; set; }

    [Column(Order = 3)]
    public int AreaId { get; set; }

    [Column(Order = 4)]
    [DisplayName("Role")]
    [EnumDataType(typeof(Roles))]
    public Roles Role { get; set; }

    [Column(Order = 5)]
    public bool InheritToAllChildren { get; set; }

    [Column(Order = 6)]
    [DisplayName("Status")]
    [EnumDataType(typeof(Statuses))]
    public Statuses Status { get; set; }

    [DisplayName("Expiry Date")]
    public DateTime? ExpiryDateUtc { get; set; }

    [DisplayName("Expiry Date")]
    public DateTime? ExpiryDateUser { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime CreatedOnUser { get; set; }

    public int LastUpdatedByUserId { get; set; }

    public DateTime LastUpdatedOnUtc { get; set; }

    public DateTime LastUpdatedOnUser { get; set; }

    public User? User { get; set; }

    public Area? Area { get; set; }

    public User? CreatedByUser { get; set; }

    public User? LastUpdatedByUser { get; set; }

    public override int Id => this.UserAreaId;

    public static void OnModelCreating(EntityTypeBuilder<UserArea> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(UserAreaId));
        UniqueKey(entity, tableName, nameof(UserId), nameof(AreaId));
        _ = entity.Property(e => e.Status).HasConversion<byte>();

        _ = entity.HasOne(e => e.User)
            .WithMany(e => e.UserAreas)
            .HasPrincipalKey(e => e.UserId)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        _ = entity.HasOne(e => e.Area)
            .WithMany()
            .HasPrincipalKey(e => e.AreaId)
            .HasForeignKey(e => e.AreaId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield break;
    }

    public override void PostQuery(ClaimsPrincipal claimsPrincipal)
    {
        base.PostQuery(claimsPrincipal);
        this.ExpiryDateUtc = this.ExpiryDateUtc?.SpecifyKindUtc();
        this.ExpiryDateUser = this.ExpiryDateUtc?.ToUserDateTimeFromUtc(claimsPrincipal);
    }

    public override void PreSave(ClaimsPrincipal claimsPrincipal)
    {
        base.PreSave(claimsPrincipal);
        this.ExpiryDateUtc = this.ExpiryDateUser?.ToUtcFromUserDateTime(claimsPrincipal);
    }
}