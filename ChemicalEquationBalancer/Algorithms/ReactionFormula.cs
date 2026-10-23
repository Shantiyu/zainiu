using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalEquationBalancer.Algorithms
{
    public struct ReactionItem
    {
        public ChemicalFormula formula;
        public int count;
        /// <summary>
        /// 后缀。例如“↓”（沉淀），“↑”（气体）
        /// </summary>
        public string suffix;
        public ElementIngredients GetElementIngredients()
        {
            return formula.GetElementIngredients().MultiplyBy(count);
        }
        public static ReactionItem Parse(string formulaString)
        {
            ReactionItem cf = new ReactionItem();
            int i = 0;
            for (; i < formulaString.Length; i++)
            {
                if (CharUtil.GetCharCase(formulaString[i]) != CharCase.number) break;
            }
            if (i == 0) cf.count = 1;
            else cf.count = int.Parse(formulaString.Substring(0, i));
            int b = i;

            i = formulaString.Length - 1;
            for (; i > 0; i--)
            {
                if (CharUtil.GetCharCase(formulaString[i]) != CharCase.other) break;
            }
            if (i != formulaString.Length - 1)
                cf.suffix = formulaString.Substring(i + 1);
            else
                cf.suffix = string.Empty;
            int e = i + 1;

            cf.formula = ChemicalFormula.Parse(formulaString.Substring(b, e - b));
            return cf;
        }
        public override string ToString()
        {
            if (count == 1) 
                return formula.ToString() + suffix;
            return count.ToString() + formula.ToString() + suffix;
        }
        public string ToStringB()
        {
            if (count == 1)
                return formula.ToString() + suffix;
            return count.ToString() + formula.ToStringB() + suffix;
        }
        public string ToStringC()
        {
            if (count == 1)
                return formula.ToStringC() + suffix;
            return count.ToString() + formula.ToStringC() + suffix;
        }
    }
    public class ReactionFormula
    {
        public List<ReactionItem> left;
        public List<ReactionItem> right;
        public string condition;
        public bool CheckBalanced()
        {
            int leftCharge = 0, rightCharge = 0;
            ElementIngredients ingredientsLeft = new ElementIngredients();
            ElementIngredients ingredientsRight = new ElementIngredients();
            foreach (var item in left) 
            {
                leftCharge += item.count * item.formula.electricCharge;
                ingredientsLeft.SelfAddBy(item.GetElementIngredients());
            }
            foreach (var item in right)
            {
                rightCharge += item.count * item.formula.electricCharge;
                ingredientsRight.SelfAddBy(item.GetElementIngredients());
            }
            if (leftCharge == rightCharge && ingredientsLeft.IsEqual(ingredientsRight)) return true;
            else return false;
        }
        public static ReactionFormula Parse(string formula)
        {
            formula = formula.Replace(" ", "");

            //string[] p = formula.Split('=');
            //if (p.Length != 2) throw new FormatException("输入的方程式不是正确的方程式：存在两个及以上等号。");

            int a = formula.IndexOf('=');
            int b = formula.LastIndexOf('=');
            if(a == -1 || b == -1) throw new FormatException("输入的方程式不是正确的方程式。");

            ReactionFormula o = new ReactionFormula();

            try
            {

                string[] left = formula.Substring(0, a).Split('+');
                string[] right = formula.Substring(b + 1).Split('+');
                if (a == b) o.condition = "";
                else o.condition = formula.Substring(a + 1, b - a - 1);
                o.left = new List<ReactionItem>();
                o.right = new List<ReactionItem>();
                foreach (var r in left)
                    o.left.Add(ReactionItem.Parse(r));
                foreach (var r in right)
                    o.right.Add(ReactionItem.Parse(r));
                return o;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new FormatException("输入的方程式不是正确的方程式。");
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            bool f = false;
            for (int i = 0; i < left.Count; i++) 
            {
                if (f)
                {
                    sb.Append('+');
                }
                f = true;
                sb.Append(left[i].ToString());
            }
            sb.Append("=");
            if (!string.IsNullOrEmpty(condition)) 
            { 
                sb.Append(condition);
                sb.Append("=");
            }
            f = false;
            for (int i = 0; i < right.Count; i++)
            {
                if (f)
                {
                    sb.Append('+');
                }
                f = true;
                sb.Append(right[i].ToString());
            }
            return sb.ToString();
        }

        public string ToStringB()
        {
            StringBuilder sb = new StringBuilder("");
            bool f = false;
            for (int i = 0; i < left.Count; i++)
            {
                if (f)
                {
                    sb.Append('+');
                }
                f = true;
                sb.Append(left[i].ToString());
            }
            sb.Append("=");
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(condition);
                sb.Append("=");
            }
            f = false;
            for (int i = 0; i < right.Count; i++)
            {
                if (f)
                {
                    sb.Append('+');
                }
                f = true;
                sb.Append(right[i].ToString());
            }
            return sb.ToString();
        }
        public string ToStringC()
        {
            StringBuilder sb = new StringBuilder("");
            bool f = false;
            for (int i = 0; i < left.Count; i++)
            {
                if (f)
                {
                    sb.Append('+');
                }
                f = true;
                sb.Append(left[i].ToStringC());
            }
            sb.Append("{EQ \\* jc0 \\* \"Font:Times New Roman\" \\* hps14 \\o(\\s\\up 6(" + condition + "),=)}");
            f = false;
            for (int i = 0; i < right.Count; i++)
            {
                if (f)
                {
                    sb.Append('+');
                }
                f = true;
                sb.Append(right[i].ToStringC());
            }
            return sb.ToString();
        }
    }
}
