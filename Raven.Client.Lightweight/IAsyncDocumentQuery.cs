using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Raven.Abstractions.Data;
using Raven.Client.Indexes;
using Raven.Client.Spatial;
using Raven.Json.Linq;

namespace Raven.Client
{
    /// <summary>
    ///     Asynchronous query against a raven index
    /// </summary>
    public interface IAsyncDocumentQuery<T> : IDocumentQueryBase<T, IAsyncDocumentQuery<T>>
    {
        string AsyncIndexQueried { get; }

        /// <summary>
        /// Register the query as a lazy-count query and return a lazy instance that will evaluate the query when needed.
        /// </summary>
        Lazy<Task<int>> CountLazilyAsync(CancellationToken token = default (CancellationToken));

        /// <summary>
        ///     Get the facets as per the specified doc with the given start and pageSize
        /// </summary>
        Task<FacetResults> GetFacetsAsync(string facetSetupDoc, int facetStart, int? facetPageSize, CancellationToken token = default (CancellationToken));

        /// <summary>
        ///     Get the facets as per the specified facets with the given start and pageSize
        /// </summary>
        Task<FacetResults> GetFacetsAsync(List<Facet> facets, int facetStart, int? facetPageSize, CancellationToken token = default (CancellationToken));
        
        /// <summary>
        ///     Get the facets lazily as per the specified doc with the given start and pageSize
        /// </summary>
        Lazy<Task<FacetResults>> GetFacetsLazyAsync(string facetSetupDoc, int facetStart, int? facetPageSize, CancellationToken token = default(CancellationToken));

        /// <summary>
        ///     Get the facets lazily as per the specified doc with the given start and pageSize
        /// </summary>
        Lazy<Task<FacetResults>> GetFacetsLazyAsync(List<Facet> facets, int facetStart, int? facetPageSize, CancellationToken token = default(CancellationToken));
        
        /// <summary>
        ///     Create the index query object for this query
        /// </summary>
        IndexQuery GetIndexQuery(bool isAsync);

        /// <summary>
        ///     Register the query as a lazy query and return a lazy
        ///     instance that will evaluate the query only when needed.
        /// Also provide a function to execute when the value is evaluated
        /// </summary>
        Lazy<Task<IEnumerable<T>>> LazilyAsync(Action<IEnumerable<T>> onEval = null);

        /// <summary>
        ///     Gets the query result. Executing this method for the first time will execute the query.
        /// </summary>
        Task<QueryResult> QueryResultAsync(CancellationToken token = default (CancellationToken));

        /// <summary>
        ///     Selects the specified fields directly from the index if the are stored. If the field is not stored in index, value
        ///     will come from document directly.
        /// </summary>
        /// <typeparam name="TProjection">Type of the projection.</typeparam>
        /// <param name="fields">Array of fields to load.</param>
        IAsyncDocumentQuery<TProjection> SelectFields<TProjection>(params string[] fields);

        /// <summary>
        ///     Selects the specified fields directly from the index if the are stored. If the field is not stored in index, value
        ///     will come from document directly.
        /// </summary>
        /// <typeparam name="TProjection">Type of the projection.</typeparam>
        /// <param name="fields">Array of fields to load.</param>
        /// <param name="projections">Array of field projections.</param>
        IAsyncDocumentQuery<TProjection> SelectFields<TProjection>(string[] fields, string[] projections);

        /// <summary>
        ///     Selects the specified fields directly from the index if the are stored. If the field is not stored in index, value
        ///     will come from document directly.
        ///     <para>Array of fields will be taken from TProjection</para>
        /// </summary>
        /// <typeparam name="TProjection">Type of the projection from which fields will be taken.</typeparam>
        IAsyncDocumentQuery<TProjection> SelectFields<TProjection>();

        /// <summary>
        ///     Transformer parameters that will be passed to transformer if one is specified.
        /// </summary>
        [Obsolete("Use SetTransformerParameters instead.")]
        void SetQueryInputs(Dictionary<string, RavenJToken> queryInputs);

        /// <summary>
        ///     Transformer parameters that will be passed to transformer if one is specified.
        /// </summary>
        IAsyncDocumentQuery<T> SetTransformerParameters(Dictionary<string, RavenJToken> transformerParameters);

        /// <summary>
        ///     Ability to use one factory to determine spatial shape that will be used in query.
        /// </summary>
        /// <param name="path">Spatial field name.</param>
        /// <param name="clause">function with spatial criteria factory</param>
        IAsyncDocumentQuery<T> Spatial(Expression<Func<T, object>> path, Func<SpatialCriteriaFactory, SpatialCriteria> clause);

        /// <summary>
        ///     Ability to use one factory to determine spatial shape that will be used in query.
        /// </summary>
        /// <param name="fieldName">Spatial field name.</param>
        /// <param name="clause">function with spatial criteria factory</param>
        IAsyncDocumentQuery<T> Spatial(string fieldName, Func<SpatialCriteriaFactory, SpatialCriteria> clause);

        /// <summary>
        ///     Executed the query and returns the results.
        /// </summary>
        Task<IList<T>> ToListAsync(CancellationToken token = default (CancellationToken));


        /// <summary>
        ///     Sets a transformer to use after executing a query
        /// </summary>
        IAsyncDocumentQuery<TTransformerResult> SetResultTransformer<TTransformer, TTransformerResult>()
            where TTransformer : AbstractTransformerCreationTask, new();

        
        /// <summary>
        ///     Returns first element or throws if sequence is empty.
        /// </summary>
        Task<T> FirstAsync();

        /// <summary>
        ///     Returns first element or default value for type if sequence is empty.
        /// </summary>
        Task<T> FirstOrDefaultAsync();

        /// <summary>
        ///     Returns first element or throws if sequence is empty or contains more than one element.
        /// </summary>
        Task<T> SingleAsync();

        /// <summary>
        ///     Returns first element or default value for given type if sequence is empty. Throws if sequence contains more than
        ///     one element.
        /// </summary>
        Task<T> SingleOrDefaultAsync();

        /// <summary>
        /// Gets the total count of records for this query
        /// </summary>
        Task<int> CountAsync(CancellationToken token = default(CancellationToken));
    } 
}
