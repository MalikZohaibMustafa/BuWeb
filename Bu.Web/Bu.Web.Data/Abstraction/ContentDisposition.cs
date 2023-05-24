namespace Bu.Web.Data.Abstraction;

public enum ContentDisposition : byte
{
    [Description("Inline")]
    Inline = 1,
    [Description("Attachment")]
    Attachment = 2,
}