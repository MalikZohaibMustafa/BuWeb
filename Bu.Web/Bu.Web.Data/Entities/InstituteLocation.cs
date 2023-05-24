namespace Bu.Web.Data.Entities;

public sealed class InstituteLocation : BaseTimestampEntity, ITrackedEntity, INamedWithParentEntity
{
    public enum Statuses : byte
    {
        [Description("Active")]
        Active = 1,
        [Description("Inactive")]
        Inactive = 2,
    }

    public ITrackedEntity TrackedEntity => this;

    [Column(Order = 0)]
    public int InstituteLocationId { get; set; }

    [Column(Order = 2)]
    public int InstituteId { get; set; }

    [Column(Order = 3)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(500)]
    [Unicode(false)]
    public string LocationName { get; set; } = null!;

    [Column(Order = 4)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(200)]
    [Unicode(false)]
    public string LocationShortName { get; set; } = null!;

    [Column(Order = 5)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(50)]
    [Unicode(false)]
    public string LocationAlias { get; set; } = null!;

    [Column(Order = 6)]
    [EnumDataType(typeof(Statuses))]
    public Statuses Status { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime CreatedOnUser { get; set; }

    public int LastUpdatedByUserId { get; set; }

    public DateTime LastUpdatedOnUtc { get; set; }

    public DateTime LastUpdatedOnUser { get; set; }

    public Institute? Institute { get; set; }

    public User? CreatedByUser { get; set; }

    public User? LastUpdatedByUser { get; set; }

    public override int Id => this.InstituteLocationId;

    public string Name => this.LocationName;

    public string ShortName => this.LocationShortName;

    public string Alias => this.LocationAlias;

    public INamedEntity? Parent => this.Institute;

    public static void OnModelCreating(EntityTypeBuilder<InstituteLocation> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(InstituteLocationId));
        UniqueKey(entity, tableName, nameof(InstituteId), nameof(InstituteLocationId));
        UniqueKey(entity, tableName, nameof(InstituteId), nameof(LocationName));
        UniqueKey(entity, tableName, nameof(InstituteId), nameof(LocationShortName));
        UniqueKey(entity, tableName, nameof(InstituteId), nameof(LocationAlias));
        _ = entity.Property(e => e.Status).HasConversion<byte>();

        _ = entity.HasOne(e => e.Institute)
            .WithMany()
            .HasPrincipalKey(e => e.InstituteId)
            .HasForeignKey(e => e.InstituteId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        IAdminContext? adminContext = (IAdminContext?)validationContext.GetService(typeof(IAdminContext));
        if (adminContext != null)
        {
            IQueryable<InstituteLocation> query = adminContext.InstituteLocations.Where(il => il.InstituteId == this.InstituteId && il.InstituteLocationId != this.InstituteLocationId);
            if (query.Any(il => il.LocationName == this.LocationName))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.LocationName))} '{this.LocationName}' is already in use.", new[] { nameof(this.LocationName) });
            }

            if (query.Any(il => il.LocationShortName == this.LocationShortName))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.LocationShortName))} '{this.LocationShortName}' is already in use.", new[] { nameof(this.LocationShortName) });
            }

            if (query.Any(il => il.LocationAlias == this.LocationAlias))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.LocationAlias))} '{this.LocationAlias}' is already in use.", new[] { nameof(this.LocationAlias) });
            }
        }
    }
}