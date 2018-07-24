using System;

namespace i1Sharp
{
    class ColorLAB
    {
        public double L { get; set; }
        public double A { get; set; }
        public double B { get; set; }


        public ColorLAB (double l, double a, double b)
        {
            this.L = l;
            this.A = a;
            this.B = b;

        }

        public override string ToString()
        {
            return "L:" + Math.Round(this.L, 2)+
                   "  A:"+ Math.Round(this.A, 2)+
                   "  B:"+ Math.Round(this.B, 2);
        }
    }
}
