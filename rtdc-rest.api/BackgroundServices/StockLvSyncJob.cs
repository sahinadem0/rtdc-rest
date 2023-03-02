using rtdc_rest.api.Helpers;
using rtdc_rest.api.Models;
using rtdc_rest.api.Services.Abstract;
using System.Text.Json;

namespace rtdc_rest.api.BackgroundServices
{
    public class StockLvSyncJob : BackgroundService
    {
        public IServiceProvider _service { get; }
        public StockLvSyncJob(IServiceProvider service)
        {
            _service = service;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //var token = GetToken();
                // await Task.Delay(1000 * 60, stoppingToken);
                try
                {
                    using (var scope = _service.CreateScope())
                    {
                        var clCardService = scope.ServiceProvider.GetRequiredService<IClCardService>();

                        var clCards = await clCardService.GetClCardListAsync();


                        foreach (var clcard in clCards)
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
                            createRetailerReqJson.taxNumber = clcard.TaxNumber;
                            createRetailerReqJson.contactName = clcard.ContactName;
                            createRetailerReqJson.country = clcard.Country;
                            createRetailerReqJson.city = clcard.City;
                            createRetailerReqJson.district = clcard.District;
                            createRetailerReqJson.address = clcard.Address;
                            createRetailerReqJson.zipCode = clcard.ZipCode;

                            string retailerJsonString = JsonSerializer.Serialize(createRetailerReqJson);

                            HttpClientHelper httpClientHelper = new();

                            httpClientHelper.SendPOSTRequest("username", "pasword", "endpoint", "postdata");

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
