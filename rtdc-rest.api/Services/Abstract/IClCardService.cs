using rtdc_rest.api.Models;
using rtdc_rest.api.Models.Dtos;
using System.Threading.Tasks;

namespace rtdc_rest.api.Services.Abstract
{
    public interface IClCardService
    {
        Task<List<ClCardDto>> GetClCardListAsync();
    }
}
