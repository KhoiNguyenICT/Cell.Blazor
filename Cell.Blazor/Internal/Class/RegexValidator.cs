using Cell.Blazor.Internal.Interface;
using System.Text.RegularExpressions;

namespace Cell.Blazor.Internal.Class
{
    public class RegexValidator : IValidator
    {
        public bool IsValid(object value, object arguments)
        {
            if (value == null)
                return true;
            string pattern = arguments as string;
            string input = value.ToString();
            Match match = new Regex(pattern).Match(input);
            if (value == null)
                return true;
            return match.Success && match.Index == 0 & match.Value.Length == input.Length;
        }
    }
}