using Markdig;

namespace Bu.Web.Common;

public static class MarkdownParser
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build();

    public static string ToHtml(string markdown)
    {
        return Markdown.ToHtml(markdown, MarkdownPipeline);
    }
}