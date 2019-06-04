using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Spline.sources
{
    class MyTask
    {
        Matrix matrix;
        double[] f;
        public double[] result { get; set; }
        public Grid grid { get; set; }
        Basis basis;
        LOS los;
        bool isReady;
        double alphaComponent, betaComponent;
        public MyTask()
        {
            grid = new Grid();
            basis = new Basis();
            los = new LOS();
        }

        public bool Make(string filePath)
        {
            if (grid.inputSpline(filePath))
            {
                Array.Resize(ref f, 4 * grid.getXSize() * grid.getYSize());
                matrix = new Matrix(4 * grid.getXSize() * grid.getYSize());
                matrix.profileDefining(grid);
                MatrixFilling();
                los = new LOS();
                result = los.makeSLAU(matrix, f);
                return true;
            }
            return false;
        }

        public bool calculateWithNewComponent(double alpha, double beta)
        {
            alphaComponent = alpha;
            betaComponent = beta;
            matrix.nullMatrix();
            for (int i = 0; i < f.Length; i++)
                f[i] = 0;
            MatrixFilling();
            los = new LOS();
            result = los.makeSLAU(matrix, f);
            return true;
        }
        void MatrixFilling()
        {
            int sizeX = grid.getXSize() - 1, sizeY = grid.getYSize() - 1;
            double hx, hy;
            double[,] localMatrix = new double[4,4];
            List<int> indexOfPoints = new List<int>();
            bool[] usedPoints = null;
            int[] indexInMatrix = new int[16];
            Array.Resize(ref usedPoints, grid.f.Count);
            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX; ++i)
                {
                    indexOfPoints.Clear();
                    for (int k = 0; k < grid.points.Count; ++k)
                    {
                        if (!usedPoints[k] &&
                        grid.points[k].x >= grid.x[i] && grid.points[k].x <= grid.x[i + 1] &&
                        grid.points[k].y >= grid.y[j] && grid.points[k].y <= grid.y[j + 1])
                        {
                            indexOfPoints.Add(k);
                            usedPoints[k] = true;
                        }
                    }
                    if (indexOfPoints.Count == 0)
                    {
                        MessageBox.Show("I can't find points on interval [" + grid.x[i].ToString() +
                            ", " + grid.x[i + 1].ToString() + "] * [" + grid.y[j].ToString() + ", " +
                            grid.y[j + 1].ToString() + "]\n you must fix it, before do something. \n");
                        //App.Current.Shutdown();
                        //Console.WriteLine("I can't find points on interval [" + grid.x[i].ToString() +
                        //    ", " + grid.x[i + 1].ToString() + "] * [" + grid.y[j].ToString() + ", " +
                        //    grid.y[j + 1].ToString() + "]\n you must fix it, before do something. \n" +
                        //    "Press any key, to exit.");
                        //Console.ReadKey();
                        //system("exit");
                    }
                    hx = grid.x[i + 1] - grid.x[i];
                    hy = grid.y[j + 1] - grid.y[j];
                    int buf = 4 * grid.calculatePosition(i, j);
                    indexInMatrix[0] = buf;
                    indexInMatrix[1] = buf + 1;
                    indexInMatrix[2] = buf + 2;
                    indexInMatrix[3] = buf + 3;
                    buf = 4 * grid.calculatePosition(i + 1, j);
                    indexInMatrix[4] = buf;
                    indexInMatrix[5] = buf + 1;
                    indexInMatrix[6] = buf + 2;
                    indexInMatrix[7] = buf + 3;
                    buf = 4 * grid.calculatePosition(i, j + 1);
                    indexInMatrix[8] = buf;
                    indexInMatrix[9] = buf + 1;
                    indexInMatrix[10] = buf + 2;
                    indexInMatrix[11] = buf + 3;
                    buf = 4 * grid.calculatePosition(i + 1, j + 1);
                    indexInMatrix[12] = buf;
                    indexInMatrix[13] = buf + 1;
                    indexInMatrix[14] = buf + 2;
                    indexInMatrix[15] = buf + 3;
                    for (int k = 0; k < indexOfPoints.Count; ++k)
                    {
                        for (int ii = 0; ii < 16; ++ii)
                        {
                            for (int jj = 0; jj < 16; ++jj)
                            {
                                matrix.setEl(indexInMatrix[ii], indexInMatrix[jj], grid.omega[k] * basis.Psi(ii, grid.x[i], grid.y[j],
                                    hx, hy, grid.points[indexOfPoints[k]].x, grid.points[indexOfPoints[k]].y) *
                                    basis.Psi(jj, grid.x[i], grid.x[j], hx, hy,
                                        grid.points[indexOfPoints[k]].x, grid.points[indexOfPoints[k]].y)
                                    + basis.secondComp(alphaComponent, ii, jj, hx, hy)
                                    + basis.thirdComp(betaComponent, ii, jj, hx, hy));
                            }
                            f[indexInMatrix[ii]] += grid.omega[k] * basis.Psi(ii, grid.x[i], grid.y[j], hx, hy,
                                grid.points[indexOfPoints[k]].x, grid.points[indexOfPoints[k]].y) * (grid.f[indexOfPoints[k]]);// + grid.error[k]);
                            /*file << basis.Psi(ii, grid.X[i], grid.Y[j], hx, hy,
                                grid.points[indexOfPoints[k]].x, grid.points[indexOfPoints[k]].y)*grid.F[indexOfPoints[k]] << "\t";*/
                        }
                        //file << endl;
                    }
                }
            }
        }
        public double valueInPoint(double x, double y)
        {
            int maxI = grid.getXSize() - 1, maxJ = grid.getYSize() - 1;
            int i = 0, j = 0;
            double summ = 0;
            while (grid.x[i] < x && i < maxI - 1)
                ++i;
            if (i > 0) --i;
            double hx = grid.x[i + 1] - grid.x[i];
            if (y > grid.y[maxJ]) y = grid.y[maxJ];
            while (grid.y[j] < y && j < maxJ - 1)
                ++j;
            if (j > 0) --j;
            double hy = grid.y[j + 1] - grid.y[j];
            int NumOfEl = 4 * grid.calculatePosition(i, j);
            for (int k = 0; k < 16; ++k)
            {
                summ += result[NumOfEl + k] * basis.Psi(k, grid.x[i], grid.y[j], hx, hy, x, y);
            }
            return summ;
        }
    }
}
