using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyInterview;
using System.Collections.Generic;

namespace MyUnitTests
{


      [TestClass]
    public class TestFriendship
    {

        // A sample test to get you started.
        //
        // Note that this is in NO WAY providing adequate test coverage
        // Please write additional tests to demonstrate your ability to
        // achieve the best test coverage with the least number of test cases.
        //
        // Note this test only exercises 2 of the Friendship methods.
        [TestMethod]
        public void testGetDirectFriends()
        {
            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Bella", "Cindy");
            friendship.makeFriend("Bella", "David");
            friendship.makeFriend("David", "Elizabeth");
            friendship.makeFriend("Cindy", "Frank");

            List<String> directFriends = friendship.getDirectFriends("David");

            List<String> expectedFriends = new List<String>();
            expectedFriends.Add("Bella");
            expectedFriends.Add("Elizabeth");

            CollectionAssert.AreEquivalent(expectedFriends, directFriends);
        }

        [TestMethod]
        public void TestAddOneFriend()
        {
            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");

            Assert.IsTrue(friendship.AreFriends("Aaron", "Bella"));
            Assert.IsTrue(friendship.AreFriends("Bella", "Aaron"));
        }


        [TestMethod]
        public void TestAddOneFriend_DirectFriend()
        {
            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");

            List<string> friendsA = friendship.getDirectFriends("Aaron");
            List<string> friendsB = friendship.getDirectFriends("Bella");

            Assert.AreEqual(1, friendsA.Count);
            Assert.AreEqual(1, friendsB.Count);


            CollectionAssert.Contains(friendsA, "Bella");
            CollectionAssert.Contains(friendsB, "Aaron");            
        }

        [TestMethod]
        public void TestAddOneFriendTwice()
        {

            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Aaron", "Bella");

            Assert.IsTrue(friendship.AreFriends("Aaron", "Bella"));
            Assert.IsTrue(friendship.AreFriends("Bella", "Aaron"));

            // second test - should be separate test method
            List<string> friendsA = friendship.getDirectFriends("Aaron");
            Assert.AreEqual(1, friendsA.Count);
        }


        [TestMethod]
        public void TestAddOneFriendTwice_DirectFriends()
        {

            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Aaron", "Bella");
            
            List<string> friendsA = friendship.getDirectFriends("Aaron");
            Assert.AreEqual(1, friendsA.Count);
        }


        [TestMethod]
        public void TestTwoFriends()
        {

            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Aaron", "Cindy");

            Assert.IsTrue(friendship.AreFriends("Aaron", "Bella"));
            Assert.IsTrue(friendship.AreFriends("Cindy", "Aaron"));
        }


        [TestMethod]
        public void TestTwoFriends_Revoke()
        {

            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Aaron", "Cindy");
            friendship.unmakeFriend("Cindy", "Aaron");

            Assert.IsTrue(friendship.AreFriends("Aaron", "Bella"));


            Assert.IsFalse(friendship.AreFriends("Aaron", "Cindy"));
            Assert.IsFalse(friendship.AreFriends("Cindy", "Aaron"));
        }

        [TestMethod]
        public void TestTwoFriends_DirectFriends()
        {

            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Aaron", "Cindy");

            List<string> friendsA = friendship.getDirectFriends("Aaron");
            Assert.AreEqual(2, friendsA.Count);
            CollectionAssert.Contains(friendsA, "Bella");
            CollectionAssert.Contains(friendsA, "Cindy");
        }


        [TestMethod]
        public void TestTwoFriends_AreNotDirectFriends()
        {

            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Aaron", "Cindy");

            Assert.IsFalse(friendship.AreFriends("Cindy", "Bella"));
        }

        [TestMethod]
        public void TestAddHimself_Nothing()
        {
            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Aaron");

            Assert.IsFalse(friendship.AreFriends("Aaron", "Aaron"));
        }

        [TestMethod]
        public void TestAddOneFriendAndHimself_DirectFriends()
        {

            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Aaron");
            friendship.makeFriend("Aaron", "Bella");

            List<string> friendsA = friendship.getDirectFriends("Aaron");
            Assert.AreEqual(1, friendsA.Count);
        }


        //
        //   indirect friends test cases
        //

        [TestMethod]
        public void TestIndirectFriends_Basic()
        {

            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Bella", "Cindy");

            List<string> friendsA = friendship.getIndirectFriends("Aaron");
            Assert.AreEqual(1, friendsA.Count);
            CollectionAssert.DoesNotContain(friendsA, "Bella");
            CollectionAssert.Contains(friendsA, "Cindy");
        }

        [TestMethod]
        public void TestGetInDirectFriends_Multi()
        {
            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Bella", "Cindy");
            friendship.makeFriend("Bella", "David");
            friendship.makeFriend("David", "Elizabeth");
            friendship.makeFriend("Cindy", "Frank");

            List<String> indirectFriends = friendship.getIndirectFriends("Aaron");

            List<String> expectedFriends = new List<String>();
            expectedFriends.Add("Cindy");
            expectedFriends.Add("David");
            expectedFriends.Add("Elizabeth");
            expectedFriends.Add("Frank");

            CollectionAssert.AreEquivalent(expectedFriends, indirectFriends);
        }

        [TestMethod]
        public void TestGetInDirectFriends_CycleRelation()
        {
            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Bella", "Cindy");
            friendship.makeFriend("Bella", "David");
            friendship.makeFriend("David", "Elizabeth");
            friendship.makeFriend("Cindy", "Frank");
            friendship.makeFriend("Frank", "Aaron");

            List<String> indirectFriends = friendship.getIndirectFriends("Aaron");

            List<String> expectedFriends = new List<String>();
            expectedFriends.Add("Cindy");
            expectedFriends.Add("David");
            expectedFriends.Add("Elizabeth");
            // expectedFriends.Add("Frank");

            CollectionAssert.AreEquivalent(expectedFriends, indirectFriends);
        }

        [TestMethod]
        public void TestGetInDirectFriends_WithNoFriends()
        {
            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            friendship.makeFriend("Bella", "Cindy");
            friendship.makeFriend("Bella", "David");
            friendship.makeFriend("David", "Elizabeth");
            friendship.makeFriend("Cindy", "Frank");

            friendship.unmakeFriend("Aaron", "Bella");

            List<String> indirectFriends = friendship.getIndirectFriends("Aaron");

            Assert.AreEqual(0, indirectFriends.Count);
        }


        [TestMethod]
        public void TestGetInDirectFriends_WithUnreachableNames()
        {

            Friendship friendship = new Friendship();
            friendship.makeFriend("Aaron", "Bella");
            // friendship.makeFriend("Bella", "Cindy");
            friendship.makeFriend("Bella", "David");
            friendship.makeFriend("David", "Elizabeth");
            friendship.makeFriend("Cindy", "Frank");

            List<String> indirectFriends = friendship.getIndirectFriends("Aaron");

            CollectionAssert.DoesNotContain(indirectFriends, "Frank");
            CollectionAssert.DoesNotContain(indirectFriends, "Cindy");

            // positive
            CollectionAssert.Contains(indirectFriends, "David");
            CollectionAssert.Contains(indirectFriends, "Elizabeth");
        }

        // TODO:  
        // test non existing names 
        //    - empty
        //    - after adding 1,2
        
        // more revoking tests
        //    revoke invalid pairs
        //    check indirect friends after revoke to disconeect graph

        // indirect friends - more than one path to an indirect friend
        // direct and should 
        //  check a cycle in the graph - should not go in the queue


        // TODO - thread-safe
    }


}
