using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.TextFormatting;
using Emgu.CV;
using Emgu.CV.Structure;
using ISIP_Algorithms.LPFiltering;

namespace ISIP_Algorithms.HPFiltering
{

    public class HPFiltering
    {
        public static Image<Gray, byte> CannyGray(Image<Gray, byte> InputImage, double T1, double T2, double sigma)
        {
            
            InputImage = LPFiltering.LPFiltering.GaussFilter(InputImage, sigma);
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);

            double fx, fy;

            int k = 5 / 2;

            double[,] matrixOfGradient = new double[InputImage.Height + 1, InputImage.Width + 1];
            double[,] matrixOfTheta = new double[InputImage.Height + 1, InputImage.Width + 1];
            int[,] matrixOfDirections = new int[InputImage.Height + 1, InputImage.Width + 1];
            double[,] Sx = new double[k + 1, k + 1];
            double[,] Sy = new double[k + 1, k + 1];

            Sx[0, 0] = -1; Sx[0, 1] = 0; Sx[0, 2] = 1; Sx[1, 0] = -2; Sx[1, 1] = 0; Sx[1, 2] = 2; Sx[2, 0] = -1; Sx[2, 1] = 0; Sx[2, 2] = 1;
            Sy[0, 0] = -1; Sy[0, 1] = -2; Sy[0, 2] = -1; Sy[1, 0] = 0; Sy[1, 1] = 0; Sy[1, 2] = 0; Sy[2, 0] = 1; Sy[2, 1] = 2; Sy[2, 2] = 1;

            for (int y = 0; y < Result.Height; y++)
                for (int x = 0; x < Result.Width; x++)
                {
                    if (y < k || y > Result.Height - k - 1 || x < k || x > Result.Width - k - 1)
                    {
                        Result.Data[y, x, 0] = 0;
                        continue;
                    }
                    fx = 0; fy = 0;
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 1; j++)
                        {
                            fx += InputImage.Data[y + i, x + j, 0] * Sx[i + 1, j + 1];
                            fy += InputImage.Data[y + i, x + j, 0] * Sy[i + 1, j + 1];

                        }
                    int norm = (int)Math.Sqrt(fx * fx + fy * fy);
                    if (norm > 255)
                        norm = 255;
                    Result.Data[y, x, 0] = (byte)norm;
                    if (Result.Data[y, x, 0] < T1)
                    {
                        Result.Data[y, x, 0] = 0;
                        matrixOfTheta[y, x] = 0;
                    }
                    else
                    {

                        double theta = Math.Atan((double)fy / fx);
                        matrixOfTheta[y, x] = theta;
                        if (matrixOfTheta[y, x] >= -Math.PI / 8 && matrixOfTheta[y, x] < Math.PI / 8)
                        {
                            //d0
                            matrixOfDirections[y, x] = 0;

                        }
                        else if (matrixOfTheta[y, x] >= -3 * Math.PI / 8 && matrixOfTheta[y, x] < -Math.PI / 8)
                        {
                            //d1
                            matrixOfDirections[y, x] = 1;


                        }
                        else if ((matrixOfTheta[y, x] >= -Math.PI / 2 && matrixOfTheta[y, x] < -3 * Math.PI / 8) || (matrixOfTheta[y, x] >= 3 * Math.PI / 8 && matrixOfTheta[y, x] <= Math.PI / 2))
                        {
                            //d2
                            matrixOfDirections[y, x] = 2;

                        }
                        else if (matrixOfTheta[y, x] >= Math.PI / 8 && matrixOfTheta[y, x] < 3 * Math.PI)
                        {
                            //d3
                            matrixOfDirections[y, x] = 3;

                        }

                    }
                }

            for (int y = k; y < Result.Height - k; y++)
                for (int x = k; x < Result.Width - k; x++)
                {

                    if (matrixOfDirections[y, x] == 0)
                    {
                        //d0
                        List<int> mask = new List<int>();
                        for (int i = -k; i <= k; i++)
                        {
                            mask.Add(Result.Data[y, x + i, 0]);
                        }
                        int index = mask.IndexOf(mask.Max());
                        for (int i = -k; i <= k; i++)
                        {
                            if (i + k != index)
                            {
                                Result.Data[y, x + i, 0] = 0;
                            }
                        }
                    }
                    else if (matrixOfDirections[y, x] == 1)
                    {
                        //d1
                        List<int> mask = new List<int>();
                        for (int i = -k; i <= k; i++)
                        {
                            mask.Add(Result.Data[y + i, x - i, 0]);
                        }
                        int index = mask.IndexOf(mask.Max());
                        for (int i = -k; i <= k; i++)
                        {
                            if (i + k != index)
                            {
                                Result.Data[y + i, x - i, 0] = 0;
                            }
                        }

                    }
                    else if (matrixOfDirections[y, x] == 2)
                    {
                        //d2
                        List<int> mask = new List<int>();
                        for (int i = -k; i <= k; i++)
                        {
                            mask.Add(Result.Data[y + i, x, 0]);
                        }
                        int index = mask.IndexOf(mask.Max());
                        for (int i = -k; i <= k; i++)
                        {
                            if (i + k != index)
                            {
                                Result.Data[y + i, x, 0] = 0;
                            }
                        }
                    }
                    else if (matrixOfDirections[y, x] == 3)
                    {
                        //d3
                        List<int> mask = new List<int>();
                        for (int i = -k; i <= k; i++)
                        {
                            mask.Add(Result.Data[y - i, x - i, 0]);
                        }
                        int index = mask.IndexOf(mask.Max());
                        for (int i = -k; i <= k; i++)
                        {
                            if (i + k != index)
                            {
                                Result.Data[y - i, x - i, 0] = 0;
                            }
                        }


                    }
                }

            Queue<Tuple<int, int>> Queue = new Queue<Tuple<int, int>>();
            for (int y = k; y < InputImage.Height - k; y++)
                for (int x = k; x < InputImage.Width - k; x++)
                {
                    if (Result.Data[y, x, 0] > T2)
                    {
                        Queue.Enqueue(new Tuple<int, int>(y, x));
                    }

                }

            while (Queue.Count != 0)
            {

                Tuple<int, int> tuple = Queue.Dequeue();
                int y = tuple.Item1;
                int x = tuple.Item2;
                Result.Data[y, x, 0] = 255;

                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i != 0 && j != 0 && Result.Data[y + i, x + j, 0] > T1 && Result.Data[y + i, x + j, 0] <= T2 /*&&  Result.Data[y+i, x+j, 0]!=255*/)
                        {

                            Queue.Enqueue(new Tuple<int, int>(y + i, x + j));
                        }
                    }


            }
            for (int y = 0; y < Result.Height; y++)
                for (int x = 0; x < Result.Width; x++)
                    if (Result.Data[y, x, 0] != 0)
                    {
                        Result.Data[y, x, 0] = 255;
                    }

            return Result;

        }

    }
}
