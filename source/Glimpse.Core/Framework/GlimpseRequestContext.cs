using System;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;

namespace Glimpse.Core.Framework
{
    /// <summary>
    /// Represents the context of a specific request, which is used as an access point to the request's <see cref="IRequestResponseAdapter"/> handle
    /// </summary>
    public class GlimpseRequestContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlimpseRequestContext" />
        /// </summary>
        /// <param name="glimpseRequestId">The Id assigned to the request by Glimpse.</param>
        /// <param name="requestResponseAdapter">The <see cref="IRequestResponseAdapter "/> of this request.</param>
        public GlimpseRequestContext(Guid glimpseRequestId, IRequestResponseAdapter requestResponseAdapter)
            : this(glimpseRequestId, requestResponseAdapter, GlimpseRuntime.Instance.Configuration.EndpointBaseUri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlimpseRequestContext" />
        /// </summary>
        /// <param name="glimpseRequestId">The Id assigned to the request by Glimpse.</param>
        /// <param name="requestResponseAdapter">The <see cref="IRequestResponseAdapter "/> of this request.</param>
        /// <param name="endpointBaseUri">The endpoint base URI.</param>
        public GlimpseRequestContext(Guid glimpseRequestId, IRequestResponseAdapter requestResponseAdapter, string endpointBaseUri)
        {
            if (requestResponseAdapter == null)
            {
                throw new ArgumentNullException("requestResponseAdapter");
            }

            if (!requestResponseAdapter.HttpRequestStore.Contains(Constants.RuntimePolicyKey))
            {
                throw new ArgumentException("The requestResponseAdapter.HttpRequestStore should contain a value for the key '" + Constants.RuntimePolicyKey + "'.");
            }

            GlimpseRequestId = glimpseRequestId;
            RequestResponseAdapter = requestResponseAdapter;
            RequestHandlingMode = RequestResponseAdapter.RequestMetadata.AbsolutePath.StartsWith(endpointBaseUri, StringComparison.InvariantCultureIgnoreCase)
                                    || ("~" + RequestResponseAdapter.RequestMetadata.AbsolutePath).StartsWith(endpointBaseUri, StringComparison.InvariantCultureIgnoreCase)
                                    ? RequestHandlingMode.ResourceRequest
                                    : RequestHandlingMode.RegularRequest;
        }

        /// <summary>
        /// Gets the Glimpse Id assigned to this request
        /// </summary>
        public Guid GlimpseRequestId { get; private set; }

        /// <summary>
        /// Gets the <see cref="IRequestResponseAdapter"/> for this request
        /// </summary>
        public IRequestResponseAdapter RequestResponseAdapter { get; private set; }

        /// <summary>
        /// Gets the active <see cref="RuntimePolicy"/> for this request
        /// </summary>
        public virtual RuntimePolicy ActiveRuntimePolicy
        {
            get
            {
                return RequestResponseAdapter.HttpRequestStore.Get<RuntimePolicy>(Constants.RuntimePolicyKey);
            }
        }

        /// <summary>
        /// Gets the <see cref="RequestHandlingMode"/> for this request
        /// </summary>
        public virtual RequestHandlingMode RequestHandlingMode { get; private set; }
    }
}