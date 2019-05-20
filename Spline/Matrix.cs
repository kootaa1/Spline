using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spline.sources
{
    class Matrix
    {
        public List<double> di, al, au, f;
        public List<int> ia, ja;

        public Matrix(int n)
        {
            di = new List<double>(n);
            f = new List<double>(n);
            al = new List<double>();
            au = new List<double>();
            ia = new List<int>();
            ja = new List<int>();
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
        public int setEl(int i, int j, double El)
        {
            if (i == j)
            {
                di[i] += El;
                return 0;
            }
            if (i > j)
            {
                int i0 = ia[i];
                int i1 = ia[i + 1];
                for (int k = i0; k < i1; k++)
                    if (ja[k] == j)
                    {
                        al[k] += El;
                        return 0;
                    }
                return -1;
            }
            else
            {
                Swap(ref i, ref j);
                int i0 = ia[i];
                int i1 = ia[i + 1];
                for (int k = i0; k < i1; k++)
                    if (ja[k] == j)
                    {
                        au[k] += El;
                        return 0;
                    }
                return -1;
            }
        }

        public void profileDefining(Grid grid)
        {
            int n = 4 * grid.getXSize() * grid.getYSize();
            int k = 0;
            ListOfAdjacency listOfAdjacency = new ListOfAdjacency();
            listOfAdjacency.fillingList(grid);
            ia.Add(0);
            for (int i = 0; i < n; i++)
            {
                di.Add(0);
                for (int j = 0; j < listOfAdjacency.list[i].Count; j++)
                {
                    ja.Add(listOfAdjacency.list[i][j]);
                    al.Add(0);
                    au.Add(0);
                    k++;
                }
                ia.Add(k);
            }
        }
        //void outMatrix();
        public double GetEl(int i, int j)
        {
            if (i == j)
            {
                return di[i];
            }
            if (i > j)
            {
                int i0 = ia[i];
                int i1 = ia[i + 1];
                for (int k = i0; k < i1; k++)
                    if (ja[k] == j)
                    {
                        return al[k];
                    }
                return 0;
            }
            else
            {
                Swap(ref i, ref j);
                int i0 = ia[i];
                int i1 = ia[i + 1];
                for (int k = i0; k < i1; k++)
                    if (ja[k] == j)
                    {
                        return au[k];
                    }
                return 0;
            }
        }
        //void nullMatrix();
    }
}
