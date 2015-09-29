using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.ReservationSystem
{
    public class ReservationDate : IComparable<ReservationDate>
    {
        DateTime _datetime;
        public ReservationDate(int year, int month, int day)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
            this._datetime = new DateTime(year, month, day);
        }

        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Day { get; private set; }

        public DateTime ToDateTime()
        {
            return _datetime;
        }

        public int CompareTo(ReservationDate other)
        {
            return this.ToDateTime().CompareTo(other.ToDateTime());
        }
    }
}
