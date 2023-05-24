namespace Bu.Web.Data.Attributes;

public sealed class TimeColumnAttribute : ColumnAttribute
{
    public TimeColumnAttribute()
    {
        this.TypeName = "TIME";
    }
}