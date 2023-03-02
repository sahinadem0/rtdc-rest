using Dapper;
using rtdc_rest.api.Models;
using rtdc_rest.api.Models.Dtos;
using rtdc_rest.api.Services.Abstract;
using System.Data.SqlClient;

namespace rtdc_rest.api.Services.Concrete
{
    public class ClCardManager : IClCardService
    {
        public async Task<List<ClCardDto>> GetClCardListAsync()
        {
            using (var connection = new SqlConnection(" Server =.; Database = tiger3; Trusted_Connection = True; MultipleActiveResultSets = true"))
{
                connection.Open();

                var sql = " SELECT DataSourceCode = CASE WHEN SUBSTRING(CLC.CODE,5,1) IN('I', 'D', 'M') THEN 'AYKIZM' " +
                    "WHEN SUBSTRING(CLC.CODE,5,1) IN('A') THEN 'AYKANT' " +
                    "WHEN SUBSTRING(CLC.CODE,5,1) IN('K') THEN 'AYKKNY' " +
                    "WHEN SUBSTRING(CLC.CODE,5,1) IN('B') THEN 'AYKIST' ELSE 'TANIMSIZ' END" +
                    ",RetailerCode = clc.code" +
                    ",RetailerRefId = clc.LOGICALREF" +
                    ",ChannelCode = 'HFS'" +
                    ",Title = clc.DEFINITION_" +
                    ",Email = clc.EMAILADDR" +
                    ",Phone = clc.TELNRS1" +
                    ",TaxOffice = clc.TAXOFFICE" +
                    ",TaxNumber = clc.TAXNR" +
                    ",ContactName = clc.INCHARGE" +
                    ",Country = clc.COUNTRY" +
                    ",City = clc.CITY" +
                    ",District = clc.DISTRICT" +
                    ",Address = clc.ADDR1 + clc.ADDR1" +
                    ",ZipCode = clc.POSTCODE" +
                    "FROM LG_412_CLCARD CLC WHERE ACTIVE = 0" +
                    "AND SUBSTRING(CLC.CODE,1,1) NOT IN('-','0','1','2','3','4','5','6','V') " +
                    "AND NOT(CLC.CODE LIKE 'DC%' )";

                //sql = " select * from LG_001_CLCARD ";
                var result = connection.Query<ClCardDto>(sql).ToList();
                return result;
            }
        }
    }
}
