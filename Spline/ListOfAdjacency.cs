using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spline.sources
{
    class ListOfAdjacency
    {
        //List<List<int>> list;
        //int[] huy = new int[5];
        public List<int>[] list;
        //int[][] list;
        public void fillingList(Grid grid)
        {
            int sizeX = grid.getXSize(), sizeY = grid.getYSize();
            int maxCount = sizeX * sizeY;
            Array.Resize(ref list, 4 * sizeX * sizeY);
            for (int j = 0; j < sizeY; ++j)
            {
                for (int i = 0; i < sizeX; ++i)
                {
                    int k = grid.calculatePosition(i, j);
                    for (int j1 = -1; j1 < 1; ++j1)
                    {
                        for (int i1 = -1; i1 < 2; ++i1)
                        {
                            int ii = i + i1, jj = j + j1, k1 = grid.calculatePosition(ii, jj);
                            if (k1 != -1 && k1 <= k) addToList(k, k1);
                        }
                    }
                    //list[i].push_back(i - 1);
                }
            }
            for (int i = 0; i < maxCount; ++i)
            {
                list[i].Sort();
            }
        }
        int addToList(int k, int k1)
        {
            int number = 4 * k;
            if(k == k1)
            {
                list[number + 1].Add(number);
                list[number + 2].Add(number);
                list[number + 2].Add(number + 1);
                list[number + 3].Add(number);
                list[number + 3].Add(number + 1);
                list[number + 3].Add(number + 2);
            }
            else
            {
                int maxI = number + 4;
                int maxJ = 4 * k1 + 4;
                for (int i = number; i < maxI; ++i)
                {
                    for (int j = 4 * k1; j < maxJ; ++j)
                    {
                        list[i].Add(j);
                    }
                }
            }
            return 0;
        }
    }
}
