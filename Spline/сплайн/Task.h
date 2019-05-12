#pragma once
#include "Matrix.h"
#include "Grid.h"
#include "Basis.h"
#include "ListOfAdjacency.h"
#include <math.h>
#include <iomanip>
#include <string>

class Task
{
public:
	Matrix a;
	vector<double> f;
	ListOfAdjacency list;
	Grid grid;
	Basis basis;
	void matrixFilling();
	void make();
	void printSpline(double hx, double hy, vector<double> result, int i);
	double meanDeviation(vector<double> result);
	double valueInPoint(double x, double y, vector<double> result);
	void nullVector(vector<double>& a);
};

extern vector<double> makeSLAU(vector<double> &_di, vector<double> &_al, vector<double> &_au, vector<int> &_ia, vector<int> &_ja, vector<double> &_F);