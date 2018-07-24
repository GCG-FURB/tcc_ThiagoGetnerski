using System;
using System.Windows.Media;

namespace i1Sharp
{
    class ColorRGB
    {

        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public SolidColorBrush cor { get; set; }

        public ColorRGB (double r, double g, double b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            
            this.cor = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
        }

        public override string ToString()
        {
            return "R:" + Math.Round(this.R, 2) + 
                 "  G: "+ Math.Round(this.G, 2) + 
                 "  B: "+ Math.Round(this.B, 2);
        }
    }
}
