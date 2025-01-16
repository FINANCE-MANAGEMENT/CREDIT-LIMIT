using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.VMS
{
    public class UtilitiesVMSAccess : IUtilitiesVMSAccess
    {
        private readonly ICommonDB _commonDB = null;
        private readonly string DealerNet_Next_Connection = "DealerNetNextConnection";

        public UtilitiesVMSAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }

        public async Task<List<Quarters>> ReadConfirmationPeriod(string ClosingStatus, string VendorCode)
        {
            List<Quarters> lstQuarter = new List<Quarters>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_CLOSING_STATUS", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(ClosingStatus))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = ClosingStatus;
                }
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                arrParams[2] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(VendorCode))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = VendorCode;
                }
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.Utilities.READ_QUARTER, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Quarters quarter = new Quarters();
                    quarter.ConfirmationPeriod = SafeTypeHandling.ConvertToString(row["QUARTER_PERIOD"]);
                    quarter.PeriodFrom = SafeTypeHandling.ConvertToDateTime(row["PERIOD_FROM"]);
                    quarter.PeriodTo = SafeTypeHandling.ConvertToDateTime(row["PERIOD_TO"]);
                    quarter.ClosingStatus = SafeTypeHandling.ConvertToString(row["CLOSING_STATUS"]);
                    lstQuarter.Add(quarter);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstQuarter;
        }

        public async Task<List<Branch>> ReadBranch()
        {
            List<Branch> lstBranch = new List<Branch>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[1];
                arrParams[0] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[0].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.Utilities.BRANCH_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Branch _branch = new Branch();
                    _branch.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    //_branch.BranchAddress = SafeTypeHandling.ConvertToString(row["BRANCH_ADDRESS"]);
                    lstBranch.Add(_branch);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstBranch;
        }

        public async Task<APIResponse> GenerateOTP(OTP otp)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[11];
                arrParams[0] = new OracleParameter("P_MOBILE_NO", OracleDbType.Varchar2, 100);
                arrParams[0].Value = otp.MobileNo;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = otp.VendorCode;
                arrParams[2] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[2].Value = otp.CreatedBy;
                arrParams[3] = new OracleParameter("P_LOCAL_IP", OracleDbType.Varchar2, 50);
                arrParams[3].Value = otp.LocalIP;
                arrParams[4] = new OracleParameter("P_PUBLIC_IP", OracleDbType.Varchar2, 50);
                arrParams[4].Value = otp.PublicIP;
                arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[6].Direction = ParameterDirection.Output;
                arrParams[7] = new OracleParameter("P_OUT_OTP_NO", OracleDbType.Varchar2, 100);
                arrParams[7].Direction = ParameterDirection.Output;
                arrParams[8] = new OracleParameter("P_OUT_MOBILE_NO", OracleDbType.Varchar2, 100);
                arrParams[8].Direction = ParameterDirection.Output;
                arrParams[9] = new OracleParameter("P_OUT_MESSAGE", OracleDbType.Varchar2, 500);
                arrParams[9].Direction = ParameterDirection.Output;
                arrParams[10] = new OracleParameter("P_PROCESS_NAME", OracleDbType.Varchar2, 250);
                arrParams[10].Value = otp.ProcessName;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.Utilities.GENERATE_OTP, arrParams, DealerNet_Next_Connection);
                string _status = SafeTypeHandling.ConvertToString(arrParams[5].Value);
                string _statusDesc = SafeTypeHandling.ConvertToString(arrParams[6].Value);
                string OTPNo = SafeTypeHandling.ConvertToString(arrParams[7].Value); // OTP Number
                string _originalMobileNo = SafeTypeHandling.ConvertToString(arrParams[8].Value); // Original Mobile Number (Decrypt)
                string _otpMessage = SafeTypeHandling.ConvertToString(arrParams[9].Value); // SMS data

                if (_status == Utilities.SUCCESS)
                {
                    if (!string.IsNullOrEmpty(OTPNo))
                    {
                        string smsTemplateName = "VMS_PARTNER_AUTHENTICATION";
                        SmsData smsData = SMS_DLT_DetailRead(smsTemplateName);
                        if (smsData == null)
                        {
                            return response = new APIResponse
                            {
                                Status = Utilities.ERROR,
                                StatusDesc = _statusDesc
                            };
                        }
                        APIResponse smsResponse = await SMSSend(smsData, otp, smsTemplateName, _originalMobileNo, _otpMessage);
                        return response = new APIResponse
                        {
                            Status = smsResponse.Status,
                            StatusDesc = _statusDesc
                        };
                    }
                }

                return response = new APIResponse
                {
                    Status = _status,
                    StatusDesc = _statusDesc
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// SMS DLT details read
        /// </summary>
        /// <param name="SMSRegistrationName"></param>
        /// <returns></returns>
        public SmsData SMS_DLT_DetailRead(string SMSRegistrationName)
        {
            SmsData smsData = null;
            // SMS Tempalete Read
            OracleParameter[] smsParams = new OracleParameter[2];
            smsParams[0] = new OracleParameter("P_SMS_REG_NAME", OracleDbType.Varchar2, 100);
            smsParams[0].Value = SMSRegistrationName;
            smsParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
            smsParams[1].Direction = ParameterDirection.Output;
            DataTable dtTemplateData = _commonDB.getDataTableStoredProc(DatabaseConstants.VendorManagementSystem.Utilities.SMS_TEMPLATE_READ, smsParams, DealerNet_Next_Connection);
            foreach (DataRow template in dtTemplateData.Rows)
            {
                smsData = new SmsData();
                smsData.SMS_REG_NAME = SafeTypeHandling.ConvertToString(template["SMS_REG_NAME"]);
                smsData.OA = SafeTypeHandling.ConvertToString(template["SMS_OA"]);
                smsData.CAMPAIGN_NAME = SafeTypeHandling.ConvertToString(template["CAMPAIGN_NAME"]);
                smsData.CIRCLE_NAME = SafeTypeHandling.ConvertToString(template["CIRCLE_NAME"]);
                smsData.USER_NAME = SafeTypeHandling.ConvertToString(template["USER_NAME"]);
                smsData.DLT_TM_ID = SafeTypeHandling.ConvertToString(template["DLT_TM_ID"]);
                smsData.DLT_PE_ID = SafeTypeHandling.ConvertToString(template["DLT_PE_ID"]);

                smsData.HEADER_NAME = SafeTypeHandling.ConvertToString(template["SMS_OA"]);
                smsData.TEMPLATE_ID = SafeTypeHandling.ConvertToString(template["TEMPLATE_ID"]);
                smsData.MESSAGE = SafeTypeHandling.ConvertToString(template["MESSAGE"]);
                smsData.API_BASE_URL = SafeTypeHandling.ConvertToString(template["API_BASE_URL"]);
                smsData.API_KEY = SafeTypeHandling.ConvertToString(template["API_KEY"]);
                smsData.SERVICE_URL = SafeTypeHandling.ConvertToString(template["SERVICE_URL"]);
            }
            return smsData;
        }

        /// <summary>
        /// SMS send to customer
        /// </summary>
        /// <param name="smsData"></param>
        /// <param name="otp"></param>
        /// <param name="SMSRegistrationName"></param>
        /// <param name="MobileNo"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public async Task<APIResponse> SMSSend(SmsData smsData, OTP otp, string SMSRegistrationName, string MobileNo, string Message)
        {
            APIResponse response = null;
            if (smsData == null || string.IsNullOrEmpty(SMSRegistrationName) || string.IsNullOrEmpty(MobileNo) || string.IsNullOrEmpty(Message))
            {
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = "Required parameter missing."
                };
            }

            try
            {
                // Need to Logic for SMS Send
                if (smsData != null)
                {
                    // SMS Send
                    MessageResponse returnResponse = await Utilities.SendSMSInfobip(smsData, MobileNo, Message);
                    if (returnResponse.Status == Utilities.SUCCESS)
                    {
                        try
                        {
                            MessageResponse messageResponse = new MessageResponse();
                            messageResponse.UniqueId = returnResponse.UniqueId;
                            messageResponse.Message = Message;
                            messageResponse.Mobile_Number = MobileNo;
                            messageResponse.MessageTemplateId = smsData.TEMPLATE_ID;
                            messageResponse.DeviceType = otp.DeviceType;
                            messageResponse.Status = returnResponse.Status;
                            messageResponse.status_desc = returnResponse.status_desc;
                            messageResponse.Remarks = returnResponse.result;
                            // SMS Log maintain
                            try
                            {
                                SMSLogUpdate(messageResponse, otp);
                            }
                            catch (Exception)
                            {

                            }

                            return response = new APIResponse
                            {
                                Status = Utilities.SUCCESS,
                                StatusDesc = "SMS sent successfully."
                            };
                        }
                        catch (Exception)
                        {
                            return response = new APIResponse
                            {
                                Status = Utilities.SUCCESS,
                                StatusDesc = "SMS sent successfully."
                            };
                        }
                    }
                    else
                    {
                        return response = new APIResponse
                        {
                            Status = Utilities.ERROR,
                            StatusDesc = "Unable to send SMS."
                        };
                    }
                }
                else
                {
                    return response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = "Unable to read SMS template."
                    };
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex.ToString());
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = "Unable to send SMS."
                };
            }
        }

        public void SMSLogUpdate(MessageResponse messageResponse, OTP otp)
        {
            try
            {
                OracleParameter[] arrParams = new OracleParameter[16];
                arrParams[0] = new OracleParameter("P_TEMPLATE_ID", OracleDbType.Varchar2, 100);
                arrParams[0].Value = messageResponse.MessageTemplateId;
                arrParams[1] = new OracleParameter("P_MOBILE_NO", OracleDbType.Varchar2, 100);
                arrParams[1].Value = messageResponse.Mobile_Number;
                arrParams[2] = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500);
                arrParams[2].Value = messageResponse.Message;
                arrParams[3] = new OracleParameter("P_MESSAGE_ID", OracleDbType.Varchar2, 250);
                arrParams[3].Value = messageResponse.UniqueId;
                arrParams[4] = new OracleParameter("P_TRANSACTION_ID", OracleDbType.Varchar2, 250);
                arrParams[4].Value = messageResponse.transaction_id;
                arrParams[5] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 250);
                arrParams[5].Value = messageResponse.Status;
                arrParams[6] = new OracleParameter("P_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[6].Value = messageResponse.status_desc;
                arrParams[7] = new OracleParameter("P_TXN_TYPE", OracleDbType.Varchar2, 250);
                arrParams[7].Value = messageResponse.TransactionType;
                arrParams[8] = new OracleParameter("P_DEVICE_SOURCE", OracleDbType.Varchar2, 250);
                arrParams[8].Value = messageResponse.DeviceType;
                arrParams[9] = new OracleParameter("P_REMARKS", OracleDbType.Varchar2, 500);
                if(!string.IsNullOrEmpty(messageResponse.Remarks))
                {
                    arrParams[9].Value = messageResponse.Remarks;
                }
                else
                {
                    arrParams[9].Value = DBNull.Value;
                }
                arrParams[10] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 250);
                arrParams[10].Value = otp.CreatedBy;
                arrParams[11] = new OracleParameter("P_LOCAL_IP", OracleDbType.Varchar2, 50);
                arrParams[11].Value = otp.LocalIP;
                arrParams[12] = new OracleParameter("P_PUBLIC_IP", OracleDbType.Varchar2, 50);
                arrParams[12].Value = otp.PublicIP;
                arrParams[13] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[13].Direction = ParameterDirection.Output;
                arrParams[14] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[14].Direction = ParameterDirection.Output;
                arrParams[15] = new OracleParameter("P_PROCESS_NAME", OracleDbType.Varchar2, 250);
                arrParams[15].Value = otp.ProcessName;

                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.VendorManagementSystem.Utilities.SMS_LOG_UPDATE, arrParams, DealerNet_Next_Connection);
                string _status = SafeTypeHandling.ConvertToString(arrParams[13].Value);
                string _statusDesc = SafeTypeHandling.ConvertToString(arrParams[14].Value);
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex.ToString());
                throw;
            }
        }

        public async Task<APIResponse> ValidateOTP(OTP otp)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[7];
                arrParams[0] = new OracleParameter("P_MOBILE_NO", OracleDbType.Varchar2, 100);
                arrParams[0].Value = otp.MobileNo;
                arrParams[1] = new OracleParameter("P_OTP_NO", OracleDbType.Varchar2, 100);
                arrParams[1].Value = otp.OTPNo;
                arrParams[2] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[2].Value = otp.CreatedBy;
                arrParams[3] = new OracleParameter("P_LOCAL_IP", OracleDbType.Varchar2, 50);
                arrParams[3].Value = otp.LocalIP;
                arrParams[4] = new OracleParameter("P_PUBLIC_IP", OracleDbType.Varchar2, 50);
                arrParams[4].Value = otp.PublicIP;
                arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[6].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.Utilities.VALIDATE_OTP, arrParams, DealerNet_Next_Connection);

                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[5].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[6].Value)
                };
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<APIResponse> UpdateOTP(OTP otp)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[7];
                arrParams[0] = new OracleParameter("P_MOBILE_NO", OracleDbType.Varchar2, 100);
                arrParams[0].Value = otp.MobileNo;
                arrParams[1] = new OracleParameter("P_OTP_NO", OracleDbType.Varchar2, 100);
                arrParams[1].Value = otp.OTPNo;
                arrParams[2] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[2].Value = otp.CreatedBy;
                arrParams[3] = new OracleParameter("P_LOCAL_IP", OracleDbType.Varchar2, 50);
                arrParams[3].Value = otp.LocalIP;
                arrParams[4] = new OracleParameter("P_PUBLIC_IP", OracleDbType.Varchar2, 50);
                arrParams[4].Value = otp.PublicIP;
                arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[6].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.Utilities.UPDATE_OTP, arrParams, DealerNet_Next_Connection);

                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[5].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[6].Value)
                };
                return response;
            }
            catch (Exception ex)
            {
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
        }

        public async Task<APIResponse> UserAU_Mapping(Users users)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, 100);
                arrParams[0].Value = users.UserId;
                arrParams[1] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2, 100);
                arrParams[1].Value = users.Branch.BranchCode;
                arrParams[2] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 100);
                arrParams[2].Value = users.Status;
                arrParams[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[3].Value = users.CreatedBy;
                arrParams[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[4].Direction = ParameterDirection.Output;
                arrParams[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.Utilities.USER_AU_MAPPING_UPDATE, arrParams, DealerNet_Next_Connection);

                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[4].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[5].Value)
                };
                return response;
            }
            catch (Exception ex)
            {
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
        }

        public async Task<List<Users>> ReadUserAU_Mapping(Users users)
        {
            List<Users> lstAUMapping = new List<Users>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, 100);
                if (users.UserId == 0)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = users.UserId;
                }
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.Utilities.USER_AU_MAPPING_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Users _user = new Users();
                    _user.UserId = SafeTypeHandling.ConvertStringToInt32(row["USER_ID"]);
                    _user.LoginID = SafeTypeHandling.ConvertToString(row["LOGIN_ID"]);
                    _user.LoginName = SafeTypeHandling.ConvertToString(row["LOGIN_NAME"]);
                    _user.Branch.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);

                    _user.Role.RoleId = SafeTypeHandling.ConvertStringToInt32(row["USER_ROLE_ID"]);
                    _user.Role.RoleName = SafeTypeHandling.ConvertToString(row["USER_ROLE_NAME"]);

                    _user.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    _user.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    _user.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPDATED_BY"]);
                    _user.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY_NAME"]);
                    _user.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    lstAUMapping.Add(_user);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstAUMapping;
        }

        public async Task<List<Users>> ReadUsersForAUMappingByRoleId(int roleId)
        {
            List<Users> lstUsers = new List<Users>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_ROLEID", OracleDbType.Varchar2);
                arrParams[0].Value = roleId;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.Utilities.READ_USERS_BY_ROLE, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Users users = new Users();
                    users.UserId = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    users.LoginID = SafeTypeHandling.ConvertToString(row["LOGIN_ID"]);
                    users.LoginName = SafeTypeHandling.ConvertToString(row["LOGIN_NAME"]);
                    users.Zone.Region.Branch.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    users.Zone.Region.RegionCode = SafeTypeHandling.ConvertToString(row["REGION"]);
                    users.Role.RoleId = SafeTypeHandling.ConvertStringToInt32(row["USER_ROLE_ID"]);
                    users.Role.RoleName = SafeTypeHandling.ConvertToString(row["USER_ROLE_NAME"]);
                    lstUsers.Add(users);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstUsers;
        }


        public async Task<APIResponse> ChangePassword(Users userPswd)
        {
            string Old_Password = Utilities.Encrypt_SHA512(userPswd.Password);
            string New_Password = Utilities.Encrypt_SHA512(userPswd.NewPassword);

            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[7];
                arrParams[0] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, 100);
                arrParams[0].Value = userPswd.UserId;
                arrParams[1] = new OracleParameter("P_OLD_PASSWORD", OracleDbType.Varchar2, 100);
                arrParams[1].Value = Old_Password;
                arrParams[2] = new OracleParameter("P_NEW_PASSWORD", OracleDbType.Varchar2, 100);
                arrParams[2].Value = New_Password;
                arrParams[3] = new OracleParameter("P_LOCAL_IP", OracleDbType.Varchar2, 50);
                arrParams[3].Value = userPswd.LocalIP;
                arrParams[4] = new OracleParameter("P_PUBLIC_IP", OracleDbType.Varchar2, 50);
                arrParams[4].Value = userPswd.PublicIP;
                arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[6].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.Utilities.VENDOR_CHANGE_PASSWORD_UPDATE, arrParams, DealerNet_Next_Connection);

                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[5].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[6].Value)
                };
                return response;
            }
            catch (Exception ex)
            {
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
        }

    }
}
