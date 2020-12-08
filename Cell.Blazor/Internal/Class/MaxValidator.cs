using Cell.Blazor.Internal.Interface;
using System;

namespace Cell.Blazor.Internal.Class
{
    public class MaxValidator : IValidator
    {
        public bool IsValid(object value, object arguments)
        {
            double num = Convert.ToDouble(arguments);
            if (value == null)
                return true;
            try
            {
                return Convert.ToDouble(value) <= num;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}