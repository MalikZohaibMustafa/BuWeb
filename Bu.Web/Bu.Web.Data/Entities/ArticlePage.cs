namespace Bu.Web.Data.Entities;

public sealed class ArticlePage : BaseTimestampEntity, ITrackedEntity, INamedWithParentEntity
{
    public ITrackedEntity TrackedEntity => this;

    [Column(Order = 0)]
    public int ArticlePageId { get; set; }

    [Column(Order = 2)]
    public int ArticleDetailId { get; set; }

    [Column(Order = 3)]
    [DisplayName("Parent Page")]
    public int? ParentArticlePageId { get; set; }

    [Column(Order = 4)]
    [DisplayName("Unique Id")]
    public string PageUniqueId { get; set; } = string.Empty;

    [Column(Order = 5)]
    [DisplayName("Menu Text")]
    public string? MenuText { get; set; }

    [Column(Order = 6)]
    [DisplayName("Sequence")]
    public int PageSequence { get; set; }

    [Column(Order = 7)]
    [DisplayName("Title")]
    public string PageTitle { get; set; } = string.Empty;

    [Column(Order = 8)]
    [DisplayName("Body")]
    public string PageBody { get; set; } = string.Empty;

    [Column(Order = 9)]
    [DisplayName("Body Format")]
    [EnumDataType(typeof(RichTextFormats))]
    public RichTextFormats PageBodyTextFormat { get; set; }

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

    public override int Id => this.ArticlePageId;

    public string Name => this.PageTitle;

    public string ShortName => this.PageTitle;

    public string Alias => this.PageTitle;

    public INamedEntity? Parent => this.ArticleDetail;

    public ArticleDetail? ArticleDetail { get; set; }

    public ArticlePage? ParentArticlePage { get; set; }

    public User? CreatedByUser { get; set; }

    public User? LastUpdatedByUser { get; set; }

    public static void OnModelCreating(EntityTypeBuilder<ArticlePage> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(ArticlePageId));
        UniqueKey(entity, tableName, nameof(ArticleDetailId), nameof(ArticlePageId));
        _ = entity.Property(e => e.PageBodyTextFormat).HasConversion<byte>();

        _ = entity.HasOne(e => e.ArticleDetail)
            .WithMany(e => e.ArticlePages)
            .HasPrincipalKey(e => e.ArticleDetailId)
            .HasForeignKey(e => e.ArticleDetailId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        _ = entity.HasOne(e => e.ParentArticlePage)
            .WithMany()
            .HasPrincipalKey(e => new { e.ArticleDetailId, e.ArticlePageId })
            .HasForeignKey(e => new { e.ArticleDetailId, e.ParentArticlePageId })
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        IAdminContext? adminContext = (IAdminContext?)validationContext.GetService(typeof(IAdminContext));
        if (adminContext != null)
        {
            IQueryable<ArticlePage> query = adminContext.ArticlePages.Where(ap => ap.ArticleDetailId == this.ArticleDetailId && ap.ArticleDetailId != this.ArticlePageId);
            if (query.Any(ap => ap.PageUniqueId == this.PageUniqueId))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.PageUniqueId))} '{this.PageUniqueId}' is already in use.", new[] { nameof(this.PageUniqueId) });
            }

            if (this.MenuText != null && query.Any(ap => ap.ParentArticlePageId == this.ParentArticlePageId && ap.MenuText == this.MenuText))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.MenuText))} '{this.MenuText}' is already in use.", new[] { nameof(this.MenuText) });
            }
        }

        // TODO Validate Page Body Html/Markdown
    }

    public override void PreSave(ClaimsPrincipal claimsPrincipal)
    {
        base.PreSave(claimsPrincipal);
        this.CreatedOnUser = this.CreatedOnUtc.SpecifyKindUtc().ToUserDateTimeFromUtc(claimsPrincipal);
        this.LastUpdatedOnUser = this.LastUpdatedOnUtc.SpecifyKindUtc().ToUserDateTimeFromUtc(claimsPrincipal);
    }

    public override void PostQuery(ClaimsPrincipal claimsPrincipal)
    {
        base.PostQuery(claimsPrincipal);
        this.CreatedOnUtc = this.CreatedOnUtc.SpecifyKindUtc();
        this.LastUpdatedOnUtc = this.LastUpdatedOnUtc.SpecifyKindUtc();

        this.CreatedOnUser = this.CreatedOnUtc.ToUserDateTimeFromUtc(claimsPrincipal);
        this.LastUpdatedOnUser = this.LastUpdatedOnUtc.ToUserDateTimeFromUtc(claimsPrincipal);
    }
}