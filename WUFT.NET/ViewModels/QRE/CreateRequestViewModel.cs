using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.QRE
{
    public class CreateRequestBaseViewModel
    {
        public CreateRequestBaseViewModel()
        {
            ErrorList = new List<String>();
        }

        [Display(Name = "Affected Warehouses: ")]
        public String AffectedWarehouses { get; set; }
        
        [Display(Name = "Errors: ")]
        [UIHint("StringList")]
        public List<String> ErrorList    { get; set; }
        
        [Required(ErrorMessage = "MRB Number is required")]
        [Display(Name = "MRB Number: ")]
        //[StringLength(7, ErrorMessage = "MRB Number must be 7 digits", MinimumLength = 7)] //vsharm6x : MRB number change to 8 digits 
        [StringLength(8, ErrorMessage = "MRB Number must be 7 or 8 digits", MinimumLength = 7)]  //vsharm6x : MRB number change to 8 digits 
        [RegularExpression(@"^\d+$", ErrorMessage = "MRB Number may only contain digits")]
        public string MRBID            { get; set; }
        
    }
}