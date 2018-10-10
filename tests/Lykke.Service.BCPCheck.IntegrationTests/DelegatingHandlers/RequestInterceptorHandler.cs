using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.BCPCheck.IntegrationTests.Utils;

namespace Lykke.Service.BCPCheck.IntegrationTests.DelegatingHandlers
{
    //Used in tests only to redirect http requests to the test fixture server
    public class RequestInterceptorHandler : DelegatingHandler
    {
        private readonly HttpClient _httpClient;

        public RequestInterceptorHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var clonedRequest = await request.CloneAsync();
            var response = await _httpClient.SendAsync(clonedRequest, cancellationToken);

            return response;
        }
    }
}
