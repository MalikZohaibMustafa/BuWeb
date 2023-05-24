namespace Bu.Web.Data.Attributes;

public sealed class UtcAttribute : Attribute
{
    public UtcAttribute(bool isUtc = true)
    {
        this.IsUtc = isUtc;
    }

    public bool IsUtc { get; }
}