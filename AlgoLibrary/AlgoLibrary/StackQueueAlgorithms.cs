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
            while(inStack.Count > 0)
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

        // TODO
    }
}
