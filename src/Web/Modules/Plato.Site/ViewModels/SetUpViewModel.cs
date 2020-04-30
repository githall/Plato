using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Plato.Site.ViewModels
{
    public class SetUpViewModel
    {

        [Required]
        public int Id { get; set; }


        [Required, DataType(DataType.Text), Display(Name = "company name")]
        [StringLength(255, MinimumLength = 4)]
        public string CompanyName { get; set; }

    }
}
