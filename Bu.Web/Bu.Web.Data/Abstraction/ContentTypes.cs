namespace Bu.Web.Data.Abstraction;

public enum ContentTypes : byte
{
    [Description("Excel Workbook (.xlsx)")]
    Xlsx = 1,
    [Description("Excel Workbook 97-2003 (.xls)")]
    Xls,
    [Description("Word Document (.docx)")]
    Docx,
    [Description("Word Document 97-2003 (.doc)")]
    Doc,
    [Description("PowerPoint Presentation (.pptx)")]
    Pptx,
    [Description("PowerPoint Presentation 97-2003 (.ppt)")]
    Ppt,
    [Description("PDF (.pdf)")]
    Pdf,
    [Description("Jpeg Image (.jpeg)")]
    Jpeg,
    [Description("Png Image (.png)")]
    Png,
}