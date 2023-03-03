using rtdc_rest.api.Helpers;
using rtdc_rest.api.Models;
using rtdc_rest.api.Services.Abstract;
using System.Linq;
using System.Text.Json;

namespace rtdc_rest.api.BackgroundServices
{
    public class ClCardSyncJob : BackgroundService
    {
        public IServiceProvider _service { get; }
        public ClCardSyncJob(IServiceProvider service)
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
                try
                {
                    using (var scope = _service.CreateScope())
                    {
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

                            HttpClientHelper httpClientHelper = new();

                            var response = httpClientHelper.SendPOSTRequest("aykanlar", "AyKanLar&2023", "/Retailers", retailerJsonString);
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
