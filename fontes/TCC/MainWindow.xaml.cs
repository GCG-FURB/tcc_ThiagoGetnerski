using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace i1Sharp
{
    
    internal partial class MainWindow : Window
    {
        private I1SharpModel model;

        private ColorLAB lab1;
        private ColorLAB lab2;
        
        private ColorRGB rgb1;
        private ColorRGB rgb2;

        private ColorCMYK cmyk1;
        private ColorCMYK cmyk2;

        private double deltaE2000;
        private double deltaE1994;
        private double deltaE1976;

        private List<ColorPantone> listaDePantones;
        private List<Historico> historico;


        private ObservableCollection<Historico> GetEmployeeList()
        {

            this.historico = BancoDeDados.SELECTHistorico();


            return new ObservableCollection<Historico>(historico);
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgEmp.ItemsSource = GetEmployeeList();
        }
        

        public MainWindow (I1SharpModel model)
        {
            this.model = model;
            InitializeComponent ();

            this.devicesList.DataContext = this.model;

            this.model.DeviceChanged += model_DeviceChanged;

            this.listaDePantones = BancoDeDados.SELECTPantone();
            
            logBox.Document.LineHeight = 1;
            logBox.Document.Blocks.Clear ();

            I1Pro.LogEvent += I1Pro64_LogEvent;
        }
        
        public void I1Pro64_LogEvent (I1Pro.LogType type, string logEntry)
        {
            if (logEntry.Length == 0)
            {
                return;
            }
            this.Dispatcher.BeginInvoke ((Action)(() =>
            {
                Paragraph p = new Paragraph (new Run (logEntry));
                switch (type)
                {
                    case I1Pro.LogType.eNormal: break;
                    case I1Pro.LogType.eError: p.Foreground = Brushes.Crimson; break;
                }
                logBox.Document.Blocks.Add (p);
                logBox.ScrollToEnd ();
            }));
        }
        
        void model_DeviceChanged (I1Pro device)
        {
            if (device != null)
            {
                device.ButtonPressed += device_ButtonPressed;
                I1Pro64_LogEvent (I1Pro.LogType.eNormal, "Dispositivo alterado - dispositivo ativo: " + device.Name);
            }
            else
            {
                I1Pro64_LogEvent (I1Pro.LogType.eNormal, "Dispositivo alterado - nenhum dispositivo ativo");
            }
        }
        
        private void calibrateButton_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                using (TemporaryCursor temporaryCursor = new TemporaryCursor (this, Cursors.Wait))
                {
                    model.CurrentDevice.Calibrate ();
                }
            }
            catch (Exception ex)
            {
                I1Pro64_LogEvent(I1Pro.LogType.eNormal, "Dispositivo desconectado");
            }
        }
 
        private void device_ButtonPressed (I1Pro device)
        {
            try
            {
                model.CurrentDevice.TriggerMeasurement (I1Pro.MeasurementModeType.eReflectanceScan);

                this.Dispatcher.BeginInvoke ((Action) (() =>
                {
                    Medicao = model.CurrentDevice.Samples;
                    var firstElement = Medicao[0];
                    ColorLAB aux = new ColorLAB(firstElement[0], firstElement[1], firstElement[2]);
                    popular(aux);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.Message, "Erro durante a medição", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void popular(ColorLAB aux)
        {
            if(lab1 == null)
            {
                lab1 = aux;
                rgb1 = Conversao.LabToRgb(lab1);
                cmyk1 = Conversao.RGBToCMYK(rgb1);

                PrimeiraCor.Fill = new SolidColorBrush(Color.FromRgb((byte)rgb1.R,
                                                                     (byte)rgb1.G,
                                                                     (byte)rgb1.B));

                TextBoxLAB1.Text = lab1.ToString();
                TextBoxRGB1.Text = rgb1.ToString();
                TextBoxCMYK1.Text = cmyk1.ToString();
                List<ColorPantone> ordenado = listaOrdenada(this.listaDePantones, this.lab1);
                PantoneProximo1.Fill = new SolidColorBrush(Color.FromRgb((byte)ordenado[0].RGB.R,
                                                                         (byte)ordenado[0].RGB.G,
                                                                         (byte)ordenado[0].RGB.B));

                TextBoxPantone1.Text = ordenado[0].Nome;
                TextBoxPantone1Delta.Text = "" + Math.Round(ordenado[0].DeltaE, 2);
                ButtonSalvarCor.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                lab2 = aux;
                rgb2 = Conversao.LabToRgb(lab2);
                cmyk2 = Conversao.RGBToCMYK(rgb2);

                SegundaCor.Fill = new SolidColorBrush(Color.FromRgb((byte)rgb2.R,
                                                                    (byte)rgb2.G,
                                                                    (byte)rgb2.B));

                TextBoxLAB2.Text = lab2.ToString();
                TextBoxRGB2.Text = rgb2.ToString();
                TextBoxCMYK2.Text = cmyk2.ToString();
                List<ColorPantone> ordenado = listaOrdenada(this.listaDePantones, this.lab2);
                PantoneProximo2.Fill = new SolidColorBrush(Color.FromRgb((byte)ordenado[0].RGB.R,
                                                                         (byte)ordenado[0].RGB.G,
                                                                         (byte)ordenado[0].RGB.B));

                TextBoxPantone2.Text = ordenado[0].Nome;
                TextBoxPantone2Delta.Text = "" + Math.Round(ordenado[0].DeltaE, 2);

                deltaE2000 = DeltaE00.DeltaE2000(lab1, lab2);
                TextBoxDeltaE2000.Text = "" + Math.Round(deltaE2000, 4);
                deltaE1994 = DeltaE94.DeltaE1994(lab1, lab2);
                TextBoxDeltaE1994.Text = "" + Math.Round(deltaE1994, 4);
                deltaE1976 = DeltaE76.DeltaE1976(lab1, lab2);
                TextBoxDeltaE1976.Text = "" + Math.Round(deltaE1976, 4);
            }
        }

        private static readonly DependencyProperty MeasurementsProperty = DependencyProperty.Register ("Measurements", typeof (List<float []>), typeof (MainWindow));
        
        public List<float []> Medicao  {
            private set {
                SetValue (MeasurementsProperty, value);
            }

            get {
                return (List <float []>)GetValue (MeasurementsProperty);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BancoDeDados.INSERTComparacao(rgb1, rgb2, deltaE2000);
            dgEmp.ItemsSource = GetEmployeeList();
        }

        public List<ColorPantone> listaOrdenada(List<ColorPantone> entrada, ColorLAB lab)
        {
            List<ColorPantone> retorno = new List<ColorPantone>();

            for (int i = 0; i < entrada.Count; i++)
            {
                ColorPantone cor = entrada[i];
                cor.DeltaE = DeltaE00.DeltaE2000(cor.LAB, lab);
                retorno.Add(cor);
            }

            retorno = retorno.OrderBy(o => o.DeltaE).ToList();
            return retorno;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            limpar();
        }

        public void limpar()
        {
            lab1 = null;
            lab2 = null;
            rgb1 = null;
            rgb2 = null;
            cmyk1 = null;
            cmyk2 = null;

            PrimeiraCor.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            SegundaCor.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            TextBoxLAB1.Text = "";
            TextBoxLAB2.Text = "";
            TextBoxRGB1.Text = "";
            TextBoxRGB2.Text = "";
            TextBoxCMYK1.Text = "";
            TextBoxCMYK2.Text = "";

            PantoneProximo1.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            PantoneProximo2.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            TextBoxPantone1.Text = "";
            TextBoxPantone2.Text = "";
            TextBoxPantone1Delta.Text = "";
            TextBoxPantone2Delta.Text = "";
            
            TextBoxDeltaE2000.Text = "";
            TextBoxDeltaE1994.Text = "";
            TextBoxDeltaE1976.Text = "";
            
            ButtonSalvarCor.Visibility = System.Windows.Visibility.Hidden;
        }
        
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var cadastroDeCor = new CadastroDeCor(lab1.L, lab1.A, lab1.B, rgb1.R, rgb1.G, rgb1.B);
            cadastroDeCor.ShowDialog();
        }
    }
}