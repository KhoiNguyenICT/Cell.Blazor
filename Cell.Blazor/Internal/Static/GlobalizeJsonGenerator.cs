using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;

namespace Cell.Blazor.Internal.Static
{
    public static class GlobalizeJsonGenerator
    {
        private static Dictionary<int, string> PositiveCurrencyMapper = new Dictionary<int, string>
        {
            {0, "$n"},
            {1, "n$"},
            {2, "$ n"},
            {3, "n $"}
        };

        private static Dictionary<int, string> NegativeCurrencyMapper = new Dictionary<int, string>
        {
            {0, "($n)"},
            {1, "-$n"},
            {2, "$-n"},
            {3, "$n-"},
            {4, "(n$)"},
            {5, "-n$"},
            {6, "n-$"},
            {7, "n$-"},
            {8, "-n $"},
            {9, "-$ n"},
            {10, "n $-"},
            {11, "$ n-"},
            {12, "$ -n"},
            {13, "n- $"},
            {14, "($ n)"},
            {15, "(n $)"}
        };

        private static Dictionary<int, string> PositivePercentMapper = new Dictionary<int, string>
        {
            {0, "n %"},
            {1, "n%"},
            {2, "%n"},
            {3, "% n"}
        };

        private static Dictionary<int, string> NumberNegativePattern = new Dictionary<int, string>
        {
            {0, "(n)"},
            {1, "-n"},
            {2, "-n"},
            {3, "n-"},
            {4, "n-"}
        };

        private static Dictionary<int, string> NegativePercentMapper = new Dictionary<int, string> {
            {0, "-n %"},
            {1, "-n%"},
            {2, "-%n"},
            {3, " %-n"},
            {4, "%n-"},
            {5, "n-%"},
            {6, "n%-"},
            {7, "-% n "},
            {8, "n %-"},
            {9, "% n-"},
            {10, "% -n"},
            {11, "n- %"}
        };

        public static Dictionary<string, object> GetGlobalizeContent(CultureInfo cultureData)
        {
            NumberFormatInfo numberFormat = cultureData.NumberFormat;
            DateTimeFormatInfo dateTimeFormat = cultureData.DateTimeFormat;
            string timeSeparator = dateTimeFormat.TimeSeparator;
            Dictionary<string,
            object> dictionary = new Dictionary<string,
            object>();
            dictionary.Add("mapper", ConvertStringArrayToString(cultureData.NumberFormat.NativeDigits, false, false));
            dictionary.Add("mapperDigits", string.Join(string.Empty, cultureData.NumberFormat.NativeDigits));
            dictionary.Add("numberSymbols", GenerateNumberSymbols(cultureData.NumberFormat, timeSeparator));
            dictionary.Add("timeSeparator", timeSeparator);
            string currencySymbol = numberFormat.CurrencySymbol;
            dictionary.Add("currencySymbol", numberFormat.CurrencySymbol);
            dictionary.Add("currencypData", GetPositiveCurrencyPercentData(numberFormat, false));
            dictionary.Add("percentpData", GetPositiveCurrencyPercentData(numberFormat, true));
            dictionary.Add("percentnData", GetNegativePercentData(numberFormat));
            dictionary.Add("currencynData", GetNegativeCurrencyData(numberFormat));
            dictionary.Add("decimalnData", DefaultPositiveNegativeData(numberFormat, true));
            dictionary.Add("decimalpData", DefaultPositiveNegativeData(numberFormat, false));
            return new Dictionary<string,
                object>
            {
                {
                    cultureData.Name,
                    new Dictionary<string, object>
                    {
                        {"numbers", dictionary},
                        {"dates", GetDateFormatOptions(dateTimeFormat)}
                    }
                }
            };
        }

        public static string GetGlobalizeJsonString(CultureInfo cultureData) => JsonConvert.SerializeObject(GetGlobalizeContent(cultureData));

        private static Dictionary<string, object> GetDateFormatOptions(DateTimeFormatInfo date) =>
            new Dictionary<string, object>
            {
                {
                    "dayPeriods", new Dictionary<string, string>
                    {
                        {"am", date.AMDesignator},
                        {"pm", date.PMDesignator}
                    }
                },
                {
                    "dateSeperator", date.DateSeparator
                },
                {
                    "days", new Dictionary<string, object>
                    {
                        {
                            "abbreviated",
                            ConvertStringArrayToString(date.AbbreviatedDayNames, true, false)
                        },
                        {
                            "short",
                            ConvertStringArrayToString(date.ShortestDayNames, true, false)
                        },
                        {
                            "wide",
                            ConvertStringArrayToString(date.DayNames, true, false)
                        }
                    }
                },
                {
                    "months", new Dictionary<string, object>
                    {
                        {
                            "abbreviated",
                            ConvertStringArrayToString(date.AbbreviatedMonthNames, false, true)
                        },
                        {
                            "wide",
                            ConvertStringArrayToString(date.MonthNames, false, true)
                        }
                    }
                },
                {
                    "eras",
                    eraData(date)
                }
            };

        private static Dictionary<string,
        object> eraData(DateTimeFormatInfo date)
        {
            int[] eras = date.Calendar.Eras;
            Dictionary<string,
            object> dictionary = new Dictionary<string,
            object>();
            foreach (int era in eras)
                dictionary.Add(era.ToString(), date.GetAbbreviatedEraName(era));
            return dictionary;
        }

        private static Dictionary<string,
        object> DefaultPositiveNegativeData(
        NumberFormatInfo numberFormat, bool isNegative)
        {
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            if (isNegative)
            {
                string[] strArray = NumberNegativePattern[numberFormat.NumberNegativePattern].Split("n");
                empty1 = strArray[0];
                empty2 = strArray[1];
            }

            return new Dictionary<string, object>
            {
                {"nlead", empty1}, {"nend", empty2},
                {
                    "groupData", new Dictionary<string, int> {{"primary", numberFormat.NumberGroupSizes[0]}}
                },
                {
                    "maximumFraction",
                    numberFormat.NumberDecimalDigits
                },
                {
                    "minimumFraction",
                    numberFormat.NumberDecimalDigits
                }
            };
        }

        private static Dictionary<string, object> GetPositiveCurrencyPercentData(NumberFormatInfo numberFormat, bool isPercent)
        {
            string str1 = (isPercent ? PositivePercentMapper : PositiveCurrencyMapper)[isPercent ? numberFormat.PercentPositivePattern : numberFormat.CurrencyPositivePattern];
            string str2 = str1.Replace("n", string.Empty);
            string str3 = string.Empty;
            string str4 = string.Empty;
            char ch = isPercent ? '%' : '$';
            if (!isPercent)
            {
                string currencySymbol = numberFormat.CurrencySymbol;
            }
            else
            {
                string percentSymbol = numberFormat.PercentSymbol;
            }

            if (str1[0].Equals(ch)) str3 = str2.Replace(isPercent ? "%" : "$", numberFormat.CurrencySymbol);
            else
                str4 = str2.Replace(isPercent ? "%" : "$",
                    isPercent ? numberFormat.PercentSymbol : numberFormat.CurrencySymbol);
            Dictionary<string, object> numberData = new Dictionary<string, object> { { "nlead", str3 }, { "nend", str4 } };
            if (isPercent) AddGroupandFractionPercentData(numberData, numberFormat);
            else AddGroupandFractionCurrencyData(numberData, numberFormat);
            return numberData;
        }

        private static Dictionary<string, object> GetNegativeCurrencyData(NumberFormatInfo numberFormat)
        {
            Dictionary<string, object> numberData = NegativePatternProcessor(NegativeCurrencyMapper[numberFormat.CurrencyNegativePattern], "$", numberFormat.CurrencySymbol);
            AddGroupandFractionCurrencyData(numberData, numberFormat);
            return numberData;
        }

        private static Dictionary<string, object> NegativePatternProcessor(string mapper, string currencySymbol, string replacer)
        {
            string[] strArray = mapper.Split("n");
            return new Dictionary<string, object>
            {
                {
                    "nlead", strArray[0].Replace(currencySymbol, replacer)
                },
                {
                    "nend", strArray[1].Replace(currencySymbol, replacer)
                }
            };
        }

        private static Dictionary<string, object> GetNegativePercentData(NumberFormatInfo numberFormat)
        {
            string mapper = NegativePercentMapper[numberFormat.PercentNegativePattern];
            string[] strArray = mapper.Split("n");
            strArray[0].Replace("$", numberFormat.CurrencySymbol);
            strArray[1].Replace("$", numberFormat.CurrencySymbol);
            Dictionary<string,
            object> numberData = NegativePatternProcessor(mapper, "%", numberFormat.PercentSymbol);
            AddGroupandFractionPercentData(numberData, numberFormat);
            return numberData;
        }

        private static void AddGroupandFractionCurrencyData(Dictionary<string, object> numberData, NumberFormatInfo numberFormat)
        {
            numberData.Add("groupSeparator", numberFormat.NumberGroupSeparator);
            numberData.Add("groupData", new Dictionary<string, int>
            {
                {
                    "primary",
                    numberFormat.CurrencyGroupSizes[0]
                }
            });
            numberData.Add("maximumFraction", numberFormat.CurrencyDecimalDigits);
            numberData.Add("minimumFraction", numberFormat.CurrencyDecimalDigits);
        }

        private static void AddGroupandFractionPercentData(Dictionary<string, object> numberData, NumberFormatInfo numberFormat)
        {
            numberData.Add("groupSeparator", numberFormat.PercentGroupSeparator);
            numberData.Add("groupData", new Dictionary<string, int>
            {
                {
                    "primary",
                    numberFormat.PercentGroupSizes[0]
                }
            });
            numberData.Add("maximumFraction", numberFormat.PercentDecimalDigits);
            numberData.Add("minimumFraction", numberFormat.PercentDecimalDigits);
        }

        private static Dictionary<string, string> GenerateNumberSymbols(NumberFormatInfo numberFormat, string timeSep)
        {
            return new Dictionary<string, string>
            {
                {
                    "decimal",
                    numberFormat.NumberDecimalSeparator
                },
                {
                    "group",
                    numberFormat.NumberGroupSeparator
                },
                {
                    "plusSign",
                    numberFormat.PositiveSign
                },
                {
                    "minusSign",
                    numberFormat.NegativeSign
                },
                {
                    "percentSign",
                    numberFormat.PercentSymbol
                },
                {
                    "nan",
                    numberFormat.NaNSymbol
                },
                {
                    "timeSeparator",
                    timeSep
                },
                {
                    "infinity",
                    numberFormat.PositiveInfinitySymbol
                }
            };
        }

        private static Dictionary<string, string> ConvertStringArrayToString(string[] array, bool weekMode, bool? monthMode)
        {
            string[] strArray = new string[7] { "sun", "mon", "tue", "wed", "thu", "fri", "sat" };
            Dictionary<string,
            string> dictionary = new Dictionary<string,
            string>();
            int index = 0;
            bool? nullable = monthMode;
            bool flag = true;
            if (nullable.GetValueOrDefault() == flag & nullable.HasValue) index = 1;
            foreach (string str in array)
            {
                if (str.Length != 0) dictionary.Add(weekMode ? strArray[index] : index.ToString(), str); ++index;
            }
            return dictionary;
        }
    }
}