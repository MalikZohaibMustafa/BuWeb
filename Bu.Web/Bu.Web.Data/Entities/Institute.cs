namespace Bu.Web.Data.Entities;

public sealed class Institute : BaseTimestampEntity, ITrackedEntity, INamedEntity
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
    public int InstituteId { get; set; }

    [Column(Order = 2)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(500)]
    [Unicode(false)]
    public string InstituteName { get; set; } = null!;

    [Column(Order = 3)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(200)]
    [Unicode(false)]
    public string InstituteShortName { get; set; } = null!;

    [Column(Order = 4)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(50)]
    [Unicode(false)]
    public string InstituteAlias { get; set; } = null!;

    [Column(Order = 5)]
    [EnumDataType(typeof(Statuses))]
    public Statuses Status { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime CreatedOnUser { get; set; }

    public int LastUpdatedByUserId { get; set; }

    public DateTime LastUpdatedOnUtc { get; set; }

    public DateTime LastUpdatedOnUser { get; set; }

    public User? CreatedByUser { get; set; }

    public User? LastUpdatedByUser { get; set; }

    public override int Id => this.InstituteId;

    public string Name => this.InstituteName;

    public string ShortName => this.InstituteShortName;

    public string Alias => this.InstituteAlias;

    public static void OnModelCreating(EntityTypeBuilder<Institute> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(InstituteId), false);
        UniqueKey(entity, tableName, nameof(InstituteName));
        UniqueKey(entity, tableName, nameof(InstituteShortName));
        UniqueKey(entity, tableName, nameof(InstituteAlias));
        _ = entity.Property(e => e.Status).HasConversion<byte>();
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        IAdminContext? adminContext = (IAdminContext?)validationContext.GetService(typeof(IAdminContext));
        if (adminContext != null)
        {
            IQueryable<Institute> query = adminContext.Institutes.Where(i => i.InstituteId != this.InstituteId);
            if (query.Any(i => i.InstituteName == this.InstituteName))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.InstituteName))} '{this.InstituteName}' is already in use.", new[] { nameof(this.InstituteName) });
            }

            if (query.Any(i => i.InstituteShortName == this.InstituteShortName))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.InstituteShortName))} '{this.InstituteShortName}' is already in use.", new[] { nameof(this.InstituteShortName) });
            }

            if (query.Any(i => i.InstituteAlias == this.InstituteAlias))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.InstituteAlias))} '{this.InstituteAlias}' is already in use.", new[] { nameof(this.InstituteAlias) });
            }
        }
    }
}