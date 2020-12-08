using System;
using Cell.Blazor.Internal.Interface;

namespace Cell.Blazor.Internal.Class
{
    public class MinValidator : IValidator
    {
        public bool IsValid(object value, object arguments)
        {
            double num = Convert.ToDouble(arguments);
            if (value == null)
                return true;
            try
            {
                return Convert.ToDouble(value) >= num;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}