using System;
using Cell.Blazor.Internal.Interface;

namespace Cell.Blazor.Internal.Class
{
    public class MaxLengthValidator : IValidator
    {
        public bool IsValid(object value, object arguments)
        {
            int int32 = Convert.ToInt32(arguments);
            return value == null || (!(value is string str) ? ((Array)value).Length : str.Length) <= int32;
        }
    }
}