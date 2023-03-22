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

            {
                SqlConnection connect = new SqlConnection(connection);
                connect.Open();

                var sql = " SELECT DataSourceCode = CASE WHEN SUBSTRING(CLC.CODE,5,1) IN('I', 'D', 'M') THEN 'AYKIZM' " +
                    "WHEN SUBSTRING(CLC.CODE,5,1) IN('A') THEN 'AYKANT' " +
                    "WHEN SUBSTRING(CLC.CODE,5,1) IN('K') THEN 'AYKKNY' " +
                    "WHEN SUBSTRING(CLC.CODE,5,1) IN('B', 'S') THEN 'AYKIST' ELSE 'TANIMSIZ' END " +
                    ",RetailerCode = CLC.code " +
                    ",RetailerRefId = CLC.LOGICALREF " +
                    ",ChannelCode = 'HFS' " +
                    ",Title = CLC.DEFINITION_ " +
                    ",Email = CLC.EMAILADDR " +
                    ",Phone = CLC.TELNRS1 " +
                    ",TaxOffice = CLC.TAXOFFICE " +
                    ",TaxNumber = CASE WHEN ISPERSCOMP = 1 THEN CLC.TCKNO ELSE CLC.TAXNR END " +
                    ",ContactName = CLC.INCHARGE " +
                    ",Country = CLC.COUNTRY " +
                    ",City = CLC.CITY " +
                    ",District = CASE WHEN CLC.TOWN = '' THEN 'MERKEZ' ELSE CLC.TOWN END " +
                    ",Address = CLC.ADDR1 + CLC.ADDR1 " +
                    ",ZipCode = CLC.POSTCODE " +
                    "FROM LG_" + companyCode + "_CLCARD CLC WHERE ACTIVE = 0 AND SUBSTRING(CLC.CODE,5,2) IN('I.', 'D.', 'M.', 'A.', 'K.', 'B.', 'S.')";

                var result = connect.Query<ClCardDto>(sql).ToList();
                return result;
            }
        }
    }
}
