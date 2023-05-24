namespace Bu.Web.Data.Attributes;

public sealed class DateTime2ColumnAttribute : ColumnAttribute
{
    public DateTime2ColumnAttribute()
    {
        this.TypeName = "DATETIME2";
    }
}