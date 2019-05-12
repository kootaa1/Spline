#pragma once
#include "Grid.h"
#include <vector>
#include <algorithm>
#include <functional>


using namespace std;

class ListOfAdjacency
{
	
public:

	vector<vector<int> > list;
	void fillingList(Grid grid);
	int addToList(int k, int k1);
};