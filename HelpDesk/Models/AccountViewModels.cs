using System;
using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First name")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "First name must be alpha characters only.")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public DateTime DateOfBirth { get; set; }
    }

    public class UserProfileModel
    {

        [Required]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public string UserName { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
    }
}
