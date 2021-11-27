using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using REghZyPacketSystem.Connections.Socketing;
using REghZyPacketSystem.Packeting.Ack;
using REghZyPacketSystem.Systems;
using REghZyPacketSystem.Utils;
using Xamarin.Forms;

namespace PhoneNotifier {
    public partial class MainPage : ContentPage {
        public ThreadPacketNetwork network;

        public ObservableCollection<String> TheMSGList { get; set; }

        public MainPage() {
            InitializeComponent();
            ClassUtils.FindAndInitialisePacketImplementations();
            this.TheMSGList = new ObservableCollection<string>();
            this.BindingContext = this;
        }

        private void SendMessage_Click(object sender, EventArgs e) {
            Log($"Sending: '{this.InputMessageBox.Text}'");
            this.network.EnqueuePacket(new Packet1Chat(this.InputMessageBox.Text));
        }

        public void Log(string msg) {
            this.TheMSGList.Add(msg);
        }

        private async void ConnectToServer_Click(object sender, EventArgs e) {
            Log("Connecting...");
            await Task.Run(async () => {
                this.network = new ThreadPacketNetwork(SocketHelper.MakeConnectionToServer(IPAddress.Parse(this.IpAddressBox.Text), 1440));
            });

            Log("Connected!!!");
            this.network.OnPacketReadFailure += this.Network_OnPacketReadFailure;
            this.network.OnPacketWriteFailure += this.Network_OnPacketWriteFailure;
            network.RegisterHandler<Packet1Chat>((p) => {
                Log($"Received: '{p.Text}'");
                return true;
            });

            this.network.StartThreads();
            Device.StartTimer(TimeSpan.FromMilliseconds(20), () => {
                this.network.ProcessPackets(1);
                return true;
            });
        }

        private void Network_OnPacketWriteFailure(REghZyPacketSystem.Exceptions.PacketWriteException e) {
            this.TheMSGList.Add("Error writing packet! " + e.ToString());
        }

        private void Network_OnPacketReadFailure(REghZyPacketSystem.Exceptions.PacketCreationException e) {
            this.TheMSGList.Add("Error reading packet! " + e.ToString());
        }

        private void ConnectToClient_Click(object sender, EventArgs e) {

        }
    }
}
