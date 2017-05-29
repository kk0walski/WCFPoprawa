using Client.ServiceReference1;
using Client.ServiceReference2;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using Contract;
using System.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DuplexOperationsClient client;
        Service1Client client2;
        InstanceContext context;
        DuplexOperationsCallback callback;
        public MainWindow()
        {
            InitializeComponent();
            callback = new DuplexOperationsCallback();
            context = new InstanceContext(callback);
            client = new DuplexOperationsClient(context);
            client2 = new Service1Client();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Info info = new Client.Info();
            info.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Select picture";
            openFile.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                              "Portable Network Graphic (*.png)|*.png";
            if(openFile.ShowDialog() == true)
            {
                string nazwaPliku = Path.GetFileName(openFile.FileName);
                try
                {
                    client2.addFile(File.OpenRead(openFile.FileName));
                    client2.putPicture(client.getID(), Path.GetFileName(openFile.FileName));
                    client.addRecord(openFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "My app", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            this.lstData.Items.Clear();
            client.getAll();
            Thread.Sleep(3000);
            this.lstData.ItemsSource = callback.List;
        }

        private void lstData_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Contract.ListItem myItem = (Contract.ListItem)((ListView)sender).SelectedItem;
            Picture picture = new Picture(myItem.ID, myItem.Name);
            picture.Show();
        }


        private void date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.lstData.Items.Clear();
            client.getLater(date.SelectedDate.Value);
            Thread.Sleep(3000);
            this.lstData.ItemsSource = callback.List;
        }

        //private void ImageRead(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFile = new OpenFileDialog();
        //    openFile.Title = "Select picture";
        //    openFile.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
        //                      "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
        //                      "Portable Network Graphic (*.png)|*.png";
        //    if (openFile.ShowDialog() == true)
        //    {
        //        imgData.Source = new BitmapImage(new Uri(openFile.FileName));
        //    }
        //}
    }

    public class DuplexOperationsCallback : ServiceReference1.IDuplexOperationsCallback
    {
        private List<ListItem> _list;

        public List<ListItem> List
        {
            get { return _list; }
            set { _list = value; }
        }

        public void Kolekcja([MessageParameter(Name = "kolekcja")] List<ListItem> kolekcja1)
        {
            List = kolekcja1;
            MessageBox.Show("Kolekcja zostala wczytana");
        }

        public void Warunek(bool value)
        {
            Console.WriteLine("Warunek: {0}", value);
        }

        public void Wynik(string result)
        {
            Console.WriteLine("Wynik: {0}", result);
        }
    }
}
