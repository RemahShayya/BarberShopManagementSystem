using AutoMapper;
using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles ="Customer")]
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
            review.EmployeeId = archivedAppointment.EmployeeId;
            review.CustomerId = archivedAppointment.CustomerId;
            review.CreatedAt = DateTime.UtcNow;

            // 5. Service ONLY handles DB
            await _reviewService.AddReview(review);
            await _reviewService.SaveReview(review);

            return Ok();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReview(Guid id)
        {
            var review = await _reviewService.GetReviewById(id);
            if (review == null)
                return NotFound();

            var dto = _mapper.Map<ReviewDTO>(review);
            return Ok(dto);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string employeeId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var reviews = await _reviewService.GetAllReviews();

            if (!string.IsNullOrEmpty(employeeId))
                reviews = reviews.Where(r => r.EmployeeId == employeeId);

            var paginated = reviews
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dto = _mapper.Map<IEnumerable<ReviewDTO>>(paginated);
            return Ok(dto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateReview(Guid id, [FromBody] CreatedReviewRequest request)
        {
            var review = await _reviewService.GetReviewById(id);
            if (review == null)
                return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.CustomerId != currentUserId)
                return Forbid();

            review.Rating = request.Rating;
            review.Comment = request.Comment;

            _reviewService.UpdateReview(review);
            await _reviewService.SaveReview(review);

            return Ok();
        }
    }
}
