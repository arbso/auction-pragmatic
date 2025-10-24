using System.ComponentModel.DataAnnotations;

namespace bid_app_pragmatic.ViewModels
{
    public class CreateAuctionViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Auction Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Starting price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Starting price must be between $0.01 and $999,999.99")]
        [Display(Name = "Starting Price")]
        public decimal StartingPrice { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
    }
}
