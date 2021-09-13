using System;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCoreIdentity.Services.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
