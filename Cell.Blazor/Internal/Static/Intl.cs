using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cell.Blazor.Internal.Static
{
    public static class Intl
    {
        public static CultureInfo CurrentCulture;

        public static Dictionary<string, string> CurrencyData =
            CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Where(c => !c.IsNeutralCulture).Select(
                culture =>
                {
                    try
                    {
                        return new RegionInfo(culture.Name);
                    }
                    catch
                    {
                        return (RegionInfo)null;
                    }
                }).Where(ri => ri != null)
            .GroupBy(ri => ri.ISOCurrencySymbol)
            .ToDictionary(
                x => x.Key,
                x => x.First().CurrencySymbol);

        public static CultureInfo DefaultCulture { get; set; } = new CultureInfo("en-US");

        public static string GetDateFormat<T>(T date, string format = null, string culture = null)
        {
            try
            {
                CultureInfo culture1 = GetCulture(culture);
                return GetNativeDigits(
                    ((object)date as IFormattable).ToString(format, culture1),
                    culture1.NumberFormat.NativeDigits);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string GetNumericFormat<T>(
            T numberValue,
            string format = null,
            string culture = null,
            string currencyCode = null)
        {
            try
            {
                CultureInfo cultureInfo = (CultureInfo)GetCulture(culture).Clone();
                string currencySymbol = cultureInfo.NumberFormat.CurrencySymbol;
                if (currencyCode != null && CurrencyData[currencyCode] != null)
                    cultureInfo.NumberFormat.CurrencySymbol = CurrencyData[currencyCode];
                string nativeDigits =
                    GetNativeDigits(
                        ((object)numberValue as IFormattable).ToString(format, cultureInfo),
                        cultureInfo.NumberFormat.NativeDigits);
                if (currencyCode != null)
                    cultureInfo.NumberFormat.CurrencySymbol = currencySymbol;
                return nativeDigits;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static int GetWeekOfYear(DateTime dateValue, string culture = null)
        {
            CultureInfo culture1 = GetCulture(culture);
            return culture1.Calendar.GetWeekOfYear(dateValue, CalendarWeekRule.FirstDay,
                culture1.DateTimeFormat.FirstDayOfWeek);
        }

        public static DateTime GetFirstDayOfWeek(DateTime dateValue, string culture = null)
        {
            int num = GetCulture(culture).DateTimeFormat.FirstDayOfWeek - dateValue.DayOfWeek;
            return dateValue.AddDays(num);
        }

        public static List<string> GetNarrowDayNames(string culture = null)
        {
            List<string> stringList = new List<string>();
            foreach (string shortestDayName in GetCulture(culture).DateTimeFormat.ShortestDayNames)
                stringList.Add(shortestDayName[0].ToString());
            return stringList;
        }

        public static CultureInfo GetCulture(string culture = null)
        {
            if (!string.IsNullOrEmpty(culture))
                return new CultureInfo(culture);
            return CurrentCulture == null ? DefaultCulture : CurrentCulture;
        }

        public static void SetCulture(CultureInfo culture)
        {
            CurrentCulture = culture;
            GlobalizeJsonGenerator.GetGlobalizeContent(culture);
        }

        public static void SetCulture(string culture) => CurrentCulture = new CultureInfo(culture);

        public static string GetNativeDigits(string formatValue, string[] nativeDigits) => formatValue
            .Replace("0", nativeDigits[0]).Replace("1", nativeDigits[1]).Replace("2", nativeDigits[2])
            .Replace("3", nativeDigits[3]).Replace("4", nativeDigits[4]).Replace("5", nativeDigits[5])
            .Replace("6", nativeDigits[6]).Replace("7", nativeDigits[7]).Replace("8", nativeDigits[8])
            .Replace("9", nativeDigits[9]);

        public static object GetCultureFormats(string cultureCode)
        {
            CultureInfo culture = GetCulture(cultureCode);
            return new Dictionary<string, object>
            {
                [cultureCode] = new Dictionary<string, object>
                {
                    ["d"] = culture.DateTimeFormat.ShortDatePattern,
                    ["D"] = culture.DateTimeFormat.LongDatePattern,
                    ["f"] = culture.DateTimeFormat.LongDatePattern + " " +
                            culture.DateTimeFormat.ShortTimePattern,
                    ["F"] = culture.DateTimeFormat.FullDateTimePattern,
                    ["g"] = culture.DateTimeFormat.ShortDatePattern + " " +
                            culture.DateTimeFormat.ShortTimePattern,
                    ["G"] = culture.DateTimeFormat.ShortDatePattern + " " +
                            culture.DateTimeFormat.LongTimePattern,
                    ["m"] = culture.DateTimeFormat.MonthDayPattern,
                    ["M"] = culture.DateTimeFormat.MonthDayPattern,
                    ["r"] = culture.DateTimeFormat.RFC1123Pattern,
                    ["R"] = culture.DateTimeFormat.RFC1123Pattern,
                    ["s"] = culture.DateTimeFormat.SortableDateTimePattern,
                    ["t"] = culture.DateTimeFormat.ShortTimePattern,
                    ["T"] = culture.DateTimeFormat.LongTimePattern,
                    ["u"] = culture.DateTimeFormat.UniversalSortableDateTimePattern,
                    ["U"] = culture.DateTimeFormat.FullDateTimePattern,
                    ["y"] = culture.DateTimeFormat.YearMonthPattern,
                    ["Y"] = culture.DateTimeFormat.YearMonthPattern
                }
            };
        }
    }
}