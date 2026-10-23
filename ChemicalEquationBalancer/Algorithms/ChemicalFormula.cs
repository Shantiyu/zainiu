using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalEquationBalancer.Algorithms
{
    public struct SingleElement
    {
        public string elementName;
        public int elementCount;
        public string superScript;
        public SingleElement(string elementName, int elementCount, string superScript)
        {
            this.elementName = elementName;
            this.elementCount = elementCount;
            this.superScript = superScript;
        }
    }
    public struct ChildFormula
    {
        public ChemicalFormula formula;
        public int count;
        public ChildFormula(ChemicalFormula formula, int count)
        {
            this.formula = formula;
            this.count = count;
        }
    }
    public struct FollowFormula
    {
        public ChemicalFormula formula;
        public int count;
        public FollowFormula(ChemicalFormula formula, int count)
        {
            this.formula = formula;
            this.count = count;
        }
    }
    public struct SuperScript
    {
        public string text;
        public SuperScript(string t)
        {
            text = t;
        }
    }
    public struct Splitter
    {
        public string text;
        public Splitter(string t)
        {
            text = t;
        }
    }
    public struct ConcatenatedFormula
    {
        public ChemicalFormula[] formulas;
        public int[] counts;
        public ConcatenatedFormula(ChemicalFormula[] formulas, int[] counts)
        {
            this.formulas = formulas;
            this.counts = counts;
        }
    }
    public struct ChemicalFormula
    {
        public List<object> elements;
        public int electricCharge;
        public ChemicalFormula(List<object> elements)
        {
            this.elements = elements;
            this.electricCharge = 0;
        }
        public static ChemicalFormula Parse(string s)
        {
            s = s.Replace('[', '(');
            s = s.Replace(']', ')');
            s = s.Replace(" ", "");
            List<object> element = new List<object>();


            List<string> splits = new List<string>();
            int last = 0;
            int deep = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(')
                {
                    deep++;
                }
                if (s[i] == ')')
                {
                    deep--;
                }
                if (s[i] == '·' && deep == 0)
                {
                    splits.Add(s.Substring(last, i - last));
                    last = i + 1;
                }
            }
            splits.Add(s.Substring(last, s.Length - last));
            ;
            if (splits.Count > 1)
            {
                ConcatenatedFormula cf = new ConcatenatedFormula();
                cf.formulas = new ChemicalFormula[splits.Count];
                cf.counts = new int[splits.Count];
                int k = 0;
                foreach (var r in splits)
                {
                    CharCase c = CharUtil.GetCharCase(r[0]);
                    if (c == CharCase.number)
                    {
                        int i = 0;
                        int count = 1;
                        for (; i < r.Length; i++)
                        {
                            if (CharUtil.GetCharCase(r[i]) != CharCase.number) break;
                        }
                        count = int.Parse(r.Substring(0, i));
                        cf.formulas[k] = Parse(r.Substring(i));
                        cf.counts[k] = count;
                    }
                    else
                    {
                        cf.formulas[k] = Parse(r);
                        cf.counts[k] = 1;
                    }
                    k++;
                }
                element.Add(cf);

                return new ChemicalFormula(element);
            }
            else 
            {
                ChildFormula bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                StringBuilder buffer = new StringBuilder("");
                bool isChild = false;
                int bufferCount = -1;
                StringBuilder bufferSuper = new StringBuilder("");
                for (int i = 0; i < s.Length; i++)
                {
                    CharCase c = CharUtil.GetCharCase(s[i]);
                    if (c == CharCase.capital)
                    {
                        if (isChild)
                        {
                            element.Add(new ChildFormula(bufferChild.formula, bufferCount == -1 ? 1 : bufferCount));
                            bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                            buffer.Clear();
                            bufferSuper.Clear();
                            bufferCount = -1;
                            isChild = false;
                        }
                        else if (buffer.Length > 0) //如果缓存中放置了上一个元素
                        {
                            element.Add(new SingleElement(buffer.ToString(), bufferCount == -1 ? 1 : bufferCount, bufferSuper.ToString()));
                            bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                            buffer.Clear();
                            bufferSuper.Clear();
                            bufferCount = -1;
                            isChild = false;
                        }
                        buffer.Append(s[i]);
                    }
                    else if (c == CharCase.lowercase)
                    {
                        if (buffer.Length > 0)
                        {
                            buffer.Append(s[i]);
                        }
                        else
                        {
                            throw new FormatException(string.Format("输入的字符串不是正确的化学式：化学元素必须以大写开头。", i, s[i]));
                        }
                    }
                    else if (c == CharCase.number)
                    {
                        throw new FormatException(string.Format("输入的字符串不是正确的化学式：存在非法字符。你是想输入上下标吗？", i, s[i]));
                    }
                    else if (c == CharCase.subscript)
                    {
                        for (; i < s.Length; i++)
                        {
                            if (CharUtil.GetCharCase(s[i]) != CharCase.subscript) break;
                            if (bufferCount == -1) bufferCount = SpecialChars.SubToNormal(s[i]) - '0';
                            else bufferCount = bufferCount * 10 + (SpecialChars.SubToNormal(s[i]) - '0');
                        }
                        i--;
                        if (isChild)
                        {
                            element.Add(new ChildFormula(bufferChild.formula, bufferCount == -1 ? 1 : bufferCount));
                            bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                            buffer.Clear();
                            bufferSuper.Clear();
                            bufferCount = -1;
                            isChild = false;
                        }
                        else if (buffer.Length > 0) //如果缓存中放置了上一个元素
                        {
                            element.Add(new SingleElement(buffer.ToString(), bufferCount == -1 ? 1 : bufferCount, bufferSuper.ToString()));
                            bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                            buffer.Clear();
                            bufferSuper.Clear();
                            bufferCount = -1;
                            isChild = false;
                        }
                    }
                    else if (c == CharCase.superscript)
                    {
                        StringBuilder sb = new StringBuilder("");
                        for (; i < s.Length; i++)
                        {
                            if (CharUtil.GetCharCase(s[i]) != CharCase.superscript) break;
                            sb.Append(s[i]);
                        }
                        i--;
                        if (i != s.Length - 1) throw new FormatException("输入的字符串不是正确的化学式：离子正负电荷的上标只能出现在化学式结尾。");
                        if (isChild)
                        {
                            element.Add(new ChildFormula(bufferChild.formula, bufferCount == -1 ? 1 : bufferCount));
                            bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                            buffer.Clear();
                            bufferSuper.Clear();
                            bufferCount = -1;
                            isChild = false;
                        }
                        else if (buffer.Length > 0) //如果缓存中放置了上一个元素
                        {
                            element.Add(new SingleElement(buffer.ToString(), bufferCount == -1 ? 1 : bufferCount, bufferSuper.ToString()));
                            bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                            buffer.Clear();
                            bufferSuper.Clear();
                            bufferCount = -1;
                            isChild = false;
                        }
                        element.Add(new SuperScript(sb.ToString()));
                        //bufferSuper.Append(s[i]);
                    }
                    else if (c == CharCase.keysymbol)
                    {
                        if (s[i] == '(')
                        {
                            if (isChild)
                            {
                                element.Add(new ChildFormula(bufferChild.formula, bufferCount == -1 ? 1 : bufferCount));
                                bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                                buffer.Clear();
                                bufferSuper.Clear();
                                bufferCount = -1;
                                isChild = false;
                            }
                            else if (buffer.Length > 0) //如果缓存中放置了上一个元素
                            {
                                element.Add(new SingleElement(buffer.ToString(), bufferCount == -1 ? 1 : bufferCount, bufferSuper.ToString()));
                                bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                                buffer.Clear();
                                bufferSuper.Clear();
                                bufferCount = -1;
                                isChild = false;
                            }

                            int begin = i + 1;
                            int p = 1;
                            for (i = i + 1; i < s.Length; i++)
                            {
                                if (s[i] == '(') p++;
                                if (s[i] == ')') p--;
                                if (p == 0) break;
                                if (i >= s.Length - 1) throw new FormatException(string.Format("输入的字符串不是正确的化学式：左括号没有与之匹配的右括号。", i));
                            }
                            int length = i - begin;
                            string child = s.Substring(begin, length);
                            bufferChild.count = 0;
                            bufferChild.formula = Parse(child);
                            isChild = true;
                        }
                        else throw new FormatException(string.Format("输入的字符串不是正确的化学式：存在非法字符。", i, s[i]));
                    }
                    else if (c == CharCase.other)
                    {
                        throw new FormatException(string.Format("输入的字符串不是正确的化学式：存在非法字符。", i, s[i]));
                    }
                }
                if (isChild)
                {
                    element.Add(new ChildFormula(bufferChild.formula, bufferCount == -1 ? 1 : bufferCount));
                    bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                    buffer.Clear();
                    bufferSuper.Clear();
                    bufferCount = -1;
                    isChild = false;
                }
                else if (buffer.Length > 0) //如果缓存中放置了上一个元素
                {
                    element.Add(new SingleElement(buffer.ToString(), bufferCount == -1 ? 1 : bufferCount, bufferSuper.ToString()));
                    bufferChild = new ChildFormula(new ChemicalFormula(), -1);
                    buffer.Clear();
                    bufferSuper.Clear();
                    bufferCount = -1;
                    isChild = false;
                }

                ChemicalFormula cf = new ChemicalFormula();
                if (element[element.Count - 1].GetType() == typeof(SuperScript))
                {
                    SuperScript ss = (SuperScript)element[element.Count - 1];
                    if (ss.text == SpecialChars.SUPER_PLUS.ToString())
                    {
                        cf.electricCharge = 1;   
                    }
                    else if (ss.text == SpecialChars.SUPER_MIN.ToString())
                    {
                        cf.electricCharge = -1;
                    }
                    else
                    {
                        try
                        {
                            string t = ss.text.Substring(0, ss.text.Length - 1);
                            int p = int.Parse(SpecialChars.SuperToNormal(t));
                            if (ss.text[ss.text.Length - 1] == SpecialChars.SUPER_PLUS)
                            {
                                cf.electricCharge = p;
                            }
                            else if (ss.text[ss.text.Length - 1] == SpecialChars.SUPER_MIN)
                            {
                                cf.electricCharge = -p;
                            }
                            else
                            {
                                throw new FormatException(string.Format("输入的字符串不是正确的化学式：上标格式不正确。"));
                            }
                        }
                        catch
                        {
                            throw new FormatException(string.Format("输入的字符串不是正确的化学式：上标格式不正确。"));
                        }
                    }
                    element.RemoveAt(element.Count - 1);
                }
                else
                {
                    cf.electricCharge = 0;
                }
                cf.elements = element;
                return cf;
            }
        }
        public override string ToString()
        {
            StringBuilder output = new StringBuilder("");
            foreach (var r in elements) 
            {
                if (r.GetType() == typeof(SingleElement))
                {
                    SingleElement s = (SingleElement)r;
                    output.Append(s.elementName);
                    if (s.elementCount > 1)
                        output.Append(SpecialChars.IntToSub(s.elementCount));
                }
                else if (r.GetType() == typeof(ChildFormula))
                {
                    ChildFormula s = (ChildFormula)r;
                    output.Append('(');
                    output.Append(s.formula.ToString());
                    output.Append(')');
                    if (s.count > 1)
                        output.Append(SpecialChars.IntToSub(s.count));
                }
                else if (r.GetType() == typeof(SuperScript))
                {
                    SuperScript s = (SuperScript)r;
                    output.Append(s.text);
                }
                else if (r.GetType() == typeof(Splitter))
                {
                    Splitter s = (Splitter)r;
                    output.Append(s.text);
                }
                else if (r.GetType() == typeof(ConcatenatedFormula))
                {
                    ConcatenatedFormula s = (ConcatenatedFormula)r;
                    StringBuilder sb = new StringBuilder("");
                    bool first = true;
                    for (int i = 0; i < s.formulas.Length; i++) 
                    {
                        if (!first) sb.Append("·");
                        sb.Append((s.counts[i] == 1 ? "" : s.counts[i].ToString()) + s.formulas[i].ToString());
                        first = false;
                    }
                    output.Append(sb.ToString());
                }
            }

            int deep = 0, maxd = 0;
            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] == '(') deep++;
                if (output[i] == ')') deep--;
                maxd = Math.Max(deep, maxd);
            }
            if (maxd > 1)
            {
                for (int i = 0; i < output.Length; i++)
                {
                    if (output[i] == '(')
                    { 
                        deep++;
                        if (deep == 1) output[i] = '[';
                    }
                    if (output[i] == ')') 
                    {
                        if (deep == 1) output[i] = ']';
                        deep--; 
                    }
                    maxd = Math.Max(deep, maxd);
                }
            }

            if (electricCharge != 0) 
            {
                if (Math.Abs(electricCharge) != 1)
                    output.Append(SpecialChars.NormalToSuper(Math.Abs(electricCharge).ToString()));
                output.Append(electricCharge > 0 ? SpecialChars.SUPER_PLUS : SpecialChars.SUPER_MIN);
            }

            return output.ToString();
        }

        public string ToStringB()
        {
            StringBuilder output = new StringBuilder("");
            foreach (var r in elements)
            {
                if (r.GetType() == typeof(SingleElement))
                {
                    SingleElement s = (SingleElement)r;
                    output.Append(s.elementName);
                    if (s.elementCount > 1)
                        output.Append(s.elementCount);
                }
                else if (r.GetType() == typeof(ChildFormula))
                {
                    ChildFormula s = (ChildFormula)r;
                    output.Append('(');
                    output.Append(s.formula.ToString());
                    output.Append(')');
                    if (s.count > 1)
                        output.Append(s.count);
                }
                else if (r.GetType() == typeof(SuperScript))
                {
                    SuperScript s = (SuperScript)r;
                    output.Append(s.text);
                }
                else if (r.GetType() == typeof(Splitter))
                {
                    Splitter s = (Splitter)r;
                    output.Append(s.text);
                }
                else if (r.GetType() == typeof(ConcatenatedFormula))
                {
                    ConcatenatedFormula s = (ConcatenatedFormula)r;
                    StringBuilder sb = new StringBuilder("");
                    bool first = true;
                    for (int i = 0; i < s.formulas.Length; i++)
                    {
                        if (!first) sb.Append("·");
                        sb.Append((s.counts[i] == 1 ? "" : s.counts[i].ToString()) + s.formulas[i].ToString());
                        first = false;
                    }
                    output.Append(sb.ToString());
                }
            }

            int deep = 0, maxd = 0;
            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] == '(') deep++;
                if (output[i] == ')') deep--;
                maxd = Math.Max(deep, maxd);
            }
            if (maxd > 1)
            {
                for (int i = 0; i < output.Length; i++)
                {
                    if (output[i] == '(')
                    {
                        deep++;
                        if (deep == 1) output[i] = '[';
                    }
                    if (output[i] == ')')
                    {
                        if (deep == 1) output[i] = ']';
                        deep--;
                    }
                    maxd = Math.Max(deep, maxd);
                }
            }

            if (electricCharge != 0)
            {
                if (Math.Abs(electricCharge) != 1)
                {
                    output.Append("{");
                    output.Append(Math.Abs(electricCharge).ToString());
                }
                output.Append(electricCharge > 0 ? "+" : "-");
                output.Append("}");
            }

            return output.ToString();
        }

        public string ToStringC()
        {
            StringBuilder output = new StringBuilder("");
            foreach (var r in elements)
            {
                if (r.GetType() == typeof(SingleElement))
                {
                    SingleElement s = (SingleElement)r;
                    output.Append(s.elementName);
                    if (s.elementCount > 1)
                    {
                        output.Append("_");
                        output.Append(s.elementCount);
                        output.Append(" ");
                    }
                }
                else if (r.GetType() == typeof(ChildFormula))
                {
                    ChildFormula s = (ChildFormula)r;
                    output.Append('(');
                    output.Append(s.formula.ToStringC());
                    output.Append(')');
                    if (s.count > 1) 
                    {
                        output.Append("_");
                        output.Append(s.count);
                        output.Append(" ");
                    }
                }
                else if (r.GetType() == typeof(SuperScript))
                {
                    SuperScript s = (SuperScript)r;
                    output.Append("^");
                    output.Append(s.text);
                    output.Append(" ");
                }
                else if (r.GetType() == typeof(Splitter))
                {
                    Splitter s = (Splitter)r;
                    output.Append(s.text);
                }
                else if (r.GetType() == typeof(ConcatenatedFormula))
                {
                    ConcatenatedFormula s = (ConcatenatedFormula)r;
                    StringBuilder sb = new StringBuilder("");
                    bool first = true;
                    for (int i = 0; i < s.formulas.Length; i++)
                    {
                        if (!first) sb.Append("·");
                        sb.Append((s.counts[i] == 1 ? "" : s.counts[i].ToString()) + s.formulas[i].ToStringC());
                        first = false;
                    }
                    output.Append(sb.ToString());
                }
            }

            int deep = 0, maxd = 0;
            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] == '(') deep++;
                if (output[i] == ')') deep--;
                maxd = Math.Max(deep, maxd);
            }
            if (maxd > 1)
            {
                for (int i = 0; i < output.Length; i++)
                {
                    if (output[i] == '(')
                    {
                        deep++;
                        if (deep == 1) output[i] = '[';
                    }
                    if (output[i] == ')')
                    {
                        if (deep == 1) output[i] = ']';
                        deep--;
                    }
                    maxd = Math.Max(deep, maxd);
                }
            }

            if (electricCharge != 0)
            {
                if (Math.Abs(electricCharge) != 1)
                {
                    output.Append("{");
                    output.Append(Math.Abs(electricCharge).ToString());
                }
                output.Append(electricCharge > 0 ? "+" : "-");
                output.Append("}");
            }

            return output.ToString();
        }

        public ElementIngredients GetElementIngredients()
        {
            ElementIngredients e = new ElementIngredients();
            e.electricCharge = electricCharge;
            foreach (var r in elements)
            {
                if (r.GetType() == typeof(SingleElement))
                {
                    SingleElement s = (SingleElement)r;
                    e.SelfAddIngredient(s.elementName, s.elementCount);
                }
                else if (r.GetType() == typeof(ChildFormula))
                {
                    ChildFormula s = (ChildFormula)r;
                    e.SelfAddBy(s.formula.GetElementIngredients().MultiplyBy(s.count));
                }
                //else if (r.GetType() == typeof(SuperScript))
                //{
                    //SuperScript s = (SuperScript)r;
                //}
                //else if (r.GetType() == typeof(Splitter))
                //{
                    //Splitter s = (Splitter)r;
                //}
                else if (r.GetType() == typeof(ConcatenatedFormula))
                {
                    ConcatenatedFormula s = (ConcatenatedFormula)r;
                    for (int i = 0; i < s.formulas.Length; i++)
                    {
                        e.SelfAddBy(s.formulas[i].GetElementIngredients().MultiplyBy(s.counts[i]));
                    }
                }
            }
            return e;
        }
    }
}
