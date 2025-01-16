using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.Interface
{
    public interface IBankBusinessLogic
    {
        Task<APIResponse> SaveBank(Bank bank);

        Task<List<Bank>> ReadBank(Bank bank);
    }

}
