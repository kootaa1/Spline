#pragma once
#include "Grid.h"
//(количество точек f)
//N
//f1, ... fn
//x1, ... xn
//(количество узлов сетки сплайна)
//M
//(узлы сплайна)
//x1, ... xm

int Grid::calculatePosistion(int i, int j)
{
	if (i >= X.size() || j >= Y.size() || i < 0 || j < 0)
		return -1;
	int k = j * X.size() + i;
	if (k >= 0)
		return k;
	else
		return -1;
}

void Grid::input()
{
	ifstream file("InputData.txt");
	int Qx, Qy, Qf;
	double buf;
	file >> Qx;
	for (int i = 0; i < Qx; ++i)
	{
		file >> buf;
		X.push_back(buf);
	}
	file >> Qy;
	for (int i = 0; i < Qy; ++i)
	{
		file >> buf;
		Y.push_back(buf);
	}

	/*file >> Qf;
	Point Pbuf;
	for (int i = 0; i < Qf; ++i)
	{
		file >> Pbuf.x >> Pbuf.y >> buf;
		F.push_back(buf);
		points.push_back(Pbuf);
	}*/
	normal_distribution<double> distr(0, 1);
	default_random_engine generator;
	for (double y = -2; y < 2 + 1e-10; y += 1)
	{
		for (double x = -2; x < 2 + 1e-10; x += 1)
		{
			error.push_back(distr(generator));//допилить выброс
			Point point;
			point.x = x; point.y = y;
			points.push_back(point);
			F.push_back(basis.F(x,y));
			omega.push_back(1);
			
		}
	}
	//error[6] += 100;
	//error[21] += 100;
	file.close();
}