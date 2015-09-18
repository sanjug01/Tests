using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{

    public class MyCompositeLeaf : ICompositeLeaf
    {
        private readonly int _data;

        public MyCompositeLeaf(int data)
        {
            _data = data;
        }

        public int Operation(string param)
        {
            return _data;
        }
    }

    public class MyComposite : IComposite
    {
        private List<ICompositeBase> _children;
        public MyComposite()
        {
            _children = new List<ICompositeBase>();
        }

        ~MyComposite()
        {
            _children.Clear();
        }

        public bool Add(ICompositeBase node)
        {
            _children.Add(node);
            return true;
        }


        public bool Remove(ICompositeBase node)
        {
            return _children.Remove(node);
        }

        public ICompositeBase GetChild(int n)
        {
            try
            {
                var child = _children[n];
                return child;
            }
            catch (ArgumentOutOfRangeException exc)
            {
                return null;
            }
        }

        // return sum of subtree
        public int Operation(string param)
        {
            int sum = 0;

            foreach(var child in _children)
            {
                sum += child.Operation(param);
            }

            System.Diagnostics.Debug.WriteLine("Summing composite operation for param:{0}", param);
            return sum;
        }

    }
}
