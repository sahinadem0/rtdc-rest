using Dapper;
using Microsoft.AspNetCore.Hosting.Server;
using rtdc_rest.api.Models;
using rtdc_rest.api.Models.Dtos;
using rtdc_rest.api.Services.Abstract;
using System.Data.SqlClient;

namespace rtdc_rest.api.Services.Concrete
{
    public class StockFlManager : IStockFlService
    {
        public async Task<List<StockFlDto>> GetStockFlListAsync()
        {
            using (var connection = new SqlConnection("Server =172.16.40.20; Database = AYK2008; User ID = PG; Password = PG2007"))
            //Server = 172.16.40.20; Database = AYK2008; Persist Security Info = True; User ID = PG; Password = PG2007
            {
                connection.Open();

                #region--commentOUT
                //var sql = " SELECT DataSourceCode = CASE WHEN SUBSTRING(CLC.CODE,5,1) IN('I', 'D', 'M') THEN 'AYKIZM' " +
                //    "WHEN SUBSTRING(CLC.CODE,5,1) IN('A') THEN 'AYKANT' " +
                //    "WHEN SUBSTRING(CLC.CODE,5,1) IN('K') THEN 'AYKKNY' " +
                //    "WHEN SUBSTRING(CLC.CODE,5,1) IN('B') THEN 'AYKIST' ELSE 'TANIMSIZ' END " +
                //    ",RetailerCode = clc.code " +
                //    ",RetailerRefId = clc.LOGICALREF " +
                //    ",ChannelCode = 'HFS' " +
                //    ",Title = clc.DEFINITION_ " +
                //    ",Email = clc.EMAILADDR " +
                //    ",Phone = clc.TELNRS1 " +
                //    ",TaxOffice = clc.TAXOFFICE " +
                //    ",TaxNumber = clc.TAXNR " +
                //    ",ContactName = clc.INCHARGE " +
                //    ",Country = clc.COUNTRY" +
                //    ",City = clc.CITY " +
                //    ",District = clc.DISTRICT " +
                //    ",Address = clc.ADDR1 + clc.ADDR1 " +
                //    ",ZipCode = clc.POSTCODE " +
                //    "FROM LG_412_CLCARD CLC WHERE ACTIVE = 0 " +
                //    "AND SUBSTRING(CLC.CODE,1,1) NOT IN('-','0','1','2','3','4','5','6','V') " +
                //    "AND NOT(CLC.CODE LIKE 'DC%' ) " +
                //    "AND CLC.TAXNR IS NOT NULL AND CLC.TAXNR<>'' AND CLC.DISTRICT IS NOT NULL AND CLC.DISTRICT<>'' ";
                #endregion

                var sql = " SELECT top 5 DataSourceCode = CASE StLinePort.SOURCEINDEX WHEN 35 THEN 'AYKIZM' " +
                    "WHEN 7 THEN 'AYKANT' WHEN 42 THEN 'AYKKNY' WHEN 50 THEN 'AYKIST' ELSE 'TANIMSIZ' END " +
                    ",ManufacturerCode = CASE StCardPort.SPECODE WHEN 'BPT' THEN 'BYR' ELSE StCardPort.SPECODE END " +
                    ",retailerCode = CLC.CODE " +
                    ", retailerRefId = CLC.LOGICALREF " +
                    ", Year = Year(StFichePort.CAPIBLOCK_CREADEDDATE)" +
                    ", Month = Month(StFichePort.CAPIBLOCK_CREADEDDATE) " +
                    ", invoiceDate = StFichePort.CAPIBLOCK_CREADEDDATE " +
                    ", invoiceDateSystem = StFichePort.CAPIBLOCK_CREADEDDATE " +
                    ", invoiceId = StFichePort.LOGICALREF " +
                    ", invoiceNo = StFichePort.FICHENO " +
                    ", invoiceLine = StLinePort.STFICHELNNO " +
                    ", ProductCode = StCardPort.PRODUCERCODE " +
                    ", ItemQuantity = SUM(CASE WHEN StLinePort.TRCODE IN (1,8) " +
                    "  THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 THEN 0 ELSE StLinePort.UINFO2 END) " +
                    "  WHEN StLinePort.IOCODE IN(3,4) THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 " +
                    "  THEN 0 ELSE StLinePort.UINFO2 END ) *-1  ELSE 0 END ) " +
                    ", QuantityInPackage = (SELECT CONVFACT2 FROM LG_412_ITMUNITA ITMN WHERE StCardPort.LOGICALREF = ITMN.ITEMREF AND LINENR = 2 ) " +
                    ", PackageQuantity = SUM(CASE WHEN StLinePort.TRCODE IN (1,8) THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 THEN 0 " +
                    "  ELSE StLinePort.UINFO2 / ITMUNITA.CONVFACT2 END) " +
                    "  WHEN StLinePort.IOCODE IN(3,4) THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 " +
                    "  THEN 0 ELSE StLinePort.UINFO2 / ITMUNITA.CONVFACT2 END ) *-1  ELSE 0 END ) " +
                    ", ItemBarcode = EK.URUNBARKODU " +
                    ", PackageBarcode = EK.KOLİBARKODU " +
                    ", LineAmount = (SELECT TOP(1) PRICE FROM LG_412_PRCLIST AS PRC " +
                    "  WHERE(StCardPort.LOGICALREF = CARDREF) AND(PTYPE = 1) ORDER BY BEGDATE DESC ) " +
                    ", discountAmount = StLinePort.DISTCOST " +
                    ", salesOrderId = StLinePort.ORDFICHEREF " +
                    ", isReturnInvoice = CASE StFichePort.TRCODE WHEN 3 THEN 1 ELSE 0 END " +
                    "  FROM LG_412_12_STFICHE StFichePort WITH(NOLOCK) " +
                    "  INNER JOIN LG_412_CLCARD CLC ON CLC.LOGICALREF = StFichePort.CLIENTREF " +
                    "  INNER JOIN LG_412_12_STLINE StLinePort WITH(NOLOCK) ON StFichePort.LOGICALREF = StLinePort.STFICHEREF " +
                    "  INNER JOIN LG_412_ITEMS StCardPort WITH(NOLOCK) ON StLinePort.STOCKREF = StCardPort.LOGICALREF " +
                    "  LEFT OUTER JOIN LG_412_ITMUNITA ITMUNITA WITH(NOLOCK) ON " +
                    "  StCardPort.LOGICALREF = ITMUNITA.ITEMREF AND ITMUNITA.LINENR = '2' " +
                    "  LEFT OUTER JOIN LG_412_ITMUNITA ITMUNITA1 WITH(NOLOCK) ON " +
                    "  StCardPort.LOGICALREF = ITMUNITA1.ITEMREF AND ITMUNITA1.LINENR = '4' " +
                    "  LEFT OUTER JOIN LG_XT1001_412 AS EK ON StCardPort.LOGICALREF = EK.PARLOGREF " +
                    "  WHERE StLinePort.LINETYPE IN(0,1) " +
                    "  AND StLinePort.SOURCEINDEX IN('35','7','42','50') AND StFichePort.CANCELLED = 0 " +
                    "  AND StCardPort.SPECODE IN('3M','BPT','WL') " +
                    "  GROUP BY StLinePort.SOURCEINDEX,StCardPort.SPECODE,StCardPort.PRODUCERCODE,StCardPort.LOGICALREF,EK.URUNBARKODU,EK.KOLİBARKODU,CLC.CODE, " +
                    "  CLC.LOGICALREF,StFichePort.CAPIBLOCK_CREADEDDATE,StFichePort.LOGICALREF,StLinePort.STFICHELNNO,StFichePort.FICHENO, " +
                    " StLinePort.DISTCOST,StLinePort.ORDFICHEREF,StFichePort.TRCODE " +
                    " ORDER BY StFichePort.CAPIBLOCK_CREADEDDATE DESC";


                //sql = " select * from LG_001_CLCARD ";
                var result = connection.Query<StockFlDto>(sql).ToList();
                return result;
            }
        }
    }
}
