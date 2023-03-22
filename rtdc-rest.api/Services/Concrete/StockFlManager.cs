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

                var sql = "DECLARE @MUTABAKAT AS INT = 0 DECLARE @ONCEKIAY AS INT = month( DATEADD( mm, DATEDIFF( mm,0,GETDATE() ) -1, 0 ) ) " +
                    "DECLARE @BUAY AS INT = month ( DATEADD( mm, DATEDIFF( mm,0,GETDATE() ), 0 ) )" +
                    "SELECT top 4 DataSourceCode = CASE STL.SOURCEINDEX WHEN 35 THEN 'AYKIZM' WHEN 7 THEN 'AYKANT' WHEN 42 THEN 'AYKKNY' WHEN 50 THEN 'AYKIST' END, " +
                    "ManufacturerCode = CASE IT.SPECODE WHEN 'BPT' THEN 'BYR' ELSE IT.SPECODE END, " +
                    "RetailerCode = CLC.code, " +
                    "RetailerRefId = CLC.LOGICALREF, " +
                    "year = CAST( Year(INV.DATE_) AS int ), " +
                    "month = CAST( Month(INV.DATE_) AS int ), " +
                    "InvoiceDate = INV.DATE_, " +
                    "InvoiceDateSystem = INV.CAPIBLOCK_CREADEDDATE, " +
                    "InvoiceId = INV.LOGICALREF, " +
                    "InvoiceNo = INV.FICHENO,InvoiceLine = STL.STFICHELNNO, " +
                    "ProductCode = SUBSTRING( IT.code,CHARINDEX('.',IT.code) + 1, LEN(IT.code) - CHARINDEX('.', IT.code) ), " +
                    "ItemQuantity = ( CASE WHEN STL.TRCODE = 3 THEN (STL.AMOUNT * STL.UINFO2) * (-1) ELSE STL.AMOUNT * STL.UINFO2 END ), " +
                    "QuantityInPackage = ( SELECT CONVFACT2 FROM LG_412_ITMUNITA ITMN WHERE IT.LOGICALREF = ITMN.ITEMREF AND LINENR = 2 ), " +
                    "PackageQuantity = ( CASE WHEN STL.TRCODE = 3 THEN ( (STL.AMOUNT * STL.UINFO2) / ITM2.CONVFACT2 ) * (-1) ELSE ( (STL.AMOUNT * STL.UINFO2) / ITM2.CONVFACT2 ) END ), " +
                    "ItemBarcode = EK.URUNBARKODU, " +
                    "PackageBarcode = EK.KOLİBARKODU, " +
                    "LineAmount = ISNULL(( CASE WHEN STL.TRCODE = 3AND STL.LINETYPE IN (0, 7) THEN (STL.vatmatrah) * (-1) WHEN STL.TRCODE = '8' AND STL.LINETYPE IN (0, 7) THEN (STL.vatmatrah) END ),0), " +
                    "DiscountAmount = CASE WHEN STL.TRCODE IN ('8') THEN STL.DISTDISC WHEN STL.TRCODE IN ('3') THEN STL.DISTDISC * -1 ELSE 0 END, " +
                    "SalesOrderId = ORF.LOGICALREF, " +
                    "IsReturnInvoice = CASE WHEN STL.TRCODE IN ('3') THEN 1 ELSE 0 END " +
                    "FROM LG_412_12_STLINE STL " +
                    "INNER JOIN LG_412_ITEMS IT ON IT.LOGICALREF = STL.STOCKREF " +
                    "INNER JOIN LG_412_12_STFICHE STF ON STF.LOGICALREF = STL.STFICHEREF " +
                    "INNER JOIN LG_412_CLCARD CLC ON CLC.LOGICALREF = STL.CLIENTREF " +
                    "INNER JOIN LG_412_12_INVOICE INV ON INV.LOGICALREF = STF.INVOICEREF " +
                    "INNER JOIN LG_412_12_ORFICHE ORF ON STL.ORDFICHEREF = ORF.LOGICALREF " +
                    "INNER JOIN LG_412_ITMUNITA ITM2 ON ITM2.ITEMREF = IT.LOGICALREF " +
                    "AND ITM2.LINENR = '2' LEFT OUTER JOIN LG_XT1001_412 AS EK ON IT.LOGICALREF = EK.PARLOGREF WHERE STL.CANCELLED = '0' AND STL.TRCODE IN ('3', '8') " +
                    "AND STL.SOURCEINDEX IN ( '34', '35', '6', '7', '41', '42', '50' ) " +
                    "AND STL.LINETYPE IN (0, 1, 7)AND IT.SPECODE IN ('3M', 'BPT', 'WL') " +
                    "AND MONTH(INV.DATE_) = CASE WHEN @MUTABAKAT = 1 THEN @ONCEKIAY ELSE @BUAY END ORDER BY INV.FICHENO ;";

                #region--oldSqlQuery
                //var sql2 = "SELECT DataSourceCode = CASE STL.SOURCEINDEX WHEN 35 THEN 'AYKIZM' WHEN 7 THEN 'AYKANT' WHEN 42 THEN 'AYKKNY' WHEN 50 THEN 'AYKIST' END ," +
                //    "ManufacturerCode = CASE IT.SPECODE WHEN 'BPT' THEN 'BYR' ELSE IT.SPECODE END ," +
                //    "RetailerCode = clc.code ," +
                //    "RetailerRefId = clc.LOGICALREF ," +
                //    "years = YEAR(INV.DATE_) ," +
                //    "months = MONTH(INV.DATE_) ," +
                //    "InvoiceDate = INV.DATE_ ," +
                //    "InvoiceDateSystem = INV.CAPIBLOCK_CREADEDDATE ," +
                //    "InvoiceId = INV.LOGICALREF ," +
                //    "InvoiceNo = INV.FICHENO ," +
                //    "InvoiceLine = STL.STFICHELNNO ," +
                //    "ProductCode = SUBSTRING(IT.code, CHARINDEX('.', IT.code) + 1, LEN(IT.code) - CHARINDEX('.', IT.code)) ," +
                //    "ItemQuantity = (CASE WHEN STL.TRCODE = 3 THEN(STL.AMOUNT * STL.UINFO2) * (-1) ELSE STL.AMOUNT* STL.UINFO2 END) ," +
                //    "QuantityInPackage = (SELECT CONVFACT2 FROM LG_"+ companyCode +"_ITMUNITA ITMN WHERE IT.LOGICALREF = ITMN.ITEMREF AND LINENR = 2 ) ," +
                //    "PackageQuantity = (CASE WHEN STL.TRCODE = 3 THEN((STL.AMOUNT * STL.UINFO2) / ITM2.CONVFACT2) * (-1) ELSE((STL.AMOUNT * STL.UINFO2) / ITM2.CONVFACT2) END) ," +
                //    "ItemBarcode = EK.URUNBARKODU ," +
                //    "PackageBarcode = EK.KOLİBARKODU ," +
                //    "LineAmount = ISNULL((CASE WHEN STL.TRCODE = 3 AND STL.LINETYPE IN(0, 7) THEN(STL.vatmatrah) * (-1) WHEN STL.TRCODE = '8' AND STL.LINETYPE IN(0, 7) THEN(STL.vatmatrah) END),0) ," +
                //    "DiscountAmount = CASE WHEN STL.TRCODE IN('8') THEN STL.DISTDISC WHEN STL.TRCODE IN('3') THEN STL.DISTDISC * -1 ELSE 0 END ," +
                //    "SalesOrderId = ORF.LOGICALREF ," +
                //    "IsReturnInvoice = CASE WHEN STL.TRCODE IN('3') THEN 'true' ELSE 'false' END " +
                //    "FROM LG_"+ companyCode +"_"+ season +"_STLINE STL " +
                //    "INNER JOIN LG_"+ companyCode +"_ITEMS IT ON IT.LOGICALREF = STL.STOCKREF " +
                //    "INNER JOIN LG_"+ companyCode +"_"+ season +"_STFICHE STF ON STF.LOGICALREF = STL.STFICHEREF " +
                //    "INNER JOIN LG_"+ companyCode +"_CLCARD CLC ON CLC.LOGICALREF = STL.CLIENTREF " +
                //    "INNER JOIN LG_"+ companyCode +"_"+ season +"_INVOICE INV ON INV.LOGICALREF = STF.INVOICEREF " +
                //    "LEFT JOIN LG_"+ companyCode +"_"+ season +"_ORFICHE ORF ON STL.ORDFICHEREF = ORF.LOGICALREF " +
                //    "INNER JOIN LG_"+ companyCode +"_ITMUNITA ITM2 ON ITM2.ITEMREF = IT.LOGICALREF AND ITM2.LINENR = '2' " +
                //    "LEFT OUTER JOIN LG_XT1001_"+ companyCode +" AS EK ON IT.LOGICALREF = EK.PARLOGREF " +
                //    "WHERE STL.CANCELLED = '0' AND STL.TRCODE IN('3','8') AND STL.SOURCEINDEX IN('34','35','6','7','41','42','50') AND STL.LINETYPE IN(0,1,7) " +
                //    "AND IT.SPECODE IN('3M','BPT','WL') ORDER BY INV.FICHENO ";
                #endregion

                var result = connect.Query<StockFlDto>(sql).ToList();
                return result;
            }
        }
    }
}
