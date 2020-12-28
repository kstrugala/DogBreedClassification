using DogBreedClassification.Api.Queries;
using DogBreedClassification.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DogBreedClassification.Api.Controllers
{
    [ApiController]
    public class ClassificationResultsController : ControllerBase
    {
        private readonly IClassificationResultsService _classificationResultsService;

        public ClassificationResultsController(IClassificationResultsService classificationResultsService)
        {
            _classificationResultsService = classificationResultsService;
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("api/ClassificationResults")]
        public async Task<IActionResult> GetAll([FromQuery] BreedQuery query)
        {
            var result = await _classificationResultsService.GetResultsPagedAsync(query);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("api/ClassificationResults/{id}")]
        public async Task<IActionResult> GetPredictionDetails(int id)
        {
            try
            {
                var result = await _classificationResultsService.GetDetailsAsync(id);
                return Ok(result);
            }
            catch(InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}