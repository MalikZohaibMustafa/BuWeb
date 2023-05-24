namespace Bu.Web.Data.Entities;

public sealed class Area : BaseTimestampEntity, ITrackedEntity, INamedWithParentEntity
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
    public int AreaId { get; set; }

    [Column(Order = 2)]
    [DisplayName("Parent Area")]
    public int? ParentAreaId { get; set; }

    [Column(Order = 3)]
    [DisplayName("Institute")]
    public int InstituteId { get; set; }

    [Column(Order = 4)]
    [DisplayName("Location")]
    public int? InstituteLocationId { get; set; }

    [Column(Order = 5)]
    [DisplayName("Department")]
    public int? DepartmentId { get; set; }

    [Column(Order = 6)]
    [DisplayName("Area Name")]
    [Lowercase]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [RegularExpression(@"^[a-z]+$")]
    [StringLength(100)]
    [Unicode(false)]
    public string AreaName { get; set; } = null!;

    [Column(Order = 7)]
    [DisplayName("Area Path")]
    [Lowercase]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [RegularExpression(@"(^\/$)|^(\/[a-z]+)+$")]
    [StringLength(UrlLength)]
    [Unicode(UrlUnicode)]
    public string AreaPath { get; set; } = null!;

    public string AreaPathWithTilde => "~" + this.AreaPath;

    [Column(Order = 8)]
    [DisplayName("Contact Office")]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(500)]
    [Unicode(false)]
    public string ContactOffice { get; set; } = null!;

    [Column(Order = 9)]
    [DisplayName("Status")]
    [EnumDataType(typeof(Statuses))]
    public Statuses Status { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime CreatedOnUser { get; set; }

    public int LastUpdatedByUserId { get; set; }

    public DateTime LastUpdatedOnUtc { get; set; }

    public DateTime LastUpdatedOnUser { get; set; }

    public Institute? Institute { get; set; }

    public InstituteLocation? InstituteLocation { get; set; }

    public Department? Department { get; set; }

    public User? CreatedByUser { get; set; }

    public User? LastUpdatedByUser { get; set; }

    public Area? ParentArea { get; set; }

    public override int Id => this.AreaId;

    public string Name => this.AreaName;

    public string ShortName => this.AreaName;

    public string Alias => this.AreaName;

    public INamedEntity? Parent => this.ParentArea;

    public static void OnModelCreating(EntityTypeBuilder<Area> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(AreaId));
        UniqueKey(entity, tableName, nameof(AreaPath));
        _ = entity.Property(e => e.Status).HasConversion<byte>();

        _ = entity.HasOne(e => e.ParentArea)
            .WithMany()
            .HasPrincipalKey(e => e.AreaId)
            .HasForeignKey(e => e.ParentAreaId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);

        _ = entity.HasOne(e => e.Institute)
            .WithMany()
            .HasPrincipalKey(e => e.InstituteId)
            .HasForeignKey(e => e.InstituteId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);

        _ = entity.HasOne(e => e.InstituteLocation)
            .WithMany()
            .HasPrincipalKey(e => new { e.InstituteId, e.InstituteLocationId })
            .HasForeignKey(e => new { e.InstituteId, e.InstituteLocationId })
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);

        _ = entity.HasOne(e => e.Department)
            .WithMany()
            .HasPrincipalKey(e => new { e.InstituteId, e.DepartmentId })
            .HasForeignKey(e => new { e.InstituteId, e.DepartmentId })
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        IAdminContext? adminContext = (IAdminContext?)validationContext.GetService(typeof(IAdminContext));
        if (adminContext != null)
        {
            if (this.ParentAreaId == null)
            {
                this.AreaPath = "/";
            }
            else
            {
                Area? parentArea = adminContext.Areas.SingleOrDefault(a => a.AreaId == this.ParentAreaId.Value);
                if (parentArea == null)
                {
                    yield return new ValidationResult("Parent Area not found.", new[] { nameof(this.ParentAreaId) });
                    yield break;
                }

                this.AreaPath = parentArea.AreaPath == RootPath
                    ? $"/{this.AreaName}"
                    : $"{parentArea.AreaPath}/{this.AreaName}";
            }

            IQueryable<Area> query = adminContext.Areas.Where(a => a.AreaId != this.AreaId);
            bool duplicateFound = query.Any(a => a.AreaName == this.AreaName);
            if (duplicateFound)
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.AreaName))} '{this.AreaName}' is already in use.", new[] { nameof(this.AreaName) });
            }

            duplicateFound = adminContext.Areas.Any(a => a.AreaPath == this.AreaPath && a.AreaId != this.AreaId);
            if (duplicateFound)
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.AreaPath))} '{this.AreaPath}' is already in use.", new[] { nameof(this.AreaPath) });
            }

            if (this.ParentAreaId == null && !string.IsNullOrEmpty(this.AreaName))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.AreaName))} '{this.AreaName}' must be empty for root level.", new[] { nameof(this.AreaName) });
            }
        }
    }

    public static readonly string RootPath = "/";
}