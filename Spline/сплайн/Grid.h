#pragma once
#include <fstream>
#include <vector>
#include <random>
#include "Basis.h"

using namespace std;

struct Point
{
	double x, y;
};

class Grid
{
public:
	Basis basis;
	vector<Point> points;
	vector<double> omega;
	vector<double> error;
	vector<double> X, Y, F;
	void input();
	int calculatePosistion(int i, int j);
};