namespace bid_app_pragmatic.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    namespace AuctionWebsite.ViewModels
    {
        public class RegisterViewModel
        {
            [Required(ErrorMessage = "Username is required")]
            [StringLength(20, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 20 characters")]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Confirm password is required")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "First name is required")]
            [StringLength(100)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last name is required")]
            [StringLength(100)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
        }
    }
}
