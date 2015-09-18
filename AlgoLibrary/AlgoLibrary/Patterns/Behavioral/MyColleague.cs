using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    public class MyColleagueA : IColleague
    {
        public IMediator Mediator { get; private set; }

        public void OperationA()
        {
            this.Mediator.RecvRequestFromColleague("operA");
        }
    }

    public class MyColleagueB : IColleague
    {
        public IMediator Mediator { get; private set; }

        public void OperationB()
        {
            this.Mediator.RecvRequestFromColleague("operB");
        }
    }
}
