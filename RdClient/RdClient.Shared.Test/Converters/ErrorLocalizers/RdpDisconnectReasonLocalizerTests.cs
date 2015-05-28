using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Converters;
using RdClient.Shared.Converters.ErrorLocalizers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Helpers;
using System.Collections.Generic;

namespace RdClient.Shared.Test.Converters.ErrorLocalizers
{
    public class TestStringTable : IStringTable
    {
        public TestStringTable()
        {
            this.Unknowns = new HashSet<string>();
        }

        public HashSet<string> Unknowns { get; private set; }

        public string GetLocalizedString(string key)
        {
            string result = "";

            if(ItIsKnown(key))
            {
                result = key + "loc";
            }

            return result;
        }

        private bool ItIsKnown(string key)
        {
            return (!this.Unknowns.Contains(key));
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
            _testStringTable.Unknowns.Add("RdpDisconnectCode_VersionMismatch_String");
            RdpDisconnectReason error = new RdpDisconnectReason(RdpDisconnectCode.VersionMismatch, 23, 0);
            string localized = _rdpDisconnectReasonLocalizer.LocalizeError(error);
            Assert.AreEqual("RdpDisconnectCode_UnknownError_Stringloc\n\nDisconnect_ErrorCode_Stringloc", localized);
        }
    }
}
