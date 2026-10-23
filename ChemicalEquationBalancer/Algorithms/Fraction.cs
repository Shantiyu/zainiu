using ChemicalEquationBalancer.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalEquationBalancer.Algorithms
{
    public struct frac
    {
        public int a, b;
        public frac(int a, int b)
        {
            this.a = a;
            this.b = b;
        }

        public override string ToString()
        {
            return a + "/" + b;
        }

        public void Reduce()
        {
            if (a == 0) {b = 1; return; }
            int k = gcd(a, b);
            a /= k;
            b /= k;
            if (a < 0 && b < 0)
            {
                a = -a; b = -b;
            }
        }

        public static int gcd(int p, int q) 
        {
            if (q == 0) { return Math.Abs(p); } 
            int r = p % q; return gcd(q, r); 
        }

        public static frac operator +(frac x, frac y)
        {
            frac f = new frac(x.a * y.b + x.b * y.a, x.b * y.b);
            f.Reduce();
            return f;
        }

        public static frac operator -(frac x, frac y)
        {
            frac f = new frac(x.a * y.b - x.b * y.a, x.b * y.b);
            f.Reduce();
            return f;
        }

        public static frac operator *(frac x, frac y)
        {
            frac f = new frac(x.a * y.a, x.b * y.b);
            f.Reduce();
            return f;
        }

        public static frac operator /(frac x, frac y)
        {
            frac f = new frac(x.a * y.b, x.b * y.a);
            f.Reduce();
            return f;
        }

        public static bool operator >(frac x, frac y)
        {
            return x.a * y.b > x.b * y.a;
        }

        public static bool operator <(frac x, frac y)
        {
            return x.a * y.b < x.b * y.a;
        }

        public static bool operator >=(frac x, frac y)
        {
            return x.a * y.b >= x.b * y.a;
        }

        public static bool operator <=(frac x, frac y)
        {
            return x.a * y.b <= x.b * y.a;
        }

        public static bool operator !=(frac x, frac y)
        {
            return x.a * y.b != x.b * y.a;
        }

        public static bool operator ==(frac x, frac y)
        {
            return x.a * y.b == x.b * y.a;
        }

        public static implicit operator frac(int L) => new frac(L, 1);

        public override bool Equals(object obj)
        {
            return obj is frac && this == (frac)obj;
        }

        public override int GetHashCode()
        {
            //“==”按交叉相乘比较，因此哈希也必须基于约分后的形式，
            //否则 2/4 与 1/2 相等却会得到不同的哈希值。
            frac f = this;
            f.Reduce();
            if (f.b < 0)
            {
                f.a = -f.a;
                f.b = -f.b;
            }
            return (f.a * 397) ^ f.b;
        }

        public static frac Pow(frac f, int p)
        {
            f.a = (int)Math.Pow(f.a, p);
            f.b = (int)Math.Pow(f.b, p);
            f.Reduce();
            return f;
        }
    }   
}
