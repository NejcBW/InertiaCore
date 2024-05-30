using System.Net;
using InertiaCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace InertiaCore.Utils;

/// <summary>
/// This class defines an action filter that sets a 303 response code after
/// a "PUT", "PATCH" or "DELETE" Inertia request. Typically, after using one
/// of these verbs, you need to redirect back to some route. When receiving the
/// 303 code, the browser will ALWAYS follow the redirect link using the GET verb.
/// </summary>
internal class InertiaActionFilter : IActionFilter
{
    private readonly IUrlHelperFactory _urlHelperFactory;

    public InertiaActionFilter(IUrlHelperFactory urlHelperFactory) => _urlHelperFactory = urlHelperFactory;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        //
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Exit:
        // - if it's not an Inertia request or
        // - if the request method is not "PUT", "PATCH" or "DELETE"
        if (!context.IsInertiaRequest()
            || !new[] { "PUT", "PATCH", "DELETE" }.Contains(context.HttpContext.Request.Method)) return;

        var destinationUrl = context.Result switch
        {
            RedirectResult result => result.Url,
            RedirectToActionResult result => GetUrl(result, context),
            RedirectToPageResult result => GetUrl(result, context),
            RedirectToRouteResult result => GetUrl(result, context),
            _ => null
        };

        // If destinationUrl is null then no filter is applied and the response
        // is sent out unmodified
        if (destinationUrl == null) return;

        // If destinationUrl is not null then add a "Location" header and set
        // its value to the value of 'destinationUrl'.
        // The "Location" HTTP header field is a required field in combination
        // with the 303 response code: https://en.wikipedia.org/wiki/HTTP_location
        context.HttpContext.Response.Headers.Add("Location", destinationUrl);

        // Set status code to 303
        context.Result = new StatusCodeResult((int)HttpStatusCode.RedirectMethod);
    }

    /// <summary>
    /// Gets the redirect URL if the "result" parameter is of type
    /// RedirectToActionResult.
    /// </summary>
    /// <param name="result">The <see cref="RedirectToActionResult"/></param>
    /// <param name="context">The <see cref="ActionContext"/></param>
    /// <returns>A string holding the redirect URL.</returns>
    private string? GetUrl(RedirectToActionResult result, ActionContext context)
    {
        var urlHelper = result.UrlHelper ?? _urlHelperFactory.GetUrlHelper(context);

        return urlHelper.Action(
            result.ActionName,
            result.ControllerName,
            result.RouteValues,
            null,
            null,
            result.Fragment);
    }

    private string? GetUrl(RedirectToPageResult result, ActionContext context)
    {
        var urlHelper = result.UrlHelper ?? _urlHelperFactory.GetUrlHelper(context);

        return urlHelper.Page(
            result.PageName,
            result.PageHandler,
            result.RouteValues,
            result.Protocol,
            result.Host,
            result.Fragment);
    }

    private string? GetUrl(RedirectToRouteResult result, ActionContext context)
    {
        var urlHelper = result.UrlHelper ?? _urlHelperFactory.GetUrlHelper(context);

        return urlHelper.RouteUrl(
            result.RouteName,
            result.RouteValues,
            null,
            null,
            result.Fragment);
    }
}