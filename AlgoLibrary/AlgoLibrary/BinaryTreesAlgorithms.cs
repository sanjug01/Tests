﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{

    class TrieNode
    {

        public bool isWord;
        public Dictionary<char, TrieNode> children;
        public string prefix;

        // Initialize your data structure here.
        public TrieNode()
        {
            prefix = "";
            isWord = false;
            children = new Dictionary<char, TrieNode>();
        }

        public bool AddWord(string word, int idx)
        {
            if (idx >= word.Length) return false;
            char crtCh = word[idx];
            TrieNode child;
            if (!children.ContainsKey(crtCh))
            {
                child = new TrieNode();
                child.prefix = this.prefix + crtCh;
                children[crtCh] = child;
            }
            else
            {
                child = children[crtCh];
            }

            child.isWord = (idx == word.Length - 1);
            child.AddWord(word, idx + 1);

            return true;
        }

        public bool FindWord(string word, int idx)
        {
            if (idx >= word.Length) return isWord;
            char crtCh = word[idx];
            if (!children.ContainsKey(crtCh))
                return false;

            return children[crtCh].FindWord(word, idx + 1);
        }

        public bool FindPrefix(string word, int idx)
        {
            if (idx >= word.Length) return true; // 
            char crtCh = word[idx];
            if (!children.ContainsKey(crtCh))
                return false;

            return children[crtCh].FindPrefix(word, idx + 1);
        }
    }


    // TODO: untested
    public class Trie
    {
        private TrieNode root;

        public Trie()
        {
            root = new TrieNode();
        }

        // Inserts a word into the trie.
        public void Insert(String word)
        {
            root.AddWord(word, 0);

        }

        // Returns if the word is in the trie.
        public bool Search(string word)
        {
            return root.FindWord(word, 0);
        }

        // Returns if there is any word in the trie
        // that starts with the given prefix.
        public bool StartsWith(string word)
        {
            return root.FindPrefix(word,0);
        }
    }

    // Your Trie object will be instantiated and called as such:
    // Trie trie = new Trie();
    // trie.Insert("somestring");
    // trie.Search("key");

    public class BinTreeNode
    {
        public long Info { get; set; }

        public BinTreeNode Left { get; set; }
        public BinTreeNode Right { get; set; }

        // building a tree 
        public BinTreeNode(long info)
        {
            this.Info = info;
            Left = null;
            Right = null;
        }

        // min value for null childrens
        public BinTreeNode(long info, long left, long right)
        {
            this.Info = info;
            if (long.MinValue != left)
                this.Left = new BinTreeNode(left);
            else
                Left = null;

            if (long.MinValue != right)
                this.Right = new BinTreeNode(right);
            else
                this.Left = null;
        }
    }

    public class BinTreeNodeExt : BinTreeNode
    {
        public BinTreeNodeExt Parent { get; set; }
        public BinTreeNodeExt(long info) : base(info)
        {
            Parent = null;
        }

        public BinTreeNodeExt(long info, long left, long right, BinTreeNodeExt parent) : base(info,left,right)
        {
            Parent = parent;
        }
    }


    public enum Colors
    {
        Red,
        Black,
    }

    public class ColoredTreeNode : BinTreeNodeExt
    {
        public Colors Color { get; set; }

        public ColoredTreeNode(long info, Colors color) : base(info)
        {
            Color = color;
        }

        public ColoredTreeNode(long info, Colors color, long left, long right, ColoredTreeNode parent) : base(info,left,right, parent)
        {
            Color = color;
        }

    }

    public class BinaryTreesAlgorithms
    {

        public BinaryTreesAlgorithms() { }

        public long[] DepthFirst(BinTreeNode tree)
        {
            Stack<BinTreeNode> stack = new Stack<BinTreeNode>();
            Queue<long> results = new Queue<long>();

            if (null != tree)
            {
                stack.Push(tree);

                while (stack.Count > 0)
                {
                    BinTreeNode node = stack.Pop();
                    if (null != node.Right) stack.Push(node.Right);
                    // push left last to search it first
                    if (null != node.Left) stack.Push(node.Left);
                    results.Enqueue(node.Info);
                }
            }

            return results.ToArray();
        }

        public long[] BreadthFirst(BinTreeNode tree)
        {
            Queue<BinTreeNode> queue = new Queue<BinTreeNode>();
            Queue<long> results = new Queue<long>();


            if(null != tree)
            {
                queue.Enqueue(tree);

                while(queue.Count > 0)
                {
                    BinTreeNode node = queue.Dequeue();
                    if (null != node.Left) queue.Enqueue(node.Left);
                    if (null != node.Right) queue.Enqueue(node.Right);
                    results.Enqueue(node.Info);
                }
            }

            return results.ToArray();
        }

        public long[] Preorder(BinTreeNode tree)
        {
            List<long> results = new List<long>();

            if (null != tree)
            {
                results.Add(tree.Info);
                results.AddRange(Preorder(tree.Left));
                results.AddRange(Preorder(tree.Right));
            }

            return results.ToArray();
        }

        public long [] Inorder(BinTreeNode tree)
        {
            List<long> results = new List<long>();

            if(null != tree)
            {
                results.AddRange(Inorder(tree.Left));
                results.Add(tree.Info);
                results.AddRange(Inorder(tree.Right));
            }

            return results.ToArray();
        }

        public void InorderNoRecursion(BinTreeNode tree, List<long> orderList)
        {
            HashSet<BinTreeNode> visited = new HashSet<BinTreeNode>();
            Stack<BinTreeNode> stack = new Stack<BinTreeNode>();

            stack.Push(tree);

            while(stack.Count > 0)
            {
                BinTreeNode crt = stack.Peek();
                if(!visited.Contains(crt))
                {
                    visited.Add(crt);
                    if(null != crt.Left)
                        stack.Push(crt.Left);
                }
                else
                {
                    orderList.Add(crt.Info);
                    stack.Pop();
                    if(null != crt.Right)
                        stack.Push(crt.Right);
                }
            }
        }

        public void PreorderNoRecursion(BinTreeNode tree, List<long> orderList)
        {
            Stack<BinTreeNode> stack = new Stack<BinTreeNode>();
            stack.Push(tree);

            while (stack.Count > 0)
            {
                BinTreeNode crt = stack.Pop();
                orderList.Add(crt.Info);
                if (null != crt.Right) stack.Push(crt.Right);
                if (null != crt.Left) stack.Push(crt.Left);
            }
        }

        public void PostorderNoRecursion(BinTreeNode tree, List<long> orderList)
        {
            Stack<BinTreeNode> stack = new Stack<BinTreeNode>();
            HashSet<BinTreeNode> visited = new HashSet<BinTreeNode>();

            stack.Push(tree);
            while(stack.Count > 0)
            {
                BinTreeNode crt = stack.Peek();
                if(! visited.Contains(crt))
                {
                    visited.Add(crt);
                    if (null != crt.Right) stack.Push(crt.Right);
                    if (null != crt.Left) stack.Push(crt.Left);
                }
                else
                {
                    orderList.Add(crt.Info);
                    stack.Pop();
                }
            }
        }


        public long[] Postorder(BinTreeNode tree)
        {
            List<long> results = new List<long>();


            if (null != tree)
            {
                if (null != tree.Left)
                    results.AddRange(Postorder(tree.Left));
                if (null != tree.Right)
                    results.AddRange(Postorder(tree.Right));

                results.Add(tree.Info);
            }

            return results.ToArray();
        }


        private void LevelSum(BinTreeNode node, int level, List<long> sums)
        {
            if (null == node)
                return;

            // add to sums[level]
            if(sums.Count <= level)
            {
                sums.Add(node.Info);
            }
            else
            {
                sums[level] += node.Info;
            }

            LevelSum(node.Left, level + 1, sums);
            LevelSum(node.Right, level + 1, sums);
        }

        /// <summary>
        /// sum all values on each level, for a BST tree
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public long[] LevelSum(BinTreeNode node)
        {
            List<long> sums = new List<long>();

            int level = 0;
            LevelSum(node, level, sums);

            return sums.ToArray();
        }

        public bool IsBST(BinTreeNode tree, out long min, out long max)
        {
            min = max = long.MinValue;

            if ( null == tree 
                || (null != tree.Left && tree.Left.Info > tree.Info)
                || (null != tree.Right && tree.Right.Info < tree.Info)
                )
                return false;

            min = max = tree.Info;

            if (null != tree.Left)
            {
                long minLeft, maxLeft;
                if (!IsBST(tree.Left, out minLeft, out maxLeft))
                    return false;

                if (maxLeft > tree.Info)
                    return false;

                min = minLeft;
            }

            if (null != tree.Right)
            {
                long minRight, maxRight;
                if (!IsBST(tree.Right, out minRight, out maxRight))
                    return false;

                if (minRight < tree.Info)
                    return false;

                max = maxRight;
            }

            return true ;
        }

        public BinTreeNode GetBSTFromPreorder(long[] elements)
        {
            if (elements.Length == 0)
                return null;
            BinTreeNode root = new BinTreeNode(elements[0]);

            //List<long> leftElements = new List<long>();
            //List<long> rightElements = new List<long>();

            int idx = 1;
            int leftMaxIdx = 0, rightMaxIdx = 0; ;
            while(idx < elements.Length && elements[idx] < root.Info)
            {
                idx++;
            }
            leftMaxIdx = idx - 1;
            

            while (idx < elements.Length && elements[idx] > root.Info)
            {
                idx++;
            }
            rightMaxIdx = idx - 1;

            if(idx != elements.Length)
            {
                throw new System.InvalidOperationException("no matching BST for the given preorder!");
            }

            long[] leftElements = elements.Skip(1).Take(leftMaxIdx).ToArray();
            long[] rightElements = elements.Skip(1 + leftMaxIdx).Take(rightMaxIdx - leftMaxIdx).ToArray();

            root.Left = GetBSTFromPreorder(leftElements);
            root.Right = GetBSTFromPreorder(rightElements);


            return root;
        }

        public BinTreeNodeExt GetCartesianTree(long[] elements, int count)
        {
            // last node is not root
            BinTreeNodeExt lastNode = null;

            if(count==1)
            {
                lastNode = new BinTreeNodeExt(elements[0]);
            }
            else if(count > 1 && count < elements.Length)
            {
                long value = elements[count];
                BinTreeNodeExt prevTree = GetCartesianTree(elements, count - 1);
                BinTreeNodeExt prevprevTree = null;
                BinTreeNodeExt newNode = new BinTreeNodeExt(elements[count]);

                // add the new node to tree
                while(null != prevTree && prevTree.Info > value)
                {
                    prevprevTree = prevTree;
                    prevTree = prevTree.Parent;
                }

                if(prevTree != null)
                {
                    newNode.Right = prevTree.Right;
                    prevTree.Right = newNode;
                    newNode.Parent = prevTree;
                }
                else
                {
                    // new node is the new root, and the rest go to left
                    newNode.Left = prevprevTree;
                    if (null != prevprevTree) prevprevTree.Parent = newNode;
                }

                if (null != newNode.Right) ((BinTreeNodeExt)newNode.Right).Parent = newNode;
                if (null != newNode.Left) ((BinTreeNodeExt)newNode.Left).Parent = newNode;
            }

            return lastNode;
        }

        public BinTreeNodeExt BuildAVLTree(long[] elements, int count)
        {
            BinTreeNodeExt lastNode = null;

            if (count == 1)
            {
                lastNode = new BinTreeNodeExt(elements[0]);
            }

            return lastNode; 
        }

        public ColoredTreeNode BuildRedBlackTree(long[] elements, int count)
        {

            ColoredTreeNode lastNode = null;

            if (count == 1)
            {
                lastNode = new ColoredTreeNode(elements[0], Colors.Black );
            }

            return lastNode;
        }
    }



}


/**
 * Definition for a binary tree node.
 * public class TreeNode {
 *     public int val;
 *     public TreeNode left;
 *     public TreeNode right;
 *     public TreeNode(int x) { val = x; }
 * }
 */
public class TreeNode
{
     public int val;
     public TreeNode left;
     public TreeNode right;
     public TreeNode(int x) { val = x; }
}

public class Solution
{

    private int KthWithSize(TreeNode root, int k, out int size)
    {
        size = 0;
        if (null == root)
            return -1;

        int leftSize = 0;
        int leftVal = KthWithSize(root.left, k, out leftSize);

        if (k <= leftSize)
        {
            size = k;
            return leftVal;
        }
        else if (k - 1 == leftSize)
        {
            size = k;
            return root.val;
        }
        else
        {
            // search right
            int rightSize;
            int val = KthWithSize(root.right, k - 1 - leftSize, out rightSize);
            size = 1 + leftSize + rightSize;
            return val;
        }
    }

    public int KthSmallest(TreeNode root, int k)
    {
        int size = 0;
        return KthWithSize(root, k, out size);
    }


    private void BinaryTreePartialPath(TreeNode node, string partialPath, IList<string> solution)
    {
        if (null == node)
        {
            return;
        }

        string newPartialPath = partialPath + "->" + node.val;

        if (null == node.left && null == node.right)
        {
            solution.Add(newPartialPath);
        }
        else
        {
            BinaryTreePartialPath(node.left, newPartialPath, solution);
            BinaryTreePartialPath(node.right, newPartialPath, solution);
        }

    }

    // easy -
    public IList<string> BinaryTreePaths(TreeNode root)
    {
        List<string> solution = new List<string>();
        if (null == root) return solution;

        if (null == root.left && null == root.right)
        {
            solution.Add(root.val.ToString());
        }

        BinaryTreePartialPath(root, root.val.ToString(), solution);
        return solution;
    }

    private void PartialPathSum(TreeNode node, int sum, List<int> path, int partialSum, IList<IList<int>> solution)
    {
        if (null == node) return;

        path.Add(node.val);
        int crtPos = path.Count;

        partialSum += node.val;

        if (null == node.left && null == node.right && partialSum == sum)
        {
            solution.Add(path.ToArray());
        }
        else
        {
            if (null != node.left)
                PartialPathSum(node.left, sum, path, partialSum, solution);

            if (null != node.right)
                PartialPathSum(node.right, sum, path, partialSum, solution);
        }

        path.RemoveAt(crtPos - 1);
        return;

    }

    public IList<IList<int>> PathSum(TreeNode root, int sum)
    {
        List<IList<int>> solution = new List<IList<int>>();

        if (null == root) return solution;

        int partialSum = 0;
        List<int> partialPath = new List<int>();

        PartialPathSum(root, sum, partialPath, partialSum, solution);

        return solution;
    }


    private bool HasPathPartialSum(TreeNode node, int sum, int partialSum)
    {
        if (null == node)
            return false;

        partialSum += node.val;
        if (null == node.left && null == node.right)
        {
            // leaf
            return partialSum == sum;
        }
        else
        {
            return HasPathPartialSum(node.left, sum, partialSum)
                || HasPathPartialSum(node.right, sum, partialSum);
        }

    }
    // easy
    public bool HasPathSum(TreeNode root, int sum)
    {
        return HasPathPartialSum(root, sum, 0);
    }


    
    private void PartialSum(TreeNode node, int partialSum)
    {
        if (node == null) return; // left or right of a non-leaf

        partialSum = partialSum * 10 + node.val;
        if (null == node.left && null == node.right)
        {
            _total += partialSum;
        }
        else
        {
            PartialSum(node.left, partialSum);
            PartialSum(node.right, partialSum);
        }
    }

    int _total = 0;
    public int SumNumbers(TreeNode root)
    {
        _total = 0;

        return _total;
    }

    int _maxSum = int.MinValue;
    public int MaxPathFromNode(TreeNode node)
    {
        if (null == node) return 0;

        int left = MaxPathFromNode(node.left);
        int right = MaxPathFromNode(node.right);

        int localMax = node.val; // max that go through node
        if (left > 0) localMax += left;
        if (right > 0) localMax += right;

        if (_maxSum < localMax) _maxSum = localMax; 

        return node.val + Math.Max(left,right);
    }
    // hard - math sum on any path, no necesary to pass via root
    public int MaxPathSum(TreeNode root)
    {
        _maxSum = int.MinValue;
        int localMax = MaxPathFromNode(root);
        return _maxSum;
    }


    public int LeftHeight(TreeNode root)
    {
        if (null == root) return 0;
        return 1 + LeftHeight(root.left);
    }

    public int RightHeight(TreeNode root)
    {
        if (null == root) return 0;
        else return 1 + RightHeight(root.right);
    }

    // medium - count complete three nodes
    public int CountCompleteNodes(TreeNode root)
    {
        if (null == root) return 0;

        int leftH = LeftHeight(root.left);
        int rightH = RightHeight(root.right);

        if (leftH == rightH)
        {
            // perfect complete, no need to check middle, bitwise power 2 for performance
            return (1 << (1 + leftH)) - 1;
        }

        return 1 + CountCompleteNodes(root.left) + CountCompleteNodes(root.right);
    }

    // easy - for BST
    public TreeNode LowestCommonAncestorInBST(TreeNode root, TreeNode p, TreeNode q)
    {
        // check null  - lets assume never

        if (p == q) return p;
        int min, max;
        if (p.val < q.val)
        {
            min = p.val; max = q.val;
        }
        else
        {
            min = q.val; max = p.val;
        }

        if (root.val < min)
            return LowestCommonAncestorInBST(root.right, p, q);
        else if (root.val > max)
            return LowestCommonAncestorInBST(root.left, p, q);
        else
            return root;
    }
    
    private void FindOneNode(TreeNode node, TreeNode p, ref bool found)
    {
        if(node == p)
        {
            found = true;
        }
        else
        {
            FindOneNode(node.left, p, ref found);
            if (!found) 
                FindOneNode(node.right, p, ref found);
        }
    }

    private TreeNode FindTwoNodes(TreeNode node, TreeNode p, TreeNode q, ref bool foundP, ref bool foundQ)
    {
        if (null == node) return null;
        if(node == p)
        {
            foundP = true;
            if (foundQ) return null;
            FindOneNode(node, q, ref foundQ);
            if (foundQ) return p;
            else return null;
        }
        if (node == q)
        {
            foundQ = true;
            if (foundP) return null;
            FindOneNode(node, p, ref foundP);
            if (foundP) return q;
            else return null;
        }

        TreeNode foundNode = FindTwoNodes(node.left, p, q, ref foundP, ref foundQ);
        if (foundP && foundQ)
            return foundNode;
        else if (!foundP && !foundQ)
            return FindTwoNodes(node.right, p, q, ref foundP, ref foundQ);

        return node; // only one found left and the other should be right
    }

    // arbitrary tree, no parent - medium hard
    public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q)
    {
        bool foundP = false;
        bool foundQ = false;

        return FindTwoNodes(root, p, q, ref foundP, ref foundQ);
    }

}