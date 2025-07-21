using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryREPO;

namespace ClassLibrarySERVICES
{
    public class OrderDetailServices :IOrderDetailServices
    {
        private readonly IOrderDetailRepositories _repo;

        public OrderDetailServices(IOrderDetailRepositories repo)
        {
            _repo = repo;
        }

    }
}
