using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.Repository.Data
{
    public static class ExtentionUtility
    {
        public static string ToStringOrEmpty(this Object value)
        {
            return String.IsNullOrEmpty(Convert.ToString(value)) == true ? "" : value.ToString().Trim();
        }
        public static DateTime StringToDate(this Object value)
        {
            if (value.ToStringOrEmpty().ToUpper().Replace("NULL", "") == "")
            {
                return Convert.ToDateTime("01/01/0001");
            }
            string DateTimeFormatMMddyyyy = ConvertDateToCustomFormat(value.ToString(), "MM/dd/yyyy", null);
            return Convert.ToDateTime(DateTimeFormatMMddyyyy);
        }
        public static string ConvertDateToCustomFormat(string InputDateString, string NeededFormat = "yyyyMMdd", string[] InputFormat = null)
        {

            CultureInfo culture = new CultureInfo("en-US");

            int size = 4 + (InputFormat == null ? 0 : InputFormat.Length);


            List<string> FormatList = new List<string>() { "MM/dd/yyyy", "MM-dd-yyyy", "yyyyMMdd", "MMddyyyy", "yyyy/MM/dd" };

            if (InputFormat != null)
            {
                FormatList.AddRange(InputFormat);
            }

            string[] formats = FormatList.ToArray();

            if (!string.IsNullOrEmpty(InputDateString.ToStringOrEmpty()))
            {
                //return DateTime.ParseExact(InputDateString, formats, culture, DateTimeStyles.AssumeLocal).ToString(NeededFormat);
                DateTime datetime;
                if (DateTime.TryParseExact(InputDateString, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out datetime))
                {
                    return DateTime.ParseExact(InputDateString, formats, culture, DateTimeStyles.AssumeLocal).ToString(NeededFormat);
                }
                else
                {
                    return InputDateString;
                }
            }
            else
            {
                return InputDateString;
            }
        }
    }
}
