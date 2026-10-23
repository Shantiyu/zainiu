using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChemicalEquationBalancer.Algorithms
{
    public class MatrixGaussElimination
    {
        public frac[,] matrix;
        public frac[] solution;
        public frac[,] extracheck;
        public frac[,] totalmatrix;
        public RichTextBox textBox;
        int rows;
        int lys;
        public SolutionType Solve(int maxtrycount = 0)
        {
            rows = matrix.GetLength(0);
            lys = matrix.GetLength(1);
            totalmatrix = (frac[,])matrix.Clone();
            if (rows < lys - 1)
            {
                if (maxtrycount == 0) return SolutionType.infinite;
                else
                {
                    int[] p;
                    if (DFS(maxtrycount, out p))
                    {
                        solution = new frac[p.Length];
                        for (int i = 0; i < p.Length; i++)
                        {
                            solution[i] = p[i];
                        }
                        return SolutionType.infiniteFound;
                    }
                    else return SolutionType.infiniteUnfound;
                }
            }
            else if (rows > lys - 1)
            {
                frac[,] newmatrix = new frac[lys - 1, lys];
                int i = 0;
                for (; i < lys - 1; i++)
                {
                    for (int j = 0; j < lys; j++)
                    {
                        newmatrix[i, j] = matrix[i, j];
                    }
                }
                extracheck = new frac[rows - lys + 1, lys];
                for (; i < rows; i++)
                {
                    for (int j = 0; j < lys; j++)
                    {
                        extracheck[i - lys + 1, j] = matrix[i, j];
                    }
                }
                matrix = newmatrix;

                var r = _Solve();
                if (r == SolutionType.unique)
                {
                    for (i = 0; i < extracheck.GetLength(0); i++)
                    {
                        frac right = 0;
                        for (int j = 0; j < extracheck.GetLength(1) - 1; j++)
                        {
                            right += solution[j] * extracheck[i, j];
                        }
                        if (right != extracheck[i, extracheck.GetLength(1) - 1]) return SolutionType.unsolvable;
                    }
                    if (CheckValid())
                        return SolutionType.unique;
                    else return SolutionType.unsolvable;
                }
                else if (r == SolutionType.unsolvable) return SolutionType.unsolvable;
                else if (r == SolutionType.infinite)
                {
                    //通过全选择（C）的方法，将totalmatrix中的lys-1个放入matrix中，直到算出唯一解。若最终算不出来，就是有无穷解。
                    int[] s = new int[totalmatrix.GetLength(0)];
                    for (int j = 0; j < totalmatrix.GetLength(0); j++) 
                    {
                        s[j] = j;
                    }
                    var p = C(s, lys - 1);
                    return p;
                }
                else
                {
                    throw new Exception("。。搞笑呢？");
                }
            }
            else
            {
                var r = _Solve();
                if (r == SolutionType.infinite && maxtrycount != 0)
                {
                    int[] p;
                    if (DFS(maxtrycount, out p))
                    {
                        solution = new frac[p.Length];
                        for (int i = 0; i < p.Length; i++)
                        {
                            solution[i] = p[i];
                        }
                        return SolutionType.infiniteFound;
                    }
                    else return SolutionType.infiniteUnfound;
                }
                else
                {
                    if (CheckValid())
                    {
                        return SolutionType.unique;
                    }
                    else
                    {
                        return SolutionType.unsolvable;
                    }
                }
            }
        }

        public bool CheckValid()
        {
            //return true;
            foreach(var r in solution)
            {
                if (r <= 0) return false;
            }
            return true;
        }

        private SolutionType C(int[] lsArray, int selectCount)
        {
            int totolcount = lsArray.Length;
            int[] currectselect = new int[selectCount];
            int last = selectCount - 1;
            for (int i = 0; i < selectCount; i++)
            {
                currectselect[i] = i;
            }
            while (true)
            {
                frac[,] newmatrix = new frac[selectCount, lys];
                for (int i = 0; i < selectCount; i++)
                {
                    for (int j = 0; j < lys; j++)
                    {
                        newmatrix[i, j] = totalmatrix[currectselect[i], j];
                    }
                }
                matrix = newmatrix;

                /*textBox.Text += "Try : ";
                for (int i = 0; i < selectCount; i++)
                {
                    textBox.Text += lsArray[currectselect[i]].ToString();
                }*/
                var r = _Solve();
                if (r == SolutionType.unique)
                {
                    if (CheckValid())
                        return SolutionType.unique;
                    else
                        return SolutionType.unsolvable;
                }
                else if (r == SolutionType.unsolvable)
                {
                    return SolutionType.unsolvable;
                }

                //textBox.Text += "\n";


                if (currectselect[last] < totolcount - 1)
                {
                    currectselect[last]++;
                }
                else
                {
                    int pos = last;
                    while (pos > 0 && currectselect[pos - 1] == currectselect[pos] - 1)
                    {
                        pos--;
                    }
                    if (pos == 0) return SolutionType.infinite;
                    currectselect[pos - 1]++;
                    for (int i = pos; i < selectCount; i++)
                    {
                        currectselect[i] = currectselect[i - 1] + 1;
                    }
                }
            }
        }

        private bool CheckSolution(ref int[] solution)
        {
            for (int i = 0; i < totalmatrix.GetLength(0); i++)
            {
                frac right = 0;
                for (int j = 0; j < totalmatrix.GetLength(1) - 1; j++)
                {
                    right += solution[j] * totalmatrix[i, j];
                }
                if (right != totalmatrix[i, totalmatrix.GetLength(1) - 1]) return false;
            }
            return true;
        }
        private bool CheckSolution(ref frac[] solution)
        {
            for (int i = 0; i < totalmatrix.GetLength(0); i++)
            {
                frac right = 0;
                for (int j = 0; j < totalmatrix.GetLength(1) - 1; j++)
                {
                    right += solution[j] * totalmatrix[i, j];
                }
                if (right != totalmatrix[i, totalmatrix.GetLength(1) - 1]) return false;
            }
            return true;
        }

        private bool DFS(int max, out int[] s)
        {
            int[] solution = new int[lys - 1];
            for (int i = 0; i < solution.Length; i++)
            {
                solution[i] = 1;
            }
            int g = solution.Length - 1;
            while (true)
            {
                while (solution[g] == max)
                {
                    solution[g] = 1;
                    g--;
                    if (g < 0) break;
                    solution[g]++;
                }
                if (g < 0) break;
                g = solution.Length - 1;
                solution[g]++;
                if (CheckSolution(ref solution))
                {
                    s = solution;
                    return true;
                }
            }
            s = null;
            return false;
        }

        private SolutionType _Solve()
        {
            rows = matrix.GetLength(0);
            lys = matrix.GetLength(1);

            if (rows < lys - 1)
            {
                return SolutionType.infinite;
            }
            for (int i = 0; i < rows; i++)
            {
                //第一步：确认当前行的主元列系数不为0
                if (matrix[i, i] == 0)
                {
                    //如果i行主元系数为0，则从下面的行里找一行系数不为0的与之交换
                    for (int j = i + 1; j < rows; j++)
                    {
                        if (matrix[j, i] != 0)
                        {
                            SwapRow(i, j);
                            break;
                        }
                    }
                }

                //第二步：将当前行主元列系数化为1
                if (matrix[i, i] != 0)
                {
                    DivideRowBy(i, matrix[i, i]);
                }

                //第三步：将其他行利用法则三消去本行主元
                for (int j = 0; j < rows; j++)
                {
                    if (i == j) continue;
                    frac t = matrix[j, i] / matrix[i, i];
                    SubtractARowByAnotherRow(j, t, i);
                }
            }

            solution = new frac[rows];
            for (int i = 0; i < rows; i++)
            {
                if (matrix[i, i] == 0)
                {
                    if (matrix[i, rows] == 0) return SolutionType.infinite;
                    else return SolutionType.unsolvable;
                }
                solution[i] = matrix[i, rows] / matrix[i, i];
            }

            return SolutionType.unique;
        }

        //法则一：交换两行
        private void SwapRow(int rowIndex1, int rowIndex2)
        {
            for (int i = 0; i < lys; i++)
            {
                frac f = matrix[rowIndex1, i];
                matrix[rowIndex1, i] = matrix[rowIndex2, i];
                matrix[rowIndex2, i] = f;
            }
        }

        //法则二：把某行乘以一个非零的数
        private void MultiplyRowBy(int rowIndex, frac mulBy)
        {
            if (mulBy == 0) throw new Exception("不能同时乘0");
            for (int i = 0; i < lys; i++)
            {
                matrix[rowIndex, i] *= mulBy;
            }
        }
        private void DivideRowBy(int rowIndex, frac divBy)
        {
            if (divBy == 0) throw new Exception("不能同时除以0");
            for (int i = 0; i < lys; i++)
            {
                matrix[rowIndex, i] /= divBy;
            }
        }

        //法则三：把一行的若干倍加到另一行上去
        private frac[] GetRowMultiplication(int rowIndex, frac mulBy)
        {
            frac[] o = new frac[lys];
            for (int i = 0; i < lys; i++)
            {
                o[i] = matrix[rowIndex, i] * mulBy;
            }
            return o;
        }
        private void RowAddBy(int rowIndex, frac[] addBy)
        {
            for (int i = 0; i < lys; i++)
            {
                matrix[rowIndex, i] += addBy[i];
            }
        }
        private void RowSubtractBy(int rowIndex, frac[] addBy)
        {
            for (int i = 0; i < lys; i++)
            {
                matrix[rowIndex, i] -= addBy[i];
            }
        }
        private void AddARowByAnotherRow(int rowFrom, frac mulBy, int rowTo)
        {
            RowAddBy(rowTo, GetRowMultiplication(rowFrom, mulBy));
        }
        private void SubtractARowByAnotherRow(int row, frac mulBy, int rowSubtractby)
        {
            RowSubtractBy(row, GetRowMultiplication(rowSubtractby, mulBy));
        }
    }
    public enum SolutionType
    {
        /// <summary>
        /// 方程组有唯一解
        /// </summary>
        unique,
        /// <summary>
        /// 方程组有无穷多组解
        /// </summary>
        infinite,
        /// <summary>
        /// 方程组无解
        /// </summary>
        unsolvable,
        /// <summary>
        /// 方程组存在无穷多组解，但无法暴力搜到
        /// </summary>
        infiniteUnfound,
        /// <summary>
        /// 方程组存在无穷多组解，但暴力搜到了
        /// </summary>
        infiniteFound
    }
}
