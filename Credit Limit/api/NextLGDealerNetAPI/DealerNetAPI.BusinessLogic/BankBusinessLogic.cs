using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic
{
   public  class BankBusinessLogic : IBankBusinessLogic
    {
        private readonly IBankAccess _bankAccess = null;
        public BankBusinessLogic(IBankAccess bankAccess)
        {
            _bankAccess = bankAccess;
        }

        public async Task<APIResponse> SaveBank(Bank bank)
        {
            var data = await _bankAccess.SaveBank(bank);
            return data;
        }

        public async Task<List<Bank>> ReadBank(Bank bank)
        {
            var data = await _bankAccess.ReadBank(bank);
            return data;
        }
    }
}
