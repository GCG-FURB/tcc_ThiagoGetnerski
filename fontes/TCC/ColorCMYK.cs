using System;

namespace i1Sharp
{
    class ColorCMYK
    {
        public double C { get; set; }
        public double M { get; set; }
        public double Y { get; set; }
        public double K { get; set; }

        public ColorCMYK (double c, double m, double y, double k)
        {
            this.C = c;
            this.M = m;
            this.Y = y;
            this.K = k;
        }

        public override string ToString()
        {
            return  "C:"  + Math.Round(this.C, 2) + 
                   "  M:" + Math.Round(this.M, 2) + 
                   "  Y:" + Math.Round(this.Y, 2) + 
                   "  K:" + Math.Round(this.K, 2);
        }
    }
}
