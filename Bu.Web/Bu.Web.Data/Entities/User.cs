namespace Bu.Web.Data.Entities;

public sealed class User : BaseTimestampEntity, ITrackedEntity, IExpiringEntity
{
    public enum Statuses : byte
    {
        [Description("Active")]
        Active = 1,
        [Description("Inactive")]
        Inactive = 2,
    }

    public ITrackedEntity TrackedEntity => this;

    public IExpiringEntity ExpiringEntity => this;

    [Column(Order = 0)]
    public int UserId { get; set; }

    [Column(Order = 2)]
    [EmailAddress]
    [Lowercase]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(EmailAddressLength)]
    [Unicode(EmailAddressUnicode)]
    public string Email { get; set; } = null!;

    [Column(Order = 3)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(PersonNameLength)]
    [Unicode(PersonNameUnicode)]
    public string Name { get; set; } = null!;

    [Column(Order = 4)]
    public int DepartmentId { get; set; }

    [Column(Order = 5)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(MobileNumberLength)]
    [Unicode(MobileNumberUnicode)]
    public string? MobileNo { get; set; }

    [Column(Order = 6)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(PhoneNumberLength)]
    [Unicode(PhoneNumberUnicode)]
    public string? PhoneNo { get; set; }

    [Column(Order = 8)]
    [EnumDataType(typeof(Statuses))]
    public Statuses Status { get; set; }

    public DateTime? ExpiryDateUtc { get; set; }

    public DateTime? ExpiryDateUser { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime CreatedOnUser { get; set; }

    public int LastUpdatedByUserId { get; set; }

    public DateTime LastUpdatedOnUtc { get; set; }

    public DateTime LastUpdatedOnUser { get; set; }

    public Department? Department { get; set; }

    public User? CreatedByUser { get; set; }

    public User? LastUpdatedByUser { get; set; }

    public List<UserRole>? UserRoles { get; set; }

    public List<UserArea>? UserAreas { get; set; }

    public override int Id => this.UserId;

    public static void OnModelCreating(EntityTypeBuilder<User> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(UserId));
        UniqueKey(entity, tableName, nameof(Email));
        _ = entity.Property(e => e.Status).HasConversion<byte>();

        _ = entity.HasOne(e => e.Department)
            .WithMany()
            .HasPrincipalKey(e => e.DepartmentId)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        IAdminContext? adminContext = (IAdminContext?)validationContext.GetService(typeof(IAdminContext));
        if (adminContext != null)
        {
            IQueryable<User> query = adminContext.Users.Where(u => u.UserId != this.UserId);
            if (query.Any(u => u.Email == this.Email))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.Email))} '{this.Email}' is already in use.", new[] { nameof(this.Email) });
            }
        }
    }

    public override string ToString()
    {
        return $"{this.Name} ({this.Email})";
    }
}