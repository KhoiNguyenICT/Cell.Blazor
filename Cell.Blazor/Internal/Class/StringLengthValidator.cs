using Cell.Blazor.Internal.Interface;
using System;

namespace Cell.Blazor.Internal.Class
{
    public class StringLengthValidator : IValidator
    {
        public bool IsValid(object value, object arguments)
        {
            int num = value == null ? 0 : ((string)value).Length;
            double[] doubleArray = CellBaseUtils.ToDoubleArray(arguments);
            if (value == null)
                return true;
            return Convert.ToDouble(num) >= doubleArray[0] && Convert.ToDouble(num) <= doubleArray[1];
        }
    }
}