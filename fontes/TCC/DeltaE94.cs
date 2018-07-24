using System;

namespace i1Sharp
{
    class DeltaE94
    {
        public static double DeltaE1994(ColorLAB lab1, ColorLAB lab2)
        {
            double deltaL = lab1.L - lab2.L;
            double deltaA = lab1.A - lab2.A;
            double deltaB = lab1.B - lab2.B;
            double c1 = Math.Sqrt(lab1.A * lab1.A + lab1.B * lab1.B);
            double c2 = Math.Sqrt(lab2.A * lab2.A + lab2.B * lab2.B);
            double deltaC = c1 - c2;
            double deltaH = deltaA * deltaA + deltaB * deltaB - deltaC * deltaC;
            deltaH = deltaH < 0 ? 0 : Math.Sqrt(deltaH);
            double sl = 1.0;
            double kc = 1.0;
            double kh = 1.0;
            double sc = 1.0 + 0.045 * c1;
            double sh = 1.0 + 0.015 * c1;
            double deltaLKlsl = deltaL / (1.0 * sl);
            double deltaCkcsc = deltaC / (kc * sc);
            double deltaHkhsh = deltaH / (kh * sh);
            double i = deltaLKlsl * deltaLKlsl + deltaCkcsc * deltaCkcsc + deltaHkhsh * deltaHkhsh;
            return i < 0 ? 0 : Math.Sqrt(i);
        }
    }
}
