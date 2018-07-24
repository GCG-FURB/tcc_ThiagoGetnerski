using System;

namespace i1Sharp
{
    class DeltaE00
    {
        public static double DeltaE2000(ColorLAB lab1, ColorLAB lab2)
        {
            double c1 = Math.Sqrt(lab1.A * lab1.A + lab1.B * lab1.B);
            double c2 = Math.Sqrt(lab2.A * lab2.A + lab2.B * lab2.B);
            double cab = (c1 + c2) / 2;
            double cMediaAB = cab * cab * cab;
            cMediaAB *= cMediaAB * cab;
            double G = 0.5d * (1 - Math.Sqrt(cMediaAB / (cMediaAB + 6103515625)));
            double ai1 = (1 + G) * lab1.A;
            double ai2 = (1 + G) * lab2.A;
            double ci1 = Math.Sqrt(ai1 * ai1 + lab1.B * lab1.B);
            double ci2 = Math.Sqrt(ai2 * ai2 + lab2.B * lab2.B);
            double hi1 = ((Math.Atan2(lab1.B, ai1) / Math.PI * 180) + 360) % 360d;
            double hi2 = ((Math.Atan2(lab2.B, ai2) / Math.PI * 180) + 360) % 360d;
            double deltaL = lab2.L - lab1.L;
            double deltaC = ci2 - ci1;
            double deltaHBarra = Math.Abs(hi1 - hi2);
            double deltaHAux;
            if (ci1 * ci2 == 0) deltaHAux = 0;
            else
            {
                if (deltaHBarra <= 180d)
                {
                    deltaHAux = hi2 - hi1;
                }
                else if (deltaHBarra > 180d && hi2 <= hi1)
                {
                    deltaHAux = hi2 - hi1 + 360.0;
                }
                else
                {
                    deltaHAux = hi2 - hi1 - 360.0;
                }
            }
            double deltaH = 2 * Math.Sqrt(ci1 * ci2) * Math.Sin((deltaHAux / 2) * Math.PI / 180);
            double LMedia = (lab1.L + lab2.L) / 2d;
            double CMedia = (ci1 + ci2) / 2d;
            double hMediaAux;
            if (ci1 * ci2 == 0) hMediaAux = 0;
            else
            {
                if (deltaHBarra <= 180d)
                {
                    hMediaAux = (hi1 + hi2) / 2;
                }
                else if (deltaHBarra > 180d && (hi1 + hi2) < 360d)
                {
                    hMediaAux = (hi1 + hi2 + 360d) / 2;
                }
                else
                {
                    hMediaAux = (hi1 + hi2 - 360d) / 2;
                }
            }
            double T = 1
                        - .17 * Math.Cos((hMediaAux - 30) * Math.PI / 180)
                        + .24 * Math.Cos((hMediaAux * 2) * Math.PI / 180)
                        + .32 * Math.Cos((hMediaAux * 3 + 6) * Math.PI / 180)
                        - .2 * Math.Cos((hMediaAux * 4 - 63) * Math.PI / 180);
            double  delta0Aux = (hMediaAux - 275) / (25);
             delta0Aux *=  delta0Aux;
            double delta0 = 30 * Math.Exp(- delta0Aux);
            double RcAux = (CMedia * CMedia * CMedia) * CMedia;
            RcAux *= RcAux * CMedia;
            double Rc = 2 * Math.Sqrt(RcAux / (RcAux + 6103515625));
            double LMediaLinha = (LMedia - 50)* (LMedia - 50);
            double Sl = 1 + ((.015d * LMediaLinha) / Math.Sqrt(20 + LMediaLinha));
            double Sc = 1 + .045d * CMedia;
            double Sh = 1 + .015 * T * CMedia;
            double Rt = -Math.Sin((2 * delta0) * Math.PI / 180) * Rc;
            double deltaL_kl_Sl = deltaL / (Sl * 1);
            double deltaC_kc_Sc = deltaC / (Sc * 1);
            double deltaH_kh_Sh = deltaH / (Sh * 1);
            double deltaE00 = Math.Sqrt(deltaL_kl_Sl * deltaL_kl_Sl
                                       + deltaC_kc_Sc * deltaC_kc_Sc
                                       + deltaH_kh_Sh * deltaH_kh_Sh
                                       + Rt * deltaC_kc_Sc * deltaH_kh_Sh
                                       );

            return deltaE00;
        }
    }
}
