using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryREPO.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClassLibraryREPO
{
    public class PaymentRepositories : GenericRepository<Payment>, IPaymentRepositories
    {
        public PaymentRepositories(FoodDeliverContext context) : base(context)
        {
        }

        public Payment? GetByVnpTxnRef(string vnpTxnRef)
        {
            return _dbSet.Include(p => p.Order).FirstOrDefault(p => p.VnpTxnRef == vnpTxnRef);
        }

        public async Task UpdatePaymentStatusAsync(string vnpTxnRef)
        {
            var payment = await _context.Payments
       .FirstOrDefaultAsync(p => p.VnpTxnRef == vnpTxnRef);

            if (payment == null)
                throw new Exception("Payment not found");

           
            payment.PaymentStatus = "Completed";

            payment.PaymentDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}
