namespace Bu.Web.Data.Contexts;

public sealed class WebDbContext : DbContext, IAdminContext
{
    public WebDbContext(string connectionString)
        : base(new DbContextOptionsBuilder<WebDbContext>().UseSqlServer(connectionString).Options)
    {
    }

    public DbSet<Area> Areas { get; set; } = null!;

    public DbSet<AreaLayout> AreaLayouts { get; set; } = null!;

    public DbSet<Article> Articles { get; set; } = null!;

    public DbSet<ArticleDetail> ArticleDetails { get; set; } = null!;

    public DbSet<ArticleMedia> ArticleMedias { get; set; } = null!;

    public DbSet<ArticlePage> ArticlePages { get; set; } = null!;

    public DbSet<Department> Departments { get; set; } = null!;

    public DbSet<EventLog> EventLogs { get; set; } = null!;

    public DbSet<Institute> Institutes { get; set; } = null!;

    public DbSet<InstituteLocation> InstituteLocations { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<UserArea> UserAreas { get; set; } = null!;

    public DbSet<UserRole> UserRoles { get; set; } = null!;

    public DbSet<WhitelistedIpAddress> WhitelistedIpAddresses { get; set; } = null!;

    public DbSet<YoutubeVideo> YoutubeVideos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        Area.OnModelCreating(modelBuilder.Entity<Area>(), nameof(this.Areas));
        AreaLayout.OnModelCreating(modelBuilder.Entity<AreaLayout>(), nameof(this.AreaLayouts));
        Article.OnModelCreating(modelBuilder.Entity<Article>(), nameof(this.Articles));
        ArticleDetail.OnModelCreating(modelBuilder.Entity<ArticleDetail>(), nameof(this.ArticleDetails));
        ArticleMedia.OnModelCreating(modelBuilder.Entity<ArticleMedia>(), nameof(this.ArticleMedias));
        ArticlePage.OnModelCreating(modelBuilder.Entity<ArticlePage>(), nameof(this.ArticlePages));
        Department.OnModelCreating(modelBuilder.Entity<Department>(), nameof(this.Departments));
        EventLog.OnModelCreating(modelBuilder.Entity<EventLog>(), nameof(this.EventLogs));
        Institute.OnModelCreating(modelBuilder.Entity<Institute>(), nameof(this.Institutes));
        InstituteLocation.OnModelCreating(modelBuilder.Entity<InstituteLocation>(), nameof(this.InstituteLocations));
        User.OnModelCreating(modelBuilder.Entity<User>(), nameof(this.Users));
        UserArea.OnModelCreating(modelBuilder.Entity<UserArea>(), nameof(this.UserAreas));
        UserRole.OnModelCreating(modelBuilder.Entity<UserRole>(), nameof(this.UserRoles));
        WhitelistedIpAddress.OnModelCreating(modelBuilder.Entity<WhitelistedIpAddress>(), nameof(this.WhitelistedIpAddresses));
        YoutubeVideo.OnModelCreating(modelBuilder.Entity<YoutubeVideo>(), nameof(this.YoutubeVideos));
    }

    public int SaveChanges(ClaimsPrincipal claimsPrincipal)
    {
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> entries = this.ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified);
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry? entry in entries)
        {
            List<ValidationResult> validationResults = new();
            IAdminContext adminContext = this;
            ((IEntity)entry.Entity).PreSave(claimsPrincipal);
            _ = entry.Entity.Validate(validationResults, adminContext);
            if (validationResults.IsNotValid())
            {
                throw new ValidationException(validationResults.First(), null, null);
            }
        }

        return this.SaveChanges();
    }
}