using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spline.sources
{
    class LOS
    {
        double[] ggl, ggu, di, vec, x0;
        int[] ig, jg;
        double[] alpha_k, beta_k, r_k, z_k, x_k, q_k, p_k, Ax, Az;
        int N, MaxNumIter = 1000000;
        double eps = 1e-20, norVec;

        void mult(ref double[] v, ref double[] az)
        {
            Array.Resize(ref v, N);
            Array.Resize(ref az, N);
            for (int i = 0; i < N; i++)
            {
                az[i] = v[i] * di[i];
                for (int j = ig[i]; j < ig[i + 1]; j++)
                {
                    az[i] += v[jg[j]] * ggl[j];
                    az[jg[j]] += v[i] * ggu[j];
                }
            }
        }
        double skalar(ref double[] v1, ref double[] v2)
        {
            double s = 0.0;
            for (int i = 0; i < N; i++)
                s += v1[i] * v2[i];
            return s;
        }

        double norma(ref double[] vect)
        {
            double s = 0.0;
            for (int i = 0; i < N; i++)
                s += vect[i] * vect[i];
            return Math.Sqrt(s);
        }
        double nevyazka(ref double[] vect)
        {
            return norma(ref vect) / norVec;
        }
        void nullVector(ref int[] a)
        {
            for (int i = 0; i < a.Length; ++i)
                a[i] = 0;
        }
        void nullVector(ref double[] a)
        {
            for (int i = 0; i < a.Length; ++i)
                a[i] = 0;
        }
        void Run()
        {
            double[] Vec = null;
            Array.Resize(ref Vec, N);
            int nev = 1, k;
            double scal_r_k, scal_new_r_k, scal_p_k, scal_rp_k, scal_Ap_k, Alpha, Beta;
            mult(ref x0, ref Az);
            for (int i = 0; i < N; i++)
            {
                r_k[i] = vec[i] - Az[i];
                z_k[i] = r_k[i];
            }
            scal_r_k = skalar(ref r_k, ref r_k);
            if (scal_r_k < eps)
                nev = 0;
            mult(ref z_k, ref p_k);//p_K
            for (int i = 0; i < N; i++)
                x_k[i] = x0[i];
            for (k = 1; k < MaxNumIter && nev != 0; k++)
            {
                scal_new_r_k = scal_r_k;
                scal_p_k = skalar(ref p_k, ref p_k);
                scal_rp_k = skalar(ref p_k, ref r_k);
                Alpha = scal_rp_k / scal_p_k;//альфа
                for (int i = 0; i < N; i++)//xk и rk
                {
                    x_k[i] += Alpha * z_k[i];
                    r_k[i] -= Alpha * p_k[i];
                }
                mult(ref r_k, ref Az);
                scal_Ap_k = skalar(ref p_k,ref  Az);
                scal_p_k = skalar(ref p_k, ref p_k);
                Beta = -scal_Ap_k / scal_p_k;//бета
                for (int i = 0; i < N; i++)//z_k, p_k
                {
                    z_k[i] = r_k[i] + Beta * z_k[i];
                    p_k[i] = Az[i] + Beta * p_k[i];
                }
                scal_r_k -= Alpha * Alpha * scal_p_k;
                if (scal_r_k < eps)
                    nev = 0;
                else
                    if (Math.Abs(scal_r_k - scal_new_r_k) < eps)
                    nev = 0;
            }
            if (k == MaxNumIter)
            {
                Console.WriteLine("Вышел по макс. итерации, решение может быть неправильным\n" +
                            "количество итераций - " + k.ToString());
                Console.ReadKey();
            }
        }

        double[] makeSLAU(ref double[] _di, ref double[] _al, ref double[] _au,
    ref int[] _ia, ref int[] _ja, ref double[] _F)
        {
            //ig.resize(N + 1, 0);
            Array.Resize(ref ig, N + 1);
            nullVector(ref ig);
            //di.resize(N, 0);
            Array.Resize(ref di, N);
            nullVector(ref di);
            //ggl.resize(ig[N], 0);
            Array.Resize(ref ggl, N);
            nullVector(ref ggl);
            //ggu.resize(ig[N], 0);
            Array.Resize(ref ggu, N);
            nullVector(ref ggu);
            //jg.resize(ig[N], 0);
            Array.Resize(ref jg, N);
            nullVector(ref jg);
            ig = _ia;
            jg = _ja;
            di = _di;
            ggl = _al;
            ggu = _au;
            N = _di.Length;
            //vec.resize(N, 0);
            Array.Resize(ref vec, N);
            nullVector(ref vec);
            //x0.resize(N, 0);
            Array.Resize(ref x0, N);
            nullVector(ref x0);
            //alpha_k.resize(N, 0);
            Array.Resize(ref alpha_k, N);
            nullVector(ref alpha_k);
            //beta_k.resize(N, 0);
            Array.Resize(ref beta_k, N);
            nullVector(ref beta_k);
            //r_k.resize(N, 0);
            Array.Resize(ref r_k, N);
            nullVector(ref r_k);
            //z_k.resize(N, 0);
            Array.Resize(ref z_k, N);
            nullVector(ref z_k);
            //x_k.resize(N, 0);
            Array.Resize(ref x_k, N);
            nullVector(ref x_k);
            //q_k.resize(N, 0);
            Array.Resize(ref q_k, N);
            nullVector(ref q_k);
            //p_k.resize(N, 0);
            Array.Resize(ref p_k, N);
            nullVector(ref p_k);
            //Az.resize(N, 0);
            Array.Resize(ref Az, N);
            nullVector(ref Az);
            //Ax.resize(N, 0);
            Array.Resize(ref Ax, N);
            nullVector(ref Ax);
            vec = _F;
            Run();
            return x_k;
        }
    }
}
