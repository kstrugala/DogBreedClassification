using DogBreedClassification.Api.EF;
using DogBreedClassification.Api.ImageHelpers;
using DogBreedClassification.Api.ML.DataModels;
using DogBreedClassification.Api.Models;
using DogBreedClassification.Shared.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.ML;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DogBreedClassification.Api.Services
{
    public class DogClassificationService : IDogClassificationService
    {
        private readonly DogBreedClassificationContext _context;
        private readonly PredictionEnginePool<InMemoryImageData, ImagePrediction> _predictionEnginePool;

        public DogClassificationService(DogBreedClassificationContext context, 
                PredictionEnginePool<InMemoryImageData, ImagePrediction> predictionEnginePool)
        {
            _context = context;
            _predictionEnginePool = predictionEnginePool;
            
        }
        public async Task<ImagePredictedLabelWithProbability> Classify(IFormFile imageFile)
        {
            var imageMemoryStream = new MemoryStream();
            await imageFile.CopyToAsync(imageMemoryStream);

            byte[] imageData = imageMemoryStream.ToArray();
            if (!imageData.IsValidImage())
                throw new ArgumentOutOfRangeException("Unsupported media type");

            var imageInputData = new InMemoryImageData(image: imageData, label: null, imageFileName: null);


            var watch = System.Diagnostics.Stopwatch.StartNew();

            var prediction = _predictionEnginePool.Predict(imageInputData);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            var predictedValues = new PredictionResult
            {
                Filename = imageFile.FileName,
                Image = imageData,
                PredictedLabel = prediction.PredictedLabel,
                Probability = prediction.Score.Max(),
                PredictionExecutionTime = elapsedMs
            };

            await _context.PredictionResults.AddAsync(predictedValues);
            await _context.SaveChangesAsync();

            var result = new ImagePredictedLabelWithProbability
            {
                PredictedLabel = prediction.PredictedLabel,
                Probability = prediction.Score.Max(),
                PredictionExecutionTime = elapsedMs,
                ImageId = imageFile.FileName,
            };

            return result;
        }
    }
}
