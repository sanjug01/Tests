using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{

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
