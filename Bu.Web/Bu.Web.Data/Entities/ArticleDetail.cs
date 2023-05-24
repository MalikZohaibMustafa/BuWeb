namespace Bu.Web.Data.Entities;

public sealed class ArticleDetail : BaseTimestampEntity, IArea, IPublished, INamedEntity
{
    public enum ArticleTypes
    {
        [Description("Static")]
        Static = 1,

        [Description("Dynamic")]
        Dynamic = 2,

        [Description("News")]
        News = 3,

        [Description("Tender")]
        Tender = 4,

        [Description("Event")]
        Event = 5,
    }

    public IPublished Published => this;

    public ITrackedEntity TrackedEntity => this;

    public IExpiringEntity ExpiringEntity => this;

    [Column(Order = 0)]
    public int ArticleDetailId { get; set; }

    [Column(Order = 2)]
    public int ArticleId { get; set; }

    [Column(Order = 3)]
    public int? PreviousArticleDetailId { get; set; }

    [Column(Order = 4)]
    [BindNever]
    [DisplayName("Area")]
    public int AreaId { get; set; }

    [Column(Order = 5)]
    [DisplayName("Layout")]
    public int AreaLayoutId { get; set; }

    [Column(Order = 6)]
    [DisplayName("Article Unique Id")]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [Required]
    public string ArticleUniqueId { get; set; } = string.Empty;

    [Column(Order = 7)]
    [DisplayName("Article Name")]
    public string ArticleName { get; set; } = string.Empty;

    [Column(Order = 8)]
    [DisplayName("Article Type")]
    [EnumDataType(typeof(ArticleTypes))]
    public ArticleTypes ArticleType { get; set; }

    [Column(Order = 10)]
    [DisplayName("Default Page")]
    public int? DefaultArticlePageId { get; set; }

    [Column(Order = 11)]
    [DisplayName("Carousel Image")]
    public int? CarouselArticleMediaId { get; set; }

    [Column(Order = 12)]
    [DisplayName("Small Carousel Image")]
    public int? SmallCarouselArticleMediaId { get; set; }

    [Column(Order = 13)]
    [DataType(DataType.Date)]
    [DisplayName("Event Start Date")]
    public DateTime? EventStartDate { get; set; }

    [Column(Order = 14)]
    [DataType(DataType.Date)]
    [DisplayName("Event End Date")]
    [CompareOtherPropertyValue(nameof(EventStartDate), CompareOtherPropertyValueAttribute.DataTypes.Date, CompareOtherPropertyValueAttribute.ComparisonTypes.GreaterThanOrEqualTo)]
    public DateTime? EventEndDate { get; set; }

    [Column(Order = 15)]
    [DisplayName("Article Short Description")]
    public string? ArticleShortDescription { get; set; }

    [DisplayName("Expiry Date/Time")]
    public DateTime? ExpiryDateUtc { get; set; }

    [DisplayName("Expiry Date/Time")]
    public DateTime? ExpiryDateUser { get; set; }

    [DisplayName("Status")]
    [EnumDataType(typeof(IPublished.Statuses))]
    public IPublished.Statuses Status { get; set; }

    [DisplayName("Published On")]
    public DateTime PublishedOnUtc { get; set; }

    [DisplayName("Published On")]
    public DateTime PublishedOnUser { get; set; }

    [DisplayName("Published By")]
    public int? PublishedByUserId { get; set; }

    [DisplayName("Remarks")]
    [NoLeadingOrTrailingWhiteSpaces]
    [Required]
    public string Remarks { get; set; } = string.Empty;

    [DisplayName("Created By")]
    public int CreatedByUserId { get; set; }

    [DisplayName("Created On")]
    public DateTime CreatedOnUtc { get; set; }

    [DisplayName("Created On")]
    public DateTime CreatedOnUser { get; set; }

    [DisplayName("Last Updated By")]
    public int LastUpdatedByUserId { get; set; }

    [DisplayName("Last Updated On")]
    public DateTime LastUpdatedOnUtc { get; set; }

    [DisplayName("Last Updated On")]
    public DateTime LastUpdatedOnUser { get; set; }

    public override int Id => this.ArticleDetailId;

    public string Name => this.ArticleName;

    public string ShortName => this.ArticleName;

    public string Alias => this.ArticleName;

    public AreaLayout? AreaLayout { get; set; }

    public Article? Article { get; set; }

    public ArticleDetail? PreviousArticleDetail { get; set; }

    public Area? Area { get; set; }

    public User? LastUpdatedByUser { get; set; }

    public User? PublishedByUser { get; set; }

    public User? CreatedByUser { get; set; }

    public ArticlePage? DefaultArticlePage { get; set; }

    public ArticleMedia? CarouselArticleMedia { get; set; }

    public ArticleMedia? SmallCarouselArticleMedia { get; set; }

    public List<ArticlePage>? ArticlePages { get; set; }

    public List<ArticleMedia>? ArticleMedias { get; set; }

    public static void OnModelCreating(EntityTypeBuilder<ArticleDetail> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(ArticleDetailId));
        UniqueKey(entity, tableName, nameof(ArticleId), nameof(ArticleDetailId));
        _ = entity.Property(e => e.Status).HasConversion<byte>();

        _ = entity.HasOne(e => e.PreviousArticleDetail)
            .WithMany()
            .HasPrincipalKey(nameof(ArticleId), nameof(ArticleDetailId))
            .HasForeignKey(nameof(ArticleId), nameof(PreviousArticleDetailId))
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);

        _ = entity.HasOne(e => e.Article)
            .WithMany(e => e.ArticleDetails)
            .HasPrincipalKey(nameof(ArticleId))
            .HasForeignKey(nameof(ArticleId))
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        _ = entity.HasOne(e => e.AreaLayout)
            .WithMany()
            .HasPrincipalKey(e => new { e.AreaId, e.AreaLayoutId })
            .HasForeignKey(e => new { e.AreaId, e.AreaLayoutId })
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        _ = entity.HasOne(e => e.DefaultArticlePage)
            .WithMany()
            .HasPrincipalKey(e => new { e.ArticleDetailId, e.ArticlePageId })
            .HasForeignKey(e => new { e.ArticleDetailId, e.DefaultArticlePageId })
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);

        _ = entity.HasOne(e => e.CarouselArticleMedia)
            .WithMany()
            .HasPrincipalKey(e => new { e.ArticleDetailId, e.ArticleMediaId })
            .HasForeignKey(e => new { e.ArticleDetailId, e.CarouselArticleMediaId })
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);

        _ = entity.HasOne(e => e.SmallCarouselArticleMedia)
            .WithMany()
            .HasPrincipalKey(e => new { e.ArticleDetailId, e.ArticleMediaId })
            .HasForeignKey(e => new { e.ArticleDetailId, e.SmallCarouselArticleMediaId })
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        switch (this.ArticleType)
        {
            case ArticleTypes.Static:
                if (this.ExpiryDateUtc != null)
                {
                    yield return new ValidationResult("Value is not allowed.", new[] { nameof(this.ExpiryDateUser) });
                }

                break;
            case ArticleTypes.Dynamic:
            case ArticleTypes.News:
            case ArticleTypes.Tender:
                if (this.ExpiryDateUtc == null)
                {
                    yield return new ValidationResult(
                        $"The {validationContext.ObjectType.GetDisplayNameForProperty(nameof(this.ExpiryDateUser))} is required.",
                        new[] { nameof(this.ExpiryDateUser) });
                }

                if ((this.ExpiryDateUser != null && this.PublishedOnUser > this.ExpiryDateUser.Value)
                    || (this.ExpiryDateUtc != null && this.PublishedOnUtc > this.ExpiryDateUtc.Value))
                {
                    yield return new ValidationResult(
                        $"The {validationContext.ObjectType.GetDisplayNameForProperty(nameof(this.ExpiryDateUser))} must be after {validationContext.ObjectType.GetDisplayNameForProperty(nameof(this.PublishedOnUser))}.",
                        new[] { nameof(this.ExpiryDateUser) });
                }

                break;
            case ArticleTypes.Event:
                if (this.EventStartDate == null)
                {
                    yield return new ValidationResult($"The {this.GetType().GetDisplayNameForProperty(nameof(this.EventStartDate))} is required.", new[] { nameof(this.EventStartDate) });
                }

                if (this.EventEndDate == null)
                {
                    yield return new ValidationResult($"The {this.GetType().GetDisplayNameForProperty(nameof(this.EventEndDate))} is required.", new[] { nameof(this.EventEndDate) });
                }

                break;
            default:
                throw new InvalidOperationException(this.ArticleType.ToString());
        }
    }

    public override void PostQuery(ClaimsPrincipal claimsPrincipal)
    {
        base.PostQuery(claimsPrincipal);
        this.ArticlePages?.ForEach(p => p.PostQuery(claimsPrincipal));
        this.ArticleMedias?.ForEach(m => m.PostQuery(claimsPrincipal));
    }

    public IEnumerable<ArticlePage> GetSortedArticlePages()
    {
        return this.ArticlePages!.SortHierarchically(p => p.ArticlePageId, p => p.ParentArticlePageId, p => p.PageSequence);
    }

    public static string GetArticleUrlWithTilde(string areaPath, ArticleTypes articleType, string articleUniqueId)
    {
        return articleType switch
        {
            ArticleTypes.Static => $"~{areaPath.TrimEnd('/')}/{articleUniqueId}",
            ArticleTypes.Dynamic => $"~{areaPath.TrimEnd('/')}/Articles/{articleUniqueId}",
            ArticleTypes.News => $"~{areaPath.TrimEnd('/')}/News/{articleUniqueId}",
            ArticleTypes.Tender => $"~{areaPath.TrimEnd('/')}/Tender/{articleUniqueId}",
            ArticleTypes.Event => $"~{areaPath.TrimEnd('/')}/Event/{articleUniqueId}",
            _ => throw new ArgumentOutOfRangeException(nameof(articleType), articleType, null),
        };
    }

    public static string GetArticleUrlWithTilde(ArticleDetail articleDetail)
    {
        return GetArticleUrlWithTilde(articleDetail.Area!.AreaPath, articleDetail.ArticleType, articleDetail.ArticleUniqueId);
    }

    public string GetArticleUrlWithTilde()
    {
        return GetArticleUrlWithTilde(this);
    }

    public static string GetArticleCarouselMediaUrlWithTilde(string areaPath, ArticleTypes articleType, string articleUniqueId)
    {
        return $"{GetArticleUrlWithTilde(areaPath, articleType, articleUniqueId)}/Carousel";
    }

    public static string GetArticleCarouselMediaUrlWithTilde(ArticleDetail articleDetail)
    {
        return GetArticleCarouselMediaUrlWithTilde(articleDetail.Area!.AreaPath, articleDetail.ArticleType, articleDetail.ArticleUniqueId);
    }

    public static string GetArticleSmallCarouselMediaUrlWithTilde(string areaPath, ArticleTypes articleType, string articleUniqueId)
    {
        return $"{GetArticleUrlWithTilde(areaPath, articleType, articleUniqueId)}/SmallCarousel";
    }

    public static string GetArticleSmallCarouselMediaUrlWithTilde(ArticleDetail articleDetail)
    {
        return GetArticleSmallCarouselMediaUrlWithTilde(articleDetail.Area!.AreaPath, articleDetail.ArticleType, articleDetail.ArticleUniqueId);
    }

    public string GetArticleCarouselMediaUrlWithTilde()
    {
        return GetArticleCarouselMediaUrlWithTilde(this);
    }

    public void Publish(List<ValidationResult> validationResults, IAdminContext adminContext, IAdminFixedContext adminFixedContext, DateTime utcNow)
    {
        Area area = adminFixedContext.AreasList.Single(a => a.AreaId == this.AreaId);
        if (area.Status != Area.Statuses.Active)
        {
            validationResults.Add(new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.AreaId))} '{area.AreaName}' status is not {Area.Statuses.Active.GetDescription()}.", new[] { nameof(this.AreaId) }));
        }

        AreaLayout areaLayout = adminFixedContext.AreaLayoutsList.Single(a => a.AreaLayoutId == this.AreaLayoutId);
        if (areaLayout.Status != AreaLayout.Statuses.Active)
        {
            validationResults.Add(new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.AreaLayoutId))} '{areaLayout.LayoutName}' status is not {AreaLayout.Statuses.Active.GetDescription()}.", new[] { nameof(this.AreaLayoutId) }));
        }

        if (this.DefaultArticlePageId == null)
        {
            validationResults.Add(new ValidationResult($"The {this.GetType().GetDisplayNameForProperty(nameof(this.DefaultArticlePageId))} is required.", new[] { nameof(this.DefaultArticlePageId) }));
        }

        bool isArticleUniqueIdInUseNow = adminContext.GetPublishedArticleDetails(utcNow).Any(ad => ad.ArticleId != this.ArticleId && ad.ArticleType == this.ArticleType && ad.ArticleUniqueId == this.ArticleUniqueId);
        if (isArticleUniqueIdInUseNow)
        {
            validationResults.Add(new ValidationResult($"The {this.GetType().GetDisplayNameForProperty(nameof(this.ArticleUniqueId))} '{this.ArticleUniqueId}' is already in use.", new[] { nameof(this.ArticleUniqueId) }));
        }

        switch (this.ArticleType)
        {
            case ArticleTypes.Static:
                break;
            case ArticleTypes.Dynamic:
                break;
            case ArticleTypes.News:
                break;
            case ArticleTypes.Tender:
                break;
            case ArticleTypes.Event:
                if (this.SmallCarouselArticleMediaId == null)
                {
                    validationResults.Add(new ValidationResult($"The {this.GetType().GetDisplayNameForProperty(nameof(this.SmallCarouselArticleMediaId))} is required.", new[] { nameof(this.SmallCarouselArticleMediaId) }));
                }

                break;
            default:
                throw new InvalidOperationException(this.ArticleType.ToString());
        }
    }
}