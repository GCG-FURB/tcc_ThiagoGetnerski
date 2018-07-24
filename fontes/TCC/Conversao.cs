using System;

namespace i1Sharp
{
    class Conversao
    {
        public static ColorRGB LabToRgb( ColorLAB lab)
        {
            double y = (lab.L + 16) / 116;
            double x = lab.A / 500 + y;
            double z = y - lab.B / 200;
            double r, g, b;
            
            x = 0.95047 * ((x * x * x > 0.008856) ? x * x * x : (x - 16 / 116) / 7.787);
            y = 1.00000 * ((y * y * y > 0.008856) ? y * y * y : (y - 16 / 116) / 7.787);
            z = 1.08883 * ((z * z * z > 0.008856) ? z * z * z : (z - 16 / 116) / 7.787);
            
            r = x * 3.2406 + y * -1.5372 + z * -0.4986;
            g = x * -0.9689 + y * 1.8758 + z * 0.0415;
            b = x * 0.0557 + y * -0.2040 + z * 1.0570;

            r = (r > 0.0031308) ? (1.055 * Math.Pow(r, 1 / 2.4) - 0.055) : 12.92 * r;
            g = (g > 0.0031308) ? (1.055 * Math.Pow(g, 1 / 2.4) - 0.055) : 12.92 * g;
            b = (b > 0.0031308) ? (1.055 * Math.Pow(b, 1 / 2.4) - 0.055) : 12.92 * b;

            ColorRGB retorno = new ColorRGB(Math.Max(0, Math.Min(1, r)) * 255,
                                            Math.Max(0, Math.Min(1, g)) * 255,
                                            Math.Max(0, Math.Min(1, b)) * 255 );

            return retorno;
        }
        

        public static ColorCMYK RGBToCMYK(ColorRGB rgb)
        {
            double k = Math.Min(1.0 - rgb.R / 255.0, Math.Min(1.0 - rgb.G / 255.0, 1.0 - rgb.B / 255.0));
            double c = (1.0 - (rgb.R / 255.0) - k) / (1.0 - k);
            double m = (1.0 - (rgb.G / 255.0) - k) / (1.0 - k);
            double y = (1.0 - (rgb.B / 255.0) - k) / (1.0 - k);
            return new ColorCMYK ( c*100, m*100, y*100, k*100 );
        }
    }
}
            