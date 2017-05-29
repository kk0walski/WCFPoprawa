using Client.ServiceReference1;
using Client.ServiceReference2;
using System;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Client
{
    /// <summary>
    /// Interaction logic for Picture.xaml
    /// </summary>
    public partial class Picture : Window
    {

        int id;
        string Name;
        DuplexOperationsClient client;
        Service1Client client2;
        InstanceContext instanceContext;
        public Picture(int ID, string name)
        {
            InitializeComponent();
            instanceContext = new InstanceContext(new CallbackBehaviorAttribute());
            client = new DuplexOperationsClient(instanceContext);
            client2 = new Service1Client();
            id = ID;
            this.Name = name;
            PngBitmapDecoder decoder = new PngBitmapDecoder(client2.getPicture(id, Name),
                                                            BitmapCreateOptions.None,
                                                            BitmapCacheOption.OnLoad);
            BitmapSource source = decoder.Frames[0];
            image.Source = source;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(textBox.Text) && string.IsNullOrEmpty(textBox1.Text))
            {
                client.resaizeImage(id, Convert.ToInt32(textBox.Text), Convert.ToInt32(textBox1.Text));
            }
        }
    }
}
