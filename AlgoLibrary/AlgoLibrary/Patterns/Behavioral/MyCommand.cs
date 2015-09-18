using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    public class MyCommand : ICommand
    {
        private readonly ICommandReceiver _receiver;
        public MyCommand(ICommandReceiver receiver)
        {
            _receiver = receiver;
        }

        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("Executing MyCmd");
            if (null != _receiver)
                _receiver.Action();
        }
    }
}
