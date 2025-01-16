using DealerNetAPI.DomainObject;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DealerNetAPI.Common
{
    public static class Utilities
    {
        public static string ERROR = "ERROR";
        public static string SUCCESS = "SUCCESS";
        public static string PARAMETER_MISSING = "Parameter Missing.";
        public static string SERVER_AUTHENTICATION_FAILED = "Server authentication failed.";
        public static string AUTHENTICATION_FAILED = "Authentication failed.";
        public static string INVALID_CLIENT_REQUEST = "Invalid Client Request.";
        public static string INVALID_REQUEST = "Invalid Request.";
        public static string DATA_VALIDATION_FAILED = "Data validation failed.";

        public static string EXCEL_FILE_EMPTY = "Excel file is empty.";
        public static string INVALID_EXCEL_FILE = "Invalid Excel or Invalid Template file.";
        public static string EXCEL_FILE_DATA_ISSUES = "Excel File data Issues.";
        public static string EXCEL_DATA_VALIDATION_FAILED = "Excel data validation failed.";
        public static string EXCEL_FILE_EXTENSION_ALLOW = "Only allow file extension .xlsx";

        public static string SOME_ERROR_OCCURED_WHILE_SUBMIT_RECORD = "Some error occured while submit the record.";
        public static string RECORD_SBUMITTED_SUCCESSFULLY = "Record submitted successfully...";
        public static string RECORD_PARTIALLY_SBUMITTED_SUCCESSFULLY = "Record partially submitted successfully...";
        public static string SOMETHING_WENT_WRONG = "Something went wrong !!";
        public static string FILE_UPLOADED_SUCCESSFULLY = "File uploaded successfully...";
        public static string SESSION_EXPIRED = "Your session has been expired. Kindly login again.";

        private const string EMAIL_SERVER = "EmailServer";
        private const string EMAIL_SENDER = "EmailSupport";
        //private const string EMAIL_SEND_TO_RPA_STATUS = "SendEmailToRPAStatus";


        #region System Projects
        public struct PROJECTS
        {
            public static string NextDNET = "NextDNET";
            public static string SchemeAII_DFI = "SchemeAII_DFI";
            public static string VMS = "VMS";
            public static string ARCreditLimit = "ARCreditLimit";
        }
        #endregion


        /// <summary>
        /// Generate Response for API
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string GenerateResponse(string statusCode, APIResponse result)
        {
            return JsonConvert.SerializeObject(new
            {
                StatusCode = statusCode,
                Status = result.Status,
                StatusDesc = result.StatusDesc,
                data = result.data
            });
        }

        /// <summary>
        /// Encrypt string for security purpose with specific key
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(this string text, string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key must have valid value.", nameof(key));
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("The text must have valid value.", nameof(text));

            var buffer = Encoding.UTF8.GetBytes(text);
            var hash = new SHA512CryptoServiceProvider();
            var aesKey = new byte[24];
            Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new ArgumentException("Parameter must not be null.", nameof(aes));

                aes.Key = aesKey;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var resultStream = new MemoryStream())
                {
                    using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(buffer))
                    {
                        plainStream.CopyTo(aesStream);
                    }

                    var result = resultStream.ToArray();
                    var combined = new byte[aes.IV.Length + result.Length];
                    Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
                    Array.ConstrainedCopy(result, 0, combined, aes.IV.Length, result.Length);

                    return Convert.ToBase64String(combined);
                }
            }
        }

        /// <summary>
        /// Decrypt string for redable with same key data encrypt
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(this string encryptedText, string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key must have valid value.", nameof(key));
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentException("The encrypted text must have valid value.", nameof(encryptedText));

            var combined = Convert.FromBase64String(encryptedText);
            var buffer = new byte[combined.Length];
            var hash = new SHA512CryptoServiceProvider();
            var aesKey = new byte[24];
            Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new ArgumentException("Parameter must not be null.", nameof(aes));

                aes.Key = aesKey;

                var iv = new byte[aes.IV.Length];
                var ciphertext = new byte[buffer.Length - iv.Length];

                Array.ConstrainedCopy(combined, 0, iv, 0, iv.Length);
                Array.ConstrainedCopy(combined, iv.Length, ciphertext, 0, ciphertext.Length);

                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var resultStream = new MemoryStream())
                {
                    using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(ciphertext))
                    {
                        plainStream.CopyTo(aesStream);
                    }

                    return Encoding.UTF8.GetString(resultStream.ToArray());
                }
            }
        }

        public static bool isValidDate(string _date)
        {
            ////var dateFormats = new[] { "dd.MM.yyyy", "dd-MM-yyyy", "dd/MM/yyyy" };
            //var dateFormats = new[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy", "M.dd.yyyy", "M-dd-yyyy", "M/dd/yyyy" };
            ////Console.WriteLine("Add a schedule for specific dates: ");
            //DateTime scheduleDate;
            //bool validDate = DateTime.TryParseExact(
            //    _date,
            //    dateFormats,
            //    DateTimeFormatInfo.InvariantInfo,
            //    DateTimeStyles.None,
            //    out scheduleDate);
            //return validDate;

            bool isDateOk = false;
            DateTime dateValue;
            if (DateTime.TryParse(_date, out dateValue))
            {
                isDateOk = true;
            }
            return isDateOk;
        }

        public static bool isValidDecimal(string _value)
        {
            bool isDecimalOk = false;
            Decimal decimalValue;
            if (Decimal.TryParse(_value, out decimalValue))
            {
                isDecimalOk = true;
            }
            return isDecimalOk;
        }

        public static bool isValidNumber(string _value)
        {
            bool isIntOk = false;
            Int32 intValue;
            if (Int32.TryParse(_value, out intValue))
            {
                isIntOk = true;
            }
            return isIntOk;
        }

        public static bool isValidMobileNo(string number)
        {
            // Regular expression used to validate a phone number.
            string motif = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

            if (number != null) return Regex.IsMatch(number, motif);
            else return false;
        }

        public static bool ValidateMailAddress(string emailAddress)
        {
            try
            {
                var email = new MailAddress(emailAddress);
                return email.Address == emailAddress.Trim();
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// For Send Email With/ Without Attachement
        /// </summary>
        /// <param name="email"></param>
        public static void SendingEmail(EMail email, IConfiguration configuration)
        {
            string eMailServer = configuration[EMAIL_SERVER].ToString();
            string eMailSender = (string.IsNullOrEmpty(email.EmailSupport) ? configuration[EMAIL_SENDER].ToString() : email.EmailSupport);

            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add(email.To);
            if (!string.IsNullOrEmpty(email.CC))
            {
                mailMsg.CC.Add(email.CC);
            }
            mailMsg.From = new MailAddress(eMailSender);
            mailMsg.Subject = email.MailSubject;

            //Single Attachment File
            if (!string.IsNullOrEmpty(email.AttachmentFileName))
            {
                Attachment atachment = new Attachment(email.AttachmentFileName);
                mailMsg.Attachments.Add(atachment);
            }
            //Multiple Attachment Files
            foreach (var item in email.Attachments)
            {
                Attachment atachment = new Attachment(item);
                mailMsg.Attachments.Add(atachment);
            }

            mailMsg.Body = email.MailBody;
            mailMsg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient(eMailServer);
            smtp.Send(mailMsg);
        }


        public static string GetPublicIPAddress()
        {
            string address = "";
            try
            {
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    address = stream.ReadToEnd();
                }

                int first = address.IndexOf("Address: ") + 9;
                int last = address.LastIndexOf("</body>");
                address = address.Substring(first, last - first);
            }
            catch (Exception)
            {

            }
            return address;
        }



        /// <summary>
        /// For Send SMS
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <param name="message"></param>
        /// <param name="messageTemplateId"></param>
        /// <returns></returns>
        public static MessageResponse SendingSMS(SmsData smsSetting, string mobileNo, string message, string messageTemplateId)
        {
            MessageResponse messageResponse = new MessageResponse();
            HttpWebResponse response = null;
            try
            {
                string timeStamp = DateTime.Now.Ticks.ToString();
                messageResponse.UniqueId = mobileNo + timeStamp;

                List<SmsData> smsdataList = new List<SmsData>();
                SmsData smsData = new SmsData();
                smsData.UNIQUE_ID = messageResponse.UniqueId; //"735694wew";
                smsData.CHANNEL = "SMS";
                smsData.LANG_ID = "0";
                smsData.OA = smsSetting.OA;
                smsData.MSISDN = mobileNo;
                smsData.MESSAGE = message;
                smsData.CAMPAIGN_NAME = smsSetting.CAMPAIGN_NAME;
                smsData.CIRCLE_NAME = smsSetting.CIRCLE_NAME;
                smsData.USER_NAME = smsSetting.USER_NAME;
                smsData.DLT_TM_ID = smsSetting.DLT_TM_ID;
                smsData.DLT_CT_ID = messageTemplateId;
                smsData.DLT_PE_ID = smsSetting.DLT_PE_ID;
                smsdataList.Add(smsData);

                SmsJsonData smsjsonData = new SmsJsonData();
                smsjsonData.keyword = "DEMO";
                smsjsonData.timeStamp = timeStamp; //"071818163530";
                smsjsonData.dataSet = smsdataList;

                byte[] byteData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(smsjsonData));

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(smsSetting.SERVICE_URL);
                request.Accept = "application/json";
                request.ContentType = "application/json";
                request.Method = "POST";
                request.ContentLength = byteData.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                response = (HttpWebResponse)request.GetResponse();

                messageResponse.status_code = SUCCESS;
                messageResponse.result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (WebException ex)
            {
                messageResponse.status_code = ERROR;
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)ex.Response;
                    messageResponse.result = "Some error occured1: " + response.StatusCode.ToString();
                }
                else
                {
                    messageResponse.result = "Some error occured2: " + ex.Status.ToString();
                }
            }
            return messageResponse;
        }

        /// <summary>
        /// Send SMS with Infobip company
        /// </summary>
        /// <param name="smsSetting"></param>
        /// <param name="mobileNo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<MessageResponse> SendSMSInfobip(SmsData smsSetting, string mobileNo, string smsBody)
        {
            MessageResponse messageResponse = new MessageResponse();
            try
            {
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(smsSetting.API_BASE_URL);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("App", smsSetting.API_KEY);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string timeStamp = DateTime.Now.Ticks.ToString();
                messageResponse.UniqueId = mobileNo + timeStamp;

                InfobipSMS infobipSMS = new InfobipSMS();
                Messages messages = new Messages();
                messages.From = smsSetting.HEADER_NAME;
                Destinations destinations = new Destinations();
                destinations.To = "91" + mobileNo;
                destinations.MessageId = messageResponse.UniqueId;
                messages.Destination.Add(destinations);
                IndiaDlt indiaDlt = new IndiaDlt();
                indiaDlt.ContentTemplateId = smsSetting.TEMPLATE_ID; // Template ID which is registered and assign unique SMS ID.
                indiaDlt.PrincipalEntityId = smsSetting.DLT_PE_ID; // This is common ID for a Orgination of any SMS send, Only Template ID need to change.
                messages.Regional.indiaDlt = indiaDlt;
                messages.Text = smsBody;
                infobipSMS.Messages.Add(messages);
                string smsbody = JsonConvert.SerializeObject(infobipSMS);

                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, smsSetting.SERVICE_URL);
                httpRequest.Content = new StringContent(smsbody, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest);
                var responseContent = response.Content.ReadAsStringAsync();

                messageResponse.Status = response.StatusCode == HttpStatusCode.OK ? Utilities.SUCCESS : Utilities.ERROR;
                messageResponse.status_desc = Convert.ToString(responseContent.Status);
                messageResponse.result = Convert.ToString(responseContent.Result);
            }
            catch (Exception ex)
            {
                messageResponse.Status = ERROR;
                messageResponse.result = "Some error occured: " + ex.ToString();
                Utilities.CreateLogFile(PROJECTS.VMS, ex.ToString());
            }
            return messageResponse;
        }


        public static DataTable ReadExcelFileOpenXML(string filePath)
        {
            DataTable excelData = new DataTable();
            try
            {
                //Lets open the existing excel file and read through its content . Open the excel using openxml sdk
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false))
                {

                    WorkbookPart workbookPart = doc.WorkbookPart;
                    SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    SharedStringTable sst = sstpart.SharedStringTable;

                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    Worksheet sheet = worksheetPart.Worksheet;

                    var cells = sheet.Descendants<Cell>();
                    var rows = sheet.Descendants<Row>();

                    //string rrrrrr = "Row count  = " + rows.LongCount().ToString();
                    //string cccccc = "Cell count = " + cells.LongCount().ToString();

                    //Loop through the Worksheet rows.
                    foreach (Row row in rows)
                    {
                        //Use the first row to add columns to DataTable.
                        if (row.RowIndex.Value == 1)
                        {
                            foreach (Cell cell in row.Descendants<Cell>())
                            {
                                excelData.Columns.Add(GetValue(doc, cell));
                            }
                        }
                        else
                        {
                            //Add rows to DataTable.
                            excelData.Rows.Add();
                            int i = 0;
                            foreach (Cell cell in row.Descendants<Cell>())
                            {
                                excelData.Rows[excelData.Rows.Count - 1][i] = GetValue(doc, cell);
                                i++;
                            }
                        }
                    }

                    #region Commented Code NOt in Use, But Working Fine.....

                    // Or... via each row
                    //foreach (Row row in rows)
                    //{
                    //    foreach (Cell c in row.Elements<Cell>())
                    //    {
                    //        if ((c.DataType != null) && (c.DataType == CellValues.SharedString))
                    //        {
                    //            int ssid = int.Parse(c.CellValue.Text);
                    //            string str = sst.ChildElements[ssid].InnerText;
                    //            Console.WriteLine("Shared string {0}: {1}", ssid, str);
                    //            excelResult.Append(string.Concat("Shared string {0}: {1}", ssid, str));
                    //        }
                    //        else if (c.CellValue != null)
                    //        {
                    //            Console.WriteLine("Cell contents: {0}", c.CellValue.Text);
                    //            excelResult.Append(string.Concat("Cell contents: {0}", c.CellValue.Text));
                    //        }
                    //    }
                    //}






                    ////create the object for workbook part  
                    //WorkbookPart workbookPart = doc.WorkbookPart;
                    //Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();

                    ////using for each loop to get the sheet from the sheetcollection  
                    //foreach (Sheet thesheet in thesheetcollection)
                    //{
                    //    excelResult.AppendLine("Excel Sheet Name : " + thesheet.Name);
                    //    excelResult.AppendLine("----------------------------------------------- ");
                    //    //statement to get the worksheet object by using the sheet id  
                    //    Worksheet theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id)).Worksheet;

                    //    SheetData thesheetdata = (SheetData)theWorksheet.GetFirstChild<SheetData>();
                    //    foreach (Row thecurrentrow in thesheetdata)
                    //    {
                    //        foreach (Cell thecurrentcell in thecurrentrow)
                    //        {
                    //            //statement to take the integer value  
                    //            string currentcellvalue = string.Empty;
                    //            if (thecurrentcell.DataType != null)
                    //            {
                    //                if (thecurrentcell.DataType == CellValues.SharedString)
                    //                {
                    //                    int id;
                    //                    if (Int32.TryParse(thecurrentcell.InnerText, out id))
                    //                    {
                    //                        SharedStringItem item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                    //                        if (item.Text != null)
                    //                        {
                    //                            //code to take the string value  
                    //                            excelResult.Append(item.Text.Text + " ");
                    //                        }
                    //                        else if (item.InnerText != null)
                    //                        {
                    //                            currentcellvalue = item.InnerText;
                    //                        }
                    //                        else if (item.InnerXml != null)
                    //                        {
                    //                            currentcellvalue = item.InnerXml;
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //            else
                    //            {
                    //                excelResult.Append(Convert.ToInt16(thecurrentcell.InnerText) + " ");
                    //            }
                    //        }
                    //        excelResult.AppendLine();
                    //    }
                    //    excelResult.Append("");
                    //    //Console.WriteLine(excelResult.ToString());
                    //    //Console.ReadLine();
                    //}

                    #endregion

                }
            }
            catch (Exception)
            {

            }
            return excelData;
        }

        private static string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = string.Empty;
            if (cell.CellValue != null)
            {
                value = cell.CellValue.InnerText;
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
                }
            }
            return value;
        }

        private static string GetExcelSheets(string FilePath, string Extension, string isHDR)
        {
            string conStr = string.Empty;
            switch (Extension)
            {
                case ".xls": //Excel 97-03
                             //conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    conStr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'", FilePath);
                    break;
                case ".xlsx": //Excel 07
                              //conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                    conStr = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0", FilePath);
                    break;
            }
            return conStr;
        }

        public static DataTable MapExcelToDictionary(string filePath)
        {
            DataTable excelData = new DataTable();
            var fileName = @filePath;
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;

                //Get sheet from excel
                var sheets = workbookPart.Workbook.Descendants<Sheet>();

                //First sheet from excel
                Sheet sheet = sheets.FirstOrDefault();

                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                var rows = worksheetPart.Worksheet.Descendants<Row>().ToList();

                //Get all data rows from sheet
                Row headerRow = rows.First();
                var headerCells = headerRow.Elements<Cell>();
                int totalColumns = headerCells.Count();

                List<string> lstHeaders = new List<string>();
                foreach (var value in headerCells)
                {
                    var stringId = Convert.ToInt32(value.InnerText);
                    lstHeaders.Add(workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(stringId).InnerText);
                }

                // Remove the header row
                //rows.RemoveAt(0); // commented by Virkant

                var VendorCode = string.Empty;

                //Iterate to all rows
                foreach (Row r in rows)
                {

                    //Use the first row to add columns to DataTable.
                    if (r.RowIndex.Value == 1)
                    {
                        foreach (Cell cell in r.Descendants<Cell>())
                        {
                            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                            {
                                var stringId = Convert.ToInt32(cell.InnerText);
                                string val = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(stringId).InnerText;
                                excelData.Columns.Add(val);
                            }
                        }
                    }
                    else
                    {
                        //Add rows to DataTable.
                        excelData.Rows.Add();
                        int columnIndex = 0;

                        //Iterate to all cell in current row
                        foreach (Cell c in r)
                        {
                            var mycolIndex = GetColumnIndexNew(c);

                            if (c.DataType != null && c.DataType == CellValues.SharedString)
                            {
                                var stringId = Convert.ToInt32(c.InnerText);
                                string val = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(stringId).InnerText;

                                #region Commented COde

                                //Find cell index and map each cell and add in key value pair
                                //switch (GetColumnIndex(c.CellReference))
                                //{
                                //    case 1:
                                //        VendorCode = val;
                                //        break;

                                //    case 2:
                                //        keyValuePairs.Add(new KeyValuePair<string, string>("VendorName", val));
                                //        break;

                                //    case 3:
                                //        keyValuePairs.Add(new KeyValuePair<string, string>("MobileNo", val));
                                //        break;

                                //    case 4:
                                //        keyValuePairs.Add(new KeyValuePair<string, string>("EmailId", val));
                                //        break;

                                //    case 5:
                                //        keyValuePairs.Add(new KeyValuePair<string, string>("CTRL_AU", val));
                                //        break;

                                //    case 6:
                                //        keyValuePairs.Add(new KeyValuePair<string, string>("PIC", val));
                                //        break;
                                //    case 7:
                                //        keyValuePairs.Add(new KeyValuePair<string, string>("Status", val));
                                //        break;
                                //}

                                #endregion

                                excelData.Rows[excelData.Rows.Count - 1][(int)mycolIndex - 1] = val;
                            }
                            else if (c.InnerText != null || c.InnerText != string.Empty)
                            {
                                //Do code here
                                excelData.Rows[excelData.Rows.Count - 1][(int)mycolIndex - 1] = c.InnerText;
                            }
                            columnIndex++;
                        }
                    }
                }
            }
            return excelData;
        }

        private static int? GetColumnIndex(string cellReference)
        {
            if (string.IsNullOrEmpty(cellReference))
            {
                return null;
            }

            string columnReference = Regex.Replace(cellReference.ToUpper(), @"[\d]", string.Empty);

            int columnNumber = -1;
            int mulitplier = 1;

            foreach (char c in columnReference.ToCharArray().Reverse())
            {
                columnNumber += mulitplier * ((int)c - 64);

                mulitplier = mulitplier * 26;
            }

            return columnNumber + 1;
        }

        public static int GetColumnIndexNew(this Cell cell)
        {
            string columnName = string.Empty;

            if (cell != null)
            {
                string cellReference = cell.CellReference?.ToString();

                if (!string.IsNullOrEmpty(cellReference))
                    // Using `Regex` to "pull out" only letters from cell reference
                    // (leave only "AB" column name from "AB123" cell reference)
                    columnName = Regex.Match(cellReference, @"[A-Z]{1,3}").Value;
            }

            // Column name validations (not null, not empty and contains only UPPERCASED letters)
            // *uppercasing may be done manually with columnName.ToUpper()
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("Column name was not defined.", nameof(columnName));
            else if (!Regex.IsMatch(columnName, @"^[A-Z]{1,3}$"))
                throw new ArgumentException("Column name is not valid.", nameof(columnName));

            int index = 0;
            int pow = 1;

            // A - 1 iteration, AA - 2 iterations, AAA - 3 iterations.
            // On each iteration pow value multiplies by 26
            // Letter number (in alphabet) + 1 multiplied by pow value
            for (int i = columnName.Length - 1; i >= 0; i--)
            {
                index += (columnName[i] - 'A' + 1) * pow;
                pow *= 26;
            }

            // Index couldn't be greater than 16384
            if (index >= 16384)
                throw new IndexOutOfRangeException("Index of provided column name (" + index + ") exceeds max range (16384).");

            return index;
        }


        #region Working , But not in Use

        //private List<VendorInvoice> ReadDataFromExcelLarge(IFormFile postedFile, string filePath, out List<Errors> errors)
        //{
        //    List<VendorInvoice> lstVendorInvoice = new List<VendorInvoice>();
        //    errors = new List<Errors>();

        //    //string excelConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0", filePath);
        //    string excelConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties='Excel 12.0 Xml;HDR=YES'", filePath);
        //    string _extension = Path.GetExtension(postedFile.FileName);
        //    OleDbConnection connection = new OleDbConnection();
        //    connection.ConnectionString = GetExcelSheets(filePath, _extension, ""); //excelConnectionString;
        //    connection.Open();
        //    DataTable dt = new DataTable();
        //    if (_extension == ".xls" || _extension == ".xlsx")
        //    {
        //        DataTable Sheets = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

        //        var s = 0;
        //        foreach (DataRow dr in Sheets.Rows)
        //        {
        //            if (s == 0)
        //            {
        //                string shtname = dr[2].ToString().Replace("'", "");
        //                //OleDbCommand command = new OleDbCommand("select * from [" + shtname + "]", connection);

        //                //OleDbDataAdapter lda = new OleDbDataAdapter(command);
        //                //DataSet ldsContactInfo = new DataSet();
        //                dt = new DataTable(shtname);

        //                //lda.Fill(dt);
        //                connection.Close();

        //                // Vikratn large File
        //                using (FileStream fs = new FileStream(@filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //                {
        //                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fs, false))
        //                    {
        //                        WorkbookPart workbookPart = doc.WorkbookPart;
        //                        SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
        //                        SharedStringTable sst = sstpart.SharedStringTable;

        //                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        //                        Worksheet sheet = worksheetPart.Worksheet;

        //                        var cells = sheet.Descendants<Cell>();
        //                        var rows = sheet.Descendants<Row>();

        //                        //string rrrrrr = "Row count  = " + rows.LongCount().ToString();
        //                        //string cccccc = "Cell count = " + cells.LongCount().ToString();

        //                        //Loop through the Worksheet rows.
        //                        foreach (Row row in rows)
        //                        {
        //                            //Use the first row to add columns to DataTable.
        //                            if (row.RowIndex.Value == 1)
        //                            {
        //                                foreach (Cell cell in row.Descendants<Cell>())
        //                                {
        //                                    dt.Columns.Add(GetValue(doc, cell));
        //                                }
        //                            }
        //                            else
        //                            {
        //                                //Add rows to DataTable.
        //                                dt.Rows.Add();
        //                                int i = 0;
        //                                foreach (Cell cell in row.Descendants<Cell>())
        //                                {
        //                                    dt.Rows[dt.Rows.Count - 1][i] = GetValue(doc, cell);
        //                                    i++;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                // Read data from datatable
        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    string _errorMsg = string.Empty;
        //                    string confirmationPeriod = Convert.ToString(dt.Rows[i]["ConfirmationPeriod"]).Trim();
        //                    string vendorCode = Convert.ToString(dt.Rows[i]["VendorCode"]).Trim();
        //                    string vendorName = Convert.ToString(dt.Rows[i]["VendorName"]).Trim();
        //                    string branchcode = Convert.ToString(dt.Rows[i]["CTRL_AU"]).Trim();
        //                    string invoiceNo = Convert.ToString(dt.Rows[i]["InvoiceNo"]).Trim();
        //                    string invoiceDate = Convert.ToString(dt.Rows[i]["InvoiceDate (mm/dd/yyyy)"]).Trim();
        //                    string closingBalance = Convert.ToString(dt.Rows[i]["ClosingBalance"]).Trim();

        //                    if (string.IsNullOrEmpty(confirmationPeriod))
        //                    {
        //                        _errorMsg += "Confirmation Period is can't be blank at Row Number. ";
        //                    }
        //                    if (string.IsNullOrEmpty(vendorCode))
        //                    {
        //                        _errorMsg += "Vendor Code is can't be blank at Row Number. ";
        //                    }
        //                    if (string.IsNullOrEmpty(vendorName))
        //                    {
        //                        _errorMsg += "Vendor Name is can't be blank at Row Number. ";
        //                    }
        //                    if (string.IsNullOrEmpty(branchcode))
        //                    {
        //                        _errorMsg += "CTRL_AU is can't be blank at Row Number. ";
        //                    }
        //                    if (string.IsNullOrEmpty(invoiceNo))
        //                    {
        //                        _errorMsg += "Invoice Number is can't be blank at Row Number. ";
        //                    }
        //                    if (string.IsNullOrEmpty(invoiceDate))
        //                    {
        //                        _errorMsg += "Invoice Date is can't be blank at Row Number. ";
        //                    }
        //                    if (string.IsNullOrEmpty(closingBalance))
        //                    {
        //                        _errorMsg += "Closing Balance is can't be blank at Row Number. ";
        //                    }

        //                    if (!string.IsNullOrEmpty(invoiceDate.Trim()))
        //                    {
        //                        try
        //                        {
        //                            invoiceDate = DateTime.FromOADate(Convert.ToDouble(invoiceDate)).ToString().Split(' ')[0];
        //                            if (!string.IsNullOrEmpty(invoiceDate.Trim()))
        //                            {
        //                                if (!Utilities.isValidDate(invoiceDate))
        //                                {
        //                                    _errorMsg += "Invoice Date is Invalid at Row Number. ";
        //                                }
        //                                else
        //                                {
        //                                    DateTime datevalue = Convert.ToDateTime(invoiceDate);
        //                                }
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {
        //                            _errorMsg += "Invoice Date is Invalid at Row Number. ";
        //                        }
        //                    }

        //                    if (!string.IsNullOrEmpty(_errorMsg.Trim()))
        //                    {
        //                        errors.Add(new Errors { Error = string.Concat(_errorMsg, " ", (i + 2)) });
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return lstVendorInvoice;
        //}

        #endregion


        public static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public static bool IsMatch4Character(string oldPwd, string newPwd)
        {
            for (int i = 0; i < oldPwd.Length - 3; i++)
            {
                if (newPwd.IndexOf(oldPwd.Substring(i, 4)) >= 0)
                {
                    return true;
                }

            }
            return false;
        }
        public static bool IsMonthName(string name)
        {
            bool result = false;
            if (name.Contains("JAN"))
                result = true;
            else if (name.Contains("FEB"))
                result = true;
            else if (name.Contains("MAR"))
                result = true;
            else if (name.Contains("APR"))
                result = true;
            else if (name.Contains("MAY"))
                result = true;
            else if (name.Contains("JUN"))
                result = true;
            else if (name.Contains("JUL"))
                result = true;
            else if (name.Contains("AUG"))
                result = true;
            else if (name.Contains("SEP"))
                result = true;
            else if (name.Contains("OCT"))
                result = true;
            else if (name.Contains("NOV"))
                result = true;
            else if (name.Contains("DEC"))
                result = true;
            else
                result = false;
            return result;
        }

        //Method for Password Encryption via SHA512 methodology
        public static string Encrypt_SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }


        public static void CreateLogFile(string ProjectName, object message)
        {
            try
            {
                string rootFolder = Directory.GetCurrentDirectory();
                string logFolder = string.Concat(rootFolder, "\\LogFile\\", ProjectName);

                //string ss = ((IWebHostEnvironment)html.ViewContext.HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment))).WebRootPath;
                //string strAssemblyPath ="" ;//Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }
                string FilePath = Path.Combine(logFolder, DateTime.Now.ToString("dd_MMM_yyyy") + "_LogFile.txt");
                File.AppendAllText(FilePath, "==============================" + DateTime.Now.ToString("dd_MMM_yyyy hh:mm:ss") + "=============================" +
                                   Environment.NewLine + message + Environment.NewLine + Environment.NewLine);

            }
            catch (Exception)
            {

            }
        }

        public static string AmountFormat(object amount)
        {
            CultureInfo indiaCultureInfo = new CultureInfo("hi-IN");
            string text = string.Format(indiaCultureInfo, "{0:c}", amount).Replace("₹", ""); // ₹ 3,20,000
            return text;
        }
        public static string AmountFormatWithoutDecimal(object amount)
        {
            CultureInfo indiaCultureInfo = new CultureInfo("hi-IN");
            string text = string.Format(indiaCultureInfo, "{0:c}", Convert.ToInt64(amount)).Replace("₹", ""); // ₹ 3,20,000
            text = text.Remove(text.Length - 3);
            return text;
        }

    }



}
