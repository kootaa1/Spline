#pragma once
#include "Task.h"

#define FORMAT "%0.14e\n"
using namespace std;

vector<double>  ggl, ggu, di, vec, x0;
vector<int> ig, jg;
//alpha_k, beta_k - коэффициенты
//x0 - вектор начального приближения
//x_k - вектор решения на k-той итерации
//r_k - вектор невязки на k-той итерации 
//z_k - вектор спуска(сопряженное направление) на k-той итерации
vector<double> alpha_k, beta_k, r_k, z_k, x_k, q_k, p_k, Ax, Az;
int N, MaxNumIter = 1000000;//N-размерность матрицы
double eps = 1e-20, norVec;
void mult(vector<double> &v, vector<double> &az)//AZ=A*v
{
	v.resize(N);
	az.resize(N);
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
double skalar(vector<double> &v1, vector<double> &v2)
{
	double s = 0.0;
	for (int i = 0; i < N; i++)
		s += v1[i] * v2[i];
	return s;
}
double norma(vector<double> &vect)
{
	double s = 0.0;
	for (int i = 0; i < N; i++)
		s += vect[i] * vect[i];
	return sqrt(s);
}
double nevyazka(vector<double> &vect)
{
	return norma(vect) / norVec;
}
void LOS()
{
	vector<double> Vec;

	Vec.resize(N);
	int nev = 1, k;
	double scal_r_k, scal_new_r_k, scal_p_k, scal_rp_k, scal_Ap_k, Alpha, Beta;
	//k=0
	mult(x0, Az);
	for (int i = 0; i < N; i++)
	{
		r_k[i] = vec[i] - Az[i];
		z_k[i] = r_k[i];
	}
	scal_r_k = skalar(r_k, r_k);
	if (scal_r_k < eps)
		nev = 0;
	mult(z_k, p_k);//p_K
	for (int i = 0; i<N; i++)
		x_k[i] = x0[i];
	for (k = 1; k < MaxNumIter &&nev != 0; k++)
	{
		scal_new_r_k = scal_r_k;
		scal_p_k = skalar(p_k, p_k);
		scal_rp_k = skalar(p_k, r_k);
		Alpha = scal_rp_k / scal_p_k;//альфа
		for (int i = 0; i < N; i++)//xk и rk
		{
			x_k[i] += Alpha*z_k[i];
			r_k[i] -= Alpha*p_k[i];
		}
		mult(r_k, Az);
		scal_Ap_k = skalar(p_k, Az);
		scal_p_k = skalar(p_k, p_k);
		Beta = -scal_Ap_k / scal_p_k;//бета
		for (int i = 0; i < N; i++)//z_k, p_k
		{
			z_k[i] = r_k[i] + Beta*z_k[i];
			p_k[i] = Az[i] + Beta*p_k[i];
		}
		scal_r_k -= Alpha*Alpha*scal_p_k;
		if (scal_r_k < eps)
			nev = 0;
		else
			if (abs(scal_r_k - scal_new_r_k) < eps)
				nev = 0;
	}
	if (k == MaxNumIter)
	{
		cout << "Вышел по макс. итерации, решение может быть неправильным"<< endl;
		cout << "количество итераций - " << k << endl;
		cin.get();
	}
	
}
void nullVector(vector<double> a)
{
	for (int i = 0; i < a.size(); ++i)
		a[i] = 0;
}
vector<double> makeSLAU(vector<double> &_di, vector<double> &_al, vector<double> &_au,
	vector<int> &_ia, vector<int> &_ja, vector<double> &_F)
{
	ig.resize(N + 1, 0);
	di.resize(N, 0);
	ggl.resize(ig[N], 0);
	ggu.resize(ig[N], 0);
	jg.resize(ig[N], 0);
	ig = _ia;
	jg = _ja;
	di = _di;
	ggl = _al;
	ggu = _au;
	N = _di.size();
	vec.resize(N,0);
	x0.resize(N, 0);
	alpha_k.resize(N, 0);
	beta_k.resize(N, 0);
	r_k.resize(N, 0);
	z_k.resize(N, 0);
	x_k.resize(N, 0);
	q_k.resize(N, 0);
	p_k.resize(N, 0);
	Az.resize(N, 0);
	Ax.resize(N, 0);
	//nullVector(vec);
	//nullVector(x0);
	//nullVector(alpha_k);
	//nullVector(beta_k);
	//nullVector(r_k);
	//nullVector(z_k);
	//nullVector(x_k);
	//nullVector(q_k);
	//nullVector(p_k);
	//nullVector(Az);
	//nullVector(Ax);
	vec = _F;
	LOS();
	return x_k;
}
//------------------------------------------------------------------------------

//void main()
//{
//	switch (method)
//	{
//	case 1:	MSG();		break;
//	case 3: MSG_LU();	break;
//	default: printf("Error");
//	}
//	system("pause");
//}