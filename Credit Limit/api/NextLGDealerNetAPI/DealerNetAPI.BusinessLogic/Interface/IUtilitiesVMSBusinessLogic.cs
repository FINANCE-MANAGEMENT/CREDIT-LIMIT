using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.Interface
{
    public interface IUtilitiesVMSBusinessLogic
    {
        Task<List<Quarters>> ReadConfirmationPeriod(string ClosingStatus, string VendorCode);
        Task<List<Branch>> ReadBranch();

        Task<APIResponse> GenerateOTP(OTP otp);
        SmsData SMS_DLT_DetailRead(string SMSRegistrationName);
        Task<APIResponse> SMSSend(SmsData smsData, OTP otp, string SMSRegistrationName, string MobileNo, string Message);
        Task<APIResponse> ValidateOTP(OTP otp);

        Task<APIResponse> UpdateOTP(OTP otp); 
        Task<APIResponse> UserAU_Mapping(Users users);

        Task<List<Users>> ReadUserAU_Mapping(Users users);
        Task<List<Users>> ReadUsersForAUMappingByRoleId(int roleId);


        Task<APIResponse> ChangePassword(Users userPswd);
        



    }
}
