namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [TestClass]
    public sealed class SerializableModelTests
    {
        [DataContract(IsReference=true)]
        private sealed class TestModel : SerializableModel
        {
            [DataMember]
            private int _number;
            [DataMember]
            private string _name;

            public TestModel()
            {
                _number = 0;
            }

            public int Number
            {
                get { return _number; }
                set { this.SetProperty(ref _number, value); }
            }

            public string Name
            {
                get { return _name; }
                set { this.SetProperty(ref _name, value); }
            }
        }

        [TestMethod]
        public void NewSerializableModel_ChangeProperty_ChangeReported()
        {
            IList<PropertyChangedEventArgs> reported = new List<PropertyChangedEventArgs>();
            TestModel model = new TestModel() { Number = 1, Name = "Name" };
            model.PropertyChanged += (sender, e) => reported.Add(e);

            ((IPersistentStatus)model).SetClean();
            model.Name += "Plop";
            model.Number += 1;

            Assert.AreEqual(3, reported.Count);
            Assert.AreEqual("Name", reported[0].PropertyName);
            Assert.AreEqual("Status", reported[1].PropertyName);
            Assert.AreEqual("Number", reported[2].PropertyName);
        }

        [TestMethod]
        public void NewSerializableModel_UnregisterObserverChange_ChangeNotReported()
        {
            IList<PropertyChangedEventArgs> reported = new List<PropertyChangedEventArgs>();
            TestModel model = new TestModel() { Number = 1, Name = "Name" };
            PropertyChangedEventHandler handler = (sender, e) => reported.Add(e);
            model.PropertyChanged += handler;

            ((IPersistentStatus)model).SetClean();
            model.Name += "Plop";
            model.PropertyChanged -= handler;
            model.Number += 1;

            Assert.AreEqual(2, reported.Count);
            Assert.AreEqual("Name", reported[0].PropertyName);
            Assert.AreEqual("Status", reported[1].PropertyName);
        }
    }
}
