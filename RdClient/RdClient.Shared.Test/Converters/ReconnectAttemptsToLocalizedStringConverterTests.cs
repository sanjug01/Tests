using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class ReconnectAttemptsToLocalizedStringConverterTests
    {
        private ReconnectAttemptsToLocalizedStringConverter _converter;
        private IStringTable _stringTable;
        int _testAttempts;

        [TestInitialize]
        public void TestSetup()
        {            
            _stringTable = new Mock.LocalizedString();
            _converter = new ReconnectAttemptsToLocalizedStringConverter();
            _converter.LocalizedString = _stringTable;
            _testAttempts = 10;
        }

        [TestMethod]
        public void ConvertResultContainsAttempts()
        {
            object result = _converter.Convert(_testAttempts, null, null, null);

            Assert.IsTrue(result is string);
            Assert.IsTrue((result as string).Contains(_testAttempts.ToString()));
        }

        [TestMethod]
        public void ConvertResultContainsMaxAttempts()
        {
            object result = _converter.Convert(_testAttempts, null, null, null);

            Assert.IsTrue(result is string);
            Assert.IsTrue((result as string).Contains(SessionModel.MaxReconnectAttempts.ToString()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertNullThrows()
        {          
            _converter.Convert(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertNonIntThrows()
        {
            string someValue = "10";
            _converter.Convert(someValue, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertThrowsIfLocalizedStringPropertyIsNull()
        {
            _converter.LocalizedString = null;
            _converter.Convert(_testAttempts, null, null, null);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertBackThrows()
        {
            string _testString = "abc";
            _converter.ConvertBack(_testString, null, null, null);

            
        }
    }
}
