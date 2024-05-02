using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocked { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client)
        {
            ClientSocked = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocked.GetStream());

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"({DateTime.Now}): Client has connecther with the username: {Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while(true) 
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message recieved! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            break;
                        default:
                            break;
                    }
                } 
                catch( Exception e )
                {
                    Console.WriteLine($"[{UID.ToString()}]: Dissconected");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocked.Close();
                    break;
                }
            }
        }
    }
}
