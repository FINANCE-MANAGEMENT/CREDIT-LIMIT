using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.Common
{
    public class SafeTypeHandling
    {
        /// <summary>
        /// Default private constructor.
        /// </summary>
        private SafeTypeHandling()
        {
        }

        /// <summary>
        /// This function does a safe conversion from an object to a string. If the 
        /// object is NULL, it returns an empty string.
        /// </summary>
        /// <param name="value">The object to be converted into string.</param>
        /// <returns>Converted string.</returns>
        public static string ConvertToString(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }
            else
            {
                return Convert.ToString(value).Trim();
            }
        }
        public static string RemoveTabEnter(string str)
        {
            var sb = new StringBuilder(str.Length);

            foreach (char i in str)
                if (i != '\n' && i != '\r' && i != '\t')
                    sb.Append(i);
                else
                    sb.Append(" ");

            return sb.ToString();
        }
        public static bool HasSpecialCharacters(string str)
        {
            //string specialCharacters = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
            string specialCharacters = @"%!@#$%^&*()?/><:;'\|}]{[_~`+=-" + "\"";
            char[] specialCharactersArray = specialCharacters.ToCharArray();

            int index = str.IndexOfAny(specialCharactersArray);
            //index == -1 no special characters
            if (index == -1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Converts to double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double ConvertToDouble(object value)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return 0.0;
            }
        }

        /// <summary>
        /// This function converts a contained Datatime in an object to a DateTime object.
        /// </summary>
        /// <param name="value">Contained datetime value in an object.</param>
        /// <returns>Converted datetime value.</returns>
        public static DateTime ConvertToDateTime(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return DateTime.MinValue;
            }
            else
            {
                return DateTime.Parse(value.ToString());
            }
        }

        /// <summary>
        /// This method converts a string into an integer.
        /// </summary>
        /// <param name="value">Contained integer value in an object.</param>
        /// <returns>Converted integer value.</returns>
        public static Int32 ConvertStringToInt32(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Int32.Parse(value.ToString());
            }
        }

        /// <summary>
        /// This method converts a string into an long integer.
        /// </summary>
        /// <param name="value">Contained long integer value in an object.</param>
        /// <returns>Converted long integer value.</returns>
        public static Int64 ConvertStringToInt64(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Int64.Parse(value.ToString());
            }
        }

        /// <summary>
        /// This method converts a contained string into a boolean.
        /// </summary>
        /// <param name="value">Contained Boolean value in an object.</param>
        /// <returns>Converted Boolean value.</returns>
        public static bool ConvertStringToBoolean(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return false;
            }
            else if (string.Compare("true", value.ToString().ToLower(), true) == 0
                                || string.Compare("yes", value.ToString().ToLower(), true) == 0
                                || string.Compare("1", value.ToString()) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method converts a boolean value to a Sharepoint Yes/No value.
        /// </summary>
        /// <param name="value">Contained boolean which has to be converted.</param>
        /// <returns>Boolean as string.</returns>
        public static string ConvertBooleanToString(object value)
        {
            string result = "No";
            if (value == null || value == DBNull.Value)
            {
                return result;
            }
            else
            {
                return Convert.ToBoolean(value) ? "Yes" : "No";
            }
        }

        /// <summary>
        /// This method converts a contained List into a delimited string.
        /// </summary>
        /// <param name="list">Contained string value in a list.</param>
        /// <returns>Converted string value.</returns>
        public static string ConvertListToString(List<string> list)
        {
            string delimiter = ";#";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(ConvertToString(list[i]));
                sb.Append(delimiter);
            }

            return sb.ToString();
        }

        /// <summary>
        /// This method converts a contained List into a delimited string.
        /// </summary>
        /// <param name="list">List as input.</param>
        /// <param name="delimiter">String delimiter.</param>
        /// <returns>Delimited string.</returns>
        public static string ConvertListToString(List<string> list, string delimiter)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(ConvertToString(list[i]));
                sb.Append(delimiter);
            }

            return sb.ToString();
        }

        /// <summary>
        /// This method converts a string dictionary into a delimited string.
        /// </summary>
        /// <param name="list">String Dictionary.</param>
        /// <returns>Delimited string.</returns>
        public static string ConvertListToString(Dictionary<string, string> list)
        {
            if (list == null)
            {
                return string.Empty;
            }

            string itemDelimiter = ";";
            string valueDelimiter = ";";
            StringBuilder sb = new StringBuilder();
            foreach (string key in list.Keys)
            {
                sb.Append(ConvertToString(key));
                sb.Append(valueDelimiter);
                sb.Append(ConvertToString(list[key]));
                sb.Append(itemDelimiter);
            }

            return sb.ToString();
        }

        /// <summary>
        /// This method will convert a string to a contained long.
        /// </summary>
        /// <param name="value">Long contained in a string.</param>
        /// <returns>Converted long value.</returns>
        public static long ConvertStringToLong(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return long.Parse(value.ToString());
            }
        }

        /// <summary>
        /// Converts the string to float.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static float ConvertStringToFloat(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }
            else
            {
                float result;
                float.TryParse(value.ToString().Trim(), out result);
                return result;
            }
        }

        public static Decimal ConvertStringToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }
            else
            {
                Decimal result;
                Decimal.TryParse(value.ToString().Trim(), out result);
                return result;
            }
        }

    }
}
