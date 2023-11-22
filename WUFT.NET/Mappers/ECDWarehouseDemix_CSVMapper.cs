using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using WUFT.MODEL;
using WUFT.NET.Util;

namespace WUFT.NET.Mappers
{
    public sealed class ECDWarehouseDemix_CSVMapper : CsvClassMap<ECD_WarehouseDemix>
    {
        public ECDWarehouseDemix_CSVMapper()
        {
            Map(m => m.SubstrateVisualID).Name("VISUAL_ID");
            Map(m => m.OriginalBoxNumber).Name("FACTORY_BOX_ID");
            Map(m => m.DestinationBoxNumber).Name("SHIPMENT_BOX_ID");
            Map(m => m.UploadedWarehouseName).Name("WAREHOUSE/PLANT");
            Map(m => m.CreatedOn).ConvertUsing(x => DateTime.UtcNow);
            Map(m => m.LastUpdateOn).ConvertUsing(x => DateTime.UtcNow);
            Map(m => m.StartDateTime).ConvertUsing(x => DateTime.UtcNow);
            Map(m => m.EndDateTime).ConvertUsing(x => DateTime.UtcNow);

        }
    }
}