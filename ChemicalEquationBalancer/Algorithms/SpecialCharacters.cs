using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalEquationBalancer.Algorithms
{
    public static class SpecialChars
    {
        public const char SUB_0 = '₀';
        public const char SUB_1 = '₁';
        public const char SUB_2 = '₂';
        public const char SUB_3 = '₃';
        public const char SUB_4 = '₄';
        public const char SUB_5 = '₅';
        public const char SUB_6 = '₆';
        public const char SUB_7 = '₇';
        public const char SUB_8 = '₈';
        public const char SUB_9 = '₉';

        public const char SUPER_0 = '⁰';
        public const char SUPER_1 = '¹';
        public const char SUPER_2 = '²';
        public const char SUPER_3 = '³';
        public const char SUPER_4 = '⁴';
        public const char SUPER_5 = '⁵';
        public const char SUPER_6 = '⁶';
        public const char SUPER_7 = '⁷';
        public const char SUPER_8 = '⁸';
        public const char SUPER_9 = '⁹';

        public const char SUPER_PLUS = '⁺';
        public const char SUPER_MIN = '⁻';
        public const char SUB_PLUS = '₊';
        public const char SUB_MIN = '₋';

        public const string SUPER = "⁰¹²³⁴⁵⁶⁷⁸⁹⁺⁻";
        public const string SUB = "₀₁₂₃₄₅₆₇₈₉₊₋";

        public static string IntToSub(int k)
        {
            if (k < 0) throw new FormatException("小于0的int值无法转换为下标。");
            StringBuilder p = new StringBuilder(k.ToString());
            for (int i = 0; i < p.Length; i++) { p[i] = SUB[p[i] - '0']; }
            return p.ToString();
        }

        public static char SubToNormal(char sub)
        {
            switch (sub)
            {
                case SUB_0: return '0';
                case SUB_1: return '1';
                case SUB_2: return '2';
                case SUB_3: return '3';
                case SUB_4: return '4';
                case SUB_5: return '5';
                case SUB_6: return '6';
                case SUB_7: return '7';
                case SUB_8: return '8';
                case SUB_9: return '9';
                case SUB_PLUS: return '+';
                case SUB_MIN: return '-';
                default: return sub;
            }
        }
        public static char SuperToNormal(char super)
        {
            switch (super)
            {
                case SUPER_0: return '0';
                case SUPER_1: return '1';
                case SUPER_2: return '2';
                case SUPER_3: return '3';
                case SUPER_4: return '4';
                case SUPER_5: return '5';
                case SUPER_6: return '6';
                case SUPER_7: return '7';
                case SUPER_8: return '8';
                case SUPER_9: return '9';
                case SUPER_PLUS: return '+';
                case SUPER_MIN: return '-';
                default: return super;
            }
        }



        public static string SuperToNormal(string super)
        {
            StringBuilder sb = new StringBuilder(super);
            for (int i = 0; i < sb.Length; i++) 
            {
                sb[i] = SuperToNormal(sb[i]);
            }
            return sb.ToString();
        }
        public static string SuperToSub(string super)
        {
            StringBuilder sb = new StringBuilder(super);
            for (int i = 0; i < sb.Length; i++)
            {
                if (CharUtil.GetCharCase(sb[i]) == CharCase.superscript)
                {
                    sb[i] = SuperToNormal(sb[i]);
                    sb[i] = NormalToSub(sb[i].ToString())[0];
                }
            }
            return sb.ToString();
        }
        public static string SubToSuper(string super)
        {
            StringBuilder sb = new StringBuilder(super);
            for (int i = 0; i < sb.Length; i++)
            {
                if (CharUtil.GetCharCase(sb[i]) == CharCase.subscript)
                {
                    sb[i] = SubToNormal(sb[i]);
                    sb[i] = NormalToSuper(sb[i].ToString())[0];
                }
            }
            return sb.ToString();
        }
        public static string SubToNormal(string super)
        {
            StringBuilder sb = new StringBuilder(super);
            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = SubToNormal(sb[i]);
            }
            return sb.ToString();
        }
        public static string NormalToSuper(string normal)
        {
            StringBuilder sb = new StringBuilder(normal);
            for (int i = 0; i < sb.Length; i++)
            {
                if (CharUtil.GetCharCase(sb[i]) == CharCase.number)
                    sb[i] = SUPER[sb[i] - '0'];
                if (sb[i] == '+') sb[i] = SUPER_PLUS;
                if (sb[i] == '-') sb[i] = SUPER_MIN;
            }
            return sb.ToString();
        }
        public static string NormalToSub(string normal)
        {
            StringBuilder sb = new StringBuilder(normal);
            for (int i = 0; i < sb.Length; i++)
            {
                if (CharUtil.GetCharCase(sb[i]) == CharCase.number)
                    sb[i] = SUB[sb[i] - '0'];
                if (sb[i] == '+') sb[i] = SUB_PLUS;
                if (sb[i] == '-') sb[i] = SUB_MIN;
            }
            return sb.ToString();
        }
       
    }
    public enum CharCase
    {
        lowercase,
        capital,
        number,
        /// <summary>
        /// 下标
        /// </summary>
        subscript,
        /// <summary>
        /// 上标
        /// </summary>
        superscript,
        /// <summary>
        /// "·" "(" ")" "[" "]"
        /// </summary>
        keysymbol,
        other
    }
    public static class CharUtil
    {
        public static CharCase GetCharCase(char c)
        {
            if (c >= 'A' && c <= 'Z') return CharCase.capital;
            if (c >= 'a' && c <= 'z') return CharCase.lowercase;
            if (c >= '0' && c <= '9') return CharCase.number;
            if (SpecialChars.SUPER.Contains(c)) return CharCase.superscript;
            if (SpecialChars.SUB.Contains(c)) return CharCase.subscript;
            if(c == '·' || c == '(' || c == ')' || c == '[' || c == ']') return CharCase.keysymbol;
            return CharCase.other;
        }
    }
}
