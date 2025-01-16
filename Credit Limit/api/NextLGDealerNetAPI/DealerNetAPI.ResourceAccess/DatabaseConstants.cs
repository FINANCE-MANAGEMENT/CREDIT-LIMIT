using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.ResourceAccess
{
    public class DatabaseConstants
    {

        /// <summary>
        /// For all common Objects related to Dealer Net Plus Application.
        /// </summary>
        public class DNET
        {
            public struct Lookup_Master
            {
                public static string LOOKUP_TYPES_READ = "DNET09_LOOKUP_MASTER_PKG.LOOKUP_TYPES_READ_PROC";
                public static string READ = "DNET09_LOOKUP_MASTER_PKG.LOOKUP_DETAILS_READ_PROC";
                public static string INSERT_UPDATE = "DNET09_LOOKUP_MASTER_PKG.LOOKUP_DETAILS_INSERT_UPD_PROC";
            }

            public struct Terms_Conditions_Master
            {
                public static string INSERT_UPDATE = "DNET_TERMS_AND_CONDITIONS_PKG.DNET_TERM_CONDITN_INS_UPD_PROC";
                public static string READ = "DNET_TERMS_AND_CONDITIONS_PKG.DFI_TERM_AND_CONDITN_READ_PROC";
                public static string READ_SCHEME_TYPES = "DNET_TERMS_AND_CONDITIONS_PKG.READ_SCHEME_TYPES_PROC";
            }

            public struct Roles
            {
                public static string READ = "DNET_MENU_MGMT_MAPPING_PKG.READ_ROLES_PROC";
            }

            public struct Menus
            {
                public static string INSERT_UPDATE = "DNET_MENU_MGMT_MAPPING_PKG.MENU_INSERT_UPD_PROC";
                public static string READ = "DNET_MENU_MGMT_MAPPING_PKG.MENU_READ_PROC";
                public static string INSERT_UPDATE_TASK_MAPPING = "DNET_MENU_MGMT_MAPPING_PKG.TASK_MAPPING_INS_UPD_PROC";
                public static string READ_TASK_MAPPING = "DNET_MENU_MGMT_MAPPING_PKG.USER_TASK_READ_PROC";
                public static string USER_TASK_ASSIGN_READ = "DNET_MENU_MGMT_MAPPING_PKG.USER_TASK_ASSIGN_READ_PROC";
                public static string MENU_UTILIZATION_LOG_INSERT = "DNET_MENU_UTILIZAION_PKG.DNET_UTILIZATION_PROC";
            }

            public struct Users
            {
                public static string READ_USER_DETAILS_BY_USER = "DNET09_NEXTDNET_UTILITY_PKG.READ_USER_DETAIL_PROC";//DNET09_DFI_UTILITY_PKG.READ_USER_DETAIL_PROC   Object name change on 13-Jun-2024 by Vikrant Kumar
                public static string SSO_VALIDATE = "DNET09_NEXTDNET_UTILITY_PKG.SSO_USER_VALIDATE_PROC"; //DNET09_DFI_UTILITY_PKG.DNET09_SSO_USER_VALIDATE_PROC   Object name change on 13-Jun-2024 by Vikrant Kumar
                public static string READ_USER_BY_ROLE = "DNET_MENU_MGMT_MAPPING_PKG.USER_READ_PROC";
                public static string USER_LOGIN = "VMS_UTILITY_PKG.USER_LOGIN_PROC";

                public static string USER_FORGOT_PASSWORD = "NEXTDNET_FORGOT_PASSWORD_PROC";
            }
        }

        /// <summary>
        /// Channel Finance Modules of all DB objects lists
        /// </summary>
        public class ChannelFinance
        {
            public struct State_Master
            {
                public static string READ = "DNET09_CF_STATE_MST_PKG.DNET09_CF_STATE_READ_PRC";
                public static string INSERT = "DNET09_CF_STATE_MST_PKG.DNET09_CF_STATE_IN_PRC";
                public static string UPDATE = "";
            }

            public struct District_Master
            {
                public static string READ = "DNET09_CF_STATE_MST_PKG.DNET09_CF_DISTRICT_READ_PRC";
                public static string INSERT = "DNET09_CF_STATE_MST_PKG.DNET09_CF_DISTRICT_IN_PRC";
                public static string UPDATE = "";
            }
            public struct FirmType_Master
            {
                public static string READ = "";
                public static string INSERT = "";
                public static string UPDATE = "";
            }
            public struct Bank_Master
            {
                public static string READ = "";
                public static string INSERT = "";
                public static string UPDATE = "";
            }
            public struct Document_Master
            {
                public static string READ = "";
                public static string INSERT = "";
                public static string UPDATE = "";
            }




        }

        /// <summary>
        /// Scheme AII/DFI Project
        /// </summary>
        public class DFIScheme
        {
            public struct Utilities
            {
                public static string READ_ZONE = "DNET09_DFI_UTILITY_PKG.DFI_ZONE_READ_PROC";
                public static string READ_REGION = "DNET09_DFI_UTILITY_PKG.DFI_REGION_READ_PROC";
                public static string READ_BRANCH = "DNET09_DFI_UTILITY_PKG.DFI_BRANCH_READ_PROC";

                public static string READ_SCHEME_TYPE = "DNET09_DFI_UTILITY_PKG.DFI_SCHEME_TYPE_READ_PROC";
                public static string READ_SCHEME_REASON_CODE = "DNET09_DFI_UTILITY_PKG.DFI_SCHEME_REASON_CD_READ_PROC";

                public static string READ_MAJOR_SALES_CHANNEL = "DNET09_DFI_UTILITY_PKG.DFI_MAJ_SALES_CHNL_READ_PROC";
                public static string READ_SALES_CHANNEL = "DNET09_DFI_UTILITY_PKG.DFI_SALES_CHANNEL_READ_PROC";
                public static string READ_CHANNEL_CODE = "DNET09_DFI_UTILITY_PKG.DFI_CHANNEL_READ_PROC";

                public static string READ_BILLING_CODE = "DNET09_DFI_UTILITY_PKG.DFI_BILLING_CODE_READ_PROC";

                public static string SCHEME_USER_AU_MAPPING_UPDATE = "DNET09_DFI_UTILITY_PKG.USER_AU_MAPPING_PROC";
                public static string SCHEME_USER_AU_MAPPING_READ = "DNET09_DFI_UTILITY_PKG.USER_AU_MAPPING_READ_PROC";

                public static string READ_USERS_BY_ROLE = "DNET09_NEXTDNET_UTILITY_PKG.READ_USER_BY_ROLE_PROC";//DNET09_DFI_UTILITY_PKG.READ_USER_BY_ROLE_PROC    Object name change on 13-Jun-2024 by Vikrant Kumar
                public static string APPROVERS_READ = "DNET09_DFI_UTILITY_PKG.APPROVER_READ_PROC";

            }

            public struct ModelMaster
            {
                public static string READ_PRODUCT_CATEGORY = "DNET09_DFI_MODEL_MASTER_PKG.PRODUCT_READ_PROC";
                public static string READ_SUB_PRODUCT_CATEGORY = "DNET09_DFI_MODEL_MASTER_PKG.SUB_PRODUCT_READ_PROC";
                public static string READ_MODEL_PRODUCT_LEVEL3 = "DNET09_DFI_MODEL_MASTER_PKG.PRODUCT_LEVEL3_READ_PROC";
                public static string READ_MODEL_CATEGORY = "DNET09_DFI_MODEL_MASTER_PKG.MODEL_CATEGORY_READ_PROC";
                public static string READ_MODEL_SUB_CATEGORY = "DNET09_DFI_MODEL_MASTER_PKG.SUB_CATEGORY_READ_PROC";
                public static string READ_MODEL_SERIES = "DNET09_DFI_MODEL_MASTER_PKG.MODEL_SERIES_READ_PROC";
                public static string READ_MODEL_STAR_RATING = "DNET09_DFI_MODEL_MASTER_PKG.STAR_RATING_READ_PROC";
                public static string READ_MODEL_YEARS = "DNET09_DFI_MODEL_MASTER_PKG.MODEL_YEAR_READ_PROC";
                public static string READ_MODELS = "DNET09_DFI_MODEL_MASTER_PKG.MODEL_READ_PROC";

                public static string UPDATE_MODELS = "DNET09_DFI_MODEL_MASTER_PKG.MODEL_UPDATE_PROC";
                public static string READ_ALL_MODELS = "DNET09_DFI_MODEL_MASTER_PKG.MODEL_READ_ALL_PROC";
            }

            public struct DFISchemeRequest
            {
                public static string SCHEME_HISTORY_READ = "DNET09_DFI_SCHEME_DETAILS_PKG.SCHEME_HISTORY_SEARCH_PROC";
                public static string INSERT_UPDATE_REQUEST = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_REQUEST_PROC";
                //public static string INSERT_UPDATE_REQUEST_CUSTOMER_DETAILS = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_CUSTOMER_DTL_INS_PROC"; // Commented 28-Feb-2024
                public static string INSERT_UPDATE_REQUEST_CUSTOMER_DETAILS = "DNET09_DFI_SCHEME_DETAILS_PKG.CUSTOMER_DTL_INSERT_PROC";
                public static string INSERT_UPDATE_REQUEST_PRODUCT_DETAILS = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_PRODUCT_DTL_INS_PROC";
                public static string INSERT_UPDATE_REQUEST_CALCULATION_DETAILS = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_CALCULATION_DTL_INS_PROC";
                public static string SCHEME_APPROVAL_INSERT_UPDATE = "DNET09_DFI_SCHEME_DETAILS_PKG.SCHEME_APPROVAL_INSERT_PROC";  //New Added, Dynamic Approval 12-Mar-2024
                public static string SCHEME_APPROVERS_REMOVE = "DNET09_DFI_SCHEME_DETAILS_PKG.SCHEME_APPROVERS_REMOVE_PROC";
                public static string UPDATE_APPROVAL_STATUS = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_SCHEME_STATUS_UPD_PROC";
                public static string SCHEME_SLAB_INSERT_UPDATE = "DNET09_DFI_SCHEME_DETAILS_PKG.SCHEME_AUTOM_SLAB_INSERT_PROC"; // Scheme Slab Insert/Update
                public static string SCHEME_SLAB_READ = "DNET09_DFI_SCHEME_DETAILS_PKG.SCHEME_AUTOM_SLAB_READ_PROC";

                public static string READ_SCHEME_REQUEST = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_REQUEST_READ_PROC";
                public static string READ_PENDING_APPROVAL_SCHEME = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_PENDING_APPROVAL_READ_PROC";
                public static string READ_ALL_APPROVER_BY_SCHEME = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_APPROVER_READ_PROC";

                public static string READ_APPROVAL_HISTORY_BY_SCHEME = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_APPROVAL_HIST_READ_PROC";
                public static string READ_TERM_CONDITION_BY_SCHEME = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_TNC_READ_BY_SCHEME_PROC";

                public static string READ_CUSTOMER_DETAILS_BY_SCHEME = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_CUSTOMER_DTL_READ_PROC";
                public static string READ_PRODUCT_DETAILS_BY_SCHEME = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_PRODUCT_DTL_READ_PROC";

                public static string READ_FILTER_PRODUCTS = "DNET09_DFI_SCHEME_DETAILS_PKG.DFI_DYNAMIC_PRODUCTS_PROC";

                public static string SCHEME_UPDATE_AFTER_APPROVED = "DNET09_DFI_SCHEME_DETAILS_PKG.SCHEME_UPD_AFTER_APPROVED_PROC";

                // Scheme Target Insert/Update/Read
                public static string SCHEME_TARGET_INSERT_TEMP = "SCHEME_AUTOM_TARGET_PKG.TARGET_INSERT_TEMP_PROC";
                public static string SCHEME_TARGET_TEMP_DATA_VALIDATE = "SCHEME_AUTOM_TARGET_PKG.TARGET_TEMP_VALID_PROC";
                public static string SCHEME_TARGET_INSERT_FINAL = "SCHEME_AUTOM_TARGET_PKG.TARGET_INSERT_MAIN_PROC";
                public static string SCHEME_TARGET_READ = "SCHEME_AUTOM_TARGET_PKG.TARGET_READ_PROC";

            }

            public struct SchemeSettlement
            {
                public static string SCHEME_CALCULATION_REQUEST_SAVE = "SCHEME_AUTOM_CALCULATION_PKG.CALCULATION_REQUEST_INS_PROC";
                public static string SCHEME_CALCULATION_REQUEST_READ = "SCHEME_AUTOM_CALCULATION_PKG.CALCULATION_REQUEST_READ_PROC";
                public static string SCHEME_CALCULATION_READ = "SCHEME_AUTOM_CALCULATION_PKG.CALCULATION_READ_PROC";
                public static string GTM_SCHEME_DATA_READ = "SCHEME_AUTOM_CALCULATION_PKG.GTM_SCHEME_DATA_READ_PROC";
            }

            public struct SerialNoApplicability
            {
                public static string SERIAL_NO_APPLICABILITY_SAVE_UPDATE = "SCHEME_AUTOM_SERIAL_NO_APP_PKG.SCH_AUTOM_SRNO_INS_UPD_PROC";
                public static string SERIAL_NO_APPLICABILITY_READ = "SCHEME_AUTOM_SERIAL_NO_APP_PKG.SCH_AUTOM_SRNO_READ_PROC";

            }

        }

        /// <summary>
        /// Vendor Management System Project
        /// </summary>
        public class VendorManagementSystem
        {
            public struct VendorMaster
            {
                public static string INSERT_UPDATE = "VMS_VENDOR_PKG.VENDOR_INSERT_UPDATE_PROC";
                public static string READ = "VMS_VENDOR_PKG.VENDOR_DETAILS_READ_PROC";
                public static string READ_PIC_MEMBERS = "VMS_VENDOR_PKG.PIC_MEMBER_READ_PROC";
                public static string PIC_REQ_BRANCH_READ = "VMS_VENDOR_PKG.PIC_REQ_BRANCH_READ_PROC";

                public static string VENDOR_PROFILE_UPDATE_REQUEST = "VMS_VENDOR_PKG.VENDOR_REG_REQUEST_INSERT_PROC";
                public static string VENDOR_PROFILE_UPDATE_REQUEST_READ = "VMS_VENDOR_PKG.VENDOR_REQUEST_READ_PROC";
                public static string VENDOR_PROFILE_UPDATE_REQUEST_APPROVAL = "VMS_VENDOR_PKG.VENDOR_REG_REQUEST_UPDATE_PROC";

            }

            public struct VendorConfirmation
            {
                // Step 1: Admin Upload the invoices for that quarter
                public static string VENDOR_INVOICE_INSERT_TEMP = "VMS_VENDOR_INVOICE_PKG.VENDOR_INVOICE_INS_TEMP_PROC";
                public static string VENDOR_INVOICE_TEMP_DATA_VALIDATE = "VMS_VENDOR_INVOICE_PKG.VENDOR_INVOICE_TEMP_VLD_PROC";
                public static string VENDOR_INVOICE_INSERT = "VMS_VENDOR_INVOICE_PKG.VENDOR_INVOICE_INSERT_PROC";
                public static string VENDOR_INVOICE_APPROVED_BY_ADMIN = "VMS_VENDOR_INVOICE_PKG.VENDOR_INVOICE_APPROVE_PROC";
                public static string VENDOR_INVOICE_APPROVED_BY_ADMIN_ROLLBACK = "VMS_VENDOR_INVOICE_PKG.INVOICE_APPROVED_ROLLBACK_PROC";
                public static string VENDOR_INVOICE_SENT_EMAIL_STATUS_UPDATE = "VMS_VENDOR_INVOICE_PKG.INV_EMAIL_SEND_STATUS_UPD_PROC";

                public static string VENDOR_INVOICE_READ = "VMS_VENDOR_INVOICE_PKG.VENDOR_INVOICE_READ_PROC";
                public static string VENDOR_INVOICE_SUMMARY_READ = "VMS_VENDOR_INVOICE_PKG.VENDOR_INV_SUMMARY_READ_PROC";


                // Step 2: Vendor/Supplier Invoice details Added by Vendor/ Supplier Self
                public static string VENDOR_CLAIM_AMOUNTS_INSERT_UPDATE = "VMS_VENDOR_CONFIRMATION_PKG.VENDOR_CLAIM_INSERT_UPD_PROC";
                public static string VENDOR_CLAIM_AMOUNTS_READ = "VMS_VENDOR_CONFIRMATION_PKG.VENDOR_CLAIM_READ_PROC";


                public static string VENDOR_INVOICE_ADDED_BY_VENDOR_INSERT_TEMP = "VMS_VENDOR_CONFIRMATION_PKG.VENDOR_INV_CNFIRM_INS_TMP_PROC";
                public static string VENDOR_INVOICE_ADDED_BY_VENDOR_TEMP_DATA_VALIDATE = "VMS_VENDOR_CONFIRMATION_PKG.VENDR_INV_CNFRM_TMP_VALID_PROC";
                public static string VENDOR_INVOICE_ADDED_BY_VENDOR_INSERT = "VMS_VENDOR_CONFIRMATION_PKG.VENDOR_INV_CONFIRM_INS_PROC";

                public static string VENDOR_INVOICE_READ_ADDED_BY_VENDOR = "VMS_VENDOR_CONFIRMATION_PKG.VENDOR_INV_CONFIRM_READ_PROC";

                // Step 3: Claim Summery for BAM
                public static string VENDOR_CONFIRMATION_READ_FOR_CLAIM_REPLY = "VMS_VENDOR_CONFIRMATION_PKG.VNDR_INV_CONFRM_SUMM_READ_PROC";

                // Claim Reply of all Invoices , which is uploaded by Vendor. => Replied by BAM
                public static string VENDOR_INVOICE_CLAIM_REPLIED_INSERT_TEMP = "VMS_VENDOR_CONFIRMATION_PKG.CLAIM_REPLY_TEMP_PROC";
                public static string VENDOR_INVOICE_CLAIM_REPLIED_TEMP_DATA_VALIDATE = "VMS_VENDOR_CONFIRMATION_PKG.CLAIM_REPLY_TEMP_VLD_PROC";
                public static string VENDOR_INVOICE_CLAIM_REPLIED_UPDATE = "VMS_VENDOR_CONFIRMATION_PKG.CLAIM_REPLY_UPDATE_PROC";

                public static string VENDOR_ACCEPTANCE_UPDATE = "VMS_VENDOR_CONFIRMATION_PKG.VENDOR_ACCEPTANCE_UPDATE_PROC";

                // Confirmation Tracker
                public static string VENDOR_CONFIRMATION_TRACKER_READ = "VMS_VENDOR_CONFIRMATION_PKG.CONFIRMATION_TRACKER_READ_PROC";

            }

            public struct VendorCommunication
            {
                public static string COMMUNICATION_TEMPLATE_REG_INSERT_UPDATE = "VMS_COMMUNICATION_PKG.TEMPLATE_INSERT_UPDATE_PROC";
                public static string COMMUNICATION_TEMPLATE_READ = "VMS_COMMUNICATION_PKG.TEMPLATE_READ_PROC";
                public static string COMMUNICATION_TEMPLATE_REQUIRED_INFO_INSERT_UPDATE = "VMS_COMMUNICATION_PKG.COMMUNICATION_REQ_FIELD_PROC";
                public static string COMMUNICATION_TEMPLATE_REQUIRED_INFO_READ = "VMS_COMMUNICATION_PKG.TEMPLATE_REQ_INFO_READ_PROC";

                public static string COMMUNICATION_SEND_TO_VENDOR_INSERT = "VMS_COMMUNICATION_PKG.COMMUNICATION_SEND_INSERT_PROC";

                public static string VENDOR_UPLOAD_FOR_COMMUNICATION_TEMP_INSERT = "VMS_COMMUNICATION_PKG.VNDR_UPLOAD_COMM_TMP_INS_PROC";
                public static string VENDOR_UPLOAD_FOR_COMMUNICATION_TEMP_VALIDATE = "VMS_COMMUNICATION_PKG.VNDR_UPLOAD_COMM_TMP_VLD_PROC";

                // Vendor Communication Acceptance
                public static string VENDOR_COMMUNICATION_SEND_TEMPLATE_READ = "VMS_COMMUNICATION_PKG.VENDOR_COMM_SEND_READ_PROC";
                public static string VENDOR_COMMUNICATION_ACCEPTANCE_SAVE = "VMS_COMMUNICATION_PKG.COMMUNICATION_ACCEPTANCE_PROC"; // Acceptance Status Update
                public static string VENDOR_COMMUNICATION_ACCEPTANCE_REQ_INFO_SAVE = "VMS_COMMUNICATION_PKG.COMMUNICATION_INFO_INSERT_PROC"; // Comm Req Info Insert
                public static string VENDOR_COMMUNICATION_ACCEPTANCE_EMAIL_SEND = "VMS_COMMUNICATION_PKG.COMM_ACCEPTANCE_EMAIL_PROC"; // Acceptance Email Send

            }

            public struct Utilities
            {
                public static string READ_QUARTER = "VMS_UTILITY_PKG.QUARTER_READ_PROC";
                public static string BRANCH_READ = "VMS_UTILITY_PKG.BRANCH_READ_PROC";


                public static string GENERATE_OTP = "VMS_OTP_MANAGE_PKG.VMS_GENERATE_OTP";
                public static string VALIDATE_OTP = "VMS_OTP_MANAGE_PKG.VMS_VALIDATE_OTP";
                public static string UPDATE_OTP = "VMS_OTP_MANAGE_PKG.UPDATE_OTP";

                public static string SMS_TEMPLATE_READ = "VMS_UTILITY_PKG.SMS_TEMPLATE_READ_PROC";
                public static string SMS_LOG_UPDATE = "VMS_UTILITY_PKG.SMS_LOG_UPDATE_PROC";

                public static string USER_AU_MAPPING_UPDATE = "VMS_UTILITY_PKG.USER_AU_MAPPING_PROC";
                public static string USER_AU_MAPPING_READ = "VMS_UTILITY_PKG.USER_AU_MAPPING_READ_PROC";
                public static string READ_USERS_BY_ROLE = "VMS_UTILITY_PKG.READ_USER_BY_ROLE_PROC";

                public static string VENDOR_CHANGE_PASSWORD_UPDATE = "VMS_UTILITY_PKG.VENDOR_CHANGE_PASSWORD_PROC";

            }
        }

        /// <summary>
        /// AR Credit Limit Project
        /// </summary>
        public class ARCreditLimitSystem
        {
            public struct Utilities
            {

                // Notes Master
                public static string SAVE_UPDATE_NOTES_MASTER = "CL_NOTES_MASTER_PKG.CL_NOTES_INSERT_PROC";
                public static string READ_NOTES_MASTER = "CL_NOTES_MASTER_PKG.CL_NOTES_READ_PROC";

                // All Masters Updated List
                public static string ALL_MASTERS_UPDATED_LIST_READ = "CL_CREDIT_LIMIT_PKG.MASTERS_UPLOAD_READ_PROC";

                // All Upload Logs Read
                public static string UPLOAD_FILES_LOG_READ = "CL_UPLOAD_LOG_READ_PROC";

                // Sales Upload
                public static string SALES_UPLOAD_TEMP_INSERT = "CL_SALES_PKG.SALES_TEMP_INSERT_PROC";
                public static string SALES_UPLOAD_TEMP_VALIDATE = "CL_SALES_PKG.SALES_TEMP_VALID_PROC";
                public static string SALES_UPLOAD_FINAL = "CL_SALES_PKG.SALES_MAIN_INSERT_PROC";
                public static string SALES_UPLOAD_READ = "CL_SALES_PKG.SALES_READ_PROC";

                // Insurance Upload
                public static string INSURNACE_UPLOAD_TEMP_INSERT = "CL_INSURANCE_PKG.INSURANCE_TEMP_INSERT_PROC";
                public static string INSURNACE_UPLOAD_TEMP_VALIDATE = "CL_INSURANCE_PKG.INSURANCE_TEMP_VALID_PROC";
                public static string INSURNACE_UPLOAD_FINAL = "CL_INSURANCE_PKG.INSURANCE_MAIN_INSERT_PROC";
                public static string INSURNACE_UPLOAD_READ = "CL_INSURANCE_PKG.INSURANCE_READ_PROC";

                // OD Upload
                public static string OD_UPLOAD_TEMP_INSERT = "CL_OD_PKG.OD_TEMP_INSERT_PROC";
                public static string OD_UPLOAD_TEMP_VALIDATE = "CL_OD_PKG.OD_TEMP_VALID_PROC";
                public static string OD_UPLOAD_FINAL = "CL_OD_PKG.OD_MAIN_INSERT_PROC";
                public static string OD_UPLOAD_READ = "CL_OD_PKG.OD_READ_PROC";

                // Collection Upload
                public static string COLLECTION_UPLOAD_TEMP_INSERT = "CL_COLLECTION_PKG.COLLECTION_TEMP_INSERT_PROC";
                public static string COLLECTION_UPLOAD_TEMP_VALIDATE = "CL_COLLECTION_PKG.COLLECTION_TEMP_VALID_PROC";
                public static string COLLECTION_UPLOAD_FINAL = "CL_COLLECTION_PKG.COLLECTION_MAIN_INSERT_PROC";
                public static string COLLECTION_UPLOAD_READ = "CL_COLLECTION_PKG.COLLECTION_READ_PROC";

                // FY Status Master
                public static string FY_STATUS_MASTER_SAVE_UPDATE = "CL_FINANCIAL_YEAR_PKG.CL_FY_STATUS_INS_PROC";
                public static string FY_STATUS_MASTER_SAVE_READ = "CL_FINANCIAL_YEAR_PKG.CL_FY_STATUS_READ_PROC";

                // FY Attachement Upload
                public static string FY_ATTACHMENT_UPLOAD_TEMP_INSERT = "CL_FINANCIAL_YEAR_PKG.FY_ATTACHMENT_TEMP_INSERT_PROC";
                public static string FY_ATTACHMENT_UPLOAD_TEMP_VALIDATE = "CL_FINANCIAL_YEAR_PKG.FY_ATTACHMENT_TEMP_VALID_PROC";
                public static string FY_ATTACHMENT_UPLOAD_FINAL = "CL_FINANCIAL_YEAR_PKG.FY_ATTACHMENT_MAIN_INSERT_PROC";
                public static string FY_ATTACHMENT_UPLOAD_READ = "CL_FINANCIAL_YEAR_PKG.FY_ATTACHMENT_READ_PROC";


            }

            public struct CreditLimit
            {
                // Branch Master
                public static string READ_BRANCH = "CL_CREDIT_LIMIT_PKG.BRANCH_READ_PROC";

                public static string HEADER_CODE_DETAIL_READ = "CL_CREDIT_LIMIT_PKG.HEADER_DTL_READ_PROC";

                public static string SALES_HISTORY_READ = "CL_CREDIT_LIMIT_PKG.SALES_HISTORY_READ_PROC";
                public static string SALES_PAYMENT_TREND_READ = "CL_CREDIT_LIMIT_PKG.SALES_PAYMENT_TREND_PROC";
                public static string OD_HISTORY_SUMMARY_READ = "CL_CREDIT_LIMIT_PKG.OD_HISTORY_SUMMARY_READ_PROC";
                public static string FUTURE_SALES_PLAN_READ = "CL_CREDIT_LIMIT_PKG.FUTURE_SALES_PLAN_READ_PROC";
                public static string FY_ATTACHMENT_READ = "CL_CREDIT_LIMIT_PKG.FINANCIAL_ATTACHMNT_READ_PROC";
                public static string NOTES_READ = "CL_CREDIT_LIMIT_PKG.CL_NOTES_READ_PROC";

                // Normal Credit Limit Request By Form
                public static string CREDIT_LIMIT_REQUEST_SAVE = "CL_CREDIT_LIMIT_PKG.CREDIT_LIMIT_REQUEST_SAVE_PROC";
                public static string FUTURE_SALES_PLAN_SAVE = "CL_CREDIT_LIMIT_PKG.FUTURE_SALES_PLAN_SAVE_PROC";
                public static string CREDIT_LIMIT_REQUEST_FINANCIAL_STATEMENT_SAVE = "CL_CREDIT_LIMIT_PKG.FINANCIAL_STATEMENT_SAVE_PROC";
                public static string CREDIT_LIMIT_REQUEST_NOTES_SAVE = "CL_CREDIT_LIMIT_PKG.REQUEST_NOTES_SAVE_PROC";

                // Bulk Upload Request By Excel
                public static string CREDIT_LIMIT_BULK_REQUEST_TEMP_SAVE = "CL_CREDIT_LIMIT_PKG.CL_BULK_UPLOAD_INS_TMP_PROC";
                public static string CREDIT_LIMIT_BULK_REQUEST_TEMP_DATA_VALIDATE = "CL_CREDIT_LIMIT_PKG.CL_BULK_UPLOAD_VALIDATE_PROC";
                public static string CREDIT_LIMIT_BULK_REQUEST_TEMP_DATA_FINAL_SUBMIT = "CL_CREDIT_LIMIT_PKG.CL_BULK_UPLOAD_INS_MAIN_PROC";
                public static string CREDIT_LIMIT_BULK_REQUEST_FUTURE_SALES_TEMP_SAVE = "CL_CREDIT_LIMIT_PKG.BULK_REQ_FUTURE_SALE_TMP_PROC";


                public static string CREDIT_LIMIT_ALL_REQUEST_READ = "CL_CREDIT_LIMIT_PKG.CL_REQUEST_READ_PROC";

                // Credit Limit Approval
                public static string CREDIT_LIMIT_APPROVAL_UPLOAD_TEMP_INSERT = "CL_CREDIT_LIMIT_PKG.REQUEST_APPROVE_INS_TMP_PROC";
                public static string CREDIT_LIMIT_APPROVAL_UPLOAD_TEMP_VALIDATE = "CL_CREDIT_LIMIT_PKG.REQUEST_APPROVE_TMP_VALID_PROC";
                public static string CREDIT_LIMIT_APPROVAL_UPLOAD_FINAL = "CL_CREDIT_LIMIT_PKG.CREDIT_LIMIT_REQUEST_UPD_PROC";

                // Auto Email send
                public static string CREDIT_LIMIT_EMAIL_READ_FOR_REQUEST = "CL_CREDIT_LIMIT_PKG.EMAIL_READ_PROC";
                public static string CREDIT_LIMIT_EMAIL_SENT_LOG_INSERT = "CL_CREDIT_LIMIT_PKG.EMAIL_SENT_LOG_PROC";



            }

        }

    }
}