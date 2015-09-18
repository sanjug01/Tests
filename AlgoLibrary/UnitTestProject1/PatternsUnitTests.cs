using System;
using AlgoLibrary;
using AlgoLibrary.Patterns;
using AlgoLibrary.Patterns.Creational;
using AlgoLibrary.Patterns.Structural;
using AlgoLibrary.Patterns.Behavioral;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlGoUnitTests
{
    [TestClass]
    public class PatternsUnitTest
    {
        #region CreationalPatterns
        [TestMethod]
        public void Test_AbstractFactory()
        {
            MyAbstractFactory f1 = new MyFactoryA();

            var p1 = f1.CreateProduct1();
            var p2 = f1.CreateProduct2();

            Assert.IsInstanceOfType(p1, typeof(MyProduct1));
            Assert.IsInstanceOfType(p2, typeof(MyProduct2));

            MyAbstractFactory f2 = new MyFactoryB();
            var p11 = f2.CreateProduct1();
            var p21 = f2.CreateProduct2();

            Assert.IsInstanceOfType(p11, typeof(MyProductOne));
            Assert.IsInstanceOfType(p21, typeof(MyProductTwo));
        }

        [TestMethod]
        public void Test_Builder()
        {
            // this test is a Director and has a Construct method
            // Construct
            string[] parts = { "wall", "door", "room", "wall", "window", "wall", "window", "wall", "fireplace" };

            {
                MyConcreteBuilder builder = new MyConcreteBuilder();
                MyBuilder b = builder;

                foreach (string part in parts)
                {
                    b.BuildPart(part);
                }

                var result = builder.GetResult();
                System.Diagnostics.Debug.WriteLine(result);

                Assert.IsTrue(result.Length > 0);
            }
        }

        [TestMethod]
        public void Test_FactoryMethod()
        {
            var factoryA = new MyConcreteCreatorFactoryA();
            var productA = factoryA.UseMyProduct();

            var factoryB = new MyConcreteCreatorFactoryB();
            var productB = factoryB.UseMyProduct();

            Assert.AreEqual(productB, "ONE");
        }


        [TestMethod]
        public void Test_Prototype()
        {
            MyConcretePrototypeA prototypeString = new MyConcretePrototypeA();
            MyConcretePrototypeB prototypeInts = new MyConcretePrototypeB();

            MyPrototype[] stringElements = new MyPrototype[] 
                {
                    prototypeString.Clone(),
                    prototypeString.Clone(),
                    prototypeString.Clone(),
                };

            MyPrototype[] intElements = new MyPrototype[]
                {
                    prototypeInts.Clone(),
                    prototypeInts.Clone(),
                    prototypeInts.Clone(),
                    prototypeInts.Clone(),
                };

            Assert.IsInstanceOfType(stringElements[1], typeof(MyConcretePrototypeA));
            Assert.IsInstanceOfType(intElements[2], typeof(MyConcretePrototypeB));
        }
        #endregion


        #region StructuralPatterns
        /// <summary>
        /// this test acts as a Client that uses an Adapter
        /// </summary>
        [TestMethod]
        public void Test_Adapter()
        {
            ITarget classAdapter1 = new MyClassAdapter1();
            classAdapter1.Request("request1");

            ITarget classAdapter2 = new MyClassAdapter2();
            classAdapter2.Request("request2");


            MyAdaptee adaptee = new MyAdaptee();
            ITarget objAdapter = new MyObjectAdapter(adaptee);
            objAdapter.Request("request3");
        }

        /// <summary>
        /// this test acts as a Client that uses an Bridge
        /// </summary>
        [TestMethod]
        public void Test_Bridge()
        {
            int result = 0;

            IBridgeImplementor implA = new MyBridgeImplementorA();
            MyRefinedAbstraction bridgeA1 = new MyRefinedAbstraction(implA);
            result = bridgeA1.Operation("opReq1");
            Assert.AreEqual(1, result);

            // switch implementation
            // alternatively may use a new abstraction, with a new implementation
            IBridgeImplementor implB = new MyBridgeImplementorB();
            bridgeA1.Implementor = implB;
            result = bridgeA1.Operation("opReq2");
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void Test_Composite()
        {
            int result = -1;

            MyComposite c1 = new MyComposite();
            c1.Add(new MyCompositeLeaf(1));
            c1.Add(new MyCompositeLeaf(2));

            MyComposite c2 = new MyComposite();
            c2.Add(new MyCompositeLeaf(3));
            c2.Add(new MyCompositeLeaf(1));


            MyComposite c0 = new MyComposite();
            c0.Add(c1);
            c0.Add(new MyCompositeLeaf(5));
            c0.Add(c2);

            result = c0.Operation("param0");
            Assert.AreEqual(12, result);

            // access to subchildren
            result = c0.GetChild(0).Operation("param1");
            Assert.AreEqual(3, result);


            // invalid child
            Assert.IsNull(c0.GetChild(-1));
            Assert.IsNull(c0.GetChild(5));
        }


        [TestMethod]
        public void Test_Decorator()
        {
            string result1, result2;
            IComponent c0 = new MyComponent("aNiceComponent");

            IComponent decoratedComponent1 =
                new MyPrefixDecorator(
                    "pre__",
                    new MyPostfixDecorator("__post", c0)
                );
            result1 = decoratedComponent1.Operation("param1");

            IComponent decoratedComponent2 =
                new MyPostfixDecorator(
                    "__post",
                    new MyPrefixDecorator("pre__", c0)
                );

            result2 = decoratedComponent2.Operation("param2");

            Assert.AreEqual(result1, result2);

        }

        [TestMethod]
        public void Test_Proxy()
        {
            int result = 0;
            ISubject mySubject = new MySubject();

            result = mySubject.Request("param1");
            Assert.AreEqual(1, result);

            ISubject myProxy = new MyProxy(mySubject);

            // adds 1000 for each proxy call

            result = myProxy.Request("param2");
            Assert.AreEqual(1001, result);

            result = myProxy.Request("param3");
            Assert.AreEqual(2001, result);

            result = myProxy.Request("");
            Assert.AreEqual(3001, result);
        }
        #endregion

        #region BehavioralPatterns

        [TestMethod]
        public void Test_ChainOfResponsability()
        {
            // build a chain
            ChainHandlerBase first = new MyHandlerA(0);
            ChainHandlerBase current = first;

            for (int i=1;i <100; i++)
            {
                if(0 == i%2)
                {
                    current.Next = new MyHandlerA(i);
                }
                else
                {
                    current.Next = new MyHandlerB(i);
                }
                current = current.Next;
            }

            Assert.AreEqual(15000000, first.HandleRequest(15));

            Assert.AreEqual(20000, first.HandleRequest(20));

            Assert.AreEqual(-1, first.HandleRequest(150));

        }

        [TestMethod]
        public void Test_Command()
        {
            // client manages receiver
            ReceiverA recvA = new ReceiverA();
            ReceiverB recvB = new ReceiverB();


            // Invoker part
            {
                MyCommand commandOne = new MyCommand(recvA);
                MyCommand commandTwo = new MyCommand(recvB);

                // execution may take in a separate thread
                commandOne.Execute();
                commandTwo.Execute();
            }

            Assert.IsTrue(true);
        }


        [TestMethod]
        public void Test_Observer()
        {
        }


        [TestMethod]
        public void Test_Template()
        {
        }

        [TestMethod]
        public void Test_Visitor()
        {
        }

        [TestMethod]
        public void Test_State()
        {
        }

        [TestMethod]
        public void Test_NextTest()
        {
        }

        // more 
        // interpreter
        // iterator
        // mediator
        // memento
        // strategy
        #endregion
    }
}
