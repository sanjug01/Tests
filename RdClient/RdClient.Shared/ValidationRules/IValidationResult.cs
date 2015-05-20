namespace RdClient.Shared.ValidationRules
{
    public enum ValidationResultStatus
    {
        Valid,
        Invalid,
        NullOrEmpty
    }

    public interface IValidationResult
    {
        ValidationResultStatus Status { get; }
        object ErrorContent { get; }
    }
}
