using Dapper;
using Microsoft.AspNetCore.Hosting.Server;
using rtdc_rest.api.Models;
using rtdc_rest.api.Models.Dtos;
using rtdc_rest.api.Services.Abstract;
using System.Data.SqlClient;
using rtdc_rest.api.config;

namespace rtdc_rest.api.Services.Concrete
{
    public class StockLvManager : IStockLvService
    {
        public async Task<List<StockLvDto>> GetStockLvListAsync()
        {
            string connection = Configuration.getLogoConnection();            
            {
                SqlConnection connect = new SqlConnection(connection);
                connect.Open(); 

                var sql = " SELECT DataSourceCode = CASE StLinePort.SOURCEINDEX WHEN 35 THEN 'AYKIZM' WHEN 7 THEN 'AYKANT'" +
                    "  WHEN 42 THEN 'AYKKNY' WHEN 50 THEN 'AYKIST' ELSE 'TANIMSIZ' END " +
                    ", ManufacturerCode = CASE StCardPort.SPECODE WHEN 'BPT' THEN 'BYR' ELSE StCardPort.SPECODE END " +
                    ", StockDate = getdate() " +
                    ", ProductCode = StCardPort.PRODUCERCODE " +
                    ", ItemQuantity = SUM(CASE WHEN StLinePort.IOCODE IN(1, 2) THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 THEN 0 ELSE StLinePort.UINFO2 END) " +
                    "  WHEN StLinePort.IOCODE IN(3,4) THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 THEN 0 ELSE StLinePort.UINFO2 END ) *-1 ELSE 0 END ) " +
                    ", QuantityInPackage = (SELECT CONVFACT2 FROM LG_412_ITMUNITA ITMN WHERE StCardPort.LOGICALREF = ITMN.ITEMREF AND LINENR = 2 ) " +
                    ", PackageQuantity = SUM(CASE WHEN StLinePort.IOCODE IN(1, 2) " +
                    "  THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 THEN 0 ELSE StLinePort.UINFO2 / ITMUNITA.CONVFACT2 END) " +
                    "  WHEN StLinePort.IOCODE IN(3,4) THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 THEN 0 ELSE StLinePort.UINFO2 / ITMUNITA.CONVFACT2 END ) *-1 ELSE 0 END ) " +
                    ", ItemBarcode = EK.URUNBARKODU " +
                    ", PackageBarcode = EK.KOLİBARKODU " +
                    ", ListPrice = (SELECT TOP(1) PRICE FROM LG_412_PRCLIST AS PRC WHERE(StCardPort.LOGICALREF = CARDREF) AND(PTYPE = 1) ORDER BY BEGDATE DESC ) " +
                    "  FROM LG_412_12_STFICHE StFichePort WITH(NOLOCK) " +
                    "  INNER JOIN LG_412_12_STLINE StLinePort WITH(NOLOCK) ON StFichePort.LOGICALREF = StLinePort.STFICHEREF " +
                    "  INNER JOIN LG_412_ITEMS StCardPort WITH(NOLOCK) ON StLinePort.STOCKREF = StCardPort.LOGICALREF " +
                    "  LEFT OUTER JOIN LG_412_ITMUNITA ITMUNITA WITH(NOLOCK) ON StCardPort.LOGICALREF = ITMUNITA.ITEMREF AND ITMUNITA.LINENR = '2' " +
                    "  LEFT OUTER JOIN LG_412_ITMUNITA ITMUNITA1 WITH(NOLOCK) ON StCardPort.LOGICALREF = ITMUNITA1.ITEMREF AND ITMUNITA1.LINENR = '4' " +
                    "  LEFT OUTER JOIN LG_XT1001_412 AS EK ON StCardPort.LOGICALREF = EK.PARLOGREF " +
                    "  WHERE StLinePort.LINETYPE IN(0,1)  AND StLinePort.SOURCEINDEX IN('35','7','42','50')  AND StFichePort.CANCELLED = 0 " +
                    "  AND StCardPort.SPECODE IN('3M','BPT','WL') " +
                    "  GROUP BY StLinePort.SOURCEINDEX,StCardPort.SPECODE,StCardPort.PRODUCERCODE,StCardPort.LOGICALREF,EK.URUNBARKODU ,EK.KOLİBARKODU ";

                var result = connect.Query<StockLvDto>(sql).ToList();
                return result;
            }
        }
    }
}
