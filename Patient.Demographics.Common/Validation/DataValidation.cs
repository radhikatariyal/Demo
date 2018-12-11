using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Demographics.Common.Validation
{
    public static class DataValidation
    {
        public static bool IsInt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            int check;

            var numberIsInteger = int.TryParse(value, NumberStyles.Any, null, out check);

            if (!numberIsInteger)
            {
                return false;
            }

            return true;
        }

        public static bool IsImagePresentAtUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return true;
            }

            HttpWebResponse response = null;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                return false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return true;
        }
    }
}
