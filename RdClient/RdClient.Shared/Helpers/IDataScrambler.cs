namespace RdClient.Shared.Helpers
{
    using RdClient.Shared.Models;

    public interface IDataScrambler
    {
        ScrambledString Scramble(string value);

        string Unscramble(ScrambledString scrambledValue);
    }
}
