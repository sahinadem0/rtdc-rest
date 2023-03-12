using Dapper;
using Microsoft.AspNetCore.Hosting.Server;
using rtdc_rest.api.Models;
using rtdc_rest.api.Models.Dtos;
using rtdc_rest.api.Services.Abstract;
using System.Data.SqlClient;
using rtdc_rest.api.config;
using System.Data;

namespace rtdc_rest.api.Services.Concrete
{
    public class ClCardManager : IClCardService
    {
        public async Task<List<ClCardDto>> GetClCardListAsync()
        {
            string connection = Configuration.getLogoConnection();

            #region commentOUT
            //using (var connection = new SqlConnection("Server =172.16.40.20; Database = AYK2008; User ID = PG; Password = PG2007"))
            //Server = 172.16.40.20; Database = AYK2008; Persist Security Info = True; User ID = PG; Password = PG2007
            #endregion

            {
                SqlConnection connect = new SqlConnection(connection);
                connect.Open();

                var sql = " SELECT DataSourceCode = CASE WHEN SUBSTRING(CLC.CODE,5,1) IN('I', 'D', 'M') THEN 'AYKIZM' " +
                    "WHEN SUBSTRING(CLC.CODE,5,1) IN('A') THEN 'AYKANT' " +
                    "WHEN SUBSTRING(CLC.CODE,5,1) IN('K') THEN 'AYKKNY' " +
                    "WHEN SUBSTRING(CLC.CODE,5,1) IN('B') THEN 'AYKIST' ELSE 'TANIMSIZ' END " +
                    ",RetailerCode = clc.code " +
                    ",RetailerRefId = clc.LOGICALREF " +
                    ",ChannelCode = 'HFS' " +
                    ",Title = clc.DEFINITION_ " +
                    ",Email = clc.EMAILADDR " +
                    ",Phone = clc.TELNRS1 " +
                    ",TaxOffice = clc.TAXOFFICE " +
                    ",TaxNumber = clc.TAXNR " +
                    ",ContactName = clc.INCHARGE " +
                    ",Country = clc.COUNTRY" +
                    ",City = clc.CITY " +
                    ",District = clc.DISTRICT " +
                    ",Address = clc.ADDR1 + clc.ADDR1 " +
                    ",ZipCode = clc.POSTCODE " +
                    "FROM LG_412_CLCARD CLC WHERE ACTIVE = 0 " +
                    "AND SUBSTRING(CLC.CODE,1,1) NOT IN('-','0','1','2','3','4','5','6','V') " +
                    "AND NOT(CLC.CODE LIKE 'DC%' ) " +
                    "AND CLC.TAXNR IS NOT NULL AND CLC.TAXNR<>'' AND CLC.DISTRICT IS NOT NULL AND CLC.DISTRICT<>'' ";

                var result = connect.Query<ClCardDto>(sql).ToList();
                return result;
            }
        }
    }
}
