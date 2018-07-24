using System;

namespace i1Sharp
{
    class ColorPantone
    {
        public ColorLAB LAB { get; set; }
        public ColorRGB RGB { get; set; }
        public String Nome { get; set; }
        public double DeltaE { get; set; }

        public ColorPantone(ColorLAB lab, ColorRGB rgb, String nome)
        {
            this.LAB = lab;
            this.RGB = rgb;
            this.Nome = nome;
        }
    }
}
