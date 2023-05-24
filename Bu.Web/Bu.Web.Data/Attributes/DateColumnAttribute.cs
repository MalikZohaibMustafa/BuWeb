namespace Bu.Web.Data.Attributes;

public sealed class DateColumnAttribute : ColumnAttribute
{
    public DateColumnAttribute()
    {
        this.TypeName = "DATE";
    }
}
