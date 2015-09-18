using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    public class MyMediator : IMediator
    {
        private List<IColleague> _colleagues;

        public void RecvRequestFromColleague(object param)
        {
            throw new NotImplementedException();
        }

        public void SendRequestToColleague(object param)
        {
            throw new NotImplementedException();
        }
    }
}
