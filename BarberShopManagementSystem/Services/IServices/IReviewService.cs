using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services.IServices
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviews();
        Task<Review?> GetReviewById(Guid id);
        Task<Review> AddReview(Review review);
        void DeleteReview(Review review);
        bool UpdateReview(Review review);
        Task SaveReview(Review review);
    }
}
