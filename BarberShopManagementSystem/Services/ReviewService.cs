using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;

namespace BarberShopManagementSystem.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IBarberShopGenericRepo<Review> _reviewRepo;
        public ReviewService(IBarberShopGenericRepo<Review> reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public async Task<Review> AddReview(Review review)
        {
            return await _reviewRepo.Insert(review);
        }

        public void DeleteReview(Review review)
        {
            _reviewRepo.Delete(review.Id);
        }

        public async Task<IEnumerable<Review>> GetAllReviews()
        {
            return await _reviewRepo.GetAll();
        }

        public async Task<Review?> GetReviewById(Guid id)
        {
            return await _reviewRepo.GetById(id);
        }

        public async Task SaveReview(Review review)
        {
            await _reviewRepo.SaveAsync();
        }

        public bool UpdateReview(Review review)
        {
            return _reviewRepo.Update(review);
        }
    }
}
