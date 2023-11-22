using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WUFT.NET.ViewModels.QRE
{
    public class ConfirmRequestViewModel : CreateRequestBaseViewModel
    {
        public ConfirmRequestViewModel()
        {
            UploadUnits = new List<UploadUnits>();
        }
        [UIHint("UploadedUnits")]
        public List<UploadUnits> UploadUnits { get; set; }
        public string QRELoadJobID { get; set; }

        public bool Merge { get; set; }

    }
}