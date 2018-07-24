using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace i1Sharp
{
    class Historico
    {
 
        public SolidColorBrush cor1 { get; set; }
        public SolidColorBrush cor2 { get; set; }
        public double deltaE { get; set; }

        public Historico(ColorRGB rgb1, ColorRGB rgb2, double deltaE)
        {
            this.deltaE = Math.Round(deltaE, 4);
            this.cor1 = new SolidColorBrush(Color.FromRgb((byte)rgb1.R, (byte)rgb1.G, (byte)rgb1.B));
            this.cor2 = new SolidColorBrush(Color.FromRgb((byte)rgb2.R, (byte)rgb2.G, (byte)rgb2.B));
        }

    }
}
