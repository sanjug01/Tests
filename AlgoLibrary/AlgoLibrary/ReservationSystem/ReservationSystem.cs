using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.ReservationSystem
{
    public class ReservationSystem
    {
        Dictionary<ReservationDate, TableReservation> reservations;
        Table[] tables;
        TimeSlot[] timeSlots;


        public bool MakeReservation(PartyGroup group, ReservationDate date, TimeSlot timeSlot)
        {
            return false;
        }

        public List<TimeSlot> GetAvailableSlotsForDate(Table table, ReservationDate date)
        {
            return null;
        }

        public TableReservation GetReservationForGroup()
        {
            return null;
        }

        



    }
}
