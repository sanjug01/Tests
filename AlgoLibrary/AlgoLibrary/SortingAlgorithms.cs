using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{

    public struct Interval
    {
        public long start;
        public long end;
    }

    public class EventPoint:IComparable<EventPoint>
    {
        public long time;
        public bool isStart;

        public int CompareTo(EventPoint other)
        {
            if (time == other.time)
            {
                if (isStart && !other.isStart)
                {
                    return -1;
                }
                else if (isStart == other.isStart)
                    return 0;
                else
                    return 1;
            }
            else
                return
                   time.CompareTo(other.time);
        }
    }

    public class SortingAlgorithms
    {
        public int MaxScheduledEvents(List<Interval> events)
        {
            int maxEvents = 0;
            int crtEvents = 0;
            List<EventPoint> points = new List<EventPoint>(2 * events.Count());

            foreach(Interval ev in events)
            {
                points.Add(new EventPoint { time = ev.start, isStart = true });
                points.Add(new EventPoint { time = ev.end, isStart = false });
            }

            points.Sort();

            foreach (EventPoint p in points)
            {
                if(p.isStart)
                {
                    crtEvents++;
                }
                else
                {
                    if (maxEvents < crtEvents)
                        maxEvents = crtEvents;
                    crtEvents--;
                }

            }

            return maxEvents;
        }

        public void IntervalUnion()
        {

        }


        private bool IntervalsIntersect(Interval i1, Interval i2)
        {
            if (i1.end < i2.start || i2.end < i1.start)
                return false;
            return true;
        }

        /// <summary>
        /// intervals are already sorted and disjoined
        /// </summary>
        /// <param name="intervals"></param>
        /// <param name="newInterval"></param>
        /// <returns></returns>
        public Interval[] AddInterval(Interval[] intervals, Interval newInterval)
        {
            List<Interval> newList = new List<Interval>();

            // binary search for first intersection
            int min = 0, max = intervals.Length-1;
            if(max < 0)
            {
                newList.Add(newInterval);
                return newList.ToArray();
            }
            if (newInterval.end < intervals[0].start)
            {
                newList.Add(newInterval);
                newList.AddRange(intervals);
                return newList.ToArray();
            }
            else if (newInterval.start > intervals[max].end)
            {
                newList.AddRange(intervals);
                newList.Add(newInterval);
                return newList.ToArray();
            } 


            int mid = -1;

            while(min < max)
            {
                mid = (max + min) / 2;

                if(intervals[mid].end < newInterval.start)
                {
                    min = mid + 1;
                }
                else
                {
                    max = mid;
                }
            }

            mid = min;
           
            for(int i = 0; i< mid; i++)
            {
                newList.Add(intervals[i]);
            }

            // merge all possible
            while (mid < intervals.Length && IntervalsIntersect(newInterval, intervals[mid]))
            {
                newInterval.start = Math.Min(newInterval.start, intervals[mid].start);
                newInterval.end = Math.Max(newInterval.end, intervals[mid].end);
                mid++;
            }
            newList.Add(newInterval);


            for(int i = mid; i< intervals.Length; i++ )
            {
                newList.Add(intervals[i]);
            }

            return newList.ToArray();
        }

        // sorts the array
        public bool RadixSort(int[] unsorted, int maxDigits)
        {
            // we don't really need maxDigits, we can stop when all get placed into the 0-bucket
            int crtDigit;
            int crtOffset = 1;
            bool isSorted = false;

            if (null == unsorted || 0 == unsorted.Length)
                return false;

            List<int>[] buckets = new List<int>[10];
            for (int i = 0; i < 10; i++)
                buckets[i] = new List<int>();

            // do the sorting
            for (int j=0; j< maxDigits && !isSorted; j++)
            {
                // clean the buckets
                foreach( var b in buckets)
                {
                    b.Clear();
                }

                // place into buckets, based on digit j
                foreach(var value in unsorted)
                {
                    crtDigit = (value / crtOffset) % 10;
                    buckets[crtDigit].Add(value);
                }


                // merge the buckets
                isSorted = (buckets[0].Count == unsorted.Length);
                int idx = 0;
                foreach(var b in buckets)
                {
                    Array.Copy(b.ToArray(), 0, unsorted, idx, b.Count);
                    idx += b.Count;
                }

                // advance offset
                crtOffset *= 10;
            }

            return true;
        }


        // sorts the array
        public bool InsertSort(int[] unsorted)
        {
            for(int i = 1; i< unsorted.Length; i++)
            {
                // all sorted up to i-1, insert unsorted[i]
                int x = unsorted[i];
                int j = i;
                while(j>0 && x < unsorted[j-1])
                {
                    unsorted[j] = unsorted[j-1];
                    j--;
                }
                unsorted[j] = x;
            }

            return true;
        }

        string StripLicence(string licencePlate)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in licencePlate.ToLower())
            {
                if (char.IsLetter(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public bool StringHasAllLetters(string word, string licence)
        {
            string wordCopy = word;
            foreach (var c in licence)
            {
                int idx = wordCopy.IndexOf(c);
                if (idx > 0)
                {
                    wordCopy = wordCopy.Remove(idx, 1);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        string MinWordForLicence(string[] dictionary, string licence)
        {
            string result = null;

            string searchLetters = StripLicence(licence);

            int minLength = int.MaxValue;
            string crtString = null;
            foreach(var w in dictionary)
            {
                if(w.Length < minLength)
                {
                    if (StringHasAllLetters(w, searchLetters))
                    {
                        minLength = w.Length;
                        crtString = w;
                    }
                }
                
            }

            return result;
        }
    }
}
