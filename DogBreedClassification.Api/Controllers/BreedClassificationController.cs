using DogBreedClassification.Api.ImageHelpers;
using DogBreedClassification.Api.ML.DataModels;
using DogBreedClassification.Api.Services;
using DogBreedClassification.Shared.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DogBreedClassification.Api.Controllers
{
    [Route("api/ClassifyBreed")]
    [ApiController]
    public class BreedClassificationController : ControllerBase
    {
        private readonly IDogClassificationService _dogClassificationService;
        public BreedClassificationController(IDogClassificationService dogClassificationService)
        {
            _dogClassificationService = dogClassificationService;
        }

        [Authorize(Policy = "User")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Route("")]
        public async Task<IActionResult> ClassifyBreed([FromForm] IFormFile imageFile)
        {
            if (imageFile.Length == 0)
                return BadRequest();
        
            try
            {
                return Ok(await _dogClassificationService.Classify(imageFile));
            }
            catch (ArgumentOutOfRangeException)
            {
                return StatusCode(StatusCodes.Status415UnsupportedMediaType);
            }
        }
    }
}
