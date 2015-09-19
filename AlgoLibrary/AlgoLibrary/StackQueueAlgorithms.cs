using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{
    public class Queue
    {
        Stack<int> inStack;
        Stack<int> outStack;

        private void MoveStacks()
        {
            while (inStack.Count > 0)
            {
                outStack.Push(inStack.Pop());
            }
        }

        // Push element x to the back of queue.
        public void Push(int x)
        {
            inStack.Push(x);
        }

        // Removes the element from front of queue.
        public void Pop()
        {
            if (outStack.Count > 0)
            {
                outStack.Pop();
            }
            else
            {
                // move inStack into outStack
                MoveStacks();
                outStack.Pop();
            }
        }

        // Get the front element.
        public int Peek()
        {
            if (outStack.Count > 0)
            {
                return outStack.Peek();
            }
            else
            {
                // move inStack into outStack
                MoveStacks();
                return outStack.Peek();
            }
        }

        // Return whether the queue is empty.
        public bool Empty()
        {
            return (inStack.Count == 0 && outStack.Count == 0);
        }
    }
    public class StackQueueAlgorithms
    {
        public StackQueueAlgorithms() { }

        public int NthUglyNumber(int n)
        {
            if (n <= 0) return 1;
            if (n == 1) return 1;


            Queue<int> twos = new Queue<int>();
            Queue<int> threes = new Queue<int>();
            Queue<int> fives = new Queue<int>();

            twos.Enqueue(2);
            threes.Enqueue(3);
            fives.Enqueue(5);

            int idx = 2;
            int min = 2;
            while (idx <= n)
            {
                min = Math.Min(Math.Min(twos.Peek(), threes.Peek()), fives.Peek());

                // dequeue, discarding duplicates
                if (min == twos.Peek())
                    twos.Dequeue();
                if (min == threes.Peek())
                    threes.Dequeue();
                if (min == fives.Peek())
                    fives.Dequeue();

                twos.Enqueue(2 * min);
                threes.Enqueue(3 * min); // don't want 3 * 2
                fives.Enqueue(5 * min);  // dont' want 5*2 and 5*3
                idx++;
            }

            // idx is n+1 and min is n-th

            return min;
        }

        public int NthUglyNumberWithSortedArray(int n)
        {
            if (n <= 0) return 1;
            if (n == 1) return 1;

            // not clear if improvement in compexity
            SortedSet<int> sortedSet = new SortedSet<int>();

            sortedSet.Add(2);
            sortedSet.Add(3);
            sortedSet.Add(5);

            int idx = 2;
            int min = 2;
            while (idx <= n)
            {
                min = sortedSet.Min;
                sortedSet.Remove(min);


                // if (idx + sortedSet.Count < n) - incorrect, may still add value
                {
                    if (!sortedSet.Contains(2 * min)) sortedSet.Add(2 * min);
                    if (!sortedSet.Contains(3 * min)) sortedSet.Add(3 * min);
                    if (!sortedSet.Contains(5 * min)) sortedSet.Add(5 * min);
                }

                idx++;
            }

            return min;
        }
    }
}