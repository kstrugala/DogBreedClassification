using DogBreedClassification.Api.ML.DataModels;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DogBreedClassification.Api.Services
{
    public interface IDogClassificationService : IService
    {
        Task<ImagePredictedLabelWithProbability> Classify(IFormFile imageFile);
    }
}
