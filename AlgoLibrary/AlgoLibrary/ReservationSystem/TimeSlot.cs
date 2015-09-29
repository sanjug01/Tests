using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.ReservationSystem
{
    public enum TimeSlotValues
    {
        T8_830,
        T830_900,
        T900_930,
        T930_10, // and so on
    }
    public class TimeSlot
    {
        private TimeSlotValues _from, _to;
        public string From { get { return TimeSlotToString(_from); } }
        public string To{ get { return TimeSlotToString(_to); } }

        private static string TimeSlotToString(TimeSlotValues timeSlot)
        {
            switch(timeSlot)
            {
                // TODO:
                case TimeSlotValues.T830_900: 
                    return("8-8:30"); // and so on
                default:
                    return "NA";
            }
        }

        public static bool operator== (TimeSlot t1, TimeSlot t2)
        {
            return (t1._from == t2._from) && (t1._to == t2._to) ;
        }
        public static bool operator !=(TimeSlot t1, TimeSlot t2)
        {
            return (t1._from != t2._from) || (t1._to != t2._to);
        }
    }
}
