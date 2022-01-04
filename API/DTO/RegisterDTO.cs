using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class RegisterDTO
    {
        [Required]
        [MaxLength(20, ErrorMessage = "Maximum Length is 15 characters")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
