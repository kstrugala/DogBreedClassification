namespace DogBreedClassification.Api.DTO
{
    public class PredictionResultDTO
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string PredictedLabel { get; set; }
        public float Probability { get; set; }
        public long PredictionExecutionTime { get; set; }
    }
}
