using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalEquationBalancer.Algorithms
{
    public static class Maths
    {
        public static int Gcd0(int[] x)
        {
            int p = x[0];
            foreach (int i in x)
            {
                if (i != 0)
                    p = Gcd(p, i);
            }
            return Math.Abs(p);
        }
        public static int Gcd(int p, int q)
        {
            if (q == 0) { return Math.Abs(p); }
            int r = p % q;
            return Gcd(q, r);
        }
        public static int Lcm(int[] Ys)
        {
            int ans = Ys[0];
            for (int i = 1; i < Ys.Length; i++)
            {
                ans = ans * Ys[i] / Gcd(ans, Ys[i]);
            }
            return ans;
        }
    }
}
