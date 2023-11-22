using System;
using System.Collections.Generic;

namespace WUFT.NET.ViewModels.QRE
{
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
            ErrorCode = 0;
        }
        public bool Success { get; set; }
        public String Message { get; set; }
        public bool ProvideSupportURL { get; set; }
        public String SupportURL { get; set; }
        public Dictionary<string, int> SubstrateList { get; set; }
        public Dictionary<string, string> SubstrateLists { get; set; } //vsharm6x
        public int ErrorCode { get; set; }
        public int[] ErrorCodes { get; set; }  //vsharm6x
        public string BoxNumber { get; set; }
        public string Disposition { get; set; }
        public string MRBID { get; set; }
        public string QRELoadJobID { get; set; }
        public string AdditionalMessage { get; set; }
        
    }
}