using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PCM.Core.Interfaces;
using System.Security.Claims;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly PCM.Application.Services.BookingService _bookingService;

        public BookingsController(PCM.Application.Services.BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Get available time slots for a court
        /// </summary>
        [HttpGet("slots/{courtId}/{date}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableSlots(int courtId, [FromRoute] string date)
        {
            try
            {
                if (!DateTime.TryParse(date, out var bookingDate))
                    return BadRequest("Định dạng ngày không hợp lệ");

                // TODO: Implement booking slots logic
                return Ok(new { message = "Not implemented yet" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a booking
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            try
            {
                var memberId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                if (memberId == 0)
                    return Unauthorized();

                var booking = await _bookingService.CreateBookingAsync(
                    memberId,
                    request.CourtId,
                    request.BookingDate,
                    request.StartTime,
                    request.EndTime,
                    request.NumberOfPlayers,
                    request.Price
                );

                if (booking == null)
                    return BadRequest("Không thể tạo lịch đặt sân");

                return Ok(new { id = booking.Id, message = "Đặt sân thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create recurring booking
        /// </summary>
        [HttpPost("create-recurring")]
        public async Task<IActionResult> CreateRecurringBooking([FromBody] CreateRecurringBookingRequest request)
        {
            try
            {
                var memberId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                if (memberId == 0)
                    return Unauthorized();

                var bookings = await _bookingService.CreateRecurringBookingAsync(
                    memberId,
                    request.CourtId,
                    request.StartDate,
                    request.StartTime,
                    request.EndTime,
                    request.NumberOfPlayers,
                    request.PricePerSession,
                    request.WeekCount,
                    request.DaysOfWeek ?? new List<int>(),
                    request.SkipConflicts
                );

                return Ok(new { count = bookings.Count(), message = $"Đã tạo {bookings.Count()} buổi đặt sân" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancel a booking
        /// </summary>
        [HttpPost("cancel/{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId, [FromBody] CancelBookingRequest request)
        {
            try
            {
                await _bookingService.CancelBookingAsync(bookingId, request.Reason ?? "");
                return Ok("Hủy đặt sân thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Check in to a booking
        /// </summary>
        [HttpPost("check-in/{bookingId}")]
        public async Task<IActionResult> CheckIn(int bookingId)
        {
            try
            {
                await _bookingService.CheckInAsync(bookingId);
                return Ok("Check-in thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get member's bookings
        /// </summary>
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var memberId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                if (memberId == 0)
                    return Unauthorized();

                var bookings = await _bookingService.GetMemberBookingsAsync(memberId, from, to);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get upcoming bookings
        /// </summary>
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingBookings()
        {
            try
            {
                var memberId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                if (memberId == 0)
                    return Unauthorized();

                var bookings = await _bookingService.GetUpcomingBookingsAsync(memberId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CreateBookingRequest
    {
        public int CourtId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int NumberOfPlayers { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateRecurringBookingRequest
    {
        public int CourtId { get; set; }
        public DateTime StartDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int NumberOfPlayers { get; set; }
        public decimal PricePerSession { get; set; }
        public int WeekCount { get; set; }
        public List<int>? DaysOfWeek { get; set; }
        public bool SkipConflicts { get; set; }
    }

    public class CancelBookingRequest
    {
        public string? Reason { get; set; }
    }
}
