namespace NewPlatform.Flexberry.AspNet.WebApi.Cors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Cors;
    using System.Web.Http.Cors;

    /// <summary>
    /// Implementation of <see cref="ICorsPolicyProvider"/> that allows all origins from HTTP request.
    /// </summary>
    /// <seealso cref="ICorsPolicyProvider" />
    public class DynamicCorsPolicyProvider : ICorsPolicyProvider
    {
        /// <summary>
        /// Keeps value for <see cref="CorsPolicy.SupportsCredentials"/>.
        /// </summary>
        private readonly bool supportsCredentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicCorsPolicyProvider"/> class.
        /// </summary>
        /// <param name="supportsCredentials">Value for <see cref="CorsPolicy.SupportsCredentials"/>.</param>
        public DynamicCorsPolicyProvider(bool supportsCredentials = false)
        {
            this.supportsCredentials = supportsCredentials;
        }

        /// <summary>
        /// Gets the <see cref="CorsPolicy" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="CorsPolicy" />.</returns>
        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var policy = new CorsPolicy
            {
                AllowAnyHeader = true,
                AllowAnyMethod = true,
                SupportsCredentials = supportsCredentials,
            };

            IEnumerable<string> origins;
            if (!request.Headers.TryGetValues("Origin", out origins))
            {
                return Task.FromResult(policy);
            }

            foreach (string origin in origins.Where(s => !string.IsNullOrEmpty(s)))
            {
                policy.Origins.Add(origin);
            }

            return Task.FromResult(policy);
        }
    }
}
