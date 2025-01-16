using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface IBankAccess
    {
        Task<APIResponse> SaveBank(Bank bank);

        Task<List<Bank>> ReadBank(Bank bank);
    }
}
