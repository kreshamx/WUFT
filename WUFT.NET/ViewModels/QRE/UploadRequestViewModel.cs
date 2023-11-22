using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WUFT.MODEL;

namespace WUFT.NET.ViewModels.QRE
{
    
    public class UploadRequestViewModel : CreateRequestBaseViewModel
    {
        public UploadRequestViewModel()
        {
            MaxUploadUnits = 0;
        }

        [Required(ErrorMessage="A file is required")]
        [Display(Name="Unit Flag File: ")]
        public HttpPostedFileBase File                { get; set; }
        public string FilePath { get; set; }
        public int MaxUploadUnits { get; set; }

        
    }
}