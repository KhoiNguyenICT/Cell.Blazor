namespace Cell.Blazor.Internal.Interface
{
    public interface IValidator
    {
        bool IsValid(object value, object arguments);
    }
}