using System;

namespace i1Sharp
{
    class DeltaE76
    {
        public static double DeltaE1976(ColorLAB lab1, ColorLAB lab2)
        {
            double retorno = Math.Sqrt(((lab1.L - lab2.L) * (lab1.L - lab2.L)) +
                                       ((lab1.A - lab2.A) * (lab1.A - lab2.A)) +
                                       ((lab1.B - lab2.B) * (lab1.B - lab2.B)));

            return retorno;
        }
    }
}
