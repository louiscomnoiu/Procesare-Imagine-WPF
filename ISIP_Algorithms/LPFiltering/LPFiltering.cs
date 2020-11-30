using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.TextFormatting;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ISIP_Algorithms.LPFiltering
{
    public class LPFiltering
    {
        public static Image<Gray, byte> UnsharpMask(Image<Gray, byte> InputImage, Image<Gray, byte> FilteredImage)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);

            for (int y = 0; y < InputImage.Height; y++)
            {
                for (int x = 0; x < InputImage.Width; x++)
                {
                    Result.Data[y, x, 0] = (byte)(InputImage.Data[y, x, 0] + (InputImage.Data[y, x, 0] - FilteredImage.Data[y, x, 0]));
                }
            }
            return Result;


        }


        public static Image<Gray, byte> GaussFilter(Image<Gray, byte> InputImage, double sigma)
        {
            Image<Gray, byte> Result = InputImage.Clone();
            int size = (int)Math.Ceiling(sigma * 4);
            
            if (size % 2 == 0)
            {
                size++;
            }
          
            int k = size / 2; 
            double[,] kernel = new double[size + 1, size + 1]; 
            double sum = 0;

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] = Math.Pow(Math.E, -(Math.Pow(i - k, 2) + Math.Pow(j - k, 2)) / (2 * Math.Pow(sigma, 2)));
                    sum += kernel[i, j];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= sum;
                }
            for (int y = k; y < InputImage.Height - k; y++)
                for (int x = k; x < InputImage.Width - k; x++)
                {
                    sum = 0;
                    for (int i = -k; i <= k; i++)
                        for (int j = -k; j <= k; j++)
                        {
                            sum += InputImage.Data[y + i, x + j, 0] * kernel[i + k, j + k];
                        }
                    Result.Data[y, x, 0] = (byte)Math.Round(sum);
                }
            return Result;


        }
    }
}
