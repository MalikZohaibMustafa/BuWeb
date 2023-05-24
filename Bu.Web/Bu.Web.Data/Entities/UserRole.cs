namespace Bu.Web.Data.Entities;

public sealed class UserRole : BaseTimestampEntity, ITrackedEntity, IExpiringEntity
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
        [Description("Super Administrator")]
        SuperAdministrator = 1,
        [Description("Administrator")]
        Administrator = 2,
        [Description("Webmaster")]
        Webmaster = 3,
    }

    public ITrackedEntity TrackedEntity => this;

    public IExpiringEntity ExpiringEntity => this;

    [Column(Order = 0)]
    public int UserRoleId { get; set; }

    [Column(Order = 2)]
    public int UserId { get; set; }

    [Column(Order = 3)]
    [EnumDataType(typeof(Roles))]
    public Roles Role { get; set; }

    [Column(Order = 6)]
    [EnumDataType(typeof(Statuses))]
    public Statuses Status { get; set; }

    [DisplayName("Expiry Date/Time")]
    public DateTime? ExpiryDateUtc { get; set; }

    [DisplayName("Expiry Date/Time")]
    public DateTime? ExpiryDateUser { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime CreatedOnUser { get; set; }

    public int LastUpdatedByUserId { get; set; }

    public DateTime LastUpdatedOnUtc { get; set; }

    public DateTime LastUpdatedOnUser { get; set; }

    public User? User { get; set; }

    public User? CreatedByUser { get; set; }

    public User? LastUpdatedByUser { get; set; }

    public override int Id => this.UserRoleId;

    public static void OnModelCreating(EntityTypeBuilder<UserRole> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(UserRoleId));
        UniqueKey(entity, tableName, nameof(UserId), nameof(Role));
        _ = entity.Property(e => e.Status).HasConversion<byte>();

        _ = entity.HasOne(e => e.User)
            .WithMany(e => e.UserRoles)
            .HasPrincipalKey(e => e.UserId)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield break;
    }
}