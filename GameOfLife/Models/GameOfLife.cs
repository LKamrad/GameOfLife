using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameOFLifeInWPF.Models
{
    class GameOfLife
    {
        static int[,] array_old = new int[42, 72];
        static int[,] array_new = new int[42, 72];
        static int[,] array = new int[42, 72];
        static bool isGrowing = true;
        private async void NextGeneration()
        {
            int temp;

            isGrowing = false;
            for (int i = 1; i < array.GetLength(0) - 1; i++)
            {

                for (int j = 1; j < array.GetLength(1) - 1; j++)
                {
                    array_old[i, j] = array[i, j];


                    temp = array[i - 1, j - 1] + array[i - 1, j] + array[i - 1, j + 1] + array[i, j - 1] + array[i, j + 1] + array[i + 1, j - 1] + array[i + 1, j] + array[i + 1, j + 1];


                    if (temp == 3 || (temp == 2 && array[i, j] == 1))
                    {
                        array_new[i, j] = 1;
                    }
                    if (temp > 3 || temp < 2)
                    {
                        array_new[i, j] = 0;
                    }


                }

            }
            for (int i = 1; i < array.GetLength(0) - 1; i++)
            {

                for (int j = 1; j < array.GetLength(1) - 1; j++)
                {
                    array[i, j] = array_new[i, j];
                    if (array_new[i, j] != array_old[i, j])
                    {
                        isGrowing = true;
                    }
                }
            }
        }
    }
}
