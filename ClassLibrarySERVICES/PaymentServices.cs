using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryREPO;

namespace ClassLibrarySERVICES
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IPaymentRepositories _paymentRepo;
        public PaymentServices(IPaymentRepositories paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

        public Payment CreatePendingPayment(int orderId, decimal amount)
        {
            var payment = new Payment
            {
                OrderId = orderId,
                AmountPaid = amount,
                PaymentStatus = "Pending",
                PaymentDate = DateTime.Now,
                PaymentMethod = "VNPay",
               // TransactionNo =,
                VnpTxnRef = DateTime.Now.Ticks.ToString()
            };

            _paymentRepo.Add(payment);
            _paymentRepo.Save();
            return payment;
        }

        public void UpdatePaymentFail(string vnpTxnRef)
        {
            var payment = _paymentRepo.GetByVnpTxnRef(vnpTxnRef);
            if (payment != null)
            {
                payment.PaymentStatus = "Fail";
                _paymentRepo.Update(payment);
                _paymentRepo.Save();
            }
        }

        public void UpdatePaymentSuccess(string vnpTxnRef, string transactionNo)
        {
            var payment = _paymentRepo.GetByVnpTxnRef(vnpTxnRef);
            if (payment != null)
            {
                payment.PaymentStatus = "Success";
                payment.TransactionNo = transactionNo;
                payment.PaymentDate = DateTime.Now;
                _paymentRepo.Update(payment);
                _paymentRepo.Save();
            }
        }
    }
}
