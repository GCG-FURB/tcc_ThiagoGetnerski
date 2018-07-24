using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace i1Sharp
{
    /// <summary>
    /// Interaction logic for CadastroDeCor.xaml
    /// </summary>
    public partial class CadastroDeCor : Window
    {
        public CadastroDeCor()
        {
            InitializeComponent();
        }

        ColorLAB LAB { get; set; }
        ColorRGB RGB { get; set; }

        public CadastroDeCor(double l, double a, double b, double r, double g, double b2) : this()
        {
            this.LAB = new ColorLAB(Math.Round(l, 2), Math.Round(a, 2), Math.Round(b, 2));
            TextBoxLAB.Text = LAB.ToString();
            this.RGB = new ColorRGB(Convert.ToInt32(r), Convert.ToInt32(g), Convert.ToInt32(b2));
            TextBoxRGB.Text = RGB.ToString();
            SolidColorBrush cor = new SolidColorBrush(Color.FromRgb((byte)RGB.R, (byte)RGB.G, (byte)RGB.B));
            RectangleCor.Fill = cor;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            String nome = TextBoxNome.Text;
            if (nome.Length > 3)
            {
                BancoDeDados.INSERTCor(this.LAB, this.RGB, nome);
                this.Close();
            }
        }
    }
}
