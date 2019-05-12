#pragma once
#include "Matrix.h"
#include "iomanip" 

void Matrix::outMatrix()
{
	ofstream out("Mmatrix.txt");
	double buf;
	for (int i = 0; i < n; ++i)
	{
		for (int j = 0; j < n; ++j)
		{
			buf = GetEl(i, j);
			//if (buf >= 0) out << 0;
			out << fixed << setprecision(4) << buf << "\t\t";
		}
		out << endl;
	}
}
void Matrix::nullMatrix()
{
	for (int i = 0; i < al.size(); ++i)
	{
		al[i] = 0;
		au[i] = 0;
	}
	for (int i = 0; i < di.size(); ++i)
	{
		di[i] = 0;
	}
}
double Matrix::GetEl(int i, int j)
{
	if (i == j)
	{
		return di[i];
	}
	if (i > j)
	{
		int i0 = ia[i];
		int i1 = ia[i + 1];
		for (int k = i0; k<i1; k++)
			if (ja[k] == j)
			{
				return al[k];
			}
		return 0;
	}
	else
	{
		swap(i, j);
		int i0 = ia[i];
		int i1 = ia[i + 1];
		for (int k = i0; k<i1; k++)
			if (ja[k] == j)
			{
				return au[k];
			}
		return 0;
	}
}
int Matrix::setEl(int i, int j, double El)
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
		swap(i, j);
		int i0 = ia[i];
		int i1 = ia[i + 1];
		for (int k = i0; k<i1; k++)
			if (ja[k] == j)
			{
				au[k] += El;
				return 0;
			}
		return -1;
	}
}
void Matrix::profileDefining(ListOfAdjacency listOfAdjacency, Grid grid)
{
	n = 4 * grid.X.size() * grid.Y.size();
	int k = 0;
	listOfAdjacency.fillingList(grid);
	ia.reserve(n + 1);
	ja.reserve(n);
	ia.push_back(0);
	di.reserve(n);
	al.reserve(4 * n);
	for (int i = 0; i < n; i++)
	{
		di.push_back(0);
		for (int j = 0; j < listOfAdjacency.list[i].size(); j++)
		{
			ja.push_back(listOfAdjacency.list[i][j]);
			al.push_back(0);
			au.push_back(0);
			k++;
		}
		ia.push_back(k);
	}
}
