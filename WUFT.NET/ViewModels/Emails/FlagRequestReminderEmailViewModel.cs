﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.Emails
{
    [Serializable]
    public class FlagRequestReminderEmailViewModel
    {
        public String Disposition      { get; set; }
        public String LotCount         { get; set; }
        public String BoxCount         { get; set; }
        public String FlaggedUnitCount { get; set; }
        public String URL              { get; set; }
        public String WarehouseName    { get; set; }
        public String MRBID            { get; set; }
    }
}