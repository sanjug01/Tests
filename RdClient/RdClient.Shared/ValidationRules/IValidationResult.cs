namespace RdClient.Shared.ValidationRules
{
    public enum ValidationResultStatus
    {
        Valid,
        Invalid,
        Empty
    }

    public interface IValidationResult
    {
        ValidationResultStatus Status { get; }
        object ErrorContent { get; }
    }
}
