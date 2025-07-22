using System.Security.Cryptography;
using System.Text;
using System.Web;
using ClassLibraryDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ClassLibrarySERVICES
{
    public class VnPayService
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }

        
        private string UrlEncodeVNPay(string value)
        {
            return HttpUtility.UrlEncode(value, Encoding.UTF8)
                              .Replace("%20", "+")
                              .Replace("%3a", "%3A")
                              .Replace("%2f", "%2F"); // Viết hoa HEX
        }

        /// ✅ Tạo URL thanh toán VNPay
        public string CreatePaymentUrl(Payment payment, HttpContext context, string? bankCode = null)
        {
            string ip = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            if (ip == "::1") ip = "127.0.0.1";

            var vnpParams = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _config["VnPay:TmnCode"] ?? "" },
                { "vnp_Amount", ((long)(payment.AmountPaid * 100)).ToString() },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", ip },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", $"Thanh toan don hang {payment.OrderId}" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", _config["VnPay:ReturnUrl"] ?? "" },
                { "vnp_TxnRef", payment.VnpTxnRef }
            };

            if (!string.IsNullOrEmpty(bankCode))
            {
                vnpParams.Add("vnp_BankCode", bankCode);
            }

            // ✅ Tạo signData (encode chuẩn)
            string signData = string.Join("&", vnpParams
                .Select(kv => $"{kv.Key}={UrlEncodeVNPay(kv.Value)}"));

            // ✅ Secure Hash phải viết thường (VNPay mẫu trả về lowercase)
            string secureHash = HmacSHA512(_config["VnPay:HashSecret"] ?? "", signData).ToLower();

            // ✅ URL thanh toán
            string paymentUrl = $"{_config["VnPay:VnpUrl"]}?{signData}&vnp_SecureHash={secureHash}";

            Console.WriteLine("====== VNPay Request ======");
            Console.WriteLine("SignData  : " + signData);
            Console.WriteLine("SecureHash: " + secureHash);
            Console.WriteLine("Final URL : " + paymentUrl);
            Console.WriteLine("===========================");

            return paymentUrl;
        }

        /// ✅ Xác thực phản hồi từ VNPay
        public bool ValidateResponse(IQueryCollection query)
        {
            var vnp_HashSecret = _config["VnPay:HashSecret"];

            var vnp_Params = query
                .Where(x =>
                    x.Key.StartsWith("vnp_") &&
                    x.Key != "vnp_SecureHash" &&
                    x.Key != "vnp_SecureHashType")
                .ToDictionary(k => k.Key, v => v.Value.ToString());

            var sorted = new SortedDictionary<string, string>(vnp_Params);

            string signData = string.Join("&", sorted
                .Select(kv => $"{kv.Key}={UrlEncodeVNPay(kv.Value)}"));

            string secureHash = HmacSHA512(vnp_HashSecret, signData).ToLower();

            Console.WriteLine("====== VNPay Return Debug ======");
            Console.WriteLine("SignData    : " + signData);
            Console.WriteLine("Local Hash  : " + secureHash);
            Console.WriteLine("VnPay Hash  : " + query["vnp_SecureHash"]);
            Console.WriteLine("===============================");

            return secureHash == query["vnp_SecureHash"];
        }

        /// ✅ HMAC-SHA512
        private string HmacSHA512(string key, string inputData)
        {
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower(); // trả về lowercase
        }
    }
}
