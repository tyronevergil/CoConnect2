using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CrudDatastore;
using Persistence;
using QueryRouting;

namespace CoConnect.Infrastructure.QueryRouting
{
    public sealed class QueryRouteExecutor
    {
        private readonly IQueryContextFactory _queryContextFactory;

        public QueryRouteExecutor(IQueryContextFactory queryContextFactory)
        {
            _queryContextFactory = queryContextFactory;
        }

        public async Task<QueryExecutionResult> ExecuteAsync(QueryRouteMatch match, CancellationToken cancellationToken = default)
        {
            using var queryContext = _queryContextFactory.CreateQueryContext();
            var specification = match.Descriptor.CreateSpecification(match.RouteValues);

            if (match.Descriptor.ResultKind == QueryRouteResultKind.Collection)
            {
                var collection = await InvokeQueryAsync("ExecuteFindAsync", queryContext, match.Descriptor.EntityType, specification, cancellationToken).ConfigureAwait(false);
                return new QueryExecutionResult(200, collection ?? Array.Empty<object>());
            }

            var single = await InvokeQueryAsync("ExecuteFindSingleAsync", queryContext, match.Descriptor.EntityType, specification, cancellationToken).ConfigureAwait(false);
            if (single == null)
                return new QueryExecutionResult(404, null);

            return new QueryExecutionResult(200, single);
        }

        private async Task<object?> InvokeQueryAsync(string methodName, QueryContext queryContext, Type entityType, object specification, CancellationToken cancellationToken)
        {
            var method = GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => string.Equals(m.Name, methodName, StringComparison.Ordinal))
                .Where(m => m.IsGenericMethodDefinition)
                .Where(m => m.GetParameters().Length == 3)
                .SingleOrDefault();

            if (method == null)
                throw new InvalidOperationException($"Query route executor does not expose the expected '{methodName}<T>' helper method.");

            var genericMethod = method.MakeGenericMethod(entityType);
            var invocationResult = genericMethod.Invoke(this, new object[] { queryContext, specification, cancellationToken });
            return await UnwrapTaskResultAsync(invocationResult, cancellationToken).ConfigureAwait(false);
        }

        private async Task<object?> ExecuteFindAsync<T>(QueryContext queryContext, object specification, CancellationToken cancellationToken)
            where T : EntityBase
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await queryContext.FindAsync((Specification<T>)specification).ConfigureAwait(false);
            return result?.ToList() ?? new List<T>();
        }

        private async Task<object?> ExecuteFindSingleAsync<T>(QueryContext queryContext, object specification, CancellationToken cancellationToken)
            where T : EntityBase
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await queryContext.FindSingleAsync((Specification<T>)specification).ConfigureAwait(false);
        }

        private static async Task<object?> UnwrapTaskResultAsync(object? invocationResult, CancellationToken cancellationToken)
        {
            if (invocationResult is not Task task)
                return invocationResult;

            await task.ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            var taskType = task.GetType();
            if (!taskType.IsGenericType)
                return null;

            return taskType.GetProperty("Result")?.GetValue(task);
        }
    }

    public sealed record QueryExecutionResult(int StatusCode, object? Payload);
}
