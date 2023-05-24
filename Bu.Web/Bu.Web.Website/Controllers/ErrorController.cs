using System.Net;
using Bu.AspNetCore.Core.Extensions;
using Bu.AspNetCore.Mvc;

namespace Bu.Web.Website.Controllers;

[AllowAnonymous]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class ErrorController : BuhoBaseController, IErrorController
{
    public ErrorController(ILogger<ErrorController> logger)
    {
        this.Logger = logger;
    }

    private ILogger Logger { get; }

    [HttpGet]
    public ViewResult Index()
    {
        ErrorViewModel model = new ErrorViewModel
        {
            RequestId = this.HttpContext.TraceIdentifier,
        };
        return this.View(nameof(this.Index), model);
    }

    [ActionName(nameof(Index))]
    [HttpPost]
    public ViewResult IndexPost()
    {
        return this.Index();
    }

    [HttpGet]
    [Route("Error/403")]
    [Route(nameof(AccessDenied))]
    public ViewResult AccessDenied(string? returnUrl)
    {
        return this.Problem(nameof(this.AccessDenied), HttpStatusCode.Forbidden, returnUrl);
    }

    [HttpPost]
    [Route("Error/403")]
    [Route(nameof(AccessDenied))]
    public ViewResult AccessDeniedPost(string? returnUrl)
    {
        return this.AccessDenied(returnUrl);
    }

    [HttpGet]
    [Route("Error/404")]
    [Route(nameof(PageNotFound))]
    public ViewResult PageNotFound()
    {
        return this.Problem(nameof(this.PageNotFound), HttpStatusCode.NotFound);
    }

    [HttpPost]
    [Route("Error/404")]
    [Route(nameof(PageNotFound))]
    public ViewResult PageNotFoundPost()
    {
        return this.PageNotFound();
    }

    [HttpGet]
    [Route("Error/405")]
    [Route(nameof(MethodNotAllowed))]
    public ViewResult MethodNotAllowed()
    {
        return this.Problem(nameof(this.PageNotFound), HttpStatusCode.MethodNotAllowed);
    }

    [HttpPost]
    [Route("Error/405")]
    [Route(nameof(MethodNotAllowed))]
    public ViewResult MethodNotAllowedPost()
    {
        return this.MethodNotAllowed();
    }

    [HttpGet]
    [Route("Error/{code}")]
    public ViewResult OtherProblem(int code)
    {
        return this.Problem(nameof(this.OtherProblem), (HttpStatusCode)code);
    }

    [HttpGet]
    [Route("Error/{code}")]
    public ViewResult OtherProblemPost(int code)
    {
        return this.OtherProblem(code);
    }

    private ViewResult Problem(string viewName, HttpStatusCode httpStatusCode, string? targetUrl = default)
    {
        var model = new ProblemPage(this.Logger, this.User, httpStatusCode, targetUrl ?? this.HttpContext.Referer());
        return this.View(viewName, model);
    }
}