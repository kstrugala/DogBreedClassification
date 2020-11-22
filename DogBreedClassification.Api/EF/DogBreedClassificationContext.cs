using DogBreedClassification.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DogBreedClassification.Api.EF
{
    public class DogBreedClassificationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<PredictionResult> PredictionResults { get; set; }

        public DogBreedClassificationContext(DbContextOptions<DogBreedClassificationContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var userBuilder = modelBuilder.Entity<User>();
            userBuilder.HasKey(x => x.Id);

            var predictionResultBuilder = modelBuilder.Entity<PredictionResult>();
            predictionResultBuilder.HasKey(x => x.Id);
        }
    }
}
