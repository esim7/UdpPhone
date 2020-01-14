using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioMessenger
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UdpClient Sender;
        UdpClient Listeniner;
        WaveIn Input;
        WaveOut Output;
        BufferedWaveProvider BufferStream;
        public MainWindow()
        {
            InitializeComponent();
            Input = new WaveIn();
            Input.WaveFormat = new WaveFormat(8000, 16, 1);
            Input.DataAvailable += VoiceInput;
            Output = new WaveOut();
            BufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            Output.Init(BufferStream);

            Task.Run(() => Listening());
        }
        private void VoiceInput(object sender, WaveInEventArgs e)
        {
            Sender = new UdpClient();
            try
            {
                Sender.Send(e.Buffer, e.Buffer.Length, "127.0.0.1", 3231); // у второго пользователя нужно выставить listener на значение этого порта 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Listening()
        {
            Listeniner = new UdpClient(3231); // у второго пользователя нужно выставить client значение этого порта
            IPEndPoint remoteIp = null;
            Output.Play();
            try
            {
                while (true)
                {
                    byte[] data = Listeniner.Receive(ref remoteIp);
                    BufferStream.AddSamples(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Listeniner.Close();
            }
        }

        private void CallClick(object sender, RoutedEventArgs e)
        {
            Input.StartRecording();
        }
    }
}
