using rtdc_rest.api.Helpers;
using rtdc_rest.api.Models;
using rtdc_rest.api.Services.Abstract;
using System.Text.Json;

namespace rtdc_rest.api.BackgroundServices
{
    public class StockFlSyncJob : BackgroundService
    {
        public IServiceProvider _service { get; }
        public StockFlSyncJob(IServiceProvider service)
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
                        var stockFlService = scope.ServiceProvider.GetRequiredService<IStockFlService>();

                        var stockFls = await StockFlService.GetStockFlListAsync();


                        foreach (var stockFl in stockFls)
                        {
                            CreateStockFlowReqJson createStockFlowReqJson = new()

                            CreateStockFlowReqJson.dataSourceCode = StockFl.DataSourceCode;
                            CreateStockFlowReqJson.manufacturerCode = StockFl.manufacturerCode;
                            CreateStockFlowReqJson.retailerCode = StockFl.retailerCode;
                            CreateStockFlowReqJson.retailerRefId = StockFl.retailerRefId;
                            CreateStockFlowReqJson.year = StockFl.year;
                            CreateStockFlowReqJson.month = StockFl.month;
                            CreateStockFlowReqJson.invoiceDate = StockFl.invoiceDate;
                            CreateStockFlowReqJson.invoiceDateSystem = StockFl.invoiceDateSystem;
                            CreateStockFlowReqJson.invoiceId = StockFl.invoiceId;
                            CreateStockFlowReqJson.invoiceNo = StockFl.invoiceNo;
                            CreateStockFlowReqJson.invoiceLine = StockFl.invoiceLine;
                            CreateStockFlowReqJson.productCode = StockFl.productCode;
                            CreateStockFlowReqJson.itemQuantity = StockFl.itemQuantity;
                            CreateStockFlowReqJson.quantityInPackage = StockFl.quantityInPackage;
                            CreateStockFlowReqJson.packageQuantity = StockFl.packageQuantity;
                            CreateStockFlowReqJson.itemBarcode = StockFl.itemBarcode;
                            CreateStockFlowReqJson.packageBarcode = StockFl.packageBarcode;
                            CreateStockFlowReqJson.lineAmount = StockFl.lineAmount;
                            CreateStockFlowReqJson.discountAmount = StockFl.discountAmount;
                            CreateStockFlowReqJson.salesOrderId = StockFl.salesOrderId;
                            CreateStockFlowReqJson.isReturnInvoice = StockFl.isReturnInvoice;

                            string stockFlJsonString = JsonSerializer.Serialize(CreateStockFlowReqJson);

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
