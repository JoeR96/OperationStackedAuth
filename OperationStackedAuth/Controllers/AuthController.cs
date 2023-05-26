using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Mvc;
using OperationStackedAuth.Requests;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace OperationStackedAuth.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CognitoUserPool _userPool;
        private readonly IAmazonCognitoIdentityProvider _cognitoClient;

        public AuthController(CognitoUserPool userPool, IAmazonCognitoIdentityProvider cognitoClient)
        {
            _userPool = userPool;
            _cognitoClient = cognitoClient;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var user = _userPool.GetUser(request.Email);


            var signUpRequest = new SignUpRequest()
            {
                Username = request.Email,
                Password = request.Password,
                ClientId = _userPool.ClientID,
            };

            try
            {
                var result = await _cognitoClient.SignUpAsync(signUpRequest);

                if (result.HttpStatusCode == HttpStatusCode.OK)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = "An error occurred during registration." });
                }
            }
            catch (UsernameExistsException)
            {
                return BadRequest(new { message = "User already exists." });
            }
            catch (Exception ex)
            {
                // You may want to log the exception for debugging purposes
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)] // Add this line

        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = _userPool.GetUser(request.Email);

            // Calculate the SECRET_HASH

            AuthFlowResponse authResponse;

            try
            {
                authResponse = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest
                {
                    Password = request.Password,
                    // Add the calculated SECRET_HASH
                });
            }
            catch (NotAuthorizedException)
            {
                return Unauthorized();
            }
            catch (UserNotFoundException)
            {
                return Unauthorized();
            }

            if (authResponse.AuthenticationResult != null)
            {
                // Decode the ID token
                var handler = new JwtSecurityTokenHandler();
                var decodedToken = handler.ReadJwtToken(authResponse.AuthenticationResult.IdToken);

                // Get the "sub" claim
                var subClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                return Ok(new AuthResponse(
                    authResponse.AuthenticationResult.IdToken,
                    authResponse.AuthenticationResult.AccessToken,
                    authResponse.AuthenticationResult.RefreshToken,
                    authResponse.AuthenticationResult.TokenType,
                    authResponse.AuthenticationResult.ExpiresIn,
subClaim
                ));
            }

            return BadRequest("Login failed");
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string accessToken = Request.Headers["Authorization"];
            if (accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
            }

            var globalSignOutRequest = new GlobalSignOutRequest
            {
                AccessToken = accessToken
            };

            try
            {
                await _cognitoClient.GlobalSignOutAsync(globalSignOutRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error logging out: {ex.Message}" });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = _userPool.GetUser(request.Email);

            try
            {
                await user.ForgotPasswordAsync();
                return Ok(new { Message = "Password reset initiated. Check your email for the verification code." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error initiating password reset: {ex.Message}" });
            }
        }

        public class ForgotPasswordRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        [HttpPost("confirm-forgot-password")]
        public async Task<IActionResult> ConfirmForgotPassword([FromBody] Requests.ConfirmForgotPasswordRequest request)
        {
            var user = _userPool.GetUser(request.Email);

            try
            {
                await user.ConfirmForgotPasswordAsync(request.VerificationCode, request.NewPassword);
                return Ok(new { Message = "Password reset successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error confirming password reset: {ex.Message}" });
            }
        }


   

    }

    internal record AuthResponse(string IdToken, string AccessToken, string RefreshToken, string TokenType, int ExpiresIn, string? UserId);
}

