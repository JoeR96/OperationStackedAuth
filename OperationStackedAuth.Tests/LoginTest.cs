using FluentAssertions;

namespace OperationStackedAuth.Tests
{
    public class LoginTest
    {
       
        const string baseUrl = "http://13.40.62.72:5001";

        [Test]
        public async Task LogIn_AndReceiveToken()
        {

            var client = new HttpClient();
            var authClient = new swaggerClient(baseUrl, client);

            var request = new LoginRequest
            {
                Email = "joeyrichardson96@gmail.com",
                Password = "Zelfdwnq9512!"
            };

            var response = await authClient.LoginAsync(request);
            response.IdToken.Should().NotBeNullOrWhiteSpace();
        }
    }
}