using System;
using System.Net.Sockets;
using System.Threading;
using REghZyPacketSystem.Connections.Socketing;
using REghZyPacketSystem.Systems;

namespace PhoneNotifier.DesktopApp {
    class Program {
        static void Main(string[] args) {
            Socket server = SocketHelper.CreateServerSocket(1440);
            server.Listen(10);
            ThreadPacketNetwork network = new ThreadPacketNetwork(SocketHelper.AcceptClientConnection(server));
            network.StartThreads();
            new Thread(() => {
                while(true) {
                    network.ProcessPackets(1);
                    Thread.Sleep(50);
                }
            }).Start();

            network.RegisterHandler<Packet1Chat>((p) => {
                Console.WriteLine($"Received: '{p.Text}'");
                return true;
            });

            while(true) {
                Console.Write("Send: ");
                network.EnqueuePacket(new Packet1Chat(Console.ReadLine()));
            }
        }
    }
}
