namespace OperationStackedAuth.Tests
{
    internal class ApiClientFactory
    {
        internal static swaggerClient CreateApiClient()
        {
            const string baseUrl = "http://13.40.62.72:5001";

            var client = new HttpClient();
            var authClient = new swaggerClient(baseUrl, client);
            return authClient;
        }
    }
}
