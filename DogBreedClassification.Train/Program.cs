using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common;
using DogBreedClassification.Shared;
using DogBreedClassification.Shared.DataModels;
using DogBreedClassification.Train.DataModels;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using Microsoft.ML.Vision;
using static Microsoft.ML.Transforms.ValueToKeyMappingEstimator;

namespace DogBreedClassification.Train
{
    class Program
    {
        static void Main(string[] args)
        {
            const string assetsRelativePath = @"../../../assets";
            string assetsPath = GetAbsolutePath(assetsRelativePath);

            string outputMlNetModelFilePath = Path.Combine(assetsPath, "outputs", "imageClassifier.zip");
            string datasetPath = Path.Combine(assetsPath, "dataset");

            // Create MLContext
            var mlContext = new MLContext(seed: 1);

            // Specify MLContext Filter to only show feedback log/traces about ImageClassification
            // This is not needed for feedback output if using the explicit MetricsCallback parameter
            mlContext.Log += FilterMLContextLog;

            // Load the initial full image-set into an IDataView and shuffle so it'll be better balanced

            IEnumerable<ImageData> images = LoadImagesFromDirectory(folder: datasetPath, useFolderNameAsLabel: true);
            IDataView fullImagesDataset = mlContext.Data.LoadFromEnumerable(images);
            IDataView shuffledFullImageFilePathsDataset = mlContext.Data.ShuffleRows(fullImagesDataset);

            // Load Images with in-memory type within the IDataView and Transform Labels to Keys (Categorical)
            IDataView shuffledFullImagesDataset = mlContext.Transforms.Conversion.
                   MapValueToKey(outputColumnName: "LabelAsKey", inputColumnName: "Label", keyOrdinality: KeyOrdinality.ByValue)
                           .Append(mlContext.Transforms.LoadRawImageBytes(
                                                           outputColumnName: "Image",
                                                           imageFolder: datasetPath,
                                                           inputColumnName: "ImagePath"))
                           .Fit(shuffledFullImageFilePathsDataset)
                           .Transform(shuffledFullImageFilePathsDataset);

            // Split the data 80:20 into train and test sets, train and evaluate

            var trainTestData = mlContext.Data.TrainTestSplit(shuffledFullImagesDataset, testFraction: 0.2);
            IDataView trainDataView = trainTestData.TrainSet;
            IDataView testDataView = trainTestData.TestSet;

            // Define the model's training pipeline using DNN default values

            var pipeline = mlContext.MulticlassClassification.Trainers
                    .ImageClassification(featureColumnName: "Image",
                                         labelColumnName: "LabelAsKey",
                                         validationSet: testDataView)
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: "PredictedLabel",
                                                                      inputColumnName: "PredictedLabel"));
            // Train/create the ML model

            Console.WriteLine("*** Training the image classification model with DNN Transfer Learning on top of the selected pre-trained model/architecture ***");

            // Measuring training time
            var watch = Stopwatch.StartNew();

            //Train
            ITransformer trainedModel = pipeline.Fit(trainDataView);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine($"Training with transfer learning took: {elapsedMs / 1000} seconds");

            // Get the quality metrics (accuracy, etc.)
            EvaluateModel(mlContext, testDataView, trainedModel);

            // Save the model to assets/outputs (You get ML.NET .zip model file and TensorFlow .pb model file)
            mlContext.Model.Save(trainedModel, trainDataView.Schema, outputMlNetModelFilePath);
            Console.WriteLine($"Model saved to: {outputMlNetModelFilePath}");


            Console.WriteLine("Press any key to finish");
            Console.ReadKey();
        }

        private static void EvaluateModel(MLContext mlContext, IDataView testDataset, ITransformer trainedModel)
        {
            Console.WriteLine("Making predictions in bulk for evaluating model's quality...");

            // Measuring time
            var watch = Stopwatch.StartNew();

            var predictionsDataView = trainedModel.Transform(testDataset);

            var metrics = mlContext.MulticlassClassification.Evaluate(predictionsDataView, labelColumnName: "LabelAsKey", predictedLabelColumnName: "PredictedLabel");
            ConsoleHelper.PrintMultiClassClassificationMetrics("TensorFlow DNN Transfer Learning", metrics);

            watch.Stop();
            var elapsed2Ms = watch.ElapsedMilliseconds;

            Console.WriteLine($"Predicting and Evaluation took: {elapsed2Ms / 1000} seconds");
        }

        private static void TrySinglePrediction(string imagesFolderPathForPredictions, MLContext mlContext, ITransformer trainedModel)
        {
            // Create prediction function to try one prediction
            var predictionEngine = mlContext.Model
                .CreatePredictionEngine<InMemoryImageData, ImagePrediction>(trainedModel);

            var testImages = FileUtils.LoadInMemoryImagesFromDirectory(
                imagesFolderPathForPredictions, false);

            var imageToPredict = testImages.First();

            var prediction = predictionEngine.Predict(imageToPredict);

            Console.WriteLine(
                $"Image Filename : [{imageToPredict.ImageFileName}], " +
                $"Scores : [{string.Join(",", prediction.Score)}], " +
                $"Predicted Label : {prediction.PredictedLabel}");
        }

        public static string GetAbsolutePath(string relativePath)
          => FileUtils.GetAbsolutePath(typeof(Program).Assembly, relativePath);

        public static IEnumerable<ImageData> LoadImagesFromDirectory(
          string folder,
          bool useFolderNameAsLabel = true)
          => FileUtils.LoadImagesFromDirectory(folder, useFolderNameAsLabel)
              .Select(x => new ImageData(x.imagePath, x.label));


        private static void FilterMLContextLog(object sender, LoggingEventArgs e)
        {
            if (e.Message.StartsWith("[Source=ImageClassificationTrainer;"))
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
