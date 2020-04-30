using System.ComponentModel.DataAnnotations;

namespace Plato.Site.ViewModels
{
    public class SetUpViewModel
    {

        [Required, StringLength(255)]
        public string SessionId { get; set; }

        [Required, DataType(DataType.Text), Display(Name = "company name")]
        [StringLength(255, MinimumLength = 4)]
        public string CompanyName { get; set; }

    }

    public class SetUpConfirmationViewModel
    {

        [Required, StringLength(255)]
        public string SessionId { get; set; }

        public string Url { get; set; }

    }

}
