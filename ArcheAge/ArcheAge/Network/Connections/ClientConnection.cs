using ArcheAge.ArcheAge.Structuring;
using LocalCommons.Logging;
using LocalCommons.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using LocalCommons.Cryptography;
using LocalCommons.Utilities;

namespace ArcheAge.ArcheAge.Network.Connections
{

    /// <summary>
    /// Connection That Used For ArcheAge Client( Game Side )
    /// </summary>
    public class ClientConnection : IConnection
    {
        //----- Static
        private static Dictionary<int, Account> m_CurrentAccounts = new Dictionary<int, Account>();
        private readonly byte _mRandom;
        //Fix by Yanlong-LI
        //The correction input of the second user, the secondary login, the counter re-connect with return to lobby due to an error
        public byte NumPck = 0;  // BUG global packet counting DD05
        public static Dictionary<int, Account> CurrentAccounts
        {
            get { return m_CurrentAccounts; }
        }

        public Account CurrentAccount { get; set; }

        public ClientConnection(Socket socket) : base(socket)
        {
            Logger.Trace("Client IP: {0} connected", this);
            DisconnectedEvent += ClientConnection_DisconnectedEvent;
            m_LittleEndian = true;
        }

        public override void SendAsync(NetPacket packet)
        {
            packet.IsArcheAgePacket = true;
            //Fix by Yanlong-LI
            //Override the counter for the current connection
            NetPacket.NumPckSc = NumPck;//Rewrite to the count of the current connection
            base.SendAsync(packet);
            //Write the counter back
            NumPck = NetPacket.NumPckSc;//Write back to the count
        }
        public void SendAsyncd(NetPacket packet)
        {
            packet.IsArcheAgePacket = false;
            base.SendAsync(packet);
        }
        void ClientConnection_DisconnectedEvent(object sender, EventArgs e)
        {
            Logger.Trace("Client IP: {0} disconnected", this);
            Dispose();
        }

        public override void HandleReceived(byte[] data)
        {
            PacketReader reader = new PacketReader(data, 0);
            //reader.Offset += 1; //Undefined Random Byte
            byte seq = reader.ReadByte();
            byte header = reader.ReadByte(); //Packet Level
            ushort opcode = reader.ReadLEUInt16(); //Packet Opcode
            //we process packages from the client
            if (header == 0x05 && seq == 00)
            {
                reader.Offset -= 2; //back to hash, count
                byte hash = reader.ReadByte(); //read hash or CRC (it does not change)
                byte count = reader.ReadByte(); //read count (encryption changes)
                //------------------------------
                //Remove switch in the future
                switch (hash)
                {
                    case 0x33:
                        opcode = 0x008E; //login to the game 5
                        break;
                    case 0x34:
                        opcode = 0x0088; //packet for relogin from the lobby
                        break;
                    //case 0x35:
                    //    opcode = 0x0088; //relogin package from the game
                    //    break;
                    case 0x36:
                        opcode = 0x008F; //entering game6
                        break;
                    case 0x37:
                        opcode = 0x008B; //entering game2
                        break;
                    case 0x38:
                        opcode = 0x008A; //entering game1
                        break;
                    case 0x39:
                        opcode = 0x008C; //entering game3
                        break;
                    case 0x3F:
                        opcode = 0x008D; //entering game4
                        break;
                        //default:
                        //    msg = "";
                        //    break;
                }
                //------------------------------
                //unpacking the packets from the client 
                //here is the decryption of the package 0005

                ////trying to decode
                ////byte[] ciphertext = new byte[] {0x13, 0x00, 0x00, 0x05, 0x39, 0x96, 0x41, 0x57, 0x3F, 0x2C, 0xEF, 0x75, 0x3E, 0xC8, 0xC1, 0x75, 0xB5, 0xD2, 0x10, 0x43, 0x62};
                //var pck = new CtoSDecrypt();

                //var size = BitConverter.GetBytes((short) data.Length);
                //data = size.Concat(data).ToArray(); //pool с Size

                //var ciphertext = Encrypt.CtoSEncrypt(data);
                //Logger.Trace("EncodeXOR:      " + Utility.ByteArrayToString(ciphertext));
                //var plaintext = pck.DecryptAes2(ciphertext);
                //Logger.Trace("EncodeAES:      " + Utility.ByteArrayToString(plaintext));
                ////"13 00 00 05" lost
                ////"16 42|50 00 01 00|D7 94 01 00|3D A5 00 00|A4 3E A5" receieved
                ////reader.Offset += 2; //passhash&count
                //Buffer.BlockCopy(plaintext, 2, data, 0, 2); //reader.ReadLEUInt16(); //Packet Opcode
                //opcode = (ushort) BitConverter.ToInt16(plaintext, 2);
            }

            if (!DelegateList.ClientHandlers.ContainsKey(header))
            {
                Logger.Trace("Received undefined packet - seq: {0}, header: {1}, opcode: 0x{2:X2}", seq, header, opcode);
                return;
            }
            try
            {
                PacketHandler<ClientConnection> handler = DelegateList.ClientHandlers[header][opcode];
                if (handler != null)
                {
                    handler.OnReceive(this, reader);
                }
                else
                {
                    Logger.Trace("Received undefined packet - seq: {0}, header: {1}, opcode: 0x{2:X2}", seq, header, opcode);
                }
            }
            catch (Exception)
            {
                Logger.Trace("Received undefined packet - seq: {0}, header: {1}, opcode: 0x{2:X2}", seq, header, opcode);
                throw;
            }
        }
    }
}
