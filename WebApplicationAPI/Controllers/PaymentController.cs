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
            var vnpData = new SortedDictionary<string, string>();

            // ✅ Thu thập tất cả tham số bắt đầu bằng "vnp_" trừ hash
            foreach (var key in Request.Query.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_") &&
                    key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                {
                    vnpData.Add(key, Request.Query[key]);
                }
            }

            // ✅ Tạo chuỗi dữ liệu để ký (KHÔNG encode)
            var signData = string.Join("&", vnpData.Select(kv => $"{kv.Key}={kv.Value}"));

            // ✅ Tạo hash local
            var localHash = HmacSHA512(_config["VnPay:HashSecret"], signData);
            var vnpSecureHash = Request.Query["vnp_SecureHash"].ToString();

            // ✅ Log debug so sánh
            Console.WriteLine("======= VNPAY RETURN DEBUG =======");
            Console.WriteLine("SignData (raw) : " + signData);
            Console.WriteLine("LocalHash      : " + localHash);
            Console.WriteLine("VnPayHash      : " + vnpSecureHash);
            Console.WriteLine("Full Query     : " + string.Join("&", Request.Query.Select(kv => $"{kv.Key}={kv.Value}")));
            Console.WriteLine("==================================");

            // ✅ So sánh chữ ký
            if (localHash == vnpSecureHash)
            {
                if (Request.Query["vnp_ResponseCode"] == "00" &&
                    Request.Query["vnp_TransactionStatus"] == "00")
                {
                    Console.WriteLine("✅ Kết quả: Thanh toán thành công");
                    return Content("✅ Thanh toán thành công!");
                }
                else
                {
                    Console.WriteLine("❌ Kết quả: Thanh toán thất bại - ResponseCode: " + Request.Query["vnp_ResponseCode"]);
                    return Content("❌ Thanh toán thất bại!");
                }
            }

            Console.WriteLine("❌ Kết quả: Sai chữ ký!");
            return Content("❌ Sai chữ ký!");
        }


        //[HttpGet("IPN")]
        //public IActionResult VnPayIpn()
        //{
        //    string rspCode = "99";
        //    string message = "Input data required";

        //    try
        //    {
        //        if (Request.Query.Count > 0)
        //        {
        //            // 1️⃣ Lấy HashSecret từ cấu hình
        //            string vnp_HashSecret = _config["VnPay:HashSecret"];

        //            // 2️⃣ Lưu dữ liệu trả về từ VNPAY vào SortedDictionary (bỏ SecureHash & SecureHashType)
        //            var vnpData = new SortedDictionary<string, string>();
        //            foreach (var key in Request.Query.Keys)
        //            {
        //                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_") &&
        //                    key != "vnp_SecureHash" && key != "vnp_SecureHashType")
        //                {
        //                    vnpData.Add(key, Request.Query[key]);
        //                }
        //            }

        //            // 3️⃣ Tạo signData theo đúng thứ tự Alphabet và encode
        //            var signData = string.Join("&", vnpData.Select(kv =>
        //                $"{kv.Key}={Uri.EscapeDataString(kv.Value).Replace("%20", "+")}"
        //            ));

        //            string localHash = HmacSHA512(vnp_HashSecret, signData);
        //            string vnpSecureHash = Request.Query["vnp_SecureHash"].ToString();

        //            // 🔥 LOG DEBUG (in ra để so sánh)
        //            _logger.LogInformation($"[VNPAY IPN] signData: {signData}");
        //            _logger.LogInformation($"[VNPAY IPN] localHash: {localHash}");
        //            _logger.LogInformation($"[VNPAY IPN] vnp_SecureHash: {vnpSecureHash}");

        //            if (localHash == vnpSecureHash)
        //            {
        //                // 4️⃣ Lấy các tham số chính
        //                long orderId = Convert.ToInt64(Request.Query["vnp_TxnRef"]);
        //                decimal vnp_Amount = Convert.ToDecimal(Request.Query["vnp_Amount"]) / 100;
        //                string vnp_ResponseCode = Request.Query["vnp_ResponseCode"];
        //                string vnp_TransactionStatus = Request.Query["vnp_TransactionStatus"];
        //                string vnp_TransactionNo = Request.Query["vnp_TransactionNo"];

        //                // 5️⃣ Kiểm tra Order trong DB
        //                var order = _orderServices.GetOrderById(orderId);
        //                if (order != null)
        //                {
        //                    if (order.TotalBill == vnp_Amount)
        //                    {
        //                        if (order.PaymentStatus == "Pending")
        //                        {
        //                            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
        //                            {
        //                                // ✅ Thanh toán thành công
        //                                order.PaymentStatus = "Success";
        //                                order.PaymentTranId = vnp_TransactionNo;
        //                                _orderServices.UpdateOrder(order);

        //                                rspCode = "00";
        //                                message = "Confirm Success";
        //                            }
        //                            else
        //                            {
        //                                // ❌ Thanh toán thất bại
        //                                order.PaymentStatus = "Failed";
        //                                _orderServices.UpdateOrder(order);

        //                                rspCode = "00"; // VNPAY vẫn yêu cầu trả về 00 nếu đã xử lý xong
        //                                message = "Confirm Success (Payment Failed)";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            rspCode = "02";
        //                            message = "Order already confirmed";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        rspCode = "04";
        //                        message = "Invalid amount";
        //                    }
        //                }
        //                else
        //                {
        //                    rspCode = "01";
        //                    message = "Order not found";
        //                }
        //            }
        //            else
        //            {
        //                rspCode = "97";
        //                message = "Invalid signature";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        rspCode = "99";
        //        message = $"Unknown error: {ex.Message}";
        //        _logger.LogError(ex, "[VNPAY IPN] Exception");
        //    }

        //    // 6️⃣ Trả kết quả JSON đúng chuẩn VNPAY
        //    var response = new { RspCode = rspCode, Message = message };
        //    return Ok(response);
        //}

        // HÀM HMAC512
        private string HmacSHA512(string key, string inputData)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                return string.Concat(hashValue.Select(b => b.ToString("x2"))).ToUpper();
            }
        }


        //private string HmacSHA512(string key, string inputData)
        //{
        //    using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
        //    {
        //        byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
        //        return BitConverter.ToString(hashValue).Replace("-", "").ToUpper();
        //    }
        //}


    }

}
