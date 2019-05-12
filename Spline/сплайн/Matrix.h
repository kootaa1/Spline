#pragma once
#include "Grid.h"
#include "ListOfAdjacency.h"
class Matrix
{
public:
	int n;
	vector<double> di, al, au, f;
	vector<int> ia, ja;

	int setEl(int i, int j, double El);
	void profileDefining(ListOfAdjacency listOfAdjacency, Grid grid);
	void outMatrix();
	double GetEl(int i, int j);
	void nullMatrix();
};