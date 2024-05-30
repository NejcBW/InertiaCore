using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace InertiaCore.Extensions;

internal static class InertiaExtensions
{
    /// <summary>
    /// Takes an object, gets its properties, select their names and intersects
    /// the obtained collection with the list of property names passed as a parameter.
    /// </summary>
    /// <param name="obj">An object containing properties to perform operation on.</param>
    /// <param name="only">A list of properties that should be returned.</param>
    /// <returns>An <see cref="IEnumerable{T}"/>containing only the property
    /// names that should be returned in the response.</returns>
    internal static IEnumerable<string> Only(this object obj, IEnumerable<string> only) =>
        obj.GetType().GetProperties().Select(c => c.Name)
            .Intersect(only, StringComparer.OrdinalIgnoreCase).ToList();

    /// <summary>
    /// Looks up the <c>X-Inertia-Partial-Data</c> header in the incoming HTTP request,
    /// splits the header's content by comma, filters for values that are not null or empty
    /// and returns a <see cref="List{T}"/> containing these values.
    /// </summary>
    /// <param name="context">An instance of <see cref="ActionContext"/> on
    /// which to execute the extension method.</param>
    /// <returns>A <see cref="List{T}"/> of requested partial data property names.</returns>
    internal static List<string> GetPartialData(this ActionContext context) =>
        context.HttpContext.Request.Headers["X-Inertia-Partial-Data"]
            .FirstOrDefault()?.Split(",")
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList() ?? new List<string>();

    /// <summary>
    /// Looks up the <c>X-Inertia-Partial-Component</c> header name in the
    /// incoming HTTP request and returns a boolean depending on whether
    /// the header's value matches the <paramref name="component"/> parameter
    /// or not.
    /// </summary>
    /// <param name="context">The instance of <see cref="ActionContext"/> on
    /// which to execute the extension method.</param>
    /// <param name="component">The component's name.</param>
    /// <returns>Returns <c>true</c> if the <paramref name="component"/>
    /// is equal to the header's value or <c>false</c> otherwise.</returns>
    internal static bool IsInertiaPartialComponent(this ActionContext context, string component) =>
        context.HttpContext.Request.Headers["X-Inertia-Partial-Component"] == component;

    /// <summary>
    /// Gets the relative URI of the HTTP request.
    /// </summary>
    /// <param name="context">An instance of <see cref="HttpContext"/> on which
    /// to execute the extension method.</param>
    /// <returns>The relative URI of the HTTP request</returns>
    internal static string RequestedUri(this HttpContext context) =>
        Uri.UnescapeDataString(context.Request.GetEncodedPathAndQuery());

    /// <summary>
    /// Gets the relative URI of the HTTP request.
    /// </summary>
    /// <param name="context">An instance of <see cref="ActionContext"/> on which
    /// to execute the extension method.</param>
    /// <returns>The relative URI of the HTTP request</returns>
    internal static string RequestedUri(this ActionContext context) => context.HttpContext.RequestedUri();

    /// <summary>
    /// An overload method that checks for the presence of <c>X-Inertia</c>
    /// header in the current HTTP request.
    /// </summary>
    /// <param name="context">An instance of <see cref="HttpContent"/> on which
    /// to execute the extension method.</param>
    /// <returns>A <see cref="Boolean"/>: <c>true</c> if the header is present,
    /// <c>false</c>if the header is not present.</returns>
    internal static bool IsInertiaRequest(this HttpContext context) =>
        bool.TryParse(context.Request.Headers["X-Inertia"], out _);

    /// <summary>
    /// An overload method that checks for the presence of <c>X-Inertia</c>
    /// header in the current HTTP request.
    /// </summary>
    /// <param name="context">An instance of <see cref="ActionContext"/> on which
    /// to execute the extension method.</param>
    /// <returns>A <see cref="Boolean"/>: <c>true</c> if the header is present,
    /// <c>false</c>if the header is not present.</returns>
    internal static bool IsInertiaRequest(this ActionContext context) => context.HttpContext.IsInertiaRequest();

    internal static string ToCamelCase(this string s) => JsonNamingPolicy.CamelCase.ConvertName(s);
}