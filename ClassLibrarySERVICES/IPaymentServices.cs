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
        Payment CreatePendingPayment(int orderId, decimal amount);
        void UpdatePaymentSuccess(string vnpTxnRef, string transactionNo);
        void UpdatePaymentFail(string vnpTxnRef);
    }
}
