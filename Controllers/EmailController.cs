using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        [HttpPost("send")]
        public IActionResult SendMail([FromBody] BookingEmailRequest request)
        {
            try
            {
                var fromAddress = new MailAddress("akashkce123@gmail.com", "VK Drop Taxi");
                const string ownerEmail = "akashkce123@gmail.com"; // ✅ Owner email
                const string fromPassword = "eqqg lfuu qvkl cqyo";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                string subject = $"Booking Confirmation - {request.BookingId}";
                string body = $@"
Hello {request.CustomerName},

Thank you for booking with VK Drop Taxi 🚖

📅 Booking ID: {request.BookingId}
👤 Name: {request.CustomerName}
📞 Phone: {request.CustomerPhone}
📧 Email: {request.CustomerEmail}

Pickup: {request.PickupLocation}
Drop: {request.DropLocation}
Vehicle: {request.VehicleType}
Trip Type: {request.TripType} ({request.TripMode})
Date & Time: {request.Date} {request.Time}

💰 Estimated Fare: ₹{request.EstimatedFare}
{(request.ActualFare.HasValue ? $"💳 Final Fare: ₹{request.ActualFare}" : "")}
Payment Status: {request.PaymentStatus} {(!string.IsNullOrEmpty(request.PaymentMethod) ? $"({request.PaymentMethod})" : "")}

Driver Details:
{(string.IsNullOrEmpty(request.DriverName) ? "Driver not yet assigned" :
         $"👨 Driver: {request.DriverName}\n📞 Phone: {request.DriverPhone}\n🚘 Vehicle No: {request.VehicleNumber}")}

Notes: {request.Notes ?? "None"}

Status: {request.Status}
Booking Date: {request.BookingDate}

We will keep you updated. 
Regards,  
VK Drop Taxi
";

                using (var message = new MailMessage()
                {
                    From = fromAddress,
                    Subject = subject,
                    Body = body
                })
                {
                    // ✅ Send to customer
                    message.To.Add(request.CustomerEmail);

                    // ✅ Send to owner also
                    message.To.Add(ownerEmail);

                    smtp.Send(message);
                }

                return Ok(new { success = true, message = "Email sent to both customer and owner successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to send email", error = ex.Message });
            }
        }
    }

    // DTO
    public class BookingEmailRequest
    {
        public string BookingId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DropLocation { get; set; } = string.Empty;
        public string TripType { get; set; } = string.Empty;
        public string TripMode { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public decimal EstimatedFare { get; set; }
        public decimal? ActualFare { get; set; }
        public string Status { get; set; } = "pending";
        public string? DriverId { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPhone { get; set; }
        public string? VehicleNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Notes { get; set; }
        public string PaymentStatus { get; set; } = "pending";
        public string? PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
