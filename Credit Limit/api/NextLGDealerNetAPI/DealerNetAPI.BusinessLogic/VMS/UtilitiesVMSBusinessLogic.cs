using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.VMS
{
    public class UtilitiesVMSBusinessLogic : IUtilitiesVMSBusinessLogic
    {
        private readonly IUtilitiesVMSAccess _utilitiesVMSAccess = null;

        public UtilitiesVMSBusinessLogic(IUtilitiesVMSAccess utilitiesVMSAccess)
        {
            _utilitiesVMSAccess = utilitiesVMSAccess;
        }

        public async Task<List<Quarters>> ReadConfirmationPeriod(string ClosingStatus, string VendorCode)
        {
            var data = await _utilitiesVMSAccess.ReadConfirmationPeriod(ClosingStatus, VendorCode);
            return data;
        }

        public async Task<List<Branch>> ReadBranch()
        {
            var data = await _utilitiesVMSAccess.ReadBranch();
            return data;
        }


        public async Task<APIResponse> GenerateOTP(OTP otp)
        {
            var data = await _utilitiesVMSAccess.GenerateOTP(otp);
            return data;
        }
        public SmsData SMS_DLT_DetailRead(string SMSRegistrationName)
        {
            var data = _utilitiesVMSAccess.SMS_DLT_DetailRead(SMSRegistrationName);
            return data;
        }
        public async Task<APIResponse> SMSSend(SmsData smsData, OTP otp, string SMSRegistrationName, string MobileNo, string Message)
        {
            var data = await _utilitiesVMSAccess.SMSSend(smsData, otp, SMSRegistrationName, MobileNo, Message);
            return data;
        }


        public async Task<APIResponse> ValidateOTP(OTP otp)
        {
            var data = await _utilitiesVMSAccess.ValidateOTP(otp);
            return data;
        }

        public async Task<APIResponse> UpdateOTP(OTP otp)
        {
            var data = await _utilitiesVMSAccess.UpdateOTP(otp);
            return data;
        }

        public async Task<APIResponse> UserAU_Mapping(Users users)
        {
            var data = await _utilitiesVMSAccess.UserAU_Mapping(users);
            return data;
        }

        public async Task<List<Users>> ReadUserAU_Mapping(Users users)
        {
            var data = await _utilitiesVMSAccess.ReadUserAU_Mapping(users);
            return data;
        }

        public async Task<List<Users>> ReadUsersForAUMappingByRoleId(int roleId)
        {
            var data = await _utilitiesVMSAccess.ReadUsersForAUMappingByRoleId(roleId);
            return data;
        }



        public async Task<APIResponse> ChangePassword(Users userPswd)
        {
            // Base64 encoding in C#
            //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("TestString");
            //Console.WriteLine(System.Convert.ToBase64String(plainTextBytes));

            // Base64 decoding in C#
            //var base64EncodedBytes = System.Convert.FromBase64String(System.Convert.ToBase64String(plainTextBytes));
            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));


            string oldPassword = string.Empty;
            string newPassword = string.Empty;


            //var base64EncodedBytesOldPswd = System.Convert.FromBase64String(userPswd.Password);
            //oldPassword = System.Text.Encoding.UTF8.GetString(base64EncodedBytesOldPswd);

            //var base64EncodedBytesNewPswd = System.Convert.FromBase64String(userPswd.NewPassword);
            //newPassword = System.Text.Encoding.UTF8.GetString(base64EncodedBytesNewPswd);


            oldPassword = userPswd.Password;
            newPassword = userPswd.NewPassword;

            APIResponse apiResponse = new APIResponse();
            string SeqNumber = "0123456789";
            string SeqString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var hasNumber = new Regex(@"[0-9]+");
            var currentYear = DateTime.Now.Year;
            bool isMonth = Utilities.IsMonthName(newPassword.ToUpper());
            string errMsg = string.Empty;

            if (string.IsNullOrEmpty(Convert.ToString(userPswd.UserId)) || userPswd.UserId == 0)
            {
                errMsg = "Please check user.";
            }
            else if (string.IsNullOrEmpty(oldPassword))
            {
                errMsg = "Please enter Old Password.";
            }
            else if (string.IsNullOrEmpty(newPassword))
            {
                errMsg = "Please enter new Password.";
            }
            else if (string.IsNullOrEmpty(userPswd.ConfirmNewPassword))
            {
                errMsg = "Please enter Confirm Password.";
            }
            else if (newPassword.Length < 10)
            {
                errMsg = "Password must contain Minimum 10 characters.";
            }
            else if (newPassword.Length > 15)
            {
                errMsg = "Password can contain max 15 characters.";
            }
            else if (hasNumber.IsMatch(newPassword) == false)
            {
                errMsg = "Password should contain At least one numeric value";
            }
            //else if (newPassword.Any(char.IsUpper) == false)
            //{
            //    errMsg = "Password must contain 1 Upper character.";
            //}
            //else if (newPassword.Any(char.IsLower) == false)
            //{
            //    errMsg = "Password must contain 1 Lower character.";
            //}
            else if (newPassword.Contains(" ") == true)
            {
                errMsg = "Password can not contain space.";
            }
            else if (newPassword.ToUpper().StartsWith("LG"))
            {
                errMsg = "Password can't start with LG";
            }
            else if (isMonth)
            {
                errMsg = "Password can't contain any month name.";
            }
            else if (newPassword.ToUpper().Contains(currentYear.ToString()))
            {
                errMsg = "Password can't contain current year.";
            }
            else if (newPassword != userPswd.ConfirmNewPassword)
            {
                errMsg = "Confirm Password is not matched.";
            }
            else if (Utilities.IsMatch4Character(oldPassword, newPassword))
            {
                errMsg = "New Password should not contain 4 or more characters that is identical to the last password.";
            }

            // If any error
            if (!string.IsNullOrEmpty(errMsg))
            {
                apiResponse.Status = Utilities.ERROR;
                apiResponse.StatusDesc = errMsg;
                return apiResponse;
            }

            string specialCh = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
            char[] specialChArray = specialCh.ToCharArray();
            bool checksprchar = false;
            foreach (char ch in specialChArray)
            {
                if (newPassword.Contains(ch))
                    checksprchar = true;
            }
            if (checksprchar == false)
            {
                errMsg = "Password must contain 1 special character.";
                apiResponse.Status = Utilities.ERROR;
                apiResponse.StatusDesc = errMsg;
                return apiResponse;
            }

            for (int i = 0; i < newPassword.Length - 2; i++)
            {
                string newPswd = newPassword.Substring(i, 3);

                if (SeqNumber.IndexOf(newPswd) >= 0 || Utilities.ReverseString(SeqNumber).IndexOf(newPswd) >= 0 ||
                    SeqString.IndexOf(newPswd) >= 0 || Utilities.ReverseString(SeqString).IndexOf(newPswd) >= 0)
                {
                    errMsg = "New Password should not contain continuation of characters/number 3-times in a row are not allowed.";
                }
            }

            // If any error
            if (!string.IsNullOrEmpty(errMsg))
            {
                apiResponse.Status = Utilities.ERROR;
                apiResponse.StatusDesc = errMsg;
                return apiResponse;
            }

            // DB object call
            var data = await _utilitiesVMSAccess.ChangePassword(userPswd);
            return data;

        }




    }
}
