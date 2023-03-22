using Dapper;
using rtdc_rest.api.Models.Dtos;
using rtdc_rest.api.Services.Abstract;
using System.Data.SqlClient;

namespace rtdc_rest.api.Services.Concrete
{
    public class StockFlManager : IStockFlService
    {
        private readonly IConfiguration _configuration;
        public StockFlManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<StockFlDto>> GetStockFlListAsync()
        {
            string connection = _configuration.GetSection("AppSettings:DbConnection").Value;
            string companyCode = _configuration.GetSection("AppSettings:CompanyCode").Value;
            string season = _configuration.GetSection("AppSettings:Season").Value;

            {
                SqlConnection connect = new SqlConnection(connection);
                connect.Open();

                var sql = "SELECT DataSourceCode = CASE STL.SOURCEINDEX WHEN 35 THEN 'AYKIZM' WHEN 7 THEN 'AYKANT' WHEN 42 THEN 'AYKKNY' WHEN 50 THEN 'AYKIST' END ," +
                    "ManufacturerCode = CASE IT.SPECODE WHEN 'BPT' THEN 'BYR' ELSE IT.SPECODE END ," +
                    "RetailerCode = clc.code ," +
                    "RetailerRefId = clc.LOGICALREF ," +
                    "years = YEAR(INV.DATE_) ," +
                    "months = MONTH(INV.DATE_) ," +
                    "InvoiceDate = INV.DATE_ ," +
                    "InvoiceDateSystem = INV.CAPIBLOCK_CREADEDDATE ," +
                    "InvoiceId = INV.LOGICALREF ," +
                    "InvoiceNo = INV.FICHENO ," +
                    "InvoiceLine = STL.STFICHELNNO ," +
                    "ProductCode = SUBSTRING(IT.code, CHARINDEX('.', IT.code) + 1, LEN(IT.code) - CHARINDEX('.', IT.code)) ," +
                    "ItemQuantity = (CASE WHEN STL.TRCODE = 3 THEN(STL.AMOUNT * STL.UINFO2) * (-1) ELSE STL.AMOUNT* STL.UINFO2 END) ," +
                    "QuantityInPackage = (SELECT CONVFACT2 FROM LG_"+ companyCode +"_ITMUNITA ITMN WHERE IT.LOGICALREF = ITMN.ITEMREF AND LINENR = 2 ) ," +
                    "PackageQuantity = (CASE WHEN STL.TRCODE = 3 THEN((STL.AMOUNT * STL.UINFO2) / ITM2.CONVFACT2) * (-1) ELSE((STL.AMOUNT * STL.UINFO2) / ITM2.CONVFACT2) END) ," +
                    "ItemBarcode = EK.URUNBARKODU ," +
                    "PackageBarcode = EK.KOLİBARKODU ," +
                    "LineAmount = ISNULL((CASE WHEN STL.TRCODE = 3 AND STL.LINETYPE IN(0, 7) THEN(STL.vatmatrah) * (-1) WHEN STL.TRCODE = '8' AND STL.LINETYPE IN(0, 7) THEN(STL.vatmatrah) END),0) ," +
                    "DiscountAmount = CASE WHEN STL.TRCODE IN('8') THEN STL.DISTDISC WHEN STL.TRCODE IN('3') THEN STL.DISTDISC * -1 ELSE 0 END ," +
                    "SalesOrderId = ORF.LOGICALREF ," +
                    "IsReturnInvoice = CASE WHEN STL.TRCODE IN('3') THEN 'true' ELSE 'false' END " +
                    "FROM LG_"+ companyCode +"_"+ season +"_STLINE STL " +
                    "INNER JOIN LG_"+ companyCode +"_ITEMS IT ON IT.LOGICALREF = STL.STOCKREF " +
                    "INNER JOIN LG_"+ companyCode +"_"+ season +"_STFICHE STF ON STF.LOGICALREF = STL.STFICHEREF " +
                    "INNER JOIN LG_"+ companyCode +"_CLCARD CLC ON CLC.LOGICALREF = STL.CLIENTREF " +
                    "INNER JOIN LG_"+ companyCode +"_"+ season +"_INVOICE INV ON INV.LOGICALREF = STF.INVOICEREF " +
                    "LEFT JOIN LG_"+ companyCode +"_"+ season +"_ORFICHE ORF ON STL.ORDFICHEREF = ORF.LOGICALREF " +
                    "INNER JOIN LG_"+ companyCode +"_ITMUNITA ITM2 ON ITM2.ITEMREF = IT.LOGICALREF AND ITM2.LINENR = '2' " +
                    "LEFT OUTER JOIN LG_XT1001_"+ companyCode +" AS EK ON IT.LOGICALREF = EK.PARLOGREF " +
                    "WHERE STL.CANCELLED = '0' AND STL.TRCODE IN('3','8') AND STL.SOURCEINDEX IN('34','35','6','7','41','42','50') AND STL.LINETYPE IN(0,1,7) " +
                    "AND IT.SPECODE IN('3M','BPT','WL') ORDER BY INV.FICHENO ";

                #region--COMMENTOUT
                //var sql = " SELECT DataSourceCode = CASE StLinePort.SOURCEINDEX WHEN 35 THEN 'AYKIZM' " +
                //    "  WHEN 7 THEN 'AYKANT' WHEN 42 THEN 'AYKKNY' WHEN 50 THEN 'AYKIST' ELSE 'TANIMSIZ' END " +
                //    ", ManufacturerCode = CASE StCardPort.SPECODE WHEN 'BPT' THEN 'BYR' ELSE StCardPort.SPECODE END " +
                //    ", retailerCode = CLC.CODE " +
                //    ", retailerRefId = CLC.LOGICALREF " +
                //    ", Year = Year(StFichePort.CAPIBLOCK_CREADEDDATE)" +
                //    ", Month = Month(StFichePort.CAPIBLOCK_CREADEDDATE) " +
                //    ", invoiceDate = StFichePort.CAPIBLOCK_CREADEDDATE " +
                //    ", invoiceDateSystem = StFichePort.CAPIBLOCK_CREADEDDATE " +
                //    ", invoiceId = StFichePort.LOGICALREF " +
                //    ", invoiceNo = StFichePort.FICHENO " +
                //    ", invoiceLine = StLinePort.STFICHELNNO " +
                //    ", ProductCode = SUBSTRING(StCardPort.code, CHARINDEX('.',StCardPort.code)+1, LEN(StCardPort.code) - CHARINDEX('.',StCardPort.code)) " +
                //    ", ItemQuantity = (CASE WHEN STL.TRCODE = 3 THEN(STL.AMOUNT * STL.UINFO2) * (-1) ELSE STL.AMOUNT * STL.UINFO2 END) " +
                //    ", QuantityInPackage = (SELECT CONVFACT2 FROM LG_" + companyCode + "_ITMUNITA ITMN WHERE StCardPort.LOGICALREF = ITMN.ITEMREF AND LINENR = 2 ) " +
                //    ", PackageQuantity = (CASE WHEN STL.TRCODE = 3 THEN ((STL.AMOUNT * STL.UINFO2) / ITM2.CONVFACT2) * (-1) ELSE ((STL.AMOUNT * STL.UINFO2) / ITM2.CONVFACT2) END) " +
                //    ", ItemBarcode = EK.URUNBARKODU " +
                //    ", PackageBarcode = EK.KOLİBARKODU " +
                //    ", LineAmount = (SELECT TOP(1) PRICE FROM LG_" + companyCode + "_PRCLIST AS PRC " +
                //    "  WHERE(StCardPort.LOGICALREF = CARDREF) AND (PTYPE = 1) ORDER BY BEGDATE DESC ) " +
                //    ", discountAmount = StLinePort.DISTCOST " +
                //    ", salesOrderId = StLinePort.ORDFICHEREF " +
                //    ", isReturnInvoice = CASE StFichePort.TRCODE WHEN 3 THEN 1 ELSE 0 END " +
                //    "  FROM LG_" + companyCode + "_" + season + "_STFICHE StFichePort WITH(NOLOCK) " +
                //    "  INNER JOIN LG_" + companyCode + "_CLCARD CLC ON CLC.LOGICALREF = StFichePort.CLIENTREF " +
                //    "  INNER JOIN LG_" + companyCode + "_" + season + "_STLINE StLinePort WITH(NOLOCK) ON StFichePort.LOGICALREF = StLinePort.STFICHEREF " +
                //    "  INNER JOIN LG_" + companyCode + "_ITEMS StCardPort WITH(NOLOCK) ON StLinePort.STOCKREF = StCardPort.LOGICALREF " +
                //    "  LEFT OUTER JOIN LG_" + companyCode + "_ITMUNITA ITMUNITA WITH(NOLOCK) ON " +
                //    "  StCardPort.LOGICALREF = ITMUNITA.ITEMREF AND ITMUNITA.LINENR = '2' " +
                //    "  LEFT OUTER JOIN LG_" + companyCode + "_ITMUNITA ITMUNITA1 WITH(NOLOCK) ON " +
                //    "  StCardPort.LOGICALREF = ITMUNITA1.ITEMREF AND ITMUNITA1.LINENR = '4' " +
                //    "  LEFT OUTER JOIN LG_XT1001_" + companyCode + " AS EK ON StCardPort.LOGICALREF = EK.PARLOGREF " +
                //    "  WHERE StLinePort.LINETYPE IN(0,1) " +
                //    "  AND StLinePort.SOURCEINDEX IN('35','7','42','50') AND StFichePort.CANCELLED = 0 " +
                //    "  AND StCardPort.SPECODE IN('3M','BPT','WL') " +
                //    "  GROUP BY StLinePort.SOURCEINDEX,StCardPort.SPECODE,StCardPort.PRODUCERCODE,StCardPort.LOGICALREF,EK.URUNBARKODU,EK.KOLİBARKODU,CLC.CODE, " +
                //    "  CLC.LOGICALREF,StFichePort.CAPIBLOCK_CREADEDDATE,StFichePort.LOGICALREF,StLinePort.STFICHELNNO,StFichePort.FICHENO, " +
                //    " StLinePort.DISTCOST,StLinePort.ORDFICHEREF,StFichePort.TRCODE ORDER BY StFichePort.CAPIBLOCK_CREADEDDATE DESC";
                #endregion

                var result = connect.Query<StockFlDto>(sql).ToList();
                return result;
            }
        }
    }
}
