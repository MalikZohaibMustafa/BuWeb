namespace Bu.Web.Data.Entities;

public sealed class Article : BaseTimestampEntity
{
    [Column(Order = 0)]
    public int ArticleId { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public override int Id => this.ArticleId;

    public List<ArticleDetail>? ArticleDetails { get; set; }

    public static void OnModelCreating(EntityTypeBuilder<Article> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(ArticleId));
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield break;
    }

    public override void PostQuery(ClaimsPrincipal claimsPrincipal)
    {
        base.PostQuery(claimsPrincipal);
        this.CreatedOnUtc = this.CreatedOnUtc.SpecifyKindUtc();

        this.ArticleDetails?.ForEach(d => d.PostQuery(claimsPrincipal));
    }
}