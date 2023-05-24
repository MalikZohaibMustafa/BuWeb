namespace Bu.Web.Data.Abstraction;

public static class ExtensionMethods
{
    public static ContentDisposition GetContentDisposition(this ContentTypes contentType)
    {
        return contentType switch
        {
            ContentTypes.Xlsx => ContentDisposition.Attachment,
            ContentTypes.Xls => ContentDisposition.Attachment,
            ContentTypes.Docx => ContentDisposition.Attachment,
            ContentTypes.Doc => ContentDisposition.Attachment,
            ContentTypes.Pptx => ContentDisposition.Attachment,
            ContentTypes.Ppt => ContentDisposition.Attachment,
            ContentTypes.Pdf => ContentDisposition.Inline,
            ContentTypes.Jpeg => ContentDisposition.Inline,
            ContentTypes.Png => ContentDisposition.Inline,
            _ => throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null),
        };
    }

    public static string GetFileNameExtension(this ContentTypes contentType)
    {
        return $".{contentType}".ToLowerInvariant();
    }

    public static string GetMimeType(this ContentTypes contentType)
    {
        MimeTypeMap.GetMimeType(contentType.GetFileNameExtension(), out string? mimeType);
        return mimeType ?? throw new InvalidOperationException(contentType.ToString());
    }

    public static bool IsPublished(this IPublished.Statuses status)
    {
        return status == IPublished.Statuses.Published;
    }

    public static bool IsRejected(this IPublished.Statuses status)
    {
        return status == IPublished.Statuses.Rejected;
    }

    public static bool IsPending(this IPublished.Statuses status)
    {
        return status == IPublished.Statuses.Pending;
    }
}