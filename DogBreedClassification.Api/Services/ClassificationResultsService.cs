using DogBreedClassification.Api.DTO;
using DogBreedClassification.Api.EF;
using DogBreedClassification.Api.Pagination;
using DogBreedClassification.Api.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DogBreedClassification.Api.Services
{
    public class ClassificationResultsService : IClassificationResultsService
    {
        private readonly DogBreedClassificationContext _context;

        public ClassificationResultsService(DogBreedClassificationContext context)
        {
            _context = context;
        }

        public async Task<PredictionResultDetailsDTO> GetDetailsAsync(int id)
        {
            var result = await _context.PredictionResults
                                    .SingleAsync(x => x.Id == id);

            string imageBase64 = Convert.ToBase64String(result.Image);

            return new PredictionResultDetailsDTO {
                Id = result.Id,
                Filename = result.Filename,
                PredictedLabel = result.PredictedLabel,
                PredictionExecutionTime = result.PredictionExecutionTime,
                Probability = result.Probability,
                Image = imageBase64
            };
        }

        public async Task<PagedResult<PredictionResultDTO>> GetResultsPagedAsync(BreedQuery query)
        {
            var page = query.Page;
            var pageSize = query.PageSize;

            // Filter

            var linqQuery = _context.PredictionResults
                                .Where(x => x.PredictedLabel.Contains(query.Breed));

            var count = await linqQuery.CountAsync();

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            if (page < 1) page = 1;

            if (totalPages == 0) totalPages = 1;

            if (page > totalPages) page = totalPages;

            var results = await linqQuery
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .Select(r => new PredictionResultDTO
                                    {
                                        Id = r.Id,
                                        Filename = r.Filename,
                                        PredictedLabel = r.PredictedLabel,
                                        Probability = r.Probability,
                                        PredictionExecutionTime = r.PredictionExecutionTime
                                    })
                                    .ToListAsync();


            return new PagedResult<PredictionResultDTO>(results, count, page, pageSize, totalPages);
        }
    }
}
