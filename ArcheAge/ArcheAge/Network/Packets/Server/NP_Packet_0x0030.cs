using LocalCommons.Network;

namespace ArcheAge.ArcheAge.Network
{
    public sealed class NP_Packet_0x0030 : NetPacket
    {
        /// <summary>
        /// the package for the entrance to the Lobby
        /// author: NLObP
        /// </summary>
        public NP_Packet_0x0030() : base(05, 0x0030)
        {
            //packages to enter the Lobby
            /*
             * [17]            S>c             0ms.            18:52:06 .528      06.07.18
             -------------------------------------------------------------------------------
              TType: ArcheageServer: GS1     Parse: 6           EnCode: off         
             ------- 0  1  2  3  4  5  6  7 -  8  9  A  B  C  D  E  F    -------------------
             the package is not defined  (size=$0B/11)
             000000 09 00 DD 05 2C B9 0D 53 | 11 42 70                    ..Ý.,¹.S.Bp
             */
            //ns.WriteHex(
            //"0900DD052CB90D53114270");
            //"0900DD05CC098C02" +
            //    "010200");
            //decrypted data from sniff package
            //3.0.0.7
            // size hash crc idx opcode data
            //"0900 DD05 CC  09  8C02   01 02 00"
            //3.0.3.0
            //"0900 DD05 1A  09  3000   010200"

            //ingameShopVersion c
            //01
            ns.Write((byte)0x01);
            //secondPriceType c
            //02
            ns.Write((byte)0x02);
            //askBuyLaborPowerPotion c
            //00
            ns.Write((byte)0x00);
        }
    }
}
