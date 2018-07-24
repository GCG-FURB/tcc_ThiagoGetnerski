using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace i1Sharp
{
    class BancoDeDados
    {
        public static void INSERTComparacao(ColorRGB rgb1, ColorRGB rgb2, double deltae2000)
        {
            try
            {
                MySqlConnection conectar = new MySqlConnection("server=localhost;database=tcc; Uid=root; pwd=Thiago145698;");
                conectar.Open();

                MySqlCommand Inserir = new MySqlCommand();
                Inserir.Connection = conectar;
                Inserir.CommandText =
                    "INSERT INTO comparacao (r1 , g1 , b1 , r2 , g2 , b2 , deltae2000) VALUES " +
                    "('" + Convert.ToString(rgb1.R) + "' , '" + Convert.ToString(rgb1.G) + "' , '" + Convert.ToString(rgb1.B) + "' , '"
                         + Convert.ToString(rgb2.R) + "' , '" + Convert.ToString(rgb2.G) + "' , '" + Convert.ToString(rgb2.B) + "' , '"
                         + Convert.ToString(deltae2000) + "');";


                Inserir.ExecuteNonQuery();
                conectar.Close();

            }
            catch (Exception e)
            {

            }
        }

        public static List<ColorPantone> SELECTPantone()
        {
            MySqlConnection conectar = new MySqlConnection("server=localhost;database=tcc; Uid=root; pwd=Thiago145698;");
            MySqlCommand select = new MySqlCommand();
            select.Connection = conectar;
            select.CommandText = "SELECT * FROM pantonecoated";
            conectar.Open();
            MySqlDataReader data = select.ExecuteReader();
            List<ColorPantone> listaDePantones = new List<ColorPantone>();

            while (data.Read())
            {
                String nome = data["pantonename"].ToString();

                String lAux = Convert.ToString(data["l"]).Replace(".", ",");
                String aAux = Convert.ToString(data["a"]).Replace(".", ",");
                String bAux = Convert.ToString(data["b"]).Replace(".", ",");
                double l = Convert.ToDouble(lAux);
                double a = Convert.ToDouble(aAux);
                double b = Convert.ToDouble(bAux);
                ColorLAB labAux = new ColorLAB(l,a,b);

                double red = Convert.ToDouble(data["red"]);
                double green = Convert.ToDouble(data["green"]);
                double blue = Convert.ToDouble(data["blue"]);
                ColorRGB rgbAux = new ColorRGB(red, green, blue);

                ColorPantone pantoneAux = new ColorPantone(labAux, rgbAux, nome);

                listaDePantones.Add(pantoneAux);
            }

            return listaDePantones;
        }

        public static List<Historico> SELECTHistorico()
        {
            MySqlConnection conectar = new MySqlConnection("server=localhost;database=tcc; Uid=root; pwd=Thiago145698;");
            MySqlCommand select = new MySqlCommand();
            select.Connection = conectar;
            select.CommandText = "SELECT * FROM comparacao ORDER BY id desc limit 10";
            conectar.Open();
            MySqlDataReader data = select.ExecuteReader();
            List<Historico> historico = new List<Historico>();

            while (data.Read())
            {
                String deltaAux = Convert.ToString(data["deltae2000"]).Replace(".", ",");
                double delta = Convert.ToDouble(deltaAux);
                
                String r1Aux = Convert.ToString(data["r1"]).Replace(".", ",");
                String g1Aux = Convert.ToString(data["g1"]).Replace(".", ",");
                String b1Aux = Convert.ToString(data["b1"]).Replace(".", ",");
                double r1 = Convert.ToDouble(r1Aux);
                double g1 = Convert.ToDouble(g1Aux);
                double b1 = Convert.ToDouble(b1Aux);
                ColorRGB rgb1Aux = new ColorRGB(r1, g1, b1);

                String r2Aux = Convert.ToString(data["r2"]).Replace(".", ",");
                String g2Aux = Convert.ToString(data["g2"]).Replace(".", ",");
                String b2Aux = Convert.ToString(data["b2"]).Replace(".", ",");
                double r2 = Convert.ToDouble(r2Aux);
                double g2 = Convert.ToDouble(g2Aux);
                double b2 = Convert.ToDouble(b2Aux);
                ColorRGB rgb2Aux = new ColorRGB(r2, g2, b2);

                Historico aux = new Historico(rgb1Aux, rgb2Aux, delta);

                historico.Add(aux);
            }

            return historico;
        }

        public static void INSERTCor(ColorLAB lab, ColorRGB rgb, String nome)
        {
                MySqlConnection conectar = new MySqlConnection("server=localhost;database=tcc; Uid=root; pwd=Thiago145698;");
                conectar.Open();

                MySqlCommand Inserir = new MySqlCommand();
                Inserir.Connection = conectar;
                Inserir.CommandText =
                    "INSERT INTO pantonecoated ( l , a , b , red , green , blue, pantonename) VALUES " +
                    "('" + Convert.ToString(lab.L) + "' , '" + Convert.ToString(lab.A) + "' , '" + Convert.ToString(lab.B) + "' , '"
                         + Convert.ToString(rgb.R) + "' , '" + Convert.ToString(rgb.G) + "' , '" + Convert.ToString(rgb.B) + "' , '"
                         + nome + "');";
            
                Inserir.ExecuteNonQuery();
                conectar.Close();

            }
        }
}
