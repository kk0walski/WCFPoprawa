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
            Uri baseAddress = new Uri("http://localhost:10009");
            Uri baseAddress2 = new Uri("net.tcp://localhost:20009");

            ServiceHost mojHost = new ServiceHost(typeof(DuplexOperations), baseAddress);
            ServiceHost mojHost2 = new ServiceHost(typeof(Service1), baseAddress2);
            ServiceEndpoint endpoint =
                mojHost.AddServiceEndpoint(typeof(IDuplexOperations),
                new WSDualHttpBinding(), "DuplexOperationService");
            NetTcpBinding b = new NetTcpBinding();
            b.TransferMode = TransferMode.Streamed;
            b.MaxReceivedMessageSize = 10000000;
            ServiceEndpoint endpoint2 = mojHost2.AddServiceEndpoint(typeof(IService1), b, "Strumien");
            ServiceMetadataBehavior smb2 = new ServiceMetadataBehavior();
            smb2.HttpGetUrl = new Uri("http://localhost:20109");
            smb2.HttpGetEnabled = true;
            mojHost2.Description.Behaviors.Add(smb2);
            ServiceDebugBehavior debug2 = mojHost2.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (debug2 == null)
            {
                mojHost2.Description.Behaviors.Add(
                        new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            }
            else
            {
                if (!debug2.IncludeExceptionDetailInFaults)
                {
                    debug2.IncludeExceptionDetailInFaults = true;
                }
            }
            //Metadane
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            mojHost.Description.Behaviors.Add(smb);
            ServiceDebugBehavior debug = mojHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            if(debug == null)
            {
                mojHost.Description.Behaviors.Add(
                        new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            }
            else
            {
                if (!debug.IncludeExceptionDetailInFaults)
                {
                    debug.IncludeExceptionDetailInFaults = true;
                }
            }
            try
            {
                mojHost.Open();
                Console.WriteLine("Serwis 1 jest uruchomiony");
                mojHost2.Open();
                Console.WriteLine("Serwis 2 jest uruchomiony");
                Console.WriteLine("Nacisnij <ENTER> aby zakonczyc");
                Console.WriteLine();
                Console.ReadLine();
                mojHost.Close();
                mojHost2.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("Wystapil wyjatek: {0}", ce.Message);
                mojHost.Abort();
            }
        }
    }
}
