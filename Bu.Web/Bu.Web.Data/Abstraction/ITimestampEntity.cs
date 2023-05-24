namespace Bu.Web.Data.Abstraction;

public interface ITimestampEntity : IEntity
{
    public byte[]? Timestamp { get; set; }
}