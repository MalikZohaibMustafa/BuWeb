namespace Bu.Web.Data.Entities;

public sealed class Department : BaseTimestampEntity, ITrackedEntity, INamedWithParentEntity
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
    public int DepartmentId { get; set; }

    [Column(Order = 2)]
    public int InstituteId { get; set; }

    [Column(Order = 3)]
    public int? InstituteLocationId { get; set; }

    [Column(Order = 4)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(500)]
    [Unicode(false)]
    public string DepartmentName { get; set; } = null!;

    [Column(Order = 5)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(200)]
    [Unicode(false)]
    public string DepartmentShortName { get; set; } = null!;

    [Column(Order = 6)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(50)]
    [Unicode(false)]
    public string DepartmentAlias { get; set; } = null!;

    [Column(Order = 7)]
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

    public User? CreatedByUser { get; set; }

    public User? LastUpdatedByUser { get; set; }

    public override int Id => this.DepartmentId;

    public string Name => this.DepartmentName;

    public string ShortName => this.DepartmentShortName;

    public string Alias => this.DepartmentAlias;

    public INamedEntity? Parent => this.Institute;

    public static void OnModelCreating(EntityTypeBuilder<Department> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(DepartmentId));
        UniqueKey(entity, tableName, nameof(InstituteId), nameof(DepartmentId));
        UniqueKey(entity, tableName, nameof(InstituteId), nameof(DepartmentName));
        UniqueKey(entity, tableName, nameof(InstituteId), nameof(DepartmentShortName));
        UniqueKey(entity, tableName, nameof(InstituteId), nameof(DepartmentAlias));
        _ = entity.Property(e => e.Status).HasConversion<byte>();

        _ = entity.HasOne(e => e.Institute)
            .WithMany()
            .HasPrincipalKey(e => e.InstituteId)
            .HasForeignKey(e => e.InstituteId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        _ = entity.HasOne(e => e.InstituteLocation)
            .WithMany()
            .HasPrincipalKey(nameof(Entities.InstituteLocation.InstituteId), nameof(Entities.InstituteLocation.InstituteLocationId))
            .HasForeignKey(nameof(InstituteId), nameof(InstituteLocationId))
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        IAdminContext? adminContext = (IAdminContext?)validationContext.GetService(typeof(IAdminContext));
        if (adminContext != null)
        {
            IQueryable<Department> query = adminContext.Departments.Where(d => d.InstituteId == this.InstituteId && d.DepartmentId != this.DepartmentId);
            if (query.Any(d => d.DepartmentName == this.DepartmentName))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.DepartmentName))} '{this.DepartmentName}' is already in use.", new[] { nameof(this.DepartmentName) });
            }

            if (query.Any(d => d.DepartmentShortName == this.DepartmentShortName))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.DepartmentShortName))} '{this.DepartmentShortName}' is already in use.", new[] { nameof(this.DepartmentShortName) });
            }

            if (query.Any(d => d.DepartmentAlias == this.DepartmentAlias))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.DepartmentAlias))} '{this.DepartmentAlias}' is already in use.", new[] { nameof(this.DepartmentAlias) });
            }
        }
    }
}