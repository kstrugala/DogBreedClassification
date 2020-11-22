using Microsoft.AspNetCore.Identity;
using System;
using System.Text.RegularExpressions;

namespace DogBreedClassification.Api.Models
{
    public class User
    {
        private static readonly Regex EmailRegex = new Regex(
           @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
           @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
           RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public int Id { get; set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Role { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        protected User(Guid guid) 
        { 
        }

        public User(string email, string role)
        {
            if (!EmailRegex.IsMatch(email))
            {
                throw new Exception($"Invalid email: {email}");
            }

            if (!Models.Role.IsValid(role))
            {
                throw new Exception($"Invalid role: {role}");
            }

            Email = email;
            Role = role.ToLowerInvariant();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public User(int id, string email, string role)
        {
            if(!EmailRegex.IsMatch(email))
            {
                throw new Exception($"Invalid email: {email}");
            }

            if (!Models.Role.IsValid(role))
            {
                throw new Exception($"Invalid role: {role}");
            }

            Id = id;
            Email = email;
            Role = role.ToLowerInvariant();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new Exception("First name cannot be empty.");
            }
            FirstName = firstName;
        }

        public void SetLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new Exception("Last name cannot be emnty.");
            }
            LastName = lastName;
        }

        public void SetPassword(string password, IPasswordHasher<User> passwordHasher)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception($"Password cannot be empty.");
            }
            PasswordHash = passwordHasher.HashPassword(this, password);
        }

        public bool ValidatePassword(string password, IPasswordHasher<User> passwordHasher)
            => passwordHasher.VerifyHashedPassword(this, PasswordHash, password) != PasswordVerificationResult.Failed;
    }
}
