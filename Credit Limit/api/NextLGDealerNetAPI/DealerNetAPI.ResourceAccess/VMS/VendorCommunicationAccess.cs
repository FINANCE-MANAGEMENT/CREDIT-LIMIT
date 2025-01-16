using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.VMS
{
    public class VendorCommunicationAccess : IVendorCommunicationAccess
    {
        private readonly string DealerNet_Next_Connection = "DealerNetNextConnection";
        private readonly ICommonDB _commonDB = null;
        private readonly IUtilitiesVMSAccess _utilitiesVMSAccess = null;

        public VendorCommunicationAccess(ICommonDB commonDB, IUtilitiesVMSAccess utilitiesVMSAccess)
        {
            _commonDB = commonDB;
            _utilitiesVMSAccess = utilitiesVMSAccess;
        }

        /// <summary>
        /// Vendor Communication Template Registration
        /// </summary>
        /// <param name="vendorAcceptance"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorCommunicationTemplateRegistration(VendorCommunication vendorCommunication)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[8];
                arrParams[0] = new OracleParameter("P_TEMPLATE_ID", OracleDbType.Varchar2, 100);
                if (vendorCommunication.TemplateId == 0 || string.IsNullOrEmpty(Convert.ToString(vendorCommunication.TemplateId)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = vendorCommunication.TemplateId;
                }
                arrParams[1] = new OracleParameter("P_TEMPLATE_NAME", OracleDbType.Varchar2, 100);
                arrParams[1].Value = vendorCommunication.TemplateName;
                arrParams[2] = new OracleParameter("P_CONTENT", OracleDbType.Clob);
                arrParams[2].Value = vendorCommunication.TemplateContent;
                arrParams[3] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 100);
                arrParams[3].Value = vendorCommunication.Status;
                arrParams[4] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[4].Value = vendorCommunication.CreatedBy;
                arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[6].Direction = ParameterDirection.Output;
                arrParams[7] = new OracleParameter("P_OUT_TEMPLATE_ID", OracleDbType.Varchar2, 500);
                arrParams[7].Direction = ParameterDirection.Output;
                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.COMMUNICATION_TEMPLATE_REG_INSERT_UPDATE, arrParams, DealerNet_Next_Connection);

                // Check Response
                response = new APIResponse
                {
                    Status = arrParams[5].Value.ToString(),
                    StatusDesc = arrParams[6].Value.ToString(),
                };

                // Required Info Save
                if (response.Status == Utilities.SUCCESS)
                {
                    string templateId = arrParams[7].Value.ToString();
                    foreach (var lookupInfo in vendorCommunication.RequiredInfo)
                    {
                        OracleParameter[] arrParamsReqInfo = new OracleParameter[7];
                        arrParamsReqInfo[0] = new OracleParameter("P_TEMPLATE_ID", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[0].Value = templateId;
                        arrParamsReqInfo[1] = new OracleParameter("P_LOOKUPID", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[1].Value = lookupInfo.Id;
                        arrParamsReqInfo[2] = new OracleParameter("P_MANDATORY", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[2].Value = lookupInfo.isChecked == true ? "Y" : "N";
                        arrParamsReqInfo[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[3].Value = vendorCommunication.CreatedBy;
                        arrParamsReqInfo[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[4].Direction = ParameterDirection.Output;
                        arrParamsReqInfo[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                        arrParamsReqInfo[5].Direction = ParameterDirection.Output;
                        arrParamsReqInfo[6] = new OracleParameter("P_USE_FIELD", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[6].Value = lookupInfo.isUsed == true ? "Y" : "N";

                        DataTable dtReqInfo = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.COMMUNICATION_TEMPLATE_REQUIRED_INFO_INSERT_UPDATE, arrParamsReqInfo, DealerNet_Next_Connection);
                    }
                }
            }
            catch (Exception ex)
            {
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString(),
                };
            }
            return response;
        }

        public async Task<List<VendorCommunication>> ReadVendorCommunicationTemplate(VendorCommunication vendorCommunication)
        {
            List<VendorCommunication> lstTemplate = new List<VendorCommunication>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_TEMPLATE_ID", OracleDbType.Varchar2, 100);
                if (vendorCommunication.TemplateId == 0 || string.IsNullOrEmpty(Convert.ToString(vendorCommunication.TemplateId)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = vendorCommunication.TemplateId;
                }
                arrParams[1] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 100);
                arrParams[1].Value = vendorCommunication.Status;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.COMMUNICATION_TEMPLATE_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    VendorCommunication _template = new VendorCommunication();
                    _template.TemplateId = SafeTypeHandling.ConvertStringToInt32(row["TEMPLATE_ID"]);
                    _template.TemplateName = SafeTypeHandling.ConvertToString(row["TEMPLATE_NAME"]);
                    _template.TemplateContent = SafeTypeHandling.ConvertToString(row["CONTENT"]);
                    _template.TemplateSendStatus = SafeTypeHandling.ConvertToString(row["TEMPLATE_SEND_STATUS"]);
                    _template.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    _template.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    _template.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY"]);
                    _template.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    lstTemplate.Add(_template);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstTemplate;
        }

        public async Task<List<Lookup>> ReadCommunicationTemplateRequiredInfo(VendorCommunication vendorCommunication)
        {
            List<Lookup> lstTemplateReqInfo = new List<Lookup>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_TEMPLATE_ID", OracleDbType.Varchar2, 100);
                if (vendorCommunication.TemplateId == 0 || string.IsNullOrEmpty(Convert.ToString(vendorCommunication.TemplateId)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = vendorCommunication.TemplateId;
                }
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                if (vendorCommunication.Vendors == null)
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = vendorCommunication.Vendors[0].VendorCode;
                }
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.COMMUNICATION_TEMPLATE_REQUIRED_INFO_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Lookup _lookup = new Lookup();
                    _lookup.Id = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    _lookup.LookupId = SafeTypeHandling.ConvertStringToInt32(row["LOOKUP_ID"]);
                    _lookup.LookupName = SafeTypeHandling.ConvertToString(row["LOOKUP_NAME"]);
                    _lookup.LookupValue = SafeTypeHandling.ConvertToString(row["LOOKUP_VAL"]);
                    _lookup.isChecked = SafeTypeHandling.ConvertStringToBoolean(row["CHECKED_STATUS"]);
                    _lookup.isUsed = SafeTypeHandling.ConvertStringToBoolean(row["USE_FIELD_STATUS"]);
                    _lookup.Value1 = SafeTypeHandling.ConvertToString(row["REQ_INFO_VALUE"]);
                    lstTemplateReqInfo.Add(_lookup);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstTemplateReqInfo;
        }


        /// <summary>
        /// Communication Send To Vendor
        /// </summary>
        /// <param name="vendorCommunication"></param>
        /// <returns></returns>
        public async Task<APIResponse> CommunicationSendToVendor(VendorCommunication vendorCommunication)
        {
            APIResponse response = null;
            try
            {
                foreach (var _vendor in vendorCommunication.Vendors)
                {
                    OracleParameter[] arrParams = new OracleParameter[6];
                    arrParams[0] = new OracleParameter("P_TEMPLATE_ID", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = vendorCommunication.TemplateId;
                    arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = _vendor.VendorCode;
                    arrParams[2] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = vendorCommunication.CreatedBy;
                    arrParams[3] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[3].Direction = ParameterDirection.Output;
                    arrParams[4] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[4].Direction = ParameterDirection.Output;
                    arrParams[5] = new OracleParameter("P_OUT_VENDOR_MOBILE_NO", OracleDbType.Varchar2, 250);
                    arrParams[5].Direction = ParameterDirection.Output;
                    DataTable dtReqInfo = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.COMMUNICATION_SEND_TO_VENDOR_INSERT, arrParams, DealerNet_Next_Connection);

                    string vendorMobileNo = SafeTypeHandling.ConvertToString(arrParams[5].Value);
                    response = new APIResponse
                    {
                        Status = SafeTypeHandling.ConvertToString(arrParams[3].Value),
                        StatusDesc = SafeTypeHandling.ConvertToString(arrParams[4].Value),
                    };

                    #region Vendor/Supplier Communication send to Vendor (SMS Send)

                    if (response.Status == Utilities.SUCCESS && !string.IsNullOrEmpty(vendorMobileNo))
                    {
                        try
                        {
                            string SMSTemplateName = "VMS_PARTNER_COMMUNICATION"; // Template Name in SMS_MST_TB

                            // SMS Template & message read
                            SmsData smsDLT = _utilitiesVMSAccess.SMS_DLT_DetailRead(SMSTemplateName);
                            if (smsDLT != null)
                            {
                                OTP otp = new OTP();
                                otp.MobileNo = vendorMobileNo;
                                otp.CreatedBy = vendorCommunication.CreatedBy;
                                otp.ProcessName = "COMMUNICATION_SEND_TO_VENDOR";

                                string message = smsDLT.MESSAGE.Replace("{var1}{var2}", "We have initiated a new communication for you"); // SMS body change
                                var smsResp = _utilitiesVMSAccess.SMSSend(smsDLT, otp, SMSTemplateName, otp.MobileNo, message);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.CreateLogFile(Utilities.PROJECTS.VMS, vendorMobileNo + "," + "Vendor communication Send, SMS Sending Issues. " + "#" + ex);
                        }
                    }

                    #endregion

                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex.ToString());
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString(),
                };
            }
            return response;
        }


        /// <summary>
        /// Vendor Upload for Communication
        /// </summary>
        /// <param name="vendors"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorUploadForCommunication(List<Vendor> vendors)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (var vendor in vendors)
            {
                try
                {
                    arrParams = new OracleParameter[12];
                    arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = vendor.VendorCode;
                    arrParams[2] = new OracleParameter("P_VENDOR_NAME", OracleDbType.Varchar2, 250);
                    arrParams[2].Value = vendor.VendorName;
                    arrParams[3] = new OracleParameter("P_CRTL_AU", OracleDbType.Varchar2, 100);
                    arrParams[3].Value = vendor.CTRL_AU;
                    arrParams[6] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[6].Value = vendor.CreatedBy;
                    arrParams[7] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[7].Value = vendor.RowNumber;
                    arrParams[10] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[10].Direction = ParameterDirection.Output;
                    arrParams[11] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[11].Direction = ParameterDirection.Output;
                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.VENDOR_UPLOAD_FOR_COMMUNICATION_TEMP_INSERT, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[10].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = arrParams[11].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", vendor.RowNumber, ": ", ex.ToString()) });
                }
            }
            // if Error Found while data inset into Temp table
            if (errors.Count > 0)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES,
                    data = errors
                };
                return response;
            }

            Int32 createdBy = (Int32)vendors[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.VENDOR_UPLOAD_FOR_COMMUNICATION_TEMP_VALIDATE, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    errors.Add(new Errors { Error = SafeTypeHandling.ConvertToString(row["REMARKS"]) });
                }

                // if Error Found while validate Temp table data
                if (errors.Count > 0)
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = Utilities.EXCEL_DATA_VALIDATION_FAILED,
                        data = errors
                    };
                    return response;
                }
                else
                {
                    response = new APIResponse
                    {
                        Status = Utilities.SUCCESS,
                        StatusDesc = Utilities.RECORD_SBUMITTED_SUCCESSFULLY,
                        data = errors
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString(),
                };
                return response;
            }


        }

        public async Task<List<VendorCommunication>> ReadVendorCommunicationSendTemplate(VendorCommunication vendorCommunication)
        {
            List<VendorCommunication> lstTemplate = new List<VendorCommunication>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_TEMPLATE_ID", OracleDbType.Varchar2, 100);
                if (vendorCommunication.TemplateId == 0 || string.IsNullOrEmpty(Convert.ToString(vendorCommunication.TemplateId)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = vendorCommunication.TemplateId;
                }
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = vendorCommunication.Vendors[0].VendorCode;
                arrParams[2] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2, 100);
                arrParams[2].Value = vendorCommunication.Vendors[0].CTRL_AU;
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.VENDOR_COMMUNICATION_SEND_TEMPLATE_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    VendorCommunication _template = new VendorCommunication();
                    _template.TemplateId = SafeTypeHandling.ConvertStringToInt32(row["TEMPLATE_ID"]);
                    _template.TemplateName = SafeTypeHandling.ConvertToString(row["TEMPLATE_NAME"]);
                    _template.TemplateContent = SafeTypeHandling.ConvertToString(row["CONTENT"]);

                    Vendor vendor = new Vendor();
                    vendor.VendorCode = SafeTypeHandling.ConvertToString(row["VENDOR_CODE"]);
                    vendor.VendorName = SafeTypeHandling.ConvertToString(row["VENDOR_NAME"]);
                    vendor.CTRL_AU = SafeTypeHandling.ConvertToString(row["CTRL_AU"]);
                    _template.Vendors.Add(vendor);

                    _template.CommunicationAcceptance = SafeTypeHandling.ConvertToString(row["VENDOR_ACCEPTANCE"]);
                    if (!string.IsNullOrEmpty(row["COMMUNICATION_SEND_DATE"].ToString()))
                    {
                        _template.CommunicationSendDate = SafeTypeHandling.ConvertToDateTime(row["COMMUNICATION_SEND_DATE"]);
                    }
                    if (!string.IsNullOrEmpty(row["VENDOR_ACCEPTANCE_DATE"].ToString()))
                    {
                        _template.CommunicationAcceptanceDate = SafeTypeHandling.ConvertToDateTime(row["VENDOR_ACCEPTANCE_DATE"]);
                    }
                    lstTemplate.Add(_template);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstTemplate;
        }

        /// <summary>
        /// Vendor Communication Acceptance & Required Info Save
        /// </summary>
        /// <param name="vendorCommunication"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorCommunicationAcceptance(VendorCommunication vendorCommunication)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_TEMPLATE_ID", OracleDbType.Varchar2, 100);
                arrParams[0].Value = vendorCommunication.TemplateId;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = vendorCommunication.Vendors[0].VendorCode;
                arrParams[2] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[2].Value = vendorCommunication.CreatedBy;
                arrParams[3] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[3].Direction = ParameterDirection.Output;
                arrParams[4] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[4].Direction = ParameterDirection.Output;
                arrParams[5] = new OracleParameter("P_OUT_VENDOR_MOBILE_NO", OracleDbType.Varchar2, 250);
                arrParams[5].Direction = ParameterDirection.Output;
                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.VENDOR_COMMUNICATION_ACCEPTANCE_SAVE, arrParams, DealerNet_Next_Connection);

                string vendorMobileNo = SafeTypeHandling.ConvertToString(arrParams[5].Value);

                // Check Response
                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[3].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[4].Value),
                };

                if (response.Status == Utilities.SUCCESS)
                {
                    // Required Info Save
                    foreach (var lookupInfo in vendorCommunication.RequiredInfo)
                    {
                        OracleParameter[] arrParamsReqInfo = new OracleParameter[7];
                        arrParamsReqInfo[0] = new OracleParameter("P_TEMPLATE_ID", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[0].Value = vendorCommunication.TemplateId;
                        arrParamsReqInfo[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[1].Value = vendorCommunication.Vendors[0].VendorCode;
                        arrParamsReqInfo[2] = new OracleParameter("P_LOOKUPID", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[2].Value = lookupInfo.Id;
                        arrParamsReqInfo[3] = new OracleParameter("P_LOOKUP_VALUE", OracleDbType.Varchar2, 500);
                        arrParamsReqInfo[3].Value = lookupInfo.Value1;
                        arrParamsReqInfo[4] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[4].Value = vendorCommunication.CreatedBy;
                        arrParamsReqInfo[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                        arrParamsReqInfo[5].Direction = ParameterDirection.Output;
                        arrParamsReqInfo[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                        arrParamsReqInfo[6].Direction = ParameterDirection.Output;
                        DataTable dtReqInfo = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.VENDOR_COMMUNICATION_ACCEPTANCE_REQ_INFO_SAVE, arrParamsReqInfo, DealerNet_Next_Connection);
                    }

                    #region Vendor/Supplier Communication Acceptance (SMS Send)

                    if (response.Status == Utilities.SUCCESS && !string.IsNullOrEmpty(vendorMobileNo))
                    {
                        try
                        {
                            string SMSTemplateName = "VMS_PARTNER_COMMUNICATION"; // Template Name in SMS_MST_TB

                            // SMS Template & message read
                            SmsData smsDLT = _utilitiesVMSAccess.SMS_DLT_DetailRead(SMSTemplateName);
                            if (smsDLT != null)
                            {
                                OTP otp = new OTP();
                                otp.MobileNo = vendorMobileNo;
                                otp.CreatedBy = vendorCommunication.CreatedBy;
                                otp.ProcessName = "COMMUNICATION_ACCEPTANCE_BY_VENDOR";

                                string message = smsDLT.MESSAGE.Replace("{var1}{var2}", "Thank you for replying the communication sent by us"); // SMS body change
                                var smsResp = _utilitiesVMSAccess.SMSSend(smsDLT, otp, SMSTemplateName, otp.MobileNo, message);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.CreateLogFile(Utilities.PROJECTS.VMS, vendorMobileNo + "," + "Vendor communication Acceptance, SMS Sending Issues. " + "#" + ex);
                        }
                    }

                    #endregion


                    // Email send to Vendor
                    try
                    {
                        OracleParameter[] arrParamsEmail = new OracleParameter[4];
                        arrParamsEmail[0] = new OracleParameter("P_TEMPLATE_ID", OracleDbType.Varchar2, 100);
                        arrParamsEmail[0].Value = vendorCommunication.TemplateId;
                        arrParamsEmail[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                        arrParamsEmail[1].Value = vendorCommunication.Vendors[0].VendorCode;
                        arrParamsEmail[2] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                        arrParamsEmail[2].Direction = ParameterDirection.Output;
                        arrParamsEmail[3] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                        arrParamsEmail[3].Direction = ParameterDirection.Output;
                        DataTable dtEmail = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorCommunication.VENDOR_COMMUNICATION_ACCEPTANCE_EMAIL_SEND, arrParamsEmail, DealerNet_Next_Connection);
                    }
                    catch (Exception)
                    {
                        
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString(),
                };
            }
            return response;
        }
    }
}
