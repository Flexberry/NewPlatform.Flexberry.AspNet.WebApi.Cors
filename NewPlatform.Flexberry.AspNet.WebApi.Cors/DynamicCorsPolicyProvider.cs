namespace NewPlatform.Flexberry.AspNet.WebApi.Cors
{
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
        private bool _supportsCredentials;

        /// <summary>
        /// Default constructor for <see cref="DynamicCorsPolicyProvider"/>.
        /// </summary>
        /// <param name="supportsCredentials">Value for <see cref="CorsPolicy.SupportsCredentials"/>.</param>
        public DynamicCorsPolicyProvider(bool supportsCredentials = false)
        {
            _supportsCredentials = supportsCredentials;
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Cors.CorsPolicy" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="T:System.Web.Cors.CorsPolicy" />.</returns>
        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var policy = new CorsPolicy() { AllowAnyHeader = true, AllowAnyMethod = true, SupportsCredentials = _supportsCredentials };

            IEnumerable<string> origins;
            if (!request.Headers.TryGetValues("Origin", out origins))
                return Task.FromResult(policy);
            
            foreach (var origin in origins.Where(s => !string.IsNullOrEmpty(s)))
            {
                policy.Origins.Add(origin);
            }

            return Task.FromResult(policy);
        }
    }
}
