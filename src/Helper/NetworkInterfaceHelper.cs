using System;
using System.Net;
using NetInfo = System.Net.NetworkInformation;
using System.Net.Sockets;

namespace VtnrNetRadioServer.Helper
{
    public class NetworkInterfaceHelper
    {
        public static IPAddress GetLocalIPv4Address()
        {
            foreach (NetInfo.NetworkInterface netif in NetInfo.NetworkInterface.GetAllNetworkInterfaces())
            {

                Console.WriteLine("Network Interface: {0}", netif.Name);
                System.Console.WriteLine("\tNetworkInterfaceType: {0}", netif.NetworkInterfaceType);
#if WINDOWS
                System.Console.WriteLine("\tIsReceiveOnly: {0}", netif.IsReceiveOnly);
#endif
                System.Console.WriteLine("\tOperationalStatus: {0}", netif.OperationalStatus);

                if (netif.OperationalStatus != NetInfo.OperationalStatus.Up)
                {
                    continue;
                }

                if (netif.NetworkInterfaceType != NetInfo.NetworkInterfaceType.Ethernet
                    && netif.NetworkInterfaceType != NetInfo.NetworkInterfaceType.Wireless80211
                )
                {
                    continue;
                }

                NetInfo.IPInterfaceProperties properties = netif.GetIPProperties();
                foreach (NetInfo.IPAddressInformation unicast in properties.UnicastAddresses)
                {
                    Console.WriteLine("\tUniCast: {0}", unicast.Address);
#if WINDOWS
                    Console.WriteLine("\t\tIsTransient: {0}", unicast.IsTransient);              
                    Console.WriteLine("\t\tIsDnsEligible: {0}", unicast.IsDnsEligible);
#endif
                    if (unicast.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return unicast.Address;
                    }
                }
            }
            return IPAddress.Any;
            throw new Exception("no interface with ip found");
        }
    }


}
