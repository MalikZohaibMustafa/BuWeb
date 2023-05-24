namespace Bu.Web.Data.Entities;

public abstract class BaseTimestampEntity : BaseEntity, ITimestampEntity
{
    [Column(Order = 1)]
    [Timestamp]
    public byte[]? Timestamp { get; set; }
}