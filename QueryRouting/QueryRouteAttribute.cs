using System;

namespace QueryRouting
{
    public enum QueryRouteResultKind
    {
        Collection = 0,
        Single = 1
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class QueryRouteAttribute : Attribute
    {
        public QueryRouteAttribute(string httpMethod, string routeTemplate, QueryRouteResultKind resultKind)
        {
            if (string.IsNullOrWhiteSpace(httpMethod))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(httpMethod));

            if (string.IsNullOrWhiteSpace(routeTemplate))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(routeTemplate));

            HttpMethod = httpMethod.Trim().ToUpperInvariant();
            RouteTemplate = routeTemplate.Trim();
            ResultKind = resultKind;
        }

        public string HttpMethod { get; }

        public string RouteTemplate { get; }

        public QueryRouteResultKind ResultKind { get; }
    }
}
