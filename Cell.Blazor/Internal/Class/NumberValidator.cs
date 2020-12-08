using Cell.Blazor.Internal.Interface;
using System;

namespace Cell.Blazor.Internal.Class
{
    public class NumberValidator : IValidator
    {
        public bool IsValid(object value, object arguments)
        {
            if (value == null)
                return true;
            try
            {
                Convert.ToDouble(value);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}