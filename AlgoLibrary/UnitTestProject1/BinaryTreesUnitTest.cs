using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgoLibrary;
using System.Collections.Generic;

namespace AlGoUnitTests
{

    [TestClass]
    public class BinaryTreesUnitTest
    {
        BinaryTreesAlgorithms _algo;
        BinTreeNode _testTree;
        long[] _treeElements;

        [TestInitialize]
        public void Setup()
        {
            _algo = new BinaryTreesAlgorithms();

            _treeElements = new long[]
            {
                5,2,1,3,8,6,11,9,15
            };

            _testTree = _algo.GetBSTFromPreorder(_treeElements);
            Assert.IsNotNull(_testTree);
        }

        [TestMethod]
        public void Test_BstFromPreorder()
        {
            long[] preorder = _algo.Preorder(_testTree);
            long[] postorder = _algo.Postorder(_testTree);
            Assert.AreEqual(_treeElements.Length, preorder.Length);
            Assert.AreEqual(_treeElements.Length, postorder.Length);

            long min, max;
            Assert.IsTrue(_algo.IsBST(_testTree, out min, out max));


            // change root to 7. it is no longer a BST
            _testTree.Info = 7;
            Assert.IsFalse(_algo.IsBST(_testTree, out min, out max));
        }

        [TestMethod]
        public void Test_BreadthOrDepthFirst()
        {
            long[] breadthFirstOrder = _algo.BreadthFirst(_testTree);
            long[] depthFirstOrder = _algo.DepthFirst(_testTree);

            Assert.IsTrue(true);
        }


        [TestMethod]
        public void Test_Postorder2()
        {

            long[] postorder = _algo.Postorder(_testTree);

            List<long> postorderList = new List<long>();
            _algo.PostorderNoRecursion(_testTree, postorderList);

            int i = 0;
            foreach (var e in postorderList)
            {
                Assert.AreEqual(postorder[i++], e);
            }

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_Preorder2()
        {
            long[] preorder = _algo.Preorder(_testTree);

            List<long> preorderList = new List<long>();
            _algo.PreorderNoRecursion(_testTree, preorderList);

            int i = 0;
            foreach (var e in preorderList)
            {
                Assert.AreEqual(preorder[i++], e);
            }

            Assert.IsTrue(true);
        }
    

        [TestMethod]
        public void Test_Inorder2()
        {
            long[] inorder = _algo.Inorder(_testTree);

            List<long> inorderList = new List<long>();
            _algo.InorderNoRecursion(_testTree, inorderList);

            int i = 0;
            foreach(var e in inorderList)
            {
                Assert.AreEqual(inorder[i++], e);
            }

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_LevelSum()
        {
            var results = _algo.LevelSum(_testTree);
            Assert.IsTrue(true);
        }
    }
}
