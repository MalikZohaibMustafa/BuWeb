namespace Bu.Web.Data.Entities;

public sealed class AreaLayout : BaseTimestampEntity, ITrackedEntity, INamedWithParentEntity, IArea
{
    public enum LayoutTypes
    {
        [Description("No Menu")]
        NoMenu = 0,

        [Description("Left Side Menu")]
        LeftMenu = 1,

        [Description("Right Side Menu")]
        RightMenu = 2,
    }

    public enum Statuses : byte
    {
        [Description("Active")]
        Active = 1,

        [Description("Inactive")]
        Inactive = 2,
    }

    public ITrackedEntity TrackedEntity => this;

    [Column(Order = 0)]
    public int AreaLayoutId { get; set; }

    [Column(Order = 2)]
    [DisplayName("Area")]
    public int AreaId { get; set; }

    [Column(Order = 3)]
    [DisplayName("Layout Type")]
    [EnumDataType(typeof(LayoutTypes))]
    public LayoutTypes LayoutType { get; set; }

    [Column(Order = 4)]
    [DisplayName("Layout Name")]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(500)]
    [Unicode(false)]
    public string LayoutName { get; set; } = null!;

    [Column(Order = 5)]
    [DisplayName("Layout Path")]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(UrlLength)]
    [Unicode(false)]
    public string LayoutPath { get; set; } = null!;

    [Column(Order = 6)]
    [DisplayName("Status")]
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

    public Area? Area { get; set; }

    public override int Id => this.AreaLayoutId;

    public string Name => this.LayoutName;

    public string ShortName => this.LayoutName;

    public string Alias => this.LayoutName;

    public INamedEntity? Parent => this.Area;

    public static void OnModelCreating(EntityTypeBuilder<AreaLayout> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(AreaLayoutId));
        UniqueKey(entity, tableName, nameof(AreaId), nameof(AreaLayoutId));
        UniqueKey(entity, tableName, nameof(AreaLayoutId), nameof(LayoutName));
        UniqueKey(entity, tableName, nameof(AreaLayoutId), nameof(LayoutPath));
        _ = entity.Property(e => e.Status).HasConversion<byte>();
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        IAdminContext? adminContext = (IAdminContext?)validationContext.GetService(typeof(IAdminContext));
        if (adminContext != null)
        {
            IQueryable<AreaLayout> query = adminContext.AreaLayouts.Where(al => al.AreaId == this.AreaId && al.AreaLayoutId != this.AreaLayoutId);
            if (query.Any(al => al.LayoutName == this.LayoutName))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.LayoutName))} '{this.LayoutName}' is already in use.", new[] { nameof(this.LayoutName) });
            }
        }
    }
}