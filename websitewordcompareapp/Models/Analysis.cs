using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace websitewordcompareapp.Models
{
    public class Analysis
    {
        [DisplayName("Page 1")]
        [Required(ErrorMessage = "Enter Page 1")]
        public string Page1 { get; set; }

        [DisplayName("Page 2")]
        [Required(ErrorMessage = "Enter Page 2")]
        public string Page2 { get; set; } 
    }
}
