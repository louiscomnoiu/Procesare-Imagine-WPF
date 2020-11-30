using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV;
using Emgu.CV.Structure;
namespace ISIP_Algorithms.PointwiseOperators
{
    public class PointwiseOperators
    {
        public static Image<Gray, byte> AffineOperator(Image<Gray, byte> InputImage, int r1, int s1, int r2, int s2)
        {
            float alfa = (float)s1 / r1;
            float beta = (float)(s2 - s1) / (r2 - r1);
            float gamma = (float)(255 - s2) / (255 - r2);

            byte[] LUT = new byte[256];

            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);
            for (int r = 0; r < 256; r++)
            {
                if (r >= 0 && r < r1)
                {
                    LUT[r] = (byte)(alfa * r + 0.5);
                }
                else if (r >= r1 && r <= r2)
                {
                    LUT[r] = (byte)(beta * (r - r1) + s1 + 0.5);
                }
                else if (r >= r2 && r <= 256 - 1)
                {
                    LUT[r] = (byte)(gamma * (r - r2) + s2+0.5);
                }
            }

            for (int y = 0; y < InputImage.Height; y++)
                for (int x = 0; x < InputImage.Width; x++)
                    Result.Data[y, x, 0] = (byte)LUT[InputImage.Data[y, x, 0]];

            return Result;
        }
    }
}
//   Result.Data[y, x, 0] = (byte)(255 - InputImage.Data[y, x, 0]);