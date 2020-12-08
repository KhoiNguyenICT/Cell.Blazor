namespace Cell.Blazor.Internal.Class
{
    public class ValidationResult
    {
        public string FieldName { get; set; }

        public bool IsValid { get; set; }

        public string Rule { get; set; }

        public string Message { get; set; }
    }
}