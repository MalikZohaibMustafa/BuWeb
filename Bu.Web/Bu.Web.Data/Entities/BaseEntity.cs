﻿namespace Bu.Web.Data.Entities;

public abstract class BaseEntity : IEntity
{
    public abstract int Id { get; }

    public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);

    public virtual void PreSave(ClaimsPrincipal claimsPrincipal)
    {
        IExpiringEntity? expiringEntity = this as IExpiringEntity;
        expiringEntity?.PreSaveChanges(claimsPrincipal);
        IPublished? publishedEntity = this as IPublished;
        publishedEntity?.PreSaveChanges(claimsPrincipal);
        ITrackedEntity? trackedEntity = this as ITrackedEntity;
        trackedEntity?.PreSaveChanges(claimsPrincipal);
    }

    public virtual void PostQuery(ClaimsPrincipal claimsPrincipal)
    {
        IExpiringEntity? expiringEntity = this as IExpiringEntity;
        expiringEntity?.PostLoad(claimsPrincipal);
        IPublished? publishedEntity = this as IPublished;
        publishedEntity?.PostLoad(claimsPrincipal);
        ITrackedEntity? trackedEntity = this as ITrackedEntity;
        trackedEntity?.PostLoad(claimsPrincipal);
    }

    public static void PrimaryKey<TEntity>(EntityTypeBuilder<TEntity> entity, string tableName, string propertyName, bool autoGenerated = true, long seed = 1)
        where TEntity : class
    {
        _ = entity.HasKey(propertyName).HasName($"PK_{tableName}");
        _ = autoGenerated ? entity.Property(propertyName).UseIdentityColumn(seed) : entity.Property(propertyName).ValueGeneratedNever();
    }

    public static void UniqueKey<TEntity>(EntityTypeBuilder<TEntity> entity, string tableName, params string[] propertyNames)
        where TEntity : class
    {
        if (!propertyNames.Any())
        {
            throw new ArgumentNullException(nameof(propertyNames));
        }

        _ = entity.HasAlternateKey(propertyNames).HasName($"UK_{tableName}_{string.Join("_", propertyNames)}");
    }

    protected static void OnModelCreating<TEntity>(EntityTypeBuilder<TEntity> entity, string tableName, string schema = "dbo")
        where TEntity : class
    {
        _ = entity.ToTable(tableName, schema);

        if (typeof(TEntity).IsAssignableTo(typeof(IArea)))
        {
            IArea.OnModelCreating(entity);
        }

        if (typeof(TEntity).IsAssignableTo(typeof(IPublished)))
        {
            IPublished.OnModelCreating(entity);
        }
        else
        {
            if (typeof(TEntity).IsAssignableTo(typeof(ITrackedEntity)))
            {
                ITrackedEntity.OnModelCreating(entity);
            }

            if (typeof(TEntity).IsAssignableTo(typeof(IExpiringEntity)))
            {
                IExpiringEntity.OnModelCreating(entity);
            }
        }
    }
}