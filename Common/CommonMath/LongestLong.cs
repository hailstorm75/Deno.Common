using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Math
{
    class LongestLong
    {
        //public const Int64 MaxValue = 9223372036854775807;
        //public const Int64 MinValue = -9223372036854775808;
        public List<long> values { get; set; }
        public long z;
        public LongestLong(List<long> l) {
            values = l;

        }

        List<long> newlist;
        public List<long> Adding(List<long> x, List<long> y, int index, bool carryover)
        {
            //we sould actually be working base max value of long.
            //for sake of example, let us say that y is the shorter number
            //we should consider each pair of longs individually
            //index should ALREADY be the last 
            //before we start we should deal with any carry overs
            //IE DO WE NEED TO ADD ANOTHER LIST TO THE START
            if (index >= 0)
            {
                if (y[index] + x[index] <= long.MaxValue && y[index] + x[index] >= long.MinValue)
                {
                    //safe to add, then move on to the next long pair
                    if (carryover)
                    {

                        newlist.Insert(0, (y[index] + x[index]) + 1);
                    }
                    else
                    {
                        newlist.Insert(0, (y[index] + x[index]));
                    }

                    index--;
                    Adding(x, y, index, false);
                }
                else
                {
                    if (carryover)
                    {
                        newlist.Insert(0, (y[index] + x[index] - long.MaxValue) + 1);
                    }
                    else
                    {
                        newlist.Insert(0, (y[index] + x[index] - long.MaxValue));
                    }
                    index--;
                    Adding(x, y, index, true);
                }
            }
            else
            {
                //bring down remaining from the longer one, which we are taking to be x
                if (x.Count == y.Count && carryover)
                {
                    newlist.Insert(0, 1);
                }
                else if (x.Count > y.Count)
                {
                    for (int i = x.Count - y.Count; i > 0; i--)
                    {
                        newlist.Insert(0, x[i - 1]);
                    }

                }

            }
            return newlist;

        }

        public List<long> Multiply(List<long> x, List<long> y, int index, bool carryover)
        {
            //multiplying is just adding y times
            //under the assumption that the number of elements in y is less than the number of elements in x
            //so now we actually need to access the number represented by list Y.
            List<long> product = new List<long>();
            int lengthofy = y.Count;
            for (int i = 0; i < y.Count; i++)
            {
                for (int j = 0; j < y[i] * (int)System.Math.Pow(long.MaxValue, (y.Count - i - 1)); j++)
                {
                    product = Adding(product, Adding(x, x, x.Count, false), product.Count, false);
                }
            }
            return product;

        }
        public LongestLong Subtract(List<long> l)
        {

            return this;
        }
        public LongestLong Multiply(List<long> l)
        {

            return this;
        }
        public LongestLong Divide(List<long> l)
        {

            return this;
        }

        public bool returnLargest(LongestLong x, LongestLong y) {
            if (x.values.Count > y.values.Count)
            {
                //current long is definitely larger than param 
                return true;
            }
            else if (x.values.Count < y.values.Count)
            {
                return false;
            }
            else
            {
                //equal number of longs
                if (x.values[x.values.Count - 1] > y.values[y.values.Count])
                {
                    return true;
                }
                else if (x.values[x.values.Count - 1] < y.values[y.values.Count])
                {
                    return false;
                }
                else
                {

                }
            }
            return false;
        }

    }
}
