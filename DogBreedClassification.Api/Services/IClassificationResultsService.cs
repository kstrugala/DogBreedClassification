using DogBreedClassification.Api.DTO;
using DogBreedClassification.Api.Pagination;
using DogBreedClassification.Api.Queries;
using System.Threading.Tasks;

namespace DogBreedClassification.Api.Services
{
    public interface IClassificationResultsService : IService
    {
        Task<PredictionResultDetailsDTO> GetDetailsAsync(int id);
        Task<PagedResult<PredictionResultDTO>> GetResultsPagedAsync(BreedQuery query);

    }
}
