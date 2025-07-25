using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;

namespace ClassLibrarySERVICES
{
    public interface IPaymentServices
    {
        Task UpdatePaymentStatusAsync(string vnpTxnRef);
        Payment CreatePendingPayment(int orderId, decimal amount);
        void UpdatePaymentSuccess(string vnpTxnRef);
        void UpdatePaymentFail(string vnpTxnRef);
    }
}
