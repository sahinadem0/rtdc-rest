using rtdc_rest.api.Helpers;
using rtdc_rest.api.Models;
using rtdc_rest.api.Services.Abstract;
using System.Text.Json;


namespace rtdc_rest.api.BackgroundServices
{
    public class ClCardSyncJob : BackgroundService
    {
        public IServiceProvider _service { get; }
        private readonly IConfiguration _configuration;
        public ClCardSyncJob(IServiceProvider service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _service.CreateScope())
                    {
                        string apiUserName = _configuration.GetSection("AppSettings:ApiUserName").Value;
                        string apiPassword = _configuration.GetSection("AppSettings:ApiPassword").Value;
                        string retailer = _configuration.GetSection("AppSettings:Retailer").Value;

                        var clCardService = scope.ServiceProvider.GetRequiredService<IClCardService>();
                        var clCards = await clCardService.GetClCardListAsync();
                        var grouppedClcArdList = clCards.GroupBy(g => g.DataSourceCode).ToList();

                        foreach (var grouppedClcard in grouppedClcArdList)
                        {
                            List<CreateRetailerReqJson> clcardList = new();
                            foreach (var clcard in grouppedClcard)
                            {
                                CreateRetailerReqJson createRetailerReqJson = new();

                                createRetailerReqJson.dataSourceCode = clcard.DataSourceCode;
                                createRetailerReqJson.retailerCode = clcard.RetailerCode;
                                createRetailerReqJson.retailerRefId = clcard.RetailerRefId;
                                createRetailerReqJson.channelCode = clcard.ChannelCode;
                                createRetailerReqJson.title = clcard.Title;
                                createRetailerReqJson.email = clcard.Email;
                                createRetailerReqJson.Phone = clcard.Phone;
                                createRetailerReqJson.taxOffice = clcard.TaxOffice;
                                createRetailerReqJson.taxNumber = string.IsNullOrEmpty(clcard.TaxNumber) ? 0 : long.Parse(clcard.TaxNumber);
                                createRetailerReqJson.contactName = clcard.ContactName;
                                createRetailerReqJson.country = clcard.Country;
                                createRetailerReqJson.city = clcard.City;
                                createRetailerReqJson.district = clcard.District;
                                createRetailerReqJson.address = clcard.Address;
                                createRetailerReqJson.zipCode = string.IsNullOrEmpty(clcard.ZipCode) ? 0 : int.Parse(clcard.ZipCode);

                                clcardList.Add(createRetailerReqJson);
                            }

                            string retailerJsonString = JsonSerializer.Serialize(clcardList);

                            HttpClientHelper httpClientHelper = new(_configuration);

                          var response = httpClientHelper.SendPOSTRequest(apiUserName.ToString(), apiPassword.ToString(), retailer.ToString(), retailerJsonString);
                        }
                       
                        await Task.Delay(1000 * 60, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    await Task.FromCanceled(stoppingToken);
                }
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
