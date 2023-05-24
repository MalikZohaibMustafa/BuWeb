namespace Bu.Web.Data.Entities;

public sealed class YoutubeVideo : BaseTimestampEntity, IArea, IPublished
{
    public IPublished Published => this;

    [Column(Order = 0)]
    public int YoutubeVideoId { get; set; }

    [Column(Order = 2)]
    [BindNever]
    [DisplayName("Area")]
    public int AreaId { get; set; }

    [Column(Order = 3)]
    [DisplayName("Youtube Url")]
    public string YoutubeUrl { get; set; } = string.Empty;

    [Column(Order = 4)]
    [DisplayName("Status")]
    [EnumDataType(typeof(IPublished.Statuses))]
    public IPublished.Statuses Status { get; set; }

    [DisplayName("Published On")]
    public DateTime PublishedOnUtc { get; set; }

    [NotMapped]
    [DisplayName("Published On")]
    public DateTime PublishedOnUser { get; set; }

    [DisplayName("Published By")]
    public int? PublishedByUserId { get; set; }

    [DisplayName("Expiry Date/Time")]
    public DateTime? ExpiryDateUtc { get; set; }

    [NotMapped]
    [DisplayName("Expiry Date/Time")]
    public DateTime? ExpiryDateUser { get; set; }

    [Column(Order = 8)]
    [DisplayName("Remarks")]
    public string Remarks { get; set; } = string.Empty;

    [DisplayName("Created By")]
    public int CreatedByUserId { get; set; }

    [DisplayName("Created On")]
    public DateTime CreatedOnUtc { get; set; }

    [NotMapped]
    [DisplayName("Created On")]
    public DateTime CreatedOnUser { get; set; }

    [DisplayName("Last Updated By")]
    public int LastUpdatedByUserId { get; set; }

    [DisplayName("Last Updated On")]
    public DateTime LastUpdatedOnUtc { get; set; }

    [DisplayName("Last Updated On")]
    public DateTime LastUpdatedOnUser { get; set; }

    [NotMapped]
    [DisplayName("Last Updated On")]
    public User? LastUpdatedByUser { get; set; }

    public override int Id => this.YoutubeVideoId;

    public Area? Area { get; set; }

    public User? PublishedByUser { get; set; }

    public User? CreatedByUser { get; set; }

    public static void OnModelCreating(EntityTypeBuilder<YoutubeVideo> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(YoutubeVideoId));
        UniqueKey(entity, tableName, nameof(AreaId), nameof(YoutubeUrl));
        _ = entity.Property(e => e.Status).HasConversion<byte>();
        _ = entity.Property(e => e.YoutubeUrl).UseCollation(Collations.CaseSensitive);
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        IAdminContext? adminContext = (IAdminContext?)validationContext.GetService(typeof(IAdminContext));
        if (adminContext != null)
        {
            IQueryable<YoutubeVideo> query = adminContext.YoutubeVideos.Where(y => y.YoutubeVideoId != this.YoutubeVideoId && y.AreaId == this.AreaId);
            if (query.Any(y => y.YoutubeUrl == this.YoutubeUrl))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.YoutubeUrl))} '{this.YoutubeUrl}' is already in use.", new[] { nameof(this.YoutubeUrl) });
            }
        }
    }
}