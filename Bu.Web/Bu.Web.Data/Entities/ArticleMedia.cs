using Microsoft.AspNetCore.Http;

namespace Bu.Web.Data.Entities;

public sealed class ArticleMedia : BaseTimestampEntity, ITrackedEntity, INamedEntity
{
    public ITrackedEntity TrackedEntity => this;

    [Column(Order = 0)]
    public int ArticleMediaId { get; set; }

    [Column(Order = 2)]
    public int ArticleDetailId { get; set; }

    [BindNever]
    [Column(Order = 3)]
    [NotZero]
    public Guid ArticleMediaGuid { get; set; }

    [Column(Order = 4)]
    [DisplayName("Unique Id")]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [Required(AllowEmptyStrings = false)]
    [StringLength(100, MinimumLength = 1)]
    public string MediaUniqueId { get; set; } = string.Empty;

    [Column(Order = 5, TypeName = "varchar(100)")]
    [DisplayName("Content Type")]
    [EnumDataType(typeof(ContentTypes))]
    public ContentTypes MediaContentType { get; set; }

    [Column(Order = 6, TypeName = "varchar(100)")]
    [DisplayName("Content Disposition")]
    [EnumDataType(typeof(ContentDisposition))]
    public ContentDisposition? MediaContentDisposition { get; set; }

    [Column(Order = 8)]
    [DisplayName("File Name")]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [Required(AllowEmptyStrings = false)]
    [StringLength(200, MinimumLength = 1)]
    public string MediaFileName { get; set; } = string.Empty;

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

    public User? CreatedByUser { get; set; }

    public User? LastUpdatedByUser { get; set; }

    public override int Id => this.ArticleMediaId;

    public string Name => this.MediaFileName;

    public string ShortName => this.MediaFileName;

    public string Alias => this.MediaFileName;

    [NotMapped]
    [DisplayName("File")]
    public IFormFile? FormFile { get; set; }

    public ArticleDetail? ArticleDetail { get; set; }

    public static void OnModelCreating(EntityTypeBuilder<ArticleMedia> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(ArticleMediaId));
        UniqueKey(entity, tableName, nameof(ArticleDetailId), nameof(ArticleMediaId));
        _ = entity.Property(e => e.MediaContentType).HasConversion<byte>().IsRequired();
        _ = entity.Property(e => e.MediaContentDisposition).HasConversion<byte>();

        _ = entity.HasOne(e => e.ArticleDetail)
            .WithMany(e => e.ArticleMedias)
            .HasPrincipalKey(e => e.ArticleDetailId)
            .HasForeignKey(e => e.ArticleDetailId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        IAdminContext? adminContext = (IAdminContext?)validationContext.GetService(typeof(IAdminContext));
        if (adminContext != null)
        {
            IQueryable<ArticleMedia> query = adminContext.ArticleMedias.Where(am => am.ArticleDetailId == this.ArticleDetailId && am.ArticleMediaId != this.ArticleMediaId);
            if (query.Any(am => am.MediaUniqueId == this.MediaUniqueId))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.MediaUniqueId))} '{this.MediaUniqueId}' is already in use.", new[] { nameof(this.MediaUniqueId) });
            }
        }
    }

    public string GetFileName(ArticleDetail articleDetail)
    {
        return Path.Combine(
            articleDetail.ArticleId.ToString(),
            "Media",
            this.ArticleMediaGuid.ToString("N"));
    }

    public string GetFileName()
    {
        return this.GetFileName(this.ArticleDetail ?? throw new InvalidOperationException());
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

    public static string GetArticleMediaUrlWithTilde(string areaPath, ArticleDetail.ArticleTypes articleType, string articleUniqueId, string mediaUniqueId)
    {
        return $"{ArticleDetail.GetArticleUrlWithTilde(areaPath, articleType, articleUniqueId)}/Media/{mediaUniqueId}";
    }

    public static string GetArticleUrlWithTilde(ArticleMedia articleMedia)
    {
        return GetArticleMediaUrlWithTilde(articleMedia.ArticleDetail!.Area!.AreaPath, articleMedia.ArticleDetail!.ArticleType, articleMedia.ArticleDetail!.ArticleUniqueId, articleMedia.MediaUniqueId);
    }

    public string GetArticleUrlWithTilde()
    {
        return GetArticleUrlWithTilde(this);
    }
}