using Cell.Blazor.Internal.Interface;
using System;

namespace Cell.Blazor.Internal.Class
{
    public class RangeValidator : IValidator
    {
        public bool IsValid(object value, object arguments)
        {
            double[] doubleArray = CellBaseUtils.ToDoubleArray(arguments);
            if (value == null)
                return true;
            return Convert.ToDouble(value) >= doubleArray[0] && Convert.ToDouble(value) <= doubleArray[1];
        }
    }
}