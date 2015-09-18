using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Behavioral
{
    public interface IMediator
    {
        void SendRequestToColleague(object param);

        /// <summary>
        /// Receives request from a colleague.
        /// Has logic to submit requests to other colleagues from the colleagues pool.
        /// </summary>
        /// <param name="param">generic param</param>
        void RecvRequestFromColleague(object param);
    }
}
