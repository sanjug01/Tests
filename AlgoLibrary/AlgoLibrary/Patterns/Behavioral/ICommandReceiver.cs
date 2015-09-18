using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    public interface ICommandReceiver
    {
        void Action();
    }

    public class ReceiverA : ICommandReceiver
    {
        public void Action()
        {
            System.Diagnostics.Debug.WriteLine("ReceiverA - Action");
        }
    }

    public class ReceiverB : ICommandReceiver
    {
        public void Action()
        {
            System.Diagnostics.Debug.WriteLine("ReceiverB - Action");
        }
    } 
}
