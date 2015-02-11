namespace RdClient.Shared.Helpers
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    static public class Extensions
    {
        /// <summary>
        /// Find the index in a list of aan object that is greater or equal to the specified value.
        /// </summary>
        /// <param name="list">List of objects in which the algorithm looks for the index.</param>
        /// <param name="value">Value for that the index is determined.</param>
        /// <param name="order">Comparison object that defines the order of elements in the list. The list is assumed to be sorted
        /// according to this object.</param>
        /// <returns>Index of the first object in the list that is greater or equal to the specified one.</returns>
        /// <remarks>If the value if less than every object in the list, the returned value is 0, because the new object may be inserted
        /// at the head of the list. If all objects in the list are less that the model, returned value is -1.</remarks>
        public static int IndexOfFirstGreaterOrEqual<TValue>(this IList<TValue> list, TValue value, IComparer<TValue> order)
        {
            Contract.Assert(null != list);
            Contract.Assert(null != order);
            Contract.Assert(null != value);

            int index = -1,
                start = 0,
                end = list.Count - 1;

            while (start <= end)
            {
                int mid = start + (end - start) / 2;
                int c = order.Compare(value, list[mid]);

                if (c > 0)
                    start = mid + 1;
                else
                    end = mid - 1;
            }

            if (start < list.Count)
                index = start;

            return index;
        }

        public static string EmptyIfNull(this string value)
        {
            return value ?? string.Empty;
        }
    }
}
