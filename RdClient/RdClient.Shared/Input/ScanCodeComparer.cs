namespace RdClient.Shared.Input
{
    using System.Collections.Generic;
    using Windows.UI.Core;

    public sealed class ScanCodeComparer : IComparer<CorePhysicalKeyStatus>
    {
        int IComparer<CorePhysicalKeyStatus>.Compare(CorePhysicalKeyStatus x, CorePhysicalKeyStatus y)
        {
            return x.ScanCode < y.ScanCode ? -1 : x.ScanCode > y.ScanCode ? 1 : CompareBoolean(x.IsExtendedKey, y.IsExtendedKey);
        }

        private static int CompareBoolean(bool x, bool y)
        {
            return x == y ? 0 : !x ? -1 : 1;
        }
    }
}
