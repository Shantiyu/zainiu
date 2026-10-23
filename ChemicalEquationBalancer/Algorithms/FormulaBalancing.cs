using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Schema;

namespace ChemicalEquationBalancer.Algorithms
{
    public class FormulaBalancing
    {
        public ReactionFormula formula;
        public RichTextBox box;
        public int[] result;
        public FormulaBalancing(ReactionFormula formula)
        {
            this.formula = formula;
        }
        frac[,] matrix;
        public BalanceResult Balance()
        {
            ToMatrix();
            MatrixGaussElimination elimination = new MatrixGaussElimination
            {
                matrix = matrix,
                textBox = box,
            };
            var r = elimination.Solve();
            if (r == SolutionType.unique)
            {
                result = ProcessSolution(elimination.solution);
                int i = 0;
                for (; i < formula.left.Count; i++)
                {
                    formula.left[i] = new ReactionItem()
                    {
                        count = result[i],
                        suffix = formula.left[i].suffix,
                        formula = formula.left[i].formula
                    };
                }
                for (; i < formula.left.Count + formula.right.Count; i++)
                {
                    int j = i - formula.left.Count;
                    formula.right[j] = new ReactionItem()
                    {
                        count = result[i],
                        suffix = formula.right[j].suffix,
                        formula = formula.right[j].formula
                    };
                }
                return BalanceResult.unique;
            }
            else if(r == SolutionType.infinite)
            {
                return BalanceResult.infinite;
            }
            else
            {
                return BalanceResult.unsolvable;
            }
        }
        private void ToMatrix()
        {
            #region 统计所有出现的元素
            ElementIngredients total = new ElementIngredients();
            foreach (var r in formula.left)
            {
                total.SelfAddBy(r.formula.GetElementIngredients());
            }
            foreach (var r in formula.right)
            {
                total.SelfAddBy(r.formula.GetElementIngredients());
            }
            #endregion

            #region 根据化学方程式写出方程组矩阵
            int lx = total.elements.Count + 1, ly = formula.left.Count + formula.right.Count + 1;
            matrix = new frac[lx, ly];
            for (int i = 0; i < total.elements.Count; i++)
            {
                for (int j = 0; j < formula.left.Count; j++)
                {
                    matrix[i + 1, j] = new frac(formula.left[j].formula.GetElementIngredients()[total.elements.ElementAt(i).Key], 1);
                }
                for (int j = 0; j < formula.right.Count; j++)
                {
                    matrix[i + 1, formula.left.Count + j] = new frac(-formula.right[j].formula.GetElementIngredients()[total.elements.ElementAt(i).Key], 1);
                }
            }
            for (int j = 0; j < formula.left.Count; j++)
            {
                matrix[0, j] = new frac(formula.left[j].formula.GetElementIngredients().electricCharge, 1);
            }
            for (int j = 0; j < formula.right.Count; j++)
            {
                matrix[0, formula.left.Count + j] = new frac(-formula.right[j].formula.GetElementIngredients().electricCharge, 1);
            }
            for (int i = 0; i < lx; i++)
            {
                matrix[i, ly - 1] = 0;
            }

            if (true)//lx == ly - 2) //元素比物质数少一个：这是最常见的情况，通过加上一条“1a = 1”求解。
            {
                frac[,] newMatrix = new frac[lx + 1, ly];
                for (int i = 1; i <= lx; i++)
                {
                    for (int j = 0; j < ly; j++)
                    {
                        newMatrix[i, j] = matrix[i-1, j];
                    }
                }
                for (int j = 0; j < ly; j++)
                {
                    newMatrix[0, j] = 0;
                }
                newMatrix[0, 0] = 1;
                newMatrix[0, ly - 1] = 1;
                matrix = newMatrix;
            }
            #endregion

            #region 简化矩阵
            int pi = 0;
            frac[,] simp = new frac[lx + 1, ly];

            for (int i = 0; i < lx + 1; i++)
            {
                int[] k = new int[ly];
                for (int j = 0; j < ly; j++)
                {
                    k[j] = matrix[i, j].a;
                }
                int g = Maths.Gcd0(k);
                //如果g是0，说明这一行全是0，不用管了
                if (g != 0)
                {
                    //如果不是0，把这一行的数进行约分
                    for (int j = 0; j < ly; j++) 
                    {
                        matrix[i, j] = matrix[i, j].a / g;
                    }
                    bool hasIn = false;
                    for (int j = 0; j < pi; j++)
                    {
                        bool equal = true;
                        for (int l = 0; l < ly; l++)
                        {
                            if (simp[j,l] != matrix[i, l])
                            {
                                equal = false;
                                break;
                            }
                        }
                        if (equal) 
                        { 
                            hasIn = true;
                            break;
                        }
                    }
                    //如果没有重复，那就加上
                    if (!hasIn)
                    {
                        for (int j = 0; j < ly; j++)
                        {
                            simp[pi, j] = matrix[i, j];
                        }
                        pi++;
                    }
                }
            }
            ;

            frac[,] simp2 = new frac[pi, ly];
            for (int i = 0; i < pi; i++)
            {
                for (int j = 0; j < ly; j++)
                {
                    simp2[i, j] = simp[i, j];
                }
            }
            //simp2[0, 0] = 1;
            matrix = simp2;
            ;
            #endregion

            #region 补充矩阵信息
            lx = matrix.GetLength(0);
            ly = matrix.GetLength(1);
            frac[,] f = matrix;
            ;
            
            ;
            #endregion
        }
        private static int[] ProcessSolution(frac[] result)
        {
            int[] bs = new int[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                bs[i] = result[i].b;
            }
            int k = Maths.Lcm(bs);

            int[] o = new int[result.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] *= k;
                o[i] = result[i].a / result[i].b;
            }
            return o;
        }
    }
    public enum BalanceResult
    {
        unique,
        infinite,
        unsolvable
    }
}
