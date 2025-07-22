using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryREPO.Repositories;

namespace ClassLibraryREPO
{
    public class PaymentRepositories : GenericRepository<Payment>, IPaymentRepositories
    {
        public PaymentRepositories(FoodDeliverContext context) : base(context)
        {
        }

        public Payment? GetByVnpTxnRef(string vnpTxnRef)
        {
            return _dbSet.FirstOrDefault(p => p.VnpTxnRef == vnpTxnRef);
        }
    }
}
