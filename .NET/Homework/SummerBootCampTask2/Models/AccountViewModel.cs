using System.ComponentModel.DataAnnotations;

namespace SummerBootCampTask2.Models
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "User name is not defined!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is not defined!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is not defined!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Visible { get; set; }
        public int Identifier { get; set; }
    }
}
