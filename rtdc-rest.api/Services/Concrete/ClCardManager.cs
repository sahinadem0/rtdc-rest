using Dapper;
using rtdc_rest.api.Models.Dtos;
using rtdc_rest.api.Services.Abstract;
using System.Data.SqlClient;

namespace rtdc_rest.api.Services.Concrete
{
    public class ClCardManager : IClCardService
    {
        private readonly IConfiguration _configuration;
        public ClCardManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<ClCardDto>> GetClCardListAsync()
        {
            string connection = _configuration.GetSection("AppSettings:DbConnection").Value;
            string companyCode = _configuration.GetSection("AppSettings:CompanyCode").Value;
            string season = _configuration.GetSection("AppSettings:Season").Value;

            {
                SqlConnection connect = new SqlConnection(connection);
                connect.Open();

                var sql = "SELECT DISTINCT CLC.CODE," +
                          "DataSourceCode =  case when INV.SOURCEINDEX = 35 then 'AYKIZM' " +
                          "WHEN INV.SOURCEINDEX = 7 then 'AYKANT' " +
                          "WHEN INV.SOURCEINDEX = 42 then 'AYKKNY' " +
                          "WHEN INV.SOURCEINDEX = 50 then 'AYKIST' END " +
                          ",RetailerCode = CLC.code " +
                          ",RetailerRefId = CLC.LOGICALREF " +
                          ",ChannelCode = CASE WHEN CLC.specode5 IN ('HFS','TIER1','SUBD') THEN CLC.SPECODE5 ELSE 'HFS' END " +
                          ",Title = CLC.DEFINITION_ " +
                          ",Email = CLC.EMAILADDR " +
                          ",Phone = CLC.TELNRS1 " +
                          ",TaxOffice = CLC.TAXOFFICE " +
                          ",TaxNumber = CASE WHEN ISPERSCOMP = 1 THEN CLC.TCKNO ELSE CLC.TAXNR END " +
                          ",ContactName = CLC.INCHARGE " +
                          ",Country = CLC.COUNTRY " +
                          ",City = CLC.CITY " +
                          ",District = CASE WHEN CLC.TOWN = '' THEN 'MERKEZ' ELSE CLC.TOWN END " +
                          ",Address = CLC. ADDR1 + CLC.ADDR1 " +
                          ",ZipCode = CLC.POSTCODE " +
                          "from LG_" + companyCode + "_CLCARD CLC  INNER  JOIN LG_" + companyCode + "_" + season + "_INVOICE INV WITH (NOLOCK) ON CLC.LOGICALREF = INV.CLIENTREF " +
                          "INNER  JOIN LG_" + companyCode + "_" + season + "_STLINE STL WITH (NOLOCK) ON INV.LOGICALREF = STL.INVOICEREF " +
                          "INNER  JOIN LG_" + companyCode + "_ITEMS ITM WITH (NOLOCK) ON STL.STOCKREF = ITM.LOGICALREF " +
                          "WHERE CLC.ACTIVE = 0 and ITM.SPECODE IN ('BPT','3M','WL') AND INV.TRCODE = 8 ";

                #region--old customers query
                //var sql = " SELECT DataSourceCode = CASE WHEN SUBSTRING(CLC.CODE,5,1) IN('I', 'D', 'M') THEN 'AYKIZM' " +
                //    "WHEN SUBSTRING(CLC.CODE,5,1) IN('A') THEN 'AYKANT' " +
                //    "WHEN SUBSTRING(CLC.CODE,5,1) IN('K') THEN 'AYKKNY' " +
                //    "WHEN SUBSTRING(CLC.CODE,5,1) IN('B', 'S') THEN 'AYKIST' ELSE 'TANIMSIZ' END " +
                //    ",RetailerCode = CLC.code " +
                //    ",RetailerRefId = CLC.LOGICALREF " +
                //    ",ChannelCode = 'HFS' " +
                //    ",Title = CLC.DEFINITION_ " +
                //    ",Email = CLC.EMAILADDR " +
                //    ",Phone = CLC.TELNRS1 " +
                //    ",TaxOffice = CLC.TAXOFFICE " +
                //    ",TaxNumber = CASE WHEN ISPERSCOMP = 1 THEN CLC.TCKNO ELSE CLC.TAXNR END " +
                //    ",ContactName = CLC.INCHARGE " +
                //    ",Country = CLC.COUNTRY " +
                //    ",City = CLC.CITY " +
                //    ",District = CASE WHEN CLC.TOWN = '' THEN 'MERKEZ' ELSE CLC.TOWN END " +
                //    ",Address = CLC.ADDR1 + CLC.ADDR1 " +
                //    ",ZipCode = CLC.POSTCODE " +
                //    "FROM LG_" + companyCode + "_CLCARD CLC WHERE ACTIVE = 0 AND SUBSTRING(CLC.CODE,5,2) IN('I.', 'D.', 'M.', 'A.', 'K.', 'B.', 'S.')";
                #endregion

                var result = connect.Query<ClCardDto>(sql).ToList();
                return result;
            }
        }
    }
}
