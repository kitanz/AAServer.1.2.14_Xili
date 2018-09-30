using LocalCommons.Cryptography;
using System;
using System.Linq;

namespace LocalCommons.Network
{
    /// <summary>
    /// Abstract Class For Writing Packets
    /// Author: Raphail
    /// </summary>
    public abstract class NetPacket
    {
        protected PacketWriter ns;
        /// <summary>
        /// The opcode
        /// </summary>
        private readonly int m_packetId;
        /// <summary>
        /// Low bytes first
        /// </summary>
        private readonly bool m_littleEndian;
        /// <summary>
        /// The packet from the server
        /// </summary>
        private bool m_IsArcheAge;
        /// <summary>
        /// The level of compression/encryption
        /// </summary>
        private readonly byte level;
        //Fix by Yanlong-LI
        /// <summary>
        /// Global packet counting DD05
        /// </summary>
        //The correction input of the second user, the secondary login, reconnection counter with return to lobby, due to an error
        public static byte NumPckSc = 0;  // BUG global packet countin DD05
        public static sbyte NumPckCs = -1; //global packet counting 0005

        /// <summary>
        /// The package from the server/client
        /// </summary>
        public bool IsArcheAgePacket
        {
            get { return m_IsArcheAge; }
            set { m_IsArcheAge = true; }
        }

        /// <summary>
        /// Creates Instance Of Any Other Packet
        /// </summary>
        /// <param name="packetId">Packet Identifier(opcode)</param>
        /// <param name="isLittleEndian">Send Data In Little Endian Or Not.</param>
        protected NetPacket(int packetId, bool isLittleEndian)
        {
            this.m_packetId = packetId;
            this.m_littleEndian = isLittleEndian;
            ns = PacketWriter.CreateInstance(4092, isLittleEndian);
        }

        /// <summary>
        /// Creates Instance Of ArcheAge Game Packet.
        /// </summary>
        /// <param name="level">Packet Level</param>
        /// <param name="packetId">Packet Identifier(opcode)</param>
        protected NetPacket(byte level, int packetId)
        {
            this.m_packetId = packetId;
            this.level = level;
            this.m_littleEndian = true;
            this.m_IsArcheAge = true;
            ns = PacketWriter.CreateInstance(4092, true);
        }

        /// <summary>
        /// Stream Where We Writing Data.
        /// </summary>
        public PacketWriter UnderlyingStream
        {
            get { return ns; }
        }

        /// <summary>
        /// Compiles Data And Return Compiled byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] Compile()
        {
            PacketWriter temporary = PacketWriter.CreateInstance(4096 * 4, m_littleEndian);
            //temporary.Write((short)(ns.Length + (m_IsArcheAge ? 6 : 2)));
            if (m_IsArcheAge)
            {
                //Серверные пакеты
                if (level == 5)
                {
                    //здесь будет шифрование пакета DD05
                    temporary.Write((short)(ns.Length + 6));

                    temporary.Write((byte)0xDD);
                    temporary.Write((byte)level);

                    //TODO: Check,Maybe can be rewritten better?
                    byte[] numPck = new byte[1];
                    numPck[0] = NumPckSc; //put the package number in the array
                    byte[] data = numPck.Concat(BitConverter.GetBytes((short)m_packetId)).ToArray(); //combined with ID
                                       data = data.Concat(ns.ToArray()).ToArray(); //combined with the body of the package
                    byte crc8 = Encrypt.Crc8(data); //considered CRC package
                    byte[] crc = new byte[1];
                    crc[0] = crc8; //put the crc in the array
                    data = crc.Concat(data).ToArray(); //added front checksum
                    byte[] encrypt = Encrypt.StoCEncrypt(data); //encrypted packet
                    temporary.Write(encrypt, 0, encrypt.Length);
                    ++NumPckSc; //the next number of the encrypted package DD05
                }
                else
                {
                    temporary.Write((short)(ns.Length + 4));

                    temporary.Write((byte)0xDD);
                    temporary.Write((byte)level);
                    temporary.Write((short)m_packetId);

                    byte[] redata = ns.ToArray();
                    temporary.Write(redata, 0, redata.Length);
                }
            }
            else
            {
                temporary.Write((short)(ns.Length + 2));
                temporary.Write((short)m_packetId);
                byte[] redata = ns.ToArray();
                temporary.Write(redata, 0, redata.Length);
            }
            PacketWriter.ReleaseInstance(ns);
            ns = null;
            byte[] compiled = temporary.ToArray();
            PacketWriter.ReleaseInstance(temporary);
            temporary = null;

            return compiled;
        }
        /// <summary>
        /// Compiles Data And Return Compiled byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] Compile0()
        {
            PacketWriter temporary = PacketWriter.CreateInstance(4096 * 4, m_littleEndian);
            temporary.Write((short)(ns.Length + (m_IsArcheAge ? 4 : 2)));
            if (m_IsArcheAge)
            {
                temporary.Write((byte)0xDD);
                temporary.Write((byte)level);
                temporary.Write((short)m_packetId);
            }
            else
            {
                temporary.Write((short)m_packetId);
            }

            byte[] redata = ns.ToArray();
            PacketWriter.ReleaseInstance(ns);
            ns = null;
            temporary.Write(redata, 0, redata.Length);
            byte[] compiled = temporary.ToArray();
            PacketWriter.ReleaseInstance(temporary);
            temporary = null;
            return compiled;
        }
        /// <summary>
        /// Compiles Data And Return Compiled byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] Compile2()
        {
            PacketWriter temporary = PacketWriter.CreateInstance(4096 * 4, m_littleEndian);

            byte[] redata = ns.ToArray();
            PacketWriter.ReleaseInstance(ns);
            ns = null;
            temporary.Write(redata, 0, redata.Length);
            byte[] compiled = temporary.ToArray();
            PacketWriter.ReleaseInstance(temporary);
            temporary = null;
            return compiled;
        }
    }
}
