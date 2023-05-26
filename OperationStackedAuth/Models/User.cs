using System.ComponentModel.DataAnnotations;

namespace OperationStackedAuth.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
    }
}
