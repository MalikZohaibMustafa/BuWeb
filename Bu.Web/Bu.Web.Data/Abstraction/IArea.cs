namespace Bu.Web.Data.Abstraction;

public interface IArea
{
    int AreaId { get; set; }

    Area? Area { get; set; }

    public static void OnModelCreating<TEntity>(EntityTypeBuilder<TEntity> entity)
        where TEntity : class
    {
        _ = entity.HasOne(e => ((IArea)e).Area)
            .WithMany()
            .HasPrincipalKey(e => e.AreaId)
            .HasForeignKey(e => ((IArea)e).AreaId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}