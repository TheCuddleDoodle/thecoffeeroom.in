using System.ComponentModel.DataAnnotations;

namespace Coffeeroom.Models.Domain
{
    public class PasswordToken
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime DateAdded{ get; set; }

        [Required]
        public bool IsValid { get; set; }
    }
}
