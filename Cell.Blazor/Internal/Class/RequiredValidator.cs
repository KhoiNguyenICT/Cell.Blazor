using Cell.Blazor.Internal.Interface;

namespace Cell.Blazor.Internal.Class
{
    public class RequiredValidator : IValidator
    {
        public bool IsValid(object value, object arguments) => value != null && (!(value is string str) || str.Trim().Length != 0);
    }
}