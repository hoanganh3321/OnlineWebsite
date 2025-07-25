﻿using System;
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
        private readonly IOrderRepositories _orderRepo;
        public PaymentServices(IPaymentRepositories paymentRepo, IOrderRepositories orderRepo)
        {
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
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

        public async Task UpdatePaymentStatusAsync(string vnpTxnRef)
        {
            var result = _paymentRepo.UpdatePaymentStatusAsync(vnpTxnRef);
        }

        public void UpdatePaymentSuccess(string vnpTxnRef)
        {
            var payment = _paymentRepo.GetByVnpTxnRef(vnpTxnRef);
            if (payment != null)
            {
             
                payment.PaymentStatus = "Completed";
                payment.PaymentDate = DateTime.Now;

                _paymentRepo.Update(payment);
              
                var order = payment.Order;
                               
                    order.OrderStatus = "Completed";
                    order.PaymentMethod = "Online";
                    order.CreatedAt = DateTime.Now;
                    _orderRepo.Update(order);
                                   
                _orderRepo.Save();
                _paymentRepo.Save();
                
            }
        }

    }
}
