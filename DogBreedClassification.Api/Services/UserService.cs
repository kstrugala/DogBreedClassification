using DogBreedClassification.Api.EF;
using DogBreedClassification.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DogBreedClassification.Api.Services
{
    public class UserService : IUserService
    {
        private readonly DogBreedClassificationContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtHandler _jwtHandler;

        public UserService(DogBreedClassificationContext context, IPasswordHasher<User> passwordHasher, IJwtHandler jwtHandler)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtHandler = jwtHandler;
        }

        public async Task SignUp(string email, string password, string firstName, string lastName)
        {
            var user = await GetUser(email);

            if (user != null)
                throw new ArgumentException( $"User with email:{email} already exists.");

            user = new User(email, Role.User);
            user.SetPassword(password, _passwordHasher);
            user.SetFirstName(firstName);
            user.SetLastName(lastName);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<string> SignIn(string email, string password)
        {
            var user = await GetUser(email);

            if (user == null)
                throw new ArgumentException("Invalid credentials");

            if (!user.ValidatePassword(password, _passwordHasher))
                throw new ArgumentException("Invalid credentials");

            var jwt = _jwtHandler.Create(user.Email, user.Role);

            return jwt;
        }

        private async Task<User> GetUser(string email)
          => await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
    }
}
