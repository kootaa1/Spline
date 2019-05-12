#pragma once
#include "Task.h"
double alpha = 1e-5, beta = 0, coef = 3.5, divCoef = 1.5;
void Task::matrixFilling()
{
	ofstream file("right.txt");
	int sizeX = grid.X.size() - 1, sizeY = grid.Y.size() - 1;
	double hx, hy;
	double localMatrix[4][4];
	vector<int> indexOfPoints;
	vector<bool> usedPoints;
	usedPoints.resize(grid.F.size(), false);
	int indexInMatrix[16];
	for (int j = 0; j < sizeY; ++j)
	{
		for (int i = 0; i < sizeX; ++i)
		{
			indexOfPoints.resize(0);
			for (int k = 0; k < grid.points.size(); ++k)
			{
				if (!usedPoints[k] &&
					grid.points[k].x >= grid.X[i] && grid.points[k].x <= grid.X[i + 1] &&
					grid.points[k].y >= grid.Y[j] && grid.points[k].y <= grid.Y[j + 1])
				{
					indexOfPoints.push_back(k);
					usedPoints[k] = true;
				}
			}
			if (indexOfPoints.size() == 0)
			{
				cout << "I can't find points on interval [" << grid.X[i]
					<< ", " << grid.X[i + 1] << "] * [" << grid.Y[j] << ", "
					<< grid.Y[j + 1] << "]\n you must fix it, before do something." << endl
					<< "Press any key, to exit." << endl;
				cin.get();
				system("exit");
			}
			hx = grid.X[i + 1] - grid.X[i];
			hy = grid.Y[j + 1] - grid.Y[j];
			int buf = 4 * grid.calculatePosistion(i, j);
			indexInMatrix[0] = buf;
			indexInMatrix[1] = buf + 1;
			indexInMatrix[2] = buf + 2;
			indexInMatrix[3] = buf + 3;
			buf = 4 * grid.calculatePosistion(i + 1, j);
			indexInMatrix[4] = buf;
			indexInMatrix[5] = buf + 1;
			indexInMatrix[6] = buf + 2;
			indexInMatrix[7] = buf + 3;
			buf = 4 * grid.calculatePosistion(i, j + 1);
			indexInMatrix[8] = buf;
			indexInMatrix[9] = buf + 1;
			indexInMatrix[10] = buf + 2;
			indexInMatrix[11] = buf + 3;
			buf = 4 * grid.calculatePosistion(i + 1, j + 1);
			indexInMatrix[12] = buf;
			indexInMatrix[13] = buf + 1;
			indexInMatrix[14] = buf + 2;
			indexInMatrix[15] = buf + 3;
			for (int k = 0; k < indexOfPoints.size(); ++k)
			{
				for (int ii = 0; ii < 16; ++ii)
				{
					for (int jj = 0; jj < 16; ++jj)
					{
						a.setEl(indexInMatrix[ii], indexInMatrix[jj], grid.omega[k]*basis.Psi(ii, grid.X[i], grid.Y[j],
							hx, hy, grid.points[indexOfPoints[k]].x, grid.points[indexOfPoints[k]].y)*
							basis.Psi(jj, grid.X[i], grid.Y[j], hx, hy, 
								grid.points[indexOfPoints[k]].x, grid.points[indexOfPoints[k]].y)
							+ basis.secondComp(alpha, ii, jj, hx, hy)
							+ basis.thirdComp(beta, ii, jj, hx, hy));
					}
					f[indexInMatrix[ii]] += grid.omega[k] * basis.Psi(ii, grid.X[i], grid.Y[j], hx, hy,
						grid.points[indexOfPoints[k]].x, grid.points[indexOfPoints[k]].y)*(grid.F[indexOfPoints[k]]+grid.error[k]);
					/*file << basis.Psi(ii, grid.X[i], grid.Y[j], hx, hy,
						grid.points[indexOfPoints[k]].x, grid.points[indexOfPoints[k]].y)*grid.F[indexOfPoints[k]] << "\t";*/
				}
				file << endl;
			}
		}
	}
	file.close();
}
void Task::make()
{
	vector<double> result;
	grid.input();
	a.profileDefining(list, grid);
	f.resize(4 * grid.X.size() * grid.Y.size(), 0);
	double meanDev;
	bool flag = true;
	int k = 0;
	for(; flag; ++k)
	{
		flag = false;
		a.nullMatrix();
		nullVector(f);
		matrixFilling();
		result = makeSLAU(a.di, a.al, a.au, a.ia, a.ja, f);
		meanDev = meanDeviation(result);
		for (int i = 0; i < grid.points.size(); ++i)
		{
			double abso = abs(grid.omega[i]*(grid.F[i] + grid.error[i]) - valueInPoint(grid.points[i].x, grid.points[i].y, result));
			if (abso > coef * meanDev + 1e-7)
			{
				flag = true;
				grid.omega[i] /= divCoef;
				cout << grid.points[i].x << "\t" << grid.points[i].y << "\t" << valueInPoint(grid.points[i].x, grid.points[i].y, result) << endl;
			}
		}
		printSpline(0.05, 0.05, result, k);
		cout << endl;
	}
	ofstream outK("k.txt");
	outK << k-1 << endl;
	outK.close();
	//a.outMatrix();
	/*ofstream out("Result.txt");
	for (int i = 0; i < result.size(); ++i)
	{
		out << result[i] << endl;
	}
	out.close();*/
}
double Task::meanDeviation(vector<double> result)
{
	double sum=0;
	for (int i = 0; i < grid.error.size(); ++i)
		sum += abs(grid.omega[i]*(grid.error[i]+grid.F[i])-valueInPoint(grid.points[i].x,grid.points[i].y,result));
	sum /= grid.error.size();
	return sum;
}
void Task::nullVector(vector<double> &a)
{
	for (int i = 0; i < a.size(); ++i)
		a[i] = 0;
}
double Task::valueInPoint(double x, double y, vector<double> result)
{
	int maxI = grid.X.size() - 1, maxJ = grid.Y.size() - 1;
	int i = 0, j = 0;
	double summ=0;
	while (grid.X[i] < x && i < maxI - 1)
		++i;
	if (i > 0) --i;
	double hx = grid.X[i + 1] - grid.X[i];
	if (y > grid.Y[maxJ]) y = grid.Y[maxJ];
	while (grid.Y[j] < y && j < maxJ - 1)
		++j;
	if (j > 0) --j;
	double hy = grid.Y[j + 1] - grid.Y[j];
	int NumOfEl = 4 * grid.calculatePosistion(i, j);
	for (int k = 0; k < 16; ++k)
	{
		summ += result[NumOfEl + k] * basis.Psi(k, grid.X[i], grid.Y[j], hx, hy, x, y);
	}
	return summ;
}

void Task::printSpline(double hx, double hy, vector<double> result, int numberOfFile)
{
	int maxI = grid.X.size() - 1, maxJ = grid.Y.size() - 1;
	int kMax = grid.points.size();
	ofstream outX;
	if(numberOfFile==0)
		outX.open("SplineX.txt");
	//ofstream layer("layer.txt");
	ofstream outF(std::to_string(numberOfFile) + "SplineF.txt");
	ofstream outY;
	if (numberOfFile == 0)
		outY.open("SplineY.txt");
	vector<int> indexesOfPoints;
	double summ,hXforBasis,hYforBasis;
	for (double y = grid.Y[0]; y < grid.Y[maxJ] + 1e-10; y += hy)
	{
		for (double x = grid.X[0]; x < grid.X[maxI] + 1e-10; x += hx)
		{
			int i = 0, j = 0;
			summ = 0;
			while (grid.X[i] < x && i < maxI - 1)
				++i;
			if (i > 0) --i;
			hXforBasis = grid.X[i + 1] - grid.X[i];
			if (y > grid.Y[maxJ]) y = grid.Y[maxJ];
			while (grid.Y[j] < y && j < maxJ - 1)
				++j;
			if (j > 0) --j;
			hYforBasis = grid.Y[j + 1] - grid.Y[j];
			int NumOfEl =4 * grid.calculatePosistion(i, j);
			for (int k = 0; k < 16; ++k)
			{
				summ += result[NumOfEl + k] * basis.Psi(k, grid.X[i], grid.Y[j], hXforBasis, hYforBasis, x, y);
			}
			/*if (i == 0)
			{
				layer << summ << endl;
			}*/
			if(y == grid.Y[0] && numberOfFile == 0) 
				outX << x << endl;
			outF << summ << endl;
		}
		if(numberOfFile == 0)
			outY << y << endl;
	}
	outF.close();
	if (numberOfFile == 0)
		outX.close();
	if (numberOfFile == 0)
		outY.close();
}
