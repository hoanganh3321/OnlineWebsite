using System.Security.Cryptography;
using System.Text;
using ClassLibrarySERVICES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPaymentServices _paymentService;
        private readonly VnPayService _vnPayService;
        private readonly IOrderServices _orderServices;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IPaymentServices paymentService, VnPayService vnPayService, IConfiguration config, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _vnPayService = vnPayService;
            _config = config;
            _logger = logger;
        }

        [HttpPost("create")]
        public IActionResult CreatePayment([FromBody] ClassLibraryDATA.DTO.PaymentRequestDto dto)
        {
            var payment = _paymentService.CreatePendingPayment(dto.OrderId, dto.Amount);
            var paymentUrl = _vnPayService.CreatePaymentUrl(payment, HttpContext);
            return Ok(new { paymentUrl });
        }


        [HttpGet("vnpay-return")]
        public IActionResult VnPayReturn()
        {
            var responseCode = Request.Query["vnp_ResponseCode"];
            var transStatus = Request.Query["vnp_TransactionStatus"];

            string message;
            if (responseCode == "00" && transStatus == "00")
            {
                // _paymentService.UpdatePaymentStatus();
                message = "✅ Thanh toán thành công!";
            }
            else
            {
                message = "❌ Thanh toán thất bại!";
            }

            // Redirect về Razor page kèm message
            return Redirect($"https://localhost:7266/Orders/ViewOrder?message={Uri.EscapeDataString(message)}");
        }


        [HttpGet("vnpay-ipn")]
        public IActionResult VnPayIpn()
        {
            var result = _vnPayService.ProcessIpn(Request.Query);

            if (!result.IsValid)
                return Ok(new { RspCode = result.RspCode, Message = result.Message });

            
             

            return Ok(new { RspCode = result.RspCode, Message = result.Message });
        }
    }

}
