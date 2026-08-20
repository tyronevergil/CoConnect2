using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QueryRouting;

namespace CoConnect.Infrastructure.QueryRouting
{
    public sealed class QueryRouteRegistry
    {
        private readonly IReadOnlyList<QueryRouteDescriptor> _descriptors;

        public QueryRouteRegistry()
        {
            _descriptors = DiscoverRoutes();
        }

        public IReadOnlyList<QueryRouteDescriptor> Descriptors => _descriptors;

        public bool TryMatch(string httpMethod, string requestPath, out QueryRouteMatch match)
        {
            foreach (var descriptor in _descriptors)
            {
                if (descriptor.TryMatch(httpMethod, requestPath, out var routeValues))
                {
                    match = new QueryRouteMatch(descriptor, routeValues);
                    return true;
                }
            }

            match = null!;
            return false;
        }

        private static IReadOnlyList<QueryRouteDescriptor> DiscoverRoutes()
        {
            var descriptors = new List<QueryRouteDescriptor>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);

            foreach (var assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray()!;
                }

                foreach (var type in types)
                {
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    foreach (var method in methods)
                    {
                        var attribute = method.GetCustomAttribute<QueryRouteAttribute>();
                        if (attribute == null)
                            continue;

                        var descriptor = new QueryRouteDescriptor(method, attribute);
                        EnsureUniqueRoute(descriptors, descriptor);
                        descriptors.Add(descriptor);
                    }
                }
            }

            return descriptors;
        }

        private static void EnsureUniqueRoute(List<QueryRouteDescriptor> descriptors, QueryRouteDescriptor descriptor)
        {
            var duplicate = descriptors.FirstOrDefault(existing =>
                string.Equals(existing.HttpMethod, descriptor.HttpMethod, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(existing.RouteTemplate, descriptor.RouteTemplate, StringComparison.OrdinalIgnoreCase));

            if (duplicate != null)
            {
                throw new InvalidOperationException($"Duplicate query route registration detected for '{descriptor.HttpMethod} {descriptor.RouteTemplate}'.");
            }
        }
    }
}
