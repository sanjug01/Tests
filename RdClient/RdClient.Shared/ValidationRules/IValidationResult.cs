namespace RdClient.Shared.ValidationRules
{
    public interface IValidationResult
    {
        bool IsValid { get; }
        object ErrorContent { get; }
    }
}
