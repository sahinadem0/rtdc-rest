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

                var sql = " SELECT dataSourceCode =  CASE WHEN SUBSTRING(CLC.CODE,5,1) IN ('I','D','M') THEN 'AYKIZM' " +
                    " WHEN SUBSTRING(CLC.CODE,5,1) IN ('A') THEN 'AYKANT'  WHEN SUBSTRING(CLC.CODE,5,1) IN ('K') THEN 'AYKKNY' " +
                    " WHEN SUBSTRING(CLC.CODE,5,1) IN ('B') THEN 'AYKIST'  ELSE 'TANIMSIZ' END, " +
                    "  retailerCode = clc.code,  retailerRefId = clc.LOGICALREF, channelCode = 'HFS', title = clc.DEFINITION_, " +
                    " email = clc.EMAILADDR, phone = clc.TELNRS1, taxOffice = clc.TAXOFFICE, taxNumber = clc.TAXNR, contactName = clc.INCHARGE, " +
                    " country = clc.COUNTRY, city = clc.CITY, district = clc.DISTRICT, aaddress = clc. ADDR1, " +
                    " zipCode = clc.POSTCODE  " +
                    " FROM LG_001_CLCARD CLC WHERE ACTIVE = 0 " +
                    " AND CLC.CODE NOT LIKE '-%' " +
                    " AND CLC.CODE NOT LIKE '0%' " +
                    " AND CLC.CODE NOT LIKE '1%' " +
                    " AND CLC.CODE NOT LIKE '2%' " +
                    " AND CLC.CODE NOT LIKE '3%' " +
                    " AND CLC.CODE NOT LIKE '4%' " +
                    " AND CLC.CODE NOT LIKE '5%' " +
                    " AND CLC.CODE NOT LIKE '6%' " +
                    " AND CLC.CODE NOT LIKE 'DC%' " +
                    " AND CLC.CODE NOT LIKE 'V%' " +
                    " AND CLC.ACTIVE=0 ";


                //sql = " select * from LG_001_CLCARD ";
                var result = connection.Query<ClCardDto>(sql).ToList();
                return result;
            }
        }
    }
}
