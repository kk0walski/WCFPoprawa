﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.ServiceModel;
using System.Threading;

namespace Contract
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        const string PATH = @"C:\Users\karol\documents\visual studio 2015\Projects\WCFPoprawa\Server\Pictures";

        public void putPicture(int id, string nazwa)
        {
            string path = Path.Combine(PATH, "plik.jpg");
            string path2 = Path.Combine(PATH, Convert.ToString(id), nazwa);
            string dir = Path.GetDirectoryName(path2);

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.Copy(path, path2, true);
        }
        public void addFile(Stream picture)
        {
            string path = Path.Combine(PATH, "plik.jpg");
            
            try
            { 
                savePicture(picture, path);
                return;
            }
            catch (IOException ex)
            {
                Console.WriteLine(String.Format("Wyjatek otwarcia pliku {0}", path));
                Console.WriteLine("Wyjatek");
                Console.WriteLine(ex.ToString());
                throw ex;
            }catch (Exception ex)
            {
                throw new System.ServiceModel.FaultException(ex.Message);
            }
        }

        private void savePicture(Stream instream, string filepath)
        {
            
            const int bufferLength = 4098;  //dlugosc bufora
            int bytecount = 0;              //licznik
            int counter = 0;                //licznik pomocniczy

            byte[] buffer = new byte[bufferLength];
            FileStream outstream = File.Open(filepath, FileMode.Create, FileAccess.Write);

            //zapisysanie danych porcjami
            while ((counter = instream.Read(buffer, 0, bufferLength)) > 0)
            {
                outstream.Write(buffer, 0, counter);
                Console.Write(".{0}", counter);
                bytecount += counter;
            }
            Console.WriteLine();
            Console.WriteLine("Zapisano {0} bajtow", bytecount);

            outstream.Close();
            instream.Close();
            Console.WriteLine();
            Console.WriteLine("Plik {0} zapisany.", filepath);
        }

        public Stream getPicture(string path)
        {
            try
            {
                FileStream imageFile = File.OpenRead(path);
                return imageFile;
            }catch(IOException ex)
            {
                Console.WriteLine(String.Format("Wyjatek otwarcia pliku {0}",
                                 path));
                Console.WriteLine("Wyjatek");
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        public Stream getPicture(int ID, string name)
        {
            string path = Path.Combine(PATH, Convert.ToString(ID), name);
            try
            {
                FileStream imageFile = File.OpenRead(path);
                return imageFile;
            }
            catch (IOException ex)
            {
                Console.WriteLine(String.Format("Wyjatek otwarcia pliku {0}",
                                                 path));
                Console.WriteLine("Wyjatek");
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class DuplexOperations : IDuplexOperations
    {
        const string PATH = @"C:\Users\karol\documents\visual studio 2015\Projects\WCFPoprawa\Server\Pictures";
        int id = 0;
        string nazwa;
        List<ImageData> obrazy;
        List<ListItem> lista;
        IDuplexOperationsCallback callback = null;

        public DuplexOperations()
        {
            lista = new List<ListItem>();
            obrazy = new List<ImageData>();
            callback = OperationContext.Current.
                GetCallbackChannel<IDuplexOperationsCallback>();
        }
        public void addRecord(string plik)
        {
            string path = Path.Combine(PATH, Convert.ToString(id), Path.GetFileName(plik));
            Image image = Image.FromFile(plik);
            FileInfo info = new FileInfo(plik);
            ImageData data = new ImageData(id, image.Width, image.Height, info.Extension, info.Name, info.CreationTime, info.Length);
            data.Widht = image.Width;
            data.Height = image.Height;
            obrazy.Add(data);
            id++;
        }

        public void getAll()
        {
            lista.Clear();
            foreach (ImageData data in obrazy)
            {
                lista.Add(new ListItem(data.ID, data.Name, data.Date, data.Size));
            }
            Thread.Sleep(10000);
            callback.Kolekcja(lista);
        }

        public void getLater(DateTime data)
        {
            lista.Clear();
            foreach (ImageData dane in obrazy)
            {
                if (dane.Date >= data)
                {
                    ListItem temp = new ListItem(dane.ID, dane.Name, dane.Date, dane.Size);
                    temp.ID = dane.ID;
                    temp.Name = dane.Name;
                    temp.Data = dane.Date;
                    temp.Size = dane.Size;
                    lista.Add(temp);
                }
            }
            Thread.Sleep(10000);
            callback.Kolekcja(lista);
        }

        public void getName(string name)
        {
            List<ListItem> wynik = new List<ListItem>();
            foreach (ImageData dane in obrazy)
            {
                if (dane.Name.Equals(name))
                {
                    wynik.Add(new ListItem(dane.ID, dane.Name, dane.Date, dane.Size));
                }
            }
            Thread.Sleep(10000);
            callback.Kolekcja(wynik);
        }

        public string getPath(string name)
        {
            foreach (ImageData dane in obrazy)
            {
                if (dane.Name.Equals(name))
                {
                    return Path.Combine(PATH, Convert.ToString(dane.ID), dane.Name);
                }
            }
            return null;
        }

        public string getPathFromId(int id)
        {
            return Path.Combine(PATH, Convert.ToString(obrazy[id].ID), obrazy[id].Name + "." + obrazy[id].Extension);
        }

        public void getSize()
        {
            long wynik = 0;
            foreach (ImageData dane in obrazy)
            {
                wynik += dane.Size;
            }
        }

        public void inSet(string namePicture)
        {
            bool wynik = false;
            foreach (ImageData dane in obrazy)
            {
                if (dane.Name.Equals(namePicture))
                {
                    wynik = true;
                    break;
                }
            }
        }

        public void resaizeImage(int id, int width, int height)
        {
            string path = Path.Combine(PATH, Convert.ToString(obrazy[id].ID), obrazy[id].Name + "." + obrazy[id].Extension);
            Image img = Image.FromFile(path);
            Image newImage = Resize(img, width, height);
            File.Delete(path);
            newImage.Save(path);
        }

        private Image Resize(Image originalImage, int w, int h)
        {
            //Original Image attributes
            int originalWidth = originalImage.Width;
            int originalHeight = originalImage.Height;

            // Figure out the ratio
            double ratioX = (double)w / (double)originalWidth;
            double ratioY = (double)h / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            Image thumbnail = new Bitmap(newWidth, newHeight);
            Graphics graphic = Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            graphic.Clear(Color.Transparent);
            graphic.DrawImage(originalImage, 0, 0, newWidth, newHeight);

            return thumbnail;
        }

        public int getID()
        {
            return id;
        }

        public void setNazwa(string name)
        {
            this.nazwa = name;
        }

        public ImageData getImageData(int id)
        {
            return obrazy[id];
        }
    }
}
