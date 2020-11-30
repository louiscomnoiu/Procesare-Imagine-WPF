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

namespace ISIP_Algorithms.Thresholding
{
    public class Thresholding
    {
        private static Image<Bgr, double> ConvertRGBtoHSV(Image<Bgr, byte> InputImage)
        {
            Image<Bgr, double> Result = new Image<Bgr, double>(InputImage.Size);
            double Rprim, Gprim, Bprim;
            double Cmax, Cmin, delta;
            double H, S=0.0, V;

            for (int y = 0; y < InputImage.Height; y++)
                for (int x = 0; x < InputImage.Width; x++)
                {
                    Rprim =(double)InputImage.Data[y, x, 2] / 255; //Rprim=R/255
                    Gprim = (double)InputImage.Data[y, x, 1] / 255; //Gprim=G/255
                    Bprim = (double)InputImage.Data[y, x, 0] / 255; //Bprim=B/255
                    Cmax = Math.Max(Rprim, Math.Max(Gprim, Bprim));
                    Cmin = Math.Min(Rprim, Math.Min(Gprim, Bprim));
                    delta = Cmax - Cmin;

                    if (delta == 0)
                        H = 0.0;
                    else if (Cmax == Rprim)
                    {
                        H = (Gprim - Bprim) / delta;
                        if (H < 0)
                            H += 6;
                        H %= 6;
                    }
                    else if(Cmax == Gprim)
                    {
                        H = (Bprim - Rprim) / delta + 2.0;
                    }
                    else
                    {
                        H = (Rprim - Gprim) / delta + 4.0;
                    }
                    H *= 60;

                    if (Cmax != 0)
                        S = delta / Cmax;

                    V = Cmax;

                    Result.Data[y, x, 0] = (byte)H;
                    Result.Data[y, x, 1] = (byte)S;
                    Result.Data[y, x, 2] = (byte)V;
                }

            return Result;
        }

        public static Image<Gray, byte> ColorHSVBinarization(Image<Bgr, byte> InputImage, System.Windows.Point lastClick, double T)
        {
            Image<Bgr, double> HSVImage = ConvertRGBtoHSV(InputImage);
            Image<Gray, byte> binaryImage = new Image<Gray, byte>(InputImage.Size);
            int x1 = (int)lastClick.X;
            int y1 = (int)lastClick.Y;
           

            for (int y = 0; y < HSVImage.Height; y++)
                for (int x = 0; x < HSVImage.Width; x++)
                {
                    if(HSVImage.Data[y1,x1,0] >= 0 && HSVImage.Data[y1, x1, 0] <=T)
                        if (Math.Abs(HSVImage.Data[y,x,0] - HSVImage.Data[y1, x1, 0]) < T)
                        {
                            binaryImage.Data[y, x, 0] = 255;
                        }
                        else
                        {
                            binaryImage.Data[y, x, 0] = 0;
                        }
                    if(HSVImage.Data[y1, x1, 0] >= 360 - T && HSVImage.Data[y1, x1, 0] <= 360)
                        if (Math.Abs(360 - Math.Abs(HSVImage.Data[y, x, 0] - HSVImage.Data[y1, x1, 0])) < T)
                        {
                            binaryImage.Data[y, x, 0] = 255;
                        }
                        else
                        {
                            binaryImage.Data[y, x, 0] = 0;
                        }


                }
            return binaryImage;
        }

        public static Image<Gray, byte> Color3DBinarization(Image<Bgr, byte> inputImage, int T, Point click)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (Math.Sqrt(Math.Pow(inputImage.Data[y, x, 2] - inputImage.Data[(int)(click.Y), (int)(click.X), 2], 2) +
                        Math.Pow(inputImage.Data[y, x, 1] - inputImage.Data[(int)(click.Y), (int)(click.X), 1], 2) +
                        Math.Pow(inputImage.Data[y, x, 0] - inputImage.Data[(int)(click.Y), (int)(click.X), 0], 2)) <= T)
                        result.Data[y, x, 0] = 0;
                    else
                        result.Data[y, x, 0] = 255;
                }
            }
            return result;
        }

        public static Image<Gray, byte> Color2DBinarization(Image<Bgr, byte> inputImage, int T, Point click)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            int sum = inputImage.Data[(int)(click.Y), (int)(click.X), 2] + inputImage.Data[(int)(click.Y), (int)(click.X), 1] + inputImage.Data[(int)(click.Y), (int)(click.X), 0];
            inputImage.Data[(int)(click.Y), (int)(click.X), 2] = (byte)(inputImage.Data[(int)(click.Y), (int)(click.X), 2] / sum);
            inputImage.Data[(int)(click.Y), (int)(click.X), 1] = (byte)(inputImage.Data[(int)(click.Y), (int)(click.X), 1] / sum);


            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (Math.Sqrt(Math.Pow(inputImage.Data[y, x, 2] - inputImage.Data[(int)(click.Y), (int)(click.X), 2], 2) +
                        Math.Pow(inputImage.Data[y, x, 1] - inputImage.Data[(int)(click.Y), (int)(click.X), 1], 2)) <= T)
                        result.Data[y, x, 0] = 0;
                    else
                        result.Data[y, x, 0] = 255;
                }
            }
            return result;
        }


    }
}