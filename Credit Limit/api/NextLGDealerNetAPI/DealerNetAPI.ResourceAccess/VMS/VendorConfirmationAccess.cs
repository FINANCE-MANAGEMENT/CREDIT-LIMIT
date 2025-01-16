using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.VMS
{
    public class VendorConfirmationAccess : IVendorConfirmationAccess
    {
        private readonly string DealerNet_Next_Connection = "DealerNetNextConnection";
        private readonly ICommonDB _commonDB = null;
        private readonly IUtilitiesVMSAccess _utilitiesVMSAccess = null;

        public VendorConfirmationAccess(ICommonDB commonDB, IUtilitiesVMSAccess utilitiesVMSAccess)
        {
            _commonDB = commonDB;
            _utilitiesVMSAccess = utilitiesVMSAccess;
        }

        /// <summary>
        /// Save Bulk confirmation Invoices by Admin
        /// </summary>
        /// <param name="vendorInvoices"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorInvoiceUpload(List<VendorInvoice> vendorInvoices)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (var invoice in vendorInvoices)
            {
                try
                {
                    arrParams = new OracleParameter[12];
                    arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = invoice.ConfirmationPeriod;
                    arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = invoice.Vendors.VendorCode;
                    arrParams[2] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = invoice.BranchCode;
                    arrParams[3] = new OracleParameter("P_INVOICE_NO", OracleDbType.Varchar2, 250);
                    arrParams[3].Value = Convert.ToString(invoice.InvoiceNo).Trim();
                    arrParams[4] = new OracleParameter("P_INVOICE_DATE", OracleDbType.Date);
                    if (invoice.InvoiceDate == null)
                    {
                        arrParams[4].Value = DBNull.Value;
                    }
                    else
                    {
                        arrParams[4].Value = invoice.InvoiceDate;
                    }
                    arrParams[5] = new OracleParameter("P_CLOSING_BAL", OracleDbType.Decimal);
                    arrParams[5].Value = invoice.ClosingBalance;
                    arrParams[6] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[6].Value = invoice.CreatedBy;
                    arrParams[7] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[7].Value = invoice.RowNumber;
                    arrParams[8] = new OracleParameter("P_LOCAL_IP", OracleDbType.Varchar2, 50);
                    arrParams[8].Value = invoice.LocalIP;
                    arrParams[9] = new OracleParameter("P_PUBLIC_IP", OracleDbType.Varchar2, 50);
                    arrParams[9].Value = invoice.PublicIP;
                    arrParams[10] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[10].Direction = ParameterDirection.Output;
                    arrParams[11] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[11].Direction = ParameterDirection.Output;
                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_INSERT_TEMP, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[10].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = arrParams[11].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", invoice.RowNumber, ": ", ex.ToString()) });
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

            Int32 createdBy = (Int32)vendorInvoices[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_TEMP_DATA_VALIDATE, arrParams, DealerNet_Next_Connection);
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

            // Finally records submit in Main Table
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_INSERT, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    errors.Add(new Errors { Error = SafeTypeHandling.ConvertToString(row["REMARKS"]) });
                }

                // if Error Found while insert invoice in Main table
                if (errors.Count > 0)
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = Utilities.SOME_ERROR_OCCURED_WHILE_SUBMIT_RECORD,
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

        /// <summary>
        /// Read Vendor Invoice
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        public async Task<List<VendorInvoice>> ReadVendorInvoice(VendorInvoice vendorInvoice)
        {
            List<VendorInvoice> lstVendorInvoice = new List<VendorInvoice>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2);
                arrParams[0].Value = vendorInvoice.ConfirmationPeriod;
                arrParams[1] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(vendorInvoice.BranchCode)))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = vendorInvoice.BranchCode;
                }
                arrParams[2] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(vendorInvoice.Vendors.VendorCode)))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = vendorInvoice.Vendors.VendorCode;
                }
                arrParams[3] = new OracleParameter("P_ADMIN_APPROVED_STATUS", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(vendorInvoice.Status)))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = vendorInvoice.Status;
                }
                arrParams[4] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[4].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    VendorInvoice _vendorInvoice = new VendorInvoice();
                    _vendorInvoice.ConfirmationPeriod = SafeTypeHandling.ConvertToString(row["QUARTER_PERIOD"]);
                    _vendorInvoice.Vendors.VendorCode = SafeTypeHandling.ConvertToString(row["VENDOR_CODE"]);
                    _vendorInvoice.Vendors.VendorName = SafeTypeHandling.ConvertToString(row["VENDOR_NAME"]);
                    _vendorInvoice.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    _vendorInvoice.InvoiceNo = SafeTypeHandling.ConvertToString(row["INVOICE_NO"]);
                    if (!string.IsNullOrEmpty(SafeTypeHandling.ConvertToString(row["INVOICE_DATE"])))
                    {
                        _vendorInvoice.InvoiceDate = SafeTypeHandling.ConvertToDateTime(row["INVOICE_DATE"]);
                    }
                    _vendorInvoice.ClosingBalance = SafeTypeHandling.ConvertStringToDecimal(row["CLOSING_BALANCE"]);
                    _vendorInvoice.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    _vendorInvoice.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _vendorInvoice.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY"]);
                    _vendorInvoice.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    _vendorInvoice.LocalIP = SafeTypeHandling.ConvertToString(row["LOCAL_IP"]);
                    _vendorInvoice.AdminApprovedStatus = SafeTypeHandling.ConvertStringToBoolean(SafeTypeHandling.ConvertToString(row["INVOICE_APPROVED_STATUS"]) == "Y" ? true : false);
                    lstVendorInvoice.Add(_vendorInvoice);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstVendorInvoice;
        }

        /// <summary>
        /// Read Vendor Invoice Summary
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        public async Task<List<VendorInvoice>> ReadVendorInvoiceSummary(VendorInvoice vendorInvoice)
        {
            List<VendorInvoice> lstVendorInvoiceSummary = new List<VendorInvoice>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2);
                arrParams[0].Value = vendorInvoice.ConfirmationPeriod;
                arrParams[1] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(vendorInvoice.BranchCode)))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = vendorInvoice.BranchCode;
                }
                arrParams[2] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(vendorInvoice.Vendors.VendorCode)))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = vendorInvoice.Vendors.VendorCode;
                }
                arrParams[3] = new OracleParameter("P_ADMIN_APPROVED_STATUS", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(vendorInvoice.Status)))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = vendorInvoice.Status;
                }
                arrParams[4] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[4].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_SUMMARY_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    VendorInvoice _vendorInvoice = new VendorInvoice();
                    _vendorInvoice.ConfirmationPeriod = SafeTypeHandling.ConvertToString(row["QUARTER_PERIOD"]);
                    _vendorInvoice.Vendors.VendorCode = SafeTypeHandling.ConvertToString(row["VENDOR_CODE"]);
                    _vendorInvoice.Vendors.VendorName = SafeTypeHandling.ConvertToString(row["VENDOR_NAME"]);
                    _vendorInvoice.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    _vendorInvoice.ClosingBalance = SafeTypeHandling.ConvertStringToDecimal(row["TOTAL_CLOSING_BALANCE"]);
                    _vendorInvoice.AdminApprovedStatus = SafeTypeHandling.ConvertStringToBoolean(SafeTypeHandling.ConvertToString(row["INVOICE_APPROVED_STATUS"]) == "Y" ? true : false);
                    _vendorInvoice.EmailSend = SafeTypeHandling.ConvertToString(row["LG_INVOICE_EMAIL_SENT_VENDOR"]);
                    lstVendorInvoiceSummary.Add(_vendorInvoice);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                throw;
            }
            return lstVendorInvoiceSummary;
        }

        /// <summary>
        /// Vendor Invoice Verify & Approve by Admin
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorInvoiceVerifyApprove(VendorInvoice vendor, Int32 createdBy)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;
            try
            {
                arrParams = new OracleParameter[9];
                arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2, 100);
                arrParams[0].Value = vendor.ConfirmationPeriod;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = vendor.Vendors.VendorCode;
                arrParams[2] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2, 100);
                arrParams[2].Value = vendor.BranchCode;
                arrParams[3] = new OracleParameter("P_APPROVED_STATUS", OracleDbType.Varchar2, 100);
                arrParams[3].Value = (vendor.AdminApprovedStatus == true ? "Y" : "N");
                arrParams[4] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[4].Value = createdBy;
                arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[6].Direction = ParameterDirection.Output;
                arrParams[7] = new OracleParameter("P_OUT_VENDOR_EMAIL", OracleDbType.Varchar2, 250);
                arrParams[7].Direction = ParameterDirection.Output;
                arrParams[8] = new OracleParameter("P_OUT_VENDOR_MOBILE", OracleDbType.Varchar2, 250);
                arrParams[8].Direction = ParameterDirection.Output;
                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_APPROVED_BY_ADMIN, arrParams, DealerNet_Next_Connection);
                // Check Response
                if (SafeTypeHandling.ConvertToString(arrParams[5].Value).Equals(Utilities.SUCCESS))
                {
                    response = new APIResponse
                    {
                        Status = Utilities.SUCCESS,
                        StatusDesc = SafeTypeHandling.ConvertToString(arrParams[6].Value),
                        data = new Vendor
                        {
                            EmailId = SafeTypeHandling.ConvertToString(arrParams[7].Value),
                            MobileNo = SafeTypeHandling.ConvertToString(arrParams[8].Value)
                        }
                    };
                }
                else
                {
                    errors.Add(new Errors { Error = SafeTypeHandling.ConvertToString(arrParams[6].Value) });
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = SafeTypeHandling.ConvertToString(arrParams[6].Value),
                        data = errors
                    };
                }
                return response;
            }
            catch (Exception ex)
            {
                errors.Add(new Errors
                {
                    Error = string.Concat("Confirmation Period: ", vendor.ConfirmationPeriod,
                                                              ", Vendor Code: ", vendor.Vendors.VendorCode,
                                                              ", Branch: ", vendor.BranchCode, " => ", ex.ToString())
                });
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = "Some error occcured.",
                    data = errors
                };
                return response;
            }
        }

        /// <summary>
        /// Vendor Invoice Approved by Admin, ROLLBACK
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorInvoiceApprovedRollback(List<VendorInvoice> vendors, Int32 createdBy)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            foreach (var vendor in vendors)
            {
                try
                {
                    OracleParameter[] arrParams = new OracleParameter[5];
                    arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = vendor.ConfirmationPeriod;
                    arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = vendor.Vendors.VendorCode;
                    arrParams[2] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = createdBy;
                    arrParams[3] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[3].Direction = ParameterDirection.Output;
                    arrParams[4] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[4].Direction = ParameterDirection.Output;
                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_APPROVED_BY_ADMIN_ROLLBACK, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[3].Value.ToString().Equals(Utilities.SUCCESS))
                    {

                    }
                    else
                    {
                        errors.Add(new Errors { Error = arrParams[4].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors
                    {
                        Error = string.Concat("Confirmation Period: ", vendor.ConfirmationPeriod,
                                                    ", Vendor Code: ", vendor.Vendors.VendorCode, " => ", ex.ToString())
                    });
                }
            }

            if (errors.Count > 0)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = "Some error occcured, while rollback the approved invoices. kinldy co-ordinate with System Administrator.",
                    data = errors
                };
            }
            else
            {
                response = new APIResponse
                {
                    Status = Utilities.SUCCESS,
                    StatusDesc = "Invoice approved rollback succcessfully...",
                    data = errors
                };
            }

            return response;
        }

        public async Task<APIResponse> LGInvoiceEmailSendToVendorStatusUpdate(VendorInvoice vendor, Int32 createdBy)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2, 100);
                arrParams[0].Value = vendor.ConfirmationPeriod;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = vendor.Vendors.VendorCode;
                arrParams[2] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 100);
                arrParams[2].Value = vendor.EmailSend;
                arrParams[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[3].Value = createdBy;
                arrParams[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[4].Direction = ParameterDirection.Output;
                arrParams[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;
                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_SENT_EMAIL_STATUS_UPDATE, arrParams, DealerNet_Next_Connection);
                // Check Response
                if (arrParams[4].Value.ToString().Equals(Utilities.ERROR))
                {
                    errors.Add(new Errors { Error = arrParams[5].Value.ToString() });
                }
            }
            catch (Exception ex)
            {
                errors.Add(new Errors
                {
                    Error = string.Concat("Confirmation Period: ", vendor.ConfirmationPeriod,
                                                ", Vendor Code: ", vendor.Vendors.VendorCode, " => ", ex.ToString())
                });
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
            }

            if (errors.Count > 0)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = "Some error occcured, while Email sent status update.",
                    data = errors
                };
            }
            else
            {
                response = new APIResponse
                {
                    Status = Utilities.SUCCESS,
                    StatusDesc = "LG Invoice dentails sent on Email succcessfully...",
                    data = errors
                };
            }

            return response;
        }



        /// <summary>
        /// Vendor Claim Amounts
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorClaimsAmount(VendorInvoice vendorClaims)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[11];
                arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2, 100);
                arrParams[0].Value = vendorClaims.ConfirmationPeriod;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = vendorClaims.Vendors.VendorCode;
                arrParams[2] = new OracleParameter("P_LG_CONFIRMED_AMT", OracleDbType.Decimal);
                arrParams[2].Value = vendorClaims.LGConfirmedAmount;
                arrParams[3] = new OracleParameter("P_RECEIVABLE_AMT", OracleDbType.Decimal);
                arrParams[3].Value = vendorClaims.TotalConfirmedAmount;
                arrParams[4] = new OracleParameter("P_CLAIM_AMT", OracleDbType.Decimal);
                arrParams[4].Value = vendorClaims.VendorClaimAmount;
                arrParams[5] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[5].Value = vendorClaims.CreatedBy;
                arrParams[6] = new OracleParameter("P_LOCAL_IP", OracleDbType.Varchar2, 50);
                arrParams[6].Value = vendorClaims.LocalIP;
                arrParams[7] = new OracleParameter("P_PUBLIC_IP", OracleDbType.Varchar2, 50);
                arrParams[7].Value = vendorClaims.PublicIP;
                arrParams[8] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[8].Direction = ParameterDirection.Output;
                arrParams[9] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[9].Direction = ParameterDirection.Output;
                arrParams[10] = new OracleParameter("P_OUT_VENDOR_MOBILE_NO", OracleDbType.Varchar2, 250);
                arrParams[10].Direction = ParameterDirection.Output;
                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_CLAIM_AMOUNTS_INSERT_UPDATE, arrParams, DealerNet_Next_Connection);

                string vendorMobileNo = SafeTypeHandling.ConvertToString(arrParams[10].Value);
                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[8].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[9].Value)
                };


                #region Providing any confirmation By Vendor/Supplier (SMS Send), If Not any claim

                if (response.Status == Utilities.SUCCESS && !string.IsNullOrEmpty(vendorMobileNo) && vendorClaims.VendorClaimAmount == 0)
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
                            otp.CreatedBy = vendorClaims.CreatedBy;
                            otp.LocalIP = vendorClaims.LocalIP;
                            otp.PublicIP = vendorClaims.PublicIP;
                            otp.ProcessName = "VENDOR_CONFIRMATION";

                            // Quarter end date prepared
                            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(vendorClaims.ConfirmationPeriod.Substring(4)));
                            var lastDayOfMonth = DateTime.DaysInMonth(Convert.ToInt32(vendorClaims.ConfirmationPeriod.Substring(0, 4)), Convert.ToInt32(vendorClaims.ConfirmationPeriod.Substring(4)));
                            string quarterEndDate = string.Concat(lastDayOfMonth, "-", monthName.Substring(0, 3), "-", vendorClaims.ConfirmationPeriod.Substring(0, 4)); // dd-MMM-yyyy

                            string message = smsDLT.MESSAGE.Replace("{var1}{var2}", "Thank you for providing confirmation as on " + quarterEndDate); // SMS body change
                            var smsResp = _utilitiesVMSAccess.SMSSend(smsDLT, otp, SMSTemplateName, otp.MobileNo, message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.CreateLogFile(Utilities.PROJECTS.VMS, vendorMobileNo + "," + "Providing any confirmation By Vendor/Supplier No Calim, SMS Sending Issues. " + "#" + ex);
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
                    StatusDesc = Utilities.SOME_ERROR_OCCURED_WHILE_SUBMIT_RECORD,
                };
            }
            return response;
        }


        /// <summary>
        /// Read Vendor/ Supplier Claim Amounts
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        public async Task<VendorInvoice> ReadVendorClaimsAmount(VendorInvoice vendorInvoice)
        {
            VendorInvoice vendorClaims = new VendorInvoice();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2);
                arrParams[0].Value = vendorInvoice.ConfirmationPeriod;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2);
                arrParams[1].Value = vendorInvoice.Vendors.VendorCode;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_CLAIM_AMOUNTS_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    vendorClaims.ConfirmationPeriod = SafeTypeHandling.ConvertToString(row["QUARTER_PERIOD"]);
                    vendorClaims.Vendors.VendorCode = SafeTypeHandling.ConvertToString(row["VENDOR_CODE"]);
                    vendorClaims.Vendors.VendorName = SafeTypeHandling.ConvertToString(row["VENDOR_NAME"]);
                    vendorClaims.Vendors.CTRL_AU = SafeTypeHandling.ConvertToString(row["CTRL_AU"]);
                    vendorClaims.LGConfirmedAmount = SafeTypeHandling.ConvertStringToDecimal(row["LG_CONFIRMED_AMT"]);
                    vendorClaims.TotalConfirmedAmount = SafeTypeHandling.ConvertStringToDecimal(row["RECEIVABLE_AMT"]);
                    vendorClaims.VendorClaimAmount = SafeTypeHandling.ConvertStringToDecimal(row["CLAIM_AMOUNT"]);
                    vendorClaims.VendorAcceptance = SafeTypeHandling.ConvertToString(row["VENDOR_ACCEPTANCE"]);
                    vendorClaims.VendorAcceptanceDate = SafeTypeHandling.ConvertToDateTime(row["VENDOR_ACCEPTANCE_DATE"]);
                    vendorClaims.BAMClaimReplied = SafeTypeHandling.ConvertToString(row["BAM_CLAIM_REPLIED"]);
                    vendorClaims.BAMClaimRepliedDate = SafeTypeHandling.ConvertToDateTime(row["BAM_CLAIM_REPLIED_DATE"]);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return vendorClaims;
        }



        /// <summary>
        /// Save Bulk confirmation Invoices by Vendor/ Supplier
        /// </summary>
        /// <param name="vendorInvoices"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorInvoiceAddedByVendor(List<VendorInvoice> vendorInvoices)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (VendorInvoice invoice in vendorInvoices)
            {
                try
                {
                    arrParams = new OracleParameter[16];
                    arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = invoice.ConfirmationPeriod;
                    arrParams[1] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = invoice.BranchCode;
                    arrParams[2] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = invoice.Vendors.VendorCode;
                    arrParams[3] = new OracleParameter("P_INVOICE_NO", OracleDbType.Varchar2, 250);
                    arrParams[3].Value = Convert.ToString(invoice.InvoiceNo).Trim();
                    arrParams[4] = new OracleParameter("P_INVOICE_DATE", OracleDbType.Date);
                    arrParams[4].Value = invoice.InvoiceDate;
                    arrParams[5] = new OracleParameter("P_INVOICE_AMT", OracleDbType.Decimal);
                    arrParams[5].Value = invoice.InvoiceAmount;
                    arrParams[6] = new OracleParameter("P_LGPO_NO", OracleDbType.Varchar2, 250);
                    arrParams[6].Value = invoice.LG_PO_No;
                    arrParams[7] = new OracleParameter("P_PAYMENT_REC", OracleDbType.Decimal);
                    arrParams[7].Value = invoice.ReceivedPaymentAmount;
                    arrParams[8] = new OracleParameter("P_BALANCE", OracleDbType.Decimal);
                    arrParams[8].Value = invoice.Balance;
                    arrParams[9] = new OracleParameter("P_VENDOR_REMARKS", OracleDbType.Varchar2, 500);
                    if (string.IsNullOrEmpty(invoice.VendorRemarks))
                    {
                        arrParams[9].Value = invoice.VendorRemarks;
                    }
                    else
                    {
                        arrParams[9].Value = invoice.VendorRemarks.Replace("\r\n", " ").Replace("\t", " ").Replace("\r", " ").Replace("\n", " ");
                    }
                    arrParams[10] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[10].Value = invoice.CreatedBy;
                    arrParams[11] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[11].Value = invoice.RowNumber;
                    arrParams[12] = new OracleParameter("P_LOCAL_IP", OracleDbType.Varchar2, 50);
                    arrParams[12].Value = invoice.LocalIP;
                    arrParams[13] = new OracleParameter("P_PUBLIC_IP", OracleDbType.Varchar2, 50);
                    arrParams[13].Value = invoice.PublicIP;
                    arrParams[14] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[14].Direction = ParameterDirection.Output;
                    arrParams[15] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[15].Direction = ParameterDirection.Output;
                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_ADDED_BY_VENDOR_INSERT_TEMP, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[14].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = SafeTypeHandling.ConvertToString(arrParams[15].Value) });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", invoice.RowNumber, ": ", ex.ToString()) });
                }
            }
            // if Error Found while data inset into Temp table
            if (errors.Count > 0)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = Utilities.DATA_VALIDATION_FAILED,
                    data = errors
                };
                return response;
            }

            Int32 createdBy = (Int32)vendorInvoices[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_ADDED_BY_VENDOR_TEMP_DATA_VALIDATE, arrParams, DealerNet_Next_Connection);
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

            // Finally records submit in Main Table
            try
            {
                arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT_VENDOR_MOBILE_NO", OracleDbType.Varchar2, 250);
                arrParams[1].Direction = ParameterDirection.Output;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_ADDED_BY_VENDOR_INSERT, arrParams, DealerNet_Next_Connection);

                string vendorMobileNo = SafeTypeHandling.ConvertToString(arrParams[1].Value);
                foreach (DataRow row in dtData.Rows)
                {
                    errors.Add(new Errors { Error = SafeTypeHandling.ConvertToString(row["REMARKS"]) });
                }

                // if Error Found while insert invoice in Main table
                if (errors.Count > 0)
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = Utilities.SOME_ERROR_OCCURED_WHILE_SUBMIT_RECORD,
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

                    #region Providing any confirmation By Vendor/Supplier (SMS Send), when claim available

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
                                otp.CreatedBy = createdBy;
                                otp.ProcessName = "VENDOR_CONFIRMATION";

                                string confirmationPeriod = vendorInvoices[0].ConfirmationPeriod;

                                // Quarter end date prepared
                                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(confirmationPeriod.Substring(4)));
                                var lastDayOfMonth = DateTime.DaysInMonth(Convert.ToInt32(confirmationPeriod.Substring(0, 4)), Convert.ToInt32(confirmationPeriod.Substring(4)));
                                string quarterEndDate = string.Concat(lastDayOfMonth, "-", monthName.Substring(0, 3), "-", confirmationPeriod.Substring(0, 4)); // dd-MMM-yyyy

                                string message = smsDLT.MESSAGE.Replace("{var1}{var2}", "Thank you for providing confirmation as on " + quarterEndDate); // SMS body change
                                var smsResp = _utilitiesVMSAccess.SMSSend(smsDLT, otp, SMSTemplateName, otp.MobileNo, message);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.CreateLogFile(Utilities.PROJECTS.VMS, vendorMobileNo + "," + "Providing any confirmation By Vendor/Supplier No Calim, SMS Sending Issues. " + "#" + ex);
                        }
                    }

                    #endregion

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

        /// <summary>
        /// Read Vendor/ Supplier Invoice Details which is Upload/Added by Vendor
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        public async Task<List<VendorInvoice>> ReadVendorInvoiceAddedByVendor(VendorInvoice vendorInvoice)
        {
            List<VendorInvoice> lstVendorInvoice = new List<VendorInvoice>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2);
                arrParams[0].Value = vendorInvoice.ConfirmationPeriod;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(vendorInvoice.Vendors.VendorCode)))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = vendorInvoice.Vendors.VendorCode;
                }
                arrParams[2] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(vendorInvoice.BranchCode)))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = vendorInvoice.BranchCode;
                }
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_READ_ADDED_BY_VENDOR, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    VendorInvoice _vendorInvoice = new VendorInvoice();
                    _vendorInvoice.ConfirmationPeriod = SafeTypeHandling.ConvertToString(row["QUARTER_PERIOD"]);
                    _vendorInvoice.Vendors.VendorCode = SafeTypeHandling.ConvertToString(row["VENDOR_CODE"]);
                    _vendorInvoice.Vendors.VendorName = SafeTypeHandling.ConvertToString(row["VENDOR_NAME"]);
                    _vendorInvoice.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    _vendorInvoice.InvoiceNo = SafeTypeHandling.ConvertToString(row["INVOICE_NO"]);
                    _vendorInvoice.InvoiceDate = SafeTypeHandling.ConvertToDateTime(row["INVOICE_DATE"]);
                    _vendorInvoice.InvoiceAmount = SafeTypeHandling.ConvertStringToDecimal(row["INVOICE_AMT"]);
                    _vendorInvoice.ReceivedPaymentAmount = SafeTypeHandling.ConvertStringToDecimal(row["PAYMENT_RECEIVED"]);
                    _vendorInvoice.Balance = SafeTypeHandling.ConvertStringToDecimal(row["BALANCE"]);
                    _vendorInvoice.LG_PO_No = SafeTypeHandling.ConvertToString(row["LGPO_NO"]);
                    _vendorInvoice.VendorRemarks = SafeTypeHandling.ConvertToString(row["VENDOR_REMARK"]);

                    _vendorInvoice.BAMAcceptance = SafeTypeHandling.ConvertToString(row["ACCEPTATION"]);
                    _vendorInvoice.PaymentDate = SafeTypeHandling.ConvertToDateTime(row["PAYMENT_DATE"]);
                    _vendorInvoice.PaymentAmount = SafeTypeHandling.ConvertStringToDecimal(row["PAYMENT_AMOUNT"]);
                    _vendorInvoice.AfterPaymentBalance = SafeTypeHandling.ConvertStringToDecimal(row["AFTER_PAYMENT_BALANCE"]);
                    _vendorInvoice.BAMRemarks = SafeTypeHandling.ConvertToString(row["BAM_REMARKS"]);

                    _vendorInvoice.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    _vendorInvoice.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _vendorInvoice.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY"]);
                    _vendorInvoice.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    lstVendorInvoice.Add(_vendorInvoice);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstVendorInvoice;
        }




        /// <summary>
        /// Read Vendor/ Supplier Confirmation Summary for check , do reply or not (Only for BAM)
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        public async Task<List<VendorInvoice>> ReadVendorConfirmationClaims(VendorInvoice vendorInvoice)
        {
            List<VendorInvoice> lstVendorInvoice = new List<VendorInvoice>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2);
                arrParams[0].Value = vendorInvoice.ConfirmationPeriod;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(vendorInvoice.Vendors.VendorCode)))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = vendorInvoice.Vendors.VendorCode;
                }
                arrParams[2] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(vendorInvoice.Status))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = vendorInvoice.Status;
                }
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                arrParams[4] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2);
                arrParams[4].Value = vendorInvoice.BranchCode;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_CONFIRMATION_READ_FOR_CLAIM_REPLY, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    VendorInvoice _vendorInvoice = new VendorInvoice();
                    _vendorInvoice.ConfirmationPeriod = SafeTypeHandling.ConvertToString(row["QUARTER_PERIOD"]);
                    _vendorInvoice.Vendors.VendorCode = SafeTypeHandling.ConvertToString(row["VENDOR_CODE"]);
                    _vendorInvoice.Vendors.VendorName = SafeTypeHandling.ConvertToString(row["VENDOR_NAME"]);
                    _vendorInvoice.Vendors.CTRL_AU = SafeTypeHandling.ConvertToString(row["CTRL_AU"]);
                    _vendorInvoice.Vendors.Status = SafeTypeHandling.ConvertToString(row["VENDOR_STATUS"]);
                    _vendorInvoice.Vendors.StatusDesc = SafeTypeHandling.ConvertToString(row["VENDOR_STATUS_DESC"]);
                    _vendorInvoice.LGConfirmedAmount = SafeTypeHandling.ConvertStringToDecimal(row["CLOSING_BALANCE_AMT"]);
                    _vendorInvoice.VendorClaimAmount = SafeTypeHandling.ConvertStringToDecimal(row["VENDOR_CLAIM_AMT"]);
                    _vendorInvoice.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    _vendorInvoice.TotalConfirmedAmount = SafeTypeHandling.ConvertStringToDecimal(row["TOTAL_CONFIRMED_AMT"]);
                    _vendorInvoice.VendorConfirmationDate = SafeTypeHandling.ConvertToDateTime(row["VENDOR_CONFIRMATION_DATE"]);
                    _vendorInvoice.ClaimReplyStatus = SafeTypeHandling.ConvertToString(row["CLAIM_REPLY_STATUS"]);
                    _vendorInvoice.ClaimReplyDate = SafeTypeHandling.ConvertToDateTime(row["CLAIM_REPLY_DATE"]);
                    _vendorInvoice.VendorAcceptanceDate = SafeTypeHandling.ConvertToDateTime(row["VENDOR_ACCEPTANCE_DATE"]);
                    lstVendorInvoice.Add(_vendorInvoice);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstVendorInvoice;
        }


        /// <summary>
        /// Update Vendor upload invoices of all claim replied remarks & status by BAM
        /// </summary>
        /// <param name="vendorInvoices"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorInvoiceClaimReplied(List<VendorInvoice> vendorInvoices)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (VendorInvoice invoice in vendorInvoices)
            {
                try
                {
                    arrParams = new OracleParameter[15];
                    arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = invoice.ConfirmationPeriod;
                    arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = invoice.Vendors.VendorCode;
                    arrParams[2] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = invoice.BranchCode;
                    arrParams[3] = new OracleParameter("P_INVOICE_NO", OracleDbType.Varchar2, 250);
                    arrParams[3].Value = invoice.InvoiceNo;
                    arrParams[4] = new OracleParameter("P_INVOICE_DATE", OracleDbType.Date);
                    arrParams[4].Value = invoice.InvoiceDate;
                    arrParams[5] = new OracleParameter("P_ACCEPTATION", OracleDbType.Varchar2, 100);
                    arrParams[5].Value = invoice.BAMAcceptance;
                    arrParams[6] = new OracleParameter("P_PAYMENT_DATE", OracleDbType.Date);
                    if (invoice.PaymentDate == null)
                    {
                        arrParams[6].Value = DBNull.Value;
                    }
                    else
                    {
                        arrParams[6].Value = invoice.PaymentDate;
                    }
                    arrParams[7] = new OracleParameter("P_PAYMENT_AMOUNT", OracleDbType.Decimal);
                    arrParams[7].Value = invoice.PaymentAmount;
                    arrParams[8] = new OracleParameter("P_BAM_REMARKS", OracleDbType.Varchar2, 500);
                    if (string.IsNullOrEmpty(invoice.BAMRemarks))
                    {
                        arrParams[8].Value = invoice.BAMRemarks;
                    }
                    else
                    {
                        arrParams[8].Value = Convert.ToString(invoice.BAMRemarks).Trim().Replace("\r\n", " ").Replace("\t", " ").Replace("\r", " ").Replace("\n", " "); ;
                    }
                    arrParams[9] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[9].Value = invoice.CreatedBy;
                    arrParams[10] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[10].Value = invoice.RowNumber;
                    arrParams[11] = new OracleParameter("P_LOCAL_IP", OracleDbType.Varchar2, 50);
                    arrParams[11].Value = invoice.LocalIP;
                    arrParams[12] = new OracleParameter("P_PUBLIC_IP", OracleDbType.Varchar2, 50);
                    arrParams[12].Value = invoice.PublicIP;
                    arrParams[13] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[13].Direction = ParameterDirection.Output;
                    arrParams[14] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[14].Direction = ParameterDirection.Output;
                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_CLAIM_REPLIED_INSERT_TEMP, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[13].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = arrParams[14].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", invoice.RowNumber, ": ", ex.ToString()) });
                }
            }
            // if Error Found while data inset into Temp table
            if (errors.Count > 0)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = Utilities.SOME_ERROR_OCCURED_WHILE_SUBMIT_RECORD,
                    data = errors
                };
                return response;
            }

            Int32 createdBy = (Int32)vendorInvoices[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_CLAIM_REPLIED_TEMP_DATA_VALIDATE, arrParams, DealerNet_Next_Connection);
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
                        StatusDesc = Utilities.DATA_VALIDATION_FAILED,
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

            // Finally records submit in Main Table
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_INVOICE_CLAIM_REPLIED_UPDATE, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    errors.Add(new Errors { Error = SafeTypeHandling.ConvertToString(row["REMARKS"]) });
                }

                // if Error Found while insert invoice in Main table
                if (errors.Count > 0)
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = Utilities.SOME_ERROR_OCCURED_WHILE_SUBMIT_RECORD,
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


        /// <summary>
        /// Vendor/ Supplier Confirmation Acceptance by Vendor.
        /// </summary>
        /// <param name="vendorAcceptance"></param>
        /// <returns></returns>
        public async Task<APIResponse> VendorInvoiceAcceptance(VendorInvoice vendorAcceptance)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[8];
                arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2, 100);
                arrParams[0].Value = vendorAcceptance.ConfirmationPeriod;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = vendorAcceptance.Vendors.VendorCode;
                arrParams[2] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[2].Value = vendorAcceptance.CreatedBy;
                arrParams[3] = new OracleParameter("P_LOCAL_IP", OracleDbType.Varchar2, 50);
                arrParams[3].Value = vendorAcceptance.LocalIP;
                arrParams[4] = new OracleParameter("P_PUBLIC_IP", OracleDbType.Varchar2, 50);
                arrParams[4].Value = vendorAcceptance.PublicIP;
                arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[6].Direction = ParameterDirection.Output;
                arrParams[7] = new OracleParameter("P_OUT_VENDOR_MOBILE_NO", OracleDbType.Varchar2, 250);
                arrParams[7].Direction = ParameterDirection.Output;
                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_ACCEPTANCE_UPDATE, arrParams, DealerNet_Next_Connection);

                string vendorMobileNo = SafeTypeHandling.ConvertToString(arrParams[7].Value);
                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[5].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[6].Value),
                };

                #region Vendor/Supplier confirmation acceptance (SMS Send)

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
                            otp.CreatedBy = vendorAcceptance.CreatedBy;
                            otp.LocalIP = vendorAcceptance.LocalIP;
                            otp.PublicIP = vendorAcceptance.PublicIP;
                            otp.ProcessName = "VENDOR_CLAIM_REPLY_ACCEPTANCE";

                            string message = smsDLT.MESSAGE.Replace("{var1}{var2}", "Thank you for accepting the claim reply given by us"); // SMS body change
                            var smsResp = _utilitiesVMSAccess.SMSSend(smsDLT, otp, SMSTemplateName, otp.MobileNo, message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.CreateLogFile(Utilities.PROJECTS.VMS, vendorMobileNo + "," + "Vendor confirmation Acceptance, SMS Sending Issues. " + "#" + ex);
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString(),
                };
            }
            return response;
        }


        /// <summary>
        /// Read Vendor/ Supplier Confirmation Tracker Logs
        /// </summary>
        /// <param name="confirmationTracker"></param>
        /// <returns></returns>
        public async Task<List<VendorInvoice>> ReadVendorConfirmationTracker(VendorInvoice confirmationTracker)
        {
            List<VendorInvoice> lstVendorConfirmationTracker = new List<VendorInvoice>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_QUARTER_PERIOD", OracleDbType.Varchar2);
                arrParams[0].Value = confirmationTracker.ConfirmationPeriod;
                arrParams[1] = new OracleParameter("P_VENDOR_CODE", OracleDbType.Varchar2);
                arrParams[1].Value = confirmationTracker.Vendors.VendorCode;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.VendorManagementSystem.VendorConfirmation.VENDOR_CONFIRMATION_TRACKER_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    VendorInvoice _vendorInvoice = new VendorInvoice();
                    _vendorInvoice.ConfirmationPeriod = SafeTypeHandling.ConvertToString(row["QUARTER_PERIOD"]);
                    _vendorInvoice.Vendors.VendorCode = SafeTypeHandling.ConvertToString(row["VENDOR_CODE"]);
                    _vendorInvoice.Vendors.VendorName = SafeTypeHandling.ConvertToString(row["VENDOR_NAME"]);
                    _vendorInvoice.Status = SafeTypeHandling.ConvertToString(row["ACTIVITY_STATUS"]);
                    _vendorInvoice.ActivityType = SafeTypeHandling.ConvertToString(row["ACTIVITY_TYPE"]);
                    _vendorInvoice.ActivityDate = SafeTypeHandling.ConvertToDateTime(row["ACTIVITY_DATE"]);
                    _vendorInvoice.Remarks = SafeTypeHandling.ConvertToString(row["REMARKS"]);
                    lstVendorConfirmationTracker.Add(_vendorInvoice);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstVendorConfirmationTracker;
        }

    }
}
