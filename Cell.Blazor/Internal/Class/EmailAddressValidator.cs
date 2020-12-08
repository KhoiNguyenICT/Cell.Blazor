using Cell.Blazor.Internal.Interface;

namespace Cell.Blazor.Internal.Class
{
    public class EmailAddressValidator : IValidator
    {
        public bool IsValid(object value, object arguments)
        {
            if (value == null)
                return true;
            string str = value as string;
            int num = 0;
            foreach (char ch in str)
            {
                if (ch == '@')
                    ++num;
            }
            return str != null && num == 1 && str[0] != '@' && str[str.Length - 1] != '@';
        }
    }
}