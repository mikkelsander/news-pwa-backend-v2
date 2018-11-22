using System.ComponentModel.DataAnnotations;

namespace PWANews.InputModels
{
    public class UserInputModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

}
