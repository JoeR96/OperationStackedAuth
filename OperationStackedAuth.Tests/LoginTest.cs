using Concise.Steps;
using FluentAssertions;

namespace OperationStackedAuth.Tests
{
    public class LoginTest
    {
        [Test]
        public async Task LogIn_AndReceiveToken()
        {
            swaggerClient authClient = ApiClientFactory.CreateApiClient();

            var request = new LoginRequest
            {
                Email = "joeyrichardson96@gmail.com",
                Password = "Zelfdwnq9512!"
            };

            await "Login snould return access token".__(async () =>
            {
                var response = await authClient.LoginAsync(request);
                response.IdToken.Should().NotBeNullOrWhiteSpace();
            });
        }
    }
}