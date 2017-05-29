using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Contract
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        void putPicture(int id, string nazwa);
        [OperationContract]
        Stream getPicture(int id, string name);
        [OperationContract]
        void addFile(Stream picture);
    }

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDuplexOperationsCallback))]
    public interface IDuplexOperations
    {
        [OperationContract(IsOneWay = true)]
        void getLater(DateTime data);

        [OperationContract(IsOneWay = true)]
        void addRecord(string nazwa);
        [OperationContract(IsOneWay = true)]
        void getAll();

        [OperationContract(IsOneWay = true)]
        void inSet(string namePicture);
        [OperationContract]
        string getPathFromId(int id);
        [OperationContract(IsOneWay = true)]
        void getSize();
        [OperationContract(IsOneWay = true)]
        void resaizeImage(int id, int width, int height);
        [OperationContract]
        string getPath(string name);
        [OperationContract(IsOneWay = true)]
        void getName(string name);
        [OperationContract]
        int getID();
        [OperationContract]
        void setNazwa(string name);
        [OperationContract]
        ImageData getImageData(int id);
    }

    [ServiceContract]
    public interface IDuplexOperationsCallback
    {
        [OperationContract(IsOneWay = true)]
        void Wynik(string result);
        [OperationContract(IsOneWay = true)]
        void Kolekcja(ListItem[] kolekcja);
        [OperationContract(IsOneWay = true)]
        void Warunek(bool value);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "Contract.ContractType".
    [DataContract]
    public class ImageData
    {
        private int _id;
        private int _width;
        private int _height;
        private string _extenstion;
        private string _name;
        private DateTime _date;
        private long _size;

        public ImageData(int id, int width, int height, string extension, string name, DateTime date, long size)
        {
            ID = id;
            Widht = width;
            Height = height;
            Extension = extension;
            Name = name;
            Date = date;
            Size = size;
        }

        [DataMember]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        [DataMember]
        public int Widht
        {
            get { return _width; }
            set { _width = value; }
        }

        [DataMember]
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DataMember]
        public string Extension
        {
            get { return _extenstion; }
            set { _extenstion = value; }
        }

        [DataMember]
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        [DataMember]
        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }
    }
    [DataContract]
    public class ListItem
    {
        private int _id;
        private string _name;
        private DateTime _date;
        private long _size;

        public ListItem(int id, string name, DateTime date, long size)
        {
            ID = id;
            Name = name;
            Data = date;
            Size = size;
        }
        public int ID {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public DateTime Data
        {
            get { return _date; }
            set { _date = value; }
        }

        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }
    }
}
