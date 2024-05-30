using System.Net;
using InertiaCore.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace InertiaCore.Utils;

/// <summary>
/// Returns a 409 Conflict response including the url in an X-Inertia-Location
/// header. Inertia will then automatically perform a redirect by setting
/// window.location = url.
/// </summary>
public class LocationResult : IActionResult
{
    private readonly string _url;

    public LocationResult(string url) => _url = url;

    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (context.IsInertiaRequest())
        {
            context.HttpContext.Response.Headers.Add("X-Inertia-Location", _url);
            await new StatusCodeResult((int)HttpStatusCode.Conflict).ExecuteResultAsync(context);
        }

        await new RedirectResult(_url).ExecuteResultAsync(context);
    }
}