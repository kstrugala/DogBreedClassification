namespace DogBreedClassification.Api.Services
{
    public interface IJwtHandler
    {
        string Create(string email, string role);
    }
}
