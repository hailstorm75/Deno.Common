using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Math
{
  public interface INumeric<T> where T : struct
  {
    T MaxValue { get; }
    T MinValue { get; }
    T Add(T a, T b);
    T Subtract(T a, T b);
    T Multiply(T a, T b);
    T Divide(T a, T b);
  }

 // public struct BaseNumeric :
 //     INumeric<short>,
 //     INumeric<int>,
 //     INumeric<long>,
 //     INumeric<float>,
 //     INumeric<double>,
 //     INumeric<decimal>
 //{
 //   public int Divide(int a, int b)
 //   {
 //     if (b == 0) throw new DivideByZeroException();
 //     return a / b;
 //   }

 //   public int Multiply(int a, int b) => a * b;

 //   public int Subtract(int a, int b) => a - b;

 //   int INumeric<int>.Add(int a, int b) => a + b;

 // }
}
