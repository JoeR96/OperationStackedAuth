using System.ComponentModel.DataAnnotations;

namespace OperationStackedAuth.Requests
{
    public class ConfirmForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string VerificationCode { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}
