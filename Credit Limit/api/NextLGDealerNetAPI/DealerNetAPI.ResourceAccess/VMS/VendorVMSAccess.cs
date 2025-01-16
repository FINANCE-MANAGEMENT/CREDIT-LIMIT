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
    public class VendorVMSAccess : IVendorVMSAccess
    {
        private readonly string DealerNet_Next_Connection = "DealerNetNextConnection";
        private readonly ICommonDB _commonDB = null;
        private readonly IUtilitiesVMSAccess _utilitiesVMSAccess = null;

        public VendorVMSAccess(ICommonDB commonDB, IUtilitiesVMSAccess utilitiesVMSAccess)
        {
            _commonDB = commonDB;
            _utilitiesVMSAccess = utilitiesVMSAccess;
        }

        /// <summary>
        /// Save/Update scheme
        /// </summary>
        /// <param name="schemeRequest"></param>
        /// <returns></returns>
        public async Task<APIResponse> SaveUpdateVendor(Vendor vendor)
        {
            APIResponse response = null;
            try
            {
                response = VendorInsertUpdate(vendor);
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
            return response;
        }

        /// <summary>
        /// Vendor/Supplier Insert Update Function.
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public APIResponse VendorInsertUpdate(Vendor vendor)
        {
            APIResponse response = null;
            try
            {
                // Vendor Details Save/ Upate.
                OracleParameter[] arrParams = new OracleParameter[21];
                arrParams[0] = new OracleParameter("P_VENDOR_ID", OracleDbType.Int64);
                if (vendor.VendorId == 0 || string.IsNullOrEmpty(Convert.ToString(vendor.VendorId)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = vendor.VendorId;
                }
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 500);
                arrParams[1].Value = vendor.VendorCode;
                arrParams[2] = new OracleParameter("P_VENDOR_NAME", OracleDbType.Varchar2, 500);
                arrParams[2].Value = vendor.VendorName;
                arrParams[3] = new OracleParameter("P_EMAIL_ID", OracleDbType.Varchar2, 500);
                arrParams[3].Value = vendor.EmailId;
                arrParams[4] = new OracleParameter("P_MOBILE_NO", OracleDbType.Varchar2, 500);
                arrParams[4].Value = vendor.MobileNo;
                arrParams[5] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2);
                arrParams[5].Value = vendor.CTRL_AU;
                arrParams[6] = new OracleParameter("P_PIC_CODE", OracleDbType.Varchar2);
                arrParams[6].Value = vendor.PIC_Code;
                arrParams[7] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[7].Value = vendor.Status;
                arrParams[8] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                arrParams[8].Value = vendor.CreatedBy;
                arrParams[9] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                arrParams[9].Direction = ParameterDirection.Output;
                arrParams[10] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[10].Direction = ParameterDirection.Output;

                arrParams[11] = new OracleParameter("P_PAN_NO", OracleDbType.Varchar2);
                if (!string.IsNullOrEmpty(vendor.PAN_No))
                {
                    arrParams[11].Value = vendor.PAN_No.ToUpper();
                }
                else
                {
                    arrParams[11].Value = DBNull.Value;
                }
                arrParams[12] = new OracleParameter("P_GST_NO", OracleDbType.Varchar2);
                if (!string.IsNullOrEmpty(vendor.GSTIN_No))
                {
                    arrParams[12].Value = vendor.GSTIN_No.ToUpper();
                }
                else
                {
                    arrParams[12].Value = DBNull.Value;
                }

                arrParams[13] = new OracleParameter("P_MSME_NO", OracleDbType.Varchar2);
                if (!string.IsNullOrEmpty(vendor.MSMERegNo))
                {
                    arrParams[13].Value = vendor.MSMERegNo.ToUpper();
                }
                else
                {
                    arrParams[13].Value = DBNull.Value;
                }
                arrParams[14] = new OracleParameter("P_ENTERPRISES_TYPE", OracleDbType.Varchar2);
                if (!string.IsNullOrEmpty(vendor.EnterprisesType))
                {
                    arrParams[14].Value = vendor.EnterprisesType.ToUpper();
                }
                else
                {
                    arrParams[14].Value = DBNull.Value;
                }
                arrParams[15] = new OracleParameter("P_MAJOR_ACTIVITY", OracleDbType.Varchar2);
                if (!string.IsNullOrEmpty(vendor.MajorActivity))
                {
                    arrParams[15].Value = vendor.MajorActivity.ToUpper();
                }
                else
                {
                    arrParams[15].Value = DBNull.Value;
                }
                arrParams[16] = new OracleParameter("P_ADDRESS", OracleDbType.Varchar2, 500);
                arrParams[16].Value = vendor.Address;
                arrParams[17] = new OracleParameter("P_CITY", OracleDbType.Varchar2, 500);
                arrParams[17].Value = vendor.City;
                arrParams[18] = new OracleParameter("P_STATE_ID", OracleDbType.Varchar2);
                arrParams[18].Value = vendor.State.StateId;
                arrParams[19] = new OracleParameter("P_PINCODE", OracleDbType.Varchar2);
                arrParams[19].Value = vendor.PinCode;
                arrParams[20] = new OracleParameter("P_OUT_PROCESS_TYPE", OracleDbType.Varchar2, 200);
                arrParams[20].Direction = ParameterDirection.Output;

                DataTable dtInvData = _commonDB.getDataTableStoredProc(DatabaseConstants.VendorManagementSystem.VendorMaster.INSERT_UPDATE, arrParams, DealerNet_Next_Connection);

                string processType = SafeTypeHandling.ConvertToString(arrParams[20].Value);
                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[9].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[10].Value)
                };


                #region Welcome Message Send To Vendor/Supplier (SMS), Only New Registration

                if (response.Status == Utilities.SUCCESS && processType.Equals("NEW_REGISTRATION") &&
                    !string.IsNullOrEmpty(vendor.MobileNo) &&
                    (vendor.VendorId == 0 || string.IsNullOrEmpty(Convert.ToString(vendor.VendorId))))
                {
                    try
                    {
                        string SMSTemplateName = "VMS_PARTNER_COMMUNICATION"; // Template Name in SMS_MST_TB

                        // SMS Template & message read
                        SmsData smsDLT = _utilitiesVMSAccess.SMS_DLT_DetailRead(SMSTemplateName);
                        if (smsDLT != null)
                        {
                            OTP otp = new OTP();
                            otp.MobileNo = vendor.MobileNo;
                            otp.CreatedBy = vendor.CreatedBy;
                            otp.ProcessName = "VENDOR_REGISTRATION";

                            // Masked Vendor/supplier code generate
                            string maskedVendorCode = vendor.VendorCode.Substring(0, 3);
                            for (int i = 3; i < vendor.VendorCode.Length; i++)
                            {
                                maskedVendorCode = maskedVendorCode + "X";
                            }

                            string message = smsDLT.MESSAGE.Replace("{var1}{var2}", "Your Code " + maskedVendorCode + " is registered on LG VMS"); // SMS body change
                            var smsResp = _utilitiesVMSAccess.SMSSend(smsDLT, otp, SMSTemplateName, otp.MobileNo, message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.CreateLogFile(Utilities.PROJECTS.VMS, vendor.VendorCode + "," + vendor.MobileNo + "," + "New Vendor Registration, SMS Sending Issues. " + "#" + ex);
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex.ToString());
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = Utilities.SOME_ERROR_OCCURED_WHILE_SUBMIT_RECORD
                };
            }
            return response;
        }

        /// <summary>
        /// Save/Update Bulk scheme
        /// </summary>
        /// <param name="schemeRequest"></param>
        /// <returns></returns>
        public async Task<APIResponse> SaveBulkVendor(List<Vendor> vendor)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            try
            {
                int counter = 0;
                foreach (var _vendor in vendor)
                {
                    // Vendor Details Save/ Upate.
                    var crudResp = VendorInsertUpdate(_vendor);

                    if (crudResp.Status == Utilities.ERROR)
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (counter + 2), ": ", crudResp.StatusDesc) });
                    }
                    counter++;
                }

                response = new APIResponse
                {
                    Status = (errors.Count == 0 ? Utilities.SUCCESS : Utilities.ERROR),
                    StatusDesc = (errors.Count == 0 ? Utilities.RECORD_SBUMITTED_SUCCESSFULLY : Utilities.RECORD_PARTIALLY_SBUMITTED_SUCCESSFULLY),
                    data = errors
                };
            }
            catch (Exception ex)
            {
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString(),
                    data = errors
                };
            }
            return response;
        }

        /// <summary>
        /// Read all vendors/ Suppliers list
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public async Task<List<Vendor>> ReadVendor(Vendor vendor)
        {
            List<Vendor> lstVendor = new List<Vendor>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_VENDOR_ID", OracleDbType.Int64);
                if (vendor.VendorId == 0 || string.IsNullOrEmpty(Convert.ToString(vendor.VendorId)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = vendor.VendorId;
                }
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2);
                arrParams[1].Value = vendor.VendorCode;
                arrParams[2] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2);
                arrParams[2].Value = vendor.CTRL_AU;
                arrParams[3] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[3].Value = vendor.Status;
                arrParams[4] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[4].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorMaster.READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Vendor _vendor = new Vendor();
                    _vendor.VendorId = SafeTypeHandling.ConvertStringToInt64(row["ID"]);
                    _vendor.VendorCode = SafeTypeHandling.ConvertToString(row["VENDOR_CODE"]);
                    _vendor.VendorName = SafeTypeHandling.ConvertToString(row["VENDOR_NAME"]);
                    _vendor.MobileNo = SafeTypeHandling.ConvertToString(row["MOBILE_NO"]);
                    _vendor.EmailId = SafeTypeHandling.ConvertToString(row["EMAIL_ID"]);
                    _vendor.CTRL_AU = SafeTypeHandling.ConvertToString(row["CTRL_AU"]);
                    _vendor.PIC_Code = SafeTypeHandling.ConvertToString(row["PIC_CODE"]);
                    _vendor.PIC_Name = SafeTypeHandling.ConvertToString(row["PIC_NAME"]);
                    _vendor.PIC_MobileNo = SafeTypeHandling.ConvertToString(row["PIC_MOBILE_NO"]);

                    _vendor.PAN_No = SafeTypeHandling.ConvertToString(row["PAN_NO"]);
                    _vendor.GSTIN_No = SafeTypeHandling.ConvertToString(row["GST_NO"]);
                    _vendor.MSMERegNo = SafeTypeHandling.ConvertToString(row["MSME_NO"]);
                    _vendor.EnterprisesType = SafeTypeHandling.ConvertToString(row["ENTERPRISES_TYPE"]);
                    _vendor.MajorActivity = SafeTypeHandling.ConvertToString(row["MAJOR_ACTIVITY"]);
                    if (!string.IsNullOrEmpty(row["STATE_ID"].ToString()))
                    {
                        _vendor.State.StateId = SafeTypeHandling.ConvertStringToInt32(row["STATE_ID"]);
                    }
                    _vendor.State.StateName = SafeTypeHandling.ConvertToString(row["STATE_NAME"]);
                    _vendor.Address = SafeTypeHandling.ConvertToString(row["ADDRESS"]);
                    _vendor.City = SafeTypeHandling.ConvertToString(row["CITY"]);
                    _vendor.PinCode = SafeTypeHandling.ConvertToString(row["PINCODE"]);

                    _vendor.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    _vendor.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    _vendor.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    _vendor.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _vendor.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY"]);
                    _vendor.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);

                    _vendor.BranchAddress = SafeTypeHandling.ConvertToString(row["BRANCH_ADDRESS"]);

                    lstVendor.Add(_vendor);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstVendor;
        }

        public async Task<List<Branch>> ReadPICRequiredBranch()
        {
            List<Branch> lstBranch = new List<Branch>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[1];
                arrParams[0] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[0].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorMaster.PIC_REQ_BRANCH_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Branch _branch = new Branch();
                    _branch.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    lstBranch.Add(_branch);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstBranch;
        }

        public async Task<List<Users>> ReadPIC_Members(Branch branch)
        {
            List<Users> lstPIC = new List<Users>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(branch.BranchCode)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = branch.BranchCode;
                }
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorMaster.READ_PIC_MEMBERS, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Users _user = new Users();
                    _user.UserId = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    _user.LoginID = SafeTypeHandling.ConvertToString(row["LOGIN_ID"]);
                    _user.LoginName = SafeTypeHandling.ConvertToString(row["LOGIN_NAME"]);
                    lstPIC.Add(_user);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstPIC;
        }

        /// <summary>
        /// Vendor Profile Update Request
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorProfileUpdateRequest(Vendor vendor)
        {
            APIResponse response = null;
            try
            {
                // Vendor Details Save/ Upate.
                OracleParameter[] arrParams = new OracleParameter[19];
                arrParams[0] = new OracleParameter("P_VENDOR_ID", OracleDbType.Int64);
                arrParams[0].Value = vendor.VendorId;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 500);
                arrParams[1].Value = vendor.VendorCode;
                arrParams[2] = new OracleParameter("P_VENDOR_NAME", OracleDbType.Varchar2, 500);
                arrParams[2].Value = vendor.VendorName;
                arrParams[3] = new OracleParameter("P_EMAIL_ID", OracleDbType.Varchar2, 500);
                arrParams[3].Value = vendor.EmailId;
                arrParams[4] = new OracleParameter("P_MOBILE_NO", OracleDbType.Varchar2, 500);
                arrParams[4].Value = vendor.MobileNo;
                arrParams[5] = new OracleParameter("P_PAN_NO", OracleDbType.Varchar2, 500);
                arrParams[5].Value = vendor.PAN_No.ToUpper();
                arrParams[6] = new OracleParameter("P_GST_NO", OracleDbType.Varchar2, 500);
                arrParams[6].Value = vendor.GSTIN_No.ToUpper();
                arrParams[7] = new OracleParameter("P_MSME_NO", OracleDbType.Varchar2, 500);
                arrParams[7].Value = vendor.MSMERegNo.ToUpper();
                arrParams[8] = new OracleParameter("P_ENTERPRISES_TYPE", OracleDbType.Varchar2, 500);
                arrParams[8].Value = vendor.EnterprisesType;
                arrParams[9] = new OracleParameter("P_MAJOR_ACTIVITY", OracleDbType.Varchar2, 500);
                arrParams[9].Value = vendor.MajorActivity;
                arrParams[10] = new OracleParameter("P_ADDRESS", OracleDbType.Varchar2, 500);
                arrParams[10].Value = vendor.Address;
                arrParams[11] = new OracleParameter("P_CITY", OracleDbType.Varchar2, 500);
                arrParams[11].Value = vendor.City;
                arrParams[12] = new OracleParameter("P_STATE_ID", OracleDbType.Varchar2);
                arrParams[12].Value = vendor.State.StateId;
                arrParams[13] = new OracleParameter("P_PINCODE", OracleDbType.Varchar2);
                arrParams[13].Value = vendor.PinCode;
                arrParams[14] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                arrParams[14].Value = vendor.CreatedBy;
                arrParams[15] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 250);
                arrParams[15].Direction = ParameterDirection.Output;
                arrParams[16] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[16].Direction = ParameterDirection.Output;
                arrParams[17] = new OracleParameter("P_OUT_REQ_ID", OracleDbType.Varchar2, 100);
                arrParams[17].Direction = ParameterDirection.Output;
                arrParams[18] = new OracleParameter("P_PROFILE_DOC_PATH", OracleDbType.Varchar2, 500);
                arrParams[18].Value = vendor.ProfileDocPath;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorMaster.VENDOR_PROFILE_UPDATE_REQUEST, arrParams, DealerNet_Next_Connection);
                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[15].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[16].Value),
                    data = JsonConvert.SerializeObject(new
                    {
                        RequestId = SafeTypeHandling.ConvertToString(arrParams[17].Value)
                    })
                };
            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
            return response;
        }

        public async Task<List<Vendor>> ReadVendorProfileUpdateRequests(Vendor vendor)
        {
            List<Vendor> lstVendor = new List<Vendor>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int64);
                if (string.IsNullOrEmpty(vendor.Id.ToString()))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = vendor.Id;
                }
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(vendor.VendorCode))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = vendor.VendorCode;
                }
                arrParams[2] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2);
                arrParams[2].Value = vendor.CTRL_AU;
                arrParams[3] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[3].Value = vendor.Status;
                arrParams[4] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[4].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorMaster.VENDOR_PROFILE_UPDATE_REQUEST_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Vendor _vendor = new Vendor();
                    _vendor.Id = SafeTypeHandling.ConvertStringToInt64(row["ID"]);
                    _vendor.VendorCode = SafeTypeHandling.ConvertToString(row["VENDOR_CODE"]);
                    _vendor.VendorName = SafeTypeHandling.ConvertToString(row["VENDOR_NAME"]);
                    _vendor.MobileNo = SafeTypeHandling.ConvertToString(row["MOBILE_NO"]);
                    _vendor.EmailId = SafeTypeHandling.ConvertToString(row["EMAIL_ID"]);
                    //_vendor.CTRL_AU = SafeTypeHandling.ConvertToString(row["CTRL_AU"]);
                    //_vendor.PIC_Code = SafeTypeHandling.ConvertToString(row["PIC_CODE"]);
                    //_vendor.PIC_Name = SafeTypeHandling.ConvertToString(row["PIC_NAME"]);
                    //_vendor.PIC_MobileNo = SafeTypeHandling.ConvertToString(row["PIC_MOBILE_NO"]);

                    _vendor.PAN_No = SafeTypeHandling.ConvertToString(row["PAN_NO"]);
                    _vendor.GSTIN_No = SafeTypeHandling.ConvertToString(row["GST_NO"]);
                    _vendor.MSMERegNo = SafeTypeHandling.ConvertToString(row["MSME_NO"]);
                    _vendor.EnterprisesType = SafeTypeHandling.ConvertToString(row["ENTERPRISES_TYPE"]);
                    _vendor.MajorActivity = SafeTypeHandling.ConvertToString(row["MAJOR_ACTIVITY"]);
                    if (!string.IsNullOrEmpty(row["STATE_ID"].ToString()))
                    {
                        _vendor.State.StateId = SafeTypeHandling.ConvertStringToInt32(row["STATE_ID"]);
                    }
                    _vendor.State.StateName = SafeTypeHandling.ConvertToString(row["STATE_NAME"]);
                    _vendor.Address = SafeTypeHandling.ConvertToString(row["ADDRESS"]);
                    _vendor.City = SafeTypeHandling.ConvertToString(row["CITY"]);
                    _vendor.PinCode = SafeTypeHandling.ConvertToString(row["PINCODE"]);
                    _vendor.Remarks = SafeTypeHandling.ConvertToString(row["REMARKS"]);
                    _vendor.ProfileDocPath = SafeTypeHandling.ConvertToString(row["PROFILE_DOC_PATH"]);

                    _vendor.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    _vendor.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    _vendor.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    _vendor.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _vendor.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY"]);
                    _vendor.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);

                    lstVendor.Add(_vendor);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstVendor;
        }

        /// <summary>
        /// Vendor Profile Udpate Request Approval
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorProfileUpdateRequestApproval(Vendor vendor)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[7];
                arrParams[0] = new OracleParameter("P_REQUEST_ID", OracleDbType.Int64);
                arrParams[0].Value = Convert.ToInt32(vendor.Id);
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = vendor.VendorCode;
                arrParams[2] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 250);
                arrParams[2].Value = vendor.Status;
                arrParams[3] = new OracleParameter("P_REMARKS", OracleDbType.Varchar2, 500);
                arrParams[3].Value = vendor.Remarks;
                arrParams[4] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                arrParams[4].Value = vendor.CreatedBy;

                arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[6].Direction = ParameterDirection.Output;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorMaster.VENDOR_PROFILE_UPDATE_REQUEST_APPROVAL, arrParams, DealerNet_Next_Connection);
                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[5].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[6].Value)
                };
            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
            return response;
        }

    }
}