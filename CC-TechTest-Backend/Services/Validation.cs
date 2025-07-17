using CC_TechTest_Backend.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CC_TechTest_Backend.Services
{
    public static class Validation
    {
        private static string GetField(string[] fields, int index)
        {
            return (fields.Length > index ? fields[index].Trim() : string.Empty);
        }

        public static bool TryParseDataRow(string row, out RowData validRow, out InvalidRowData invalidRow)
        {
            validRow = new();
            invalidRow = null;
            int failedValidationFlag = 0;

            string[] fieldData = row.Split('|');

            string mpan = GetField(fieldData, 0);
            string meterSerial = GetField(fieldData, 1);
            string dateOfInstallation = GetField(fieldData, 2);
            string addressLine1 = GetField(fieldData, 3);
            string postcode = GetField(fieldData, 4);
            
            if (!TryValidateMpan(mpan, ref validRow))
                failedValidationFlag += (int)InvalidRowData.FieldFlags.MPAN;
            if (!TryValidateMeterSerial(meterSerial, ref validRow))
                failedValidationFlag += (int)InvalidRowData.FieldFlags.MeterSerial;
            if (!TryValidateDateOfInstallation(dateOfInstallation, ref validRow))
                failedValidationFlag += (int)InvalidRowData.FieldFlags.DateOfInstallation;
            if (!TryValidateAddressLine1(addressLine1, ref validRow))
                failedValidationFlag += (int)InvalidRowData.FieldFlags.AddressLine1;
            if (!TryValidatePostcode(postcode, ref validRow))
                failedValidationFlag += (int)InvalidRowData.FieldFlags.Postcode;

            if (failedValidationFlag > 0)
            {
                invalidRow = new()
                {
                    MPAN = mpan,
                    MeterSerial = meterSerial,
                    DateOfInstallation = dateOfInstallation,
                    AddressLine1 = addressLine1,
                    Postcode = postcode,
                    FailedField = failedValidationFlag
                };
                return false;
            } else
                return true;
        }

        public static bool TryValidateMpan(string mpan, ref RowData rowData)
        {
            // Expected SQL type is Numeric(13,0), is mandatory and formatted: 9(13,0)
            if (!Regex.IsMatch(mpan, @"^\d{13}$"))
                return false;

            rowData.MPAN = long.Parse(mpan);
            return true;
        }

        public static bool TryValidateMeterSerial(string meterSerial, ref RowData rowData)
        {
            // Expected SQL type is VarChar(10) and is mandatory
            if (Encoding.UTF8.GetByteCount(meterSerial) > 10 || meterSerial == string.Empty)
                return false;

            rowData.MeterSerial = meterSerial;
            return true;
        }

        public static bool TryValidateDateOfInstallation(string date, ref RowData rowData)
        {
            // Expected SQL type is Date in YYYYMMDD format and is mandatory
            bool isDate = DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDate);
            if (!isDate || validDate >= DateTime.Now)
                return false;

            rowData.DateOfInstallation = validDate;
            return true;
        }

        public static bool TryValidateAddressLine1(string address, ref RowData rowData)
        {
            // Expected SQL type is VarChar(40)
            if (Encoding.UTF8.GetByteCount(address) > 40)
                return false;
            if (address == string.Empty)
                rowData.AddressLine1 = null;
            else
                rowData.AddressLine1 = address;
            return true;
        }

        public static bool TryValidatePostcode(string postcode, ref RowData rowData)
        {
            // Expected SQL type is VarChar(10) and formatted: XX9 9XX (XOR with empty string as field is optional)
            if (Encoding.UTF8.GetByteCount(postcode) > 10 || (!Regex.IsMatch(postcode, @"^[A-Z]{2}\d\s\d[A-Z]{2}$") ^ postcode == string.Empty))
                return false;

            if (postcode == string.Empty)
                rowData.Postcode = null;
            else
                rowData.Postcode = postcode;
            return true;
        }
    }
}
