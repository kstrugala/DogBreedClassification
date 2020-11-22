using System.Threading.Tasks;

namespace DogBreedClassification.Api.Services
{
    public interface IUserService : IService
    {
        Task SignUp(string email, string password, string firstName, string lastName);
        Task<string> SignIn(string email, string password);
    }
}
