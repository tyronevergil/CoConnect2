using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CrudDatastore;
using QueryRouting;

namespace CoConnect.Infrastructure.QueryRouting
{
    public sealed class QueryRouteDescriptor
    {
        private readonly string[] _routeSegments;
        private readonly string[] _routeParameterNames;

        public QueryRouteDescriptor(MethodInfo factoryMethod, QueryRouteAttribute attribute)
        {
            FactoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

            HttpMethod = attribute.HttpMethod;
            RouteTemplate = NormalizeRouteTemplate(attribute.RouteTemplate);
            ResultKind = attribute.ResultKind;
            SpecificationType = factoryMethod.ReturnType;
            EntityType = GetSpecificationEntityType(factoryMethod.ReturnType);

            _routeSegments = SplitPath(RouteTemplate);
            _routeParameterNames = _routeSegments
                .Where(IsRouteParameterSegment)
                .Select(segment => segment[1..^1])
                .ToArray();

            ValidateFactoryMethod();
        }

        public MethodInfo FactoryMethod { get; }

        public QueryRouteAttribute Attribute { get; }

        public string HttpMethod { get; }

        public string RouteTemplate { get; }

        public QueryRouteResultKind ResultKind { get; }

        public Type SpecificationType { get; }

        public Type EntityType { get; }

        public IReadOnlyList<string> RouteParameterNames => _routeParameterNames;

        public bool TryMatch(string httpMethod, string requestPath, out IReadOnlyDictionary<string, string> routeValues)
        {
            routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.Equals(HttpMethod, httpMethod, StringComparison.OrdinalIgnoreCase))
                return false;

            var requestSegments = SplitPath(requestPath);
            if (requestSegments.Length != _routeSegments.Length)
                return false;

            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < _routeSegments.Length; i++)
            {
                var routeSegment = _routeSegments[i];
                var requestSegment = requestSegments[i];

                if (IsRouteParameterSegment(routeSegment))
                {
                    values[routeSegment[1..^1]] = requestSegment;
                    continue;
                }

                if (!string.Equals(routeSegment, requestSegment, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            routeValues = values;
            return true;
        }

        public object CreateSpecification(IReadOnlyDictionary<string, string> routeValues)
        {
            var parameters = FactoryMethod.GetParameters();
            if (parameters.Length != RouteParameterNames.Count)
            {
                throw new InvalidOperationException($"Query route '{RouteTemplate}' expects {RouteParameterNames.Count} route values, but factory method '{FactoryMethod.Name}' has {parameters.Length} parameter(s).");
            }

            var arguments = new object?[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var routeParameterName = RouteParameterNames[i];
                if (!routeValues.TryGetValue(routeParameterName, out var rawValue))
                {
                    throw new InvalidOperationException($"Missing route value '{routeParameterName}' for query route '{RouteTemplate}'.");
                }

                arguments[i] = ConvertRouteValue(rawValue, parameters[i].ParameterType);
            }

            var specification = FactoryMethod.Invoke(null, arguments);
            if (specification == null)
                throw new InvalidOperationException($"Query route factory method '{FactoryMethod.DeclaringType?.FullName}.{FactoryMethod.Name}' returned null.");

            return specification;
        }

        private void ValidateFactoryMethod()
        {
            if (!FactoryMethod.IsStatic)
                throw new InvalidOperationException($"Query route factory method '{FactoryMethod.DeclaringType?.FullName}.{FactoryMethod.Name}' must be static.");

            if (_routeParameterNames.Length != FactoryMethod.GetParameters().Length)
            {
                throw new InvalidOperationException($"Query route '{RouteTemplate}' must declare the same number of route parameters as factory method '{FactoryMethod.Name}'.");
            }
        }

        private static Type GetSpecificationEntityType(Type specificationType)
        {
            var current = specificationType;
            while (current != null && current != typeof(object))
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(Specification<>))
                    return current.GetGenericArguments()[0];

                current = current.BaseType;
            }

            throw new InvalidOperationException($"Specification type '{specificationType.FullName}' does not inherit from CrudDatastore.Specification<T>.");
        }

        private static string NormalizeRouteTemplate(string routeTemplate)
        {
            var normalized = routeTemplate.Trim();
            if (!normalized.StartsWith('/'))
                normalized = "/" + normalized;

            if (normalized.Length > 1 && normalized.EndsWith('/'))
                normalized = normalized.TrimEnd('/');

            return normalized;
        }

        private static string[] SplitPath(string path)
        {
            var normalized = path.Trim();
            if (!normalized.StartsWith('/'))
                normalized = "/" + normalized;

            if (normalized.Length > 1 && normalized.EndsWith('/'))
                normalized = normalized.TrimEnd('/');

            return normalized
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        private static bool IsRouteParameterSegment(string segment)
        {
            return segment.Length >= 3 && segment.StartsWith("{") && segment.EndsWith("}");
        }

        private static object? ConvertRouteValue(string rawValue, Type targetType)
        {
            var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (nonNullableType == typeof(string))
                return rawValue;

            if (nonNullableType == typeof(Guid))
                return Guid.Parse(rawValue);

            if (nonNullableType == typeof(int))
                return int.Parse(rawValue, CultureInfo.InvariantCulture);

            if (nonNullableType == typeof(long))
                return long.Parse(rawValue, CultureInfo.InvariantCulture);

            if (nonNullableType == typeof(decimal))
                return decimal.Parse(rawValue, CultureInfo.InvariantCulture);

            if (nonNullableType == typeof(double))
                return double.Parse(rawValue, CultureInfo.InvariantCulture);

            if (nonNullableType == typeof(bool))
                return bool.Parse(rawValue);

            if (nonNullableType.IsEnum)
                return Enum.Parse(nonNullableType, rawValue, ignoreCase: true);

            return Convert.ChangeType(rawValue, nonNullableType, CultureInfo.InvariantCulture);
        }
    }

    public sealed record QueryRouteMatch(QueryRouteDescriptor Descriptor, IReadOnlyDictionary<string, string> RouteValues);
}
