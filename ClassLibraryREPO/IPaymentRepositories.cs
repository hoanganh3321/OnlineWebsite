using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryREPO.Repositories;

namespace ClassLibraryREPO
{
    public interface IPaymentRepositories : IGenericRepository<Payment>
    {
        Task UpdatePaymentStatusAsync(string vnpTxnRef);
        Payment? GetByVnpTxnRef(string vnpTxnRef);
    }
}
