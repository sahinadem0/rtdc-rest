using rtdc_rest.api.Helpers;
using rtdc_rest.api.Models;
using rtdc_rest.api.Services.Abstract;
using System.Linq;
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
                try
                {
                    using (var scope = _service.CreateScope())
                    {
                        var stockLvService = scope.ServiceProvider.GetRequiredService<IStockLvService>();
                        var stockLvs = await stockLvService.GetStockLvListAsync();
                        var grouppedStockLvList = stockLvs.GroupBy(g => g.dataSourceCode).ToList();

                        foreach (var grouppedStockLv in grouppedStockLvList)
                        {
                            List<CreateStockLevelReqJson> stockLvList = new();
                            foreach (var stockLv in grouppedStockLv)
                            {
                                CreateStockLevelReqJson createStockLevelReqJson = new();

                                createStockLevelReqJson.dataSourceCode = stockLv.dataSourceCode;
                                createStockLevelReqJson.manufacturerCode = stockLv.manufacturerCode;
                                createStockLevelReqJson.stockDate = stockLv.stockDate;
                                createStockLevelReqJson.productCode = stockLv.productCode;
                                createStockLevelReqJson.itemQuantity = stockLv.itemQuantity;
                                createStockLevelReqJson.quantityInPackage = stockLv.quantityInPackage;
                                createStockLevelReqJson.packageQuantity = stockLv.packageQuantity;
                                createStockLevelReqJson.itemBarcode = stockLv.itemBarcode;
                                createStockLevelReqJson.packageBarcode = stockLv.packageBarcode;
                                createStockLevelReqJson.listPrice = stockLv.listPrice;

                                stockLvList.Add(createStockLevelReqJson);
                            }

                            string retailerJsonString = JsonSerializer.Serialize(stockLvList);
                            HttpClientHelper httpClientHelper = new();
                            var response = httpClientHelper.SendPOSTRequest("aykanlar", "AyKanLar&2023", "/StockLevels", retailerJsonString);

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
