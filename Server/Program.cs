using Contract;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost mojHost = new ServiceHost(typeof(DuplexOperations));
            ServiceEndpoint endpoint = mojHost.Description.Endpoints.Find(typeof(Contract.IDuplexOperations));
            ServiceHost mojHost2 = new ServiceHost(typeof(Service1));
            ServiceEndpoint endpoint2 = mojHost2.Description.Endpoints.Find(typeof(Contract.IService1));
            try
            {
                Console.WriteLine("\n--> Endpointy");
                Console.WriteLine("\nSerwise endpoint {0}:", endpoint.Name);
                Console.WriteLine("Binding: {0}", endpoint.Binding.ToString());
                Console.WriteLine("ListenUri: {0}", endpoint.ListenUri.ToString());
                Console.WriteLine("\nSerwise endpoint {0}:", endpoint2.Name);
                Console.WriteLine("Binding: {0}", endpoint2.Binding.ToString());
                Console.WriteLine("ListenUri: {0}", endpoint2.ListenUri.ToString());
                mojHost.Open();
                Console.WriteLine("Serwis 1 jest uruchomiony");
                ContractDescription cd = ContractDescription.GetContract(typeof(IDuplexOperations));
                Console.WriteLine("Informacje o kontrakcie");
                Type contractType = cd.ContractType;
                Console.WriteLine("\tContract type: {0}", contractType.ToString());
                string name = cd.Name;
                Console.WriteLine("\tName: {0}", name);
                OperationDescriptionCollection odc = cd.Operations;
                Console.WriteLine("\tOperacje");
                foreach(OperationDescription od in odc)
                {
                    Console.WriteLine("\t\t" + od.Name);
                }
                mojHost2.Open();
                Console.WriteLine("Serwis 2 jest uruchomiony");
                ContractDescription cd2 = ContractDescription.GetContract(typeof(IService1));
                Console.WriteLine("Informacje o kontrakcie");
                Type contractType2 = cd2.ContractType;
                Console.WriteLine("\tContract type: {0}", contractType2.ToString());
                string name2 = cd2.Name;
                Console.WriteLine("\tName: {0}", name2);
                OperationDescriptionCollection odc2 = cd2.Operations;
                Console.WriteLine("\tOperacje");
                foreach (OperationDescription od2 in odc2)
                {
                    Console.WriteLine("\t\t" + od2.Name);
                }
                Console.WriteLine("Nacisnij <ENTER> aby zakonczyc");
                Console.WriteLine();
                Console.ReadLine();
                mojHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("Wystapil wyjatek: {0}", ce.Message);
                mojHost.Abort();
            }
        }
    }
}
