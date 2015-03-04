using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Converters;
using RdClient.Shared.Converters.ErrorLocalizers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Helpers;

namespace RdClient.Shared.Test.Converters.ErrorLocalizers
{
    public class TestStringTable : IStringTable
    {
        public bool IsUnknown { get; set; }

        public string GetLocalizedString(string key)
        {
            string result = key;

            if(IsUnknown == false)
            {
                result += "loc";
            }

            return result;
        }
    }


    [TestClass]
    public class RdpDisconnectReasonLocalizerTests
    {
        private TestStringTable _testStringTable;
        private TypeToLocalizedStringConverter _typeToLocalizedStringConverter;
        private RdpDisconnectReasonLocalizer _rdpDisconnectReasonLocalizer;

        [TestInitialize]
        public void RdpDisconnectReasonLocalizer_Init()
        {
            _typeToLocalizedStringConverter = new TypeToLocalizedStringConverter();
            _testStringTable = new TestStringTable();
            _typeToLocalizedStringConverter.LocalizedString = _testStringTable;
            _rdpDisconnectReasonLocalizer = new RdpDisconnectReasonLocalizer();
            _rdpDisconnectReasonLocalizer.TypeToLocalizedStringConverter = _typeToLocalizedStringConverter;
        }

        [TestMethod]
        public void RdpDisconnectReasonLocalizer_LocalizeExtended()
        {
            RdpDisconnectReason error = new RdpDisconnectReason(RdpDisconnectCode.TimeSkew, 23, 23);
            string localized = _rdpDisconnectReasonLocalizer.LocalizeError(error);

            Assert.AreEqual("RdpDisconnectCode_TimeSkew_Stringloc\n\nDisconnect_ExtendedErrorCode_Stringloc", localized);
        }

        [TestMethod]
        public void RdpDisconnectReasonLocalizer_Localize()
        {
            RdpDisconnectReason error = new RdpDisconnectReason(RdpDisconnectCode.SSLHandshakeFailed, 23, 0);
            string localized = _rdpDisconnectReasonLocalizer.LocalizeError(error);

            Assert.AreEqual("RdpDisconnectCode_SSLHandshakeFailed_Stringloc\n\nDisconnect_ErrorCode_Stringloc", localized);
        }

        [TestMethod]
        public void RdpDisconnectReasonLocalizer_LocalizeUnknown()
        {
            _testStringTable.IsUnknown = true;
            RdpDisconnectReason error = new RdpDisconnectReason(RdpDisconnectCode.VersionMismatch, 23, 0);
            string localized = _rdpDisconnectReasonLocalizer.LocalizeError(error);

            Assert.AreEqual("RdpDisconnectCode_UnknownError_String\n\nDisconnect_ErrorCode_String", localized);
        }
    }
}
