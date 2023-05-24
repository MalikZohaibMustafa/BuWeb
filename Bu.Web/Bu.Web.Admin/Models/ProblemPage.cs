namespace Bu.Web.Admin.Models;

public sealed class ProblemPage
{
    public ProblemPage(ILogger logger, IPrincipal? user, HttpStatusCode httpStatusCode, string? targetUrl)
    {
        this.HttpStatusCode = httpStatusCode;
        this.TargetUrl = targetUrl;
        string username = user?.Identity?.Name ?? "[N/A]";
        logger.LogWarning($"Status Code {this.HttpStatusCodeString}; User '{username}'; URL: {targetUrl}");
    }

    public string? TargetUrl { get; }

    public HttpStatusCode HttpStatusCode { get; }

    public string HttpStatusCodeString => $"{(int)this.HttpStatusCode} ({this.HttpStatusCode})";
}