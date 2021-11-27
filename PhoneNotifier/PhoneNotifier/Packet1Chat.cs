using REghZyPacketSystem.Packeting;
using REghZyPacketSystem.Streams;

namespace PhoneNotifier {
    [PacketImplementation]
    public class Packet1Chat : Packet {
        public string Text { get; set; }

        public Packet1Chat(string text = null) {
            this.Text = text;
        }

        static Packet1Chat() {
            Register<Packet1Chat>(1, () => new Packet1Chat(null));
        }

        public override ushort GetPayloadSize() {
            return (ushort) this.Text.GetBytesUTF16WL();
        }

        public override void ReadPayLoad(IDataInput input, ushort length) {
            this.Text = PacketUtils.ReadStringUTF16WL(input);
        }

        public override void WritePayload(IDataOutput output) {
            PacketUtils.WriteUTF16WL(this.Text, output);
        }
    }
}
