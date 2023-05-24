namespace Bu.Web.Data.Abstraction;

public interface IExpiringEntity
{
    DateTime? ExpiryDateUtc { get; set; }

    DateTime? ExpiryDateUser { get; set; }

    void SetExpiry(DateTime? expiryDateUser)
    {
        this.ExpiryDateUser = expiryDateUser;
    }

    void SetNoExpiry()
    {
        this.SetExpiry(null);
    }

    void PreSaveChanges(ClaimsPrincipal claimsPrincipal)
    {
        this.ExpiryDateUtc = this.ExpiryDateUser?.ToUtcFromUserDateTime(claimsPrincipal);
    }

    void PostLoad(ClaimsPrincipal claimsPrincipal)
    {
        this.ExpiryDateUtc = this.ExpiryDateUtc?.SpecifyKindUtc();
        this.ExpiryDateUser = this.ExpiryDateUtc?.ToUserDateTimeFromUtc(claimsPrincipal);
    }

    static void OnModelCreating<TEntity>(EntityTypeBuilder<TEntity> entity)
        where TEntity : class
    {
        int columnOrder = 40_000;
        _ = entity.Property(nameof(ExpiryDateUtc)).HasColumnOrder(columnOrder);
        entity.Ignore(nameof(ExpiryDateUser));
    }
}