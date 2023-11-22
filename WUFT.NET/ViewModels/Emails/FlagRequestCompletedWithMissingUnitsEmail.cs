using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WUFT.NET.Views.Shared;

namespace WUFT.NET.ViewModels.Emails
{
    [Serializable]
    public class FlagRequestCompletedWithMissingUnitsEmailViewModel
    {
        public FlagRequestCompletedWithMissingUnitsEmailViewModel()
        {
            DownloadURL = "http://" + ConfigurationManager.AppSettings["BaseURL"] + "/File/ExportSingleRequest/";
            ViewURL = "http://" + ConfigurationManager.AppSettings["BaseURL"] + "/QRE/ViewRequest/";
            Boxes = new List<CompletedBoxSummary>();
            MissingUnits = new List<LotBoxUnit>();
            UnitsFoundOnStopRequest = new List<LotBoxUnit>();
        }
        public string Disposition                      { get; set; }
        public List<CompletedBoxSummary> Boxes         { get; set; }

        public List<LotBoxUnit> MissingUnits           { get; set; }
        public string Warehouse                        { get; set; }
        public string Requestor                        { get; set; }
        public string MRBID                            { get; set; }
        public string DownloadURL                      { get; set; }
        public string ViewURL                          { get; set; }
        public List<LotBoxUnit> UnitsFoundOnStopRequest { get; set; }
    }
}