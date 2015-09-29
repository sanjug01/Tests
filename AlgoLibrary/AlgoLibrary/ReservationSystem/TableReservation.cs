using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.ReservationSystem
{
    enum ReserveType
    {
        Free, 
        Attempt,
        Final
    }


    public class TableReservation
    {
        public Table Table {get; set;}
        public PartyGroup Group{get; set;}

        public TimeSlot TimeSlot { get; set; }
    }
}
