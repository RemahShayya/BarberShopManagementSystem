using AutoMapper;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IArchivedAppointmentService _archivedAppointmentService;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService reviewService, IMapper mapper, IArchivedAppointmentService archivedAppointmentService)
        {
            _reviewService = reviewService;
            _archivedAppointmentService= archivedAppointmentService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(CreatedReviewRequest request)
        {
            // 1. Get appointment (via appointment service OR repo if that's how you do it elsewhere)
            var archivedAppointment = await _archivedAppointmentService.GetByToken(request.Token);

            if (archivedAppointment == null)
                return BadRequest("Invalid token");


            // 3. Business rule: prevent duplicate review
            var allReviews = await _reviewService.GetAllReviews();

            var exists = allReviews.Any(r => r.AppointmentId == archivedAppointment.Id);

            if (exists)
                return BadRequest("Already reviewed");

            // 4. Map entity (controller responsibility in your system)
            var review = _mapper.Map<Review>(request);

            review.AppointmentId = archivedAppointment.Id;
            review.BarberId = archivedAppointment.BarberId;
            review.CustomerId = archivedAppointment.CustomerId;
            review.CreatedAt = DateTime.UtcNow;

            // 5. Service ONLY handles DB
            await _reviewService.AddReview(review);
            await _reviewService.SaveReview(review);

            return Ok();
        }
    }
}
