﻿using rtdc_rest.api.Models.Dtos;

namespace rtdc_rest.api.Services.Abstract
{
    public interface IStockFlService
    {
        Task<List<StockFlDto>> GetStockFlListAsync();
    }
}