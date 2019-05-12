#pragma once
#include "ListOfAdjacency.h"
void BubbleSort(vector<int> &A)
{
	for (int i = 0; i < A.size(); i++)
	{
		for (int j = 0; j < A.size() - i - 1; j++)
		{
			if (A[j] > A[j + 1])
			{
				int temp = A[j];
				A[j] = A[j + 1];
				A[j + 1] = temp;
			}
		}
	}
}
void ListOfAdjacency::fillingList(Grid grid)
{
	int sizeX = grid.X.size(), sizeY = grid.Y.size();
	int maxCount = sizeX * sizeY;
	list.resize(4 * grid.X.size()* grid.Y.size());
	for (int j = 0; j < sizeY; ++j)
	{
		for (int i = 0; i < sizeX; ++i)
		{
			int k = grid.calculatePosistion(i, j);
			for (int j1 = -1; j1 < 1; ++j1)
			{
				for (int i1 = -1; i1 < 2; ++i1)
				{
					int ii = i + i1, jj = j + j1, k1 = grid.calculatePosistion(ii, jj);
					if (k1 != -1 && k1 <= k) addToList(k, k1);
				}
			}
		//list[i].push_back(i - 1);
		}
	}
	for (int i = 0; i < maxCount; ++i)
	{
		BubbleSort(list[i]);
	}
}
int ListOfAdjacency::addToList(int k, int k1)
{ 
	if (list.size() == 0) return -1;
	int number = 4 * k;
	if (k == k1)
	{
		list[number + 1].push_back(number);
		list[number + 2].push_back(number);
		list[number + 2].push_back(number + 1);
		list[number + 3].push_back(number);
		list[number + 3].push_back(number + 1);
		list[number + 3].push_back(number + 2);
	}
	else //(k1 < k)
	{
		int maxI = number + 4;
		int maxJ = 4 * k1 + 4;
		for (int i = number; i < maxI; ++i)
		{
			for (int j = 4 * k1; j < maxJ; ++j)
			{
				list[i].push_back(j);
			}
		}
	}
	return 0;
}