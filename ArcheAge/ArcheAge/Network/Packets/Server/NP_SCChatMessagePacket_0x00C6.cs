using ArcheAge.ArcheAge.Network.Connections;
using LocalCommons.Network;
using LocalCommons.Utilities;

namespace ArcheAge.ArcheAge.Network
{
    public sealed class NP_SCChatMessagePacket_0x00C6 : NetPacket
    {
        public NP_SCChatMessagePacket_0x00C6(ClientConnection net, short chatId, string msg, string msg2) : base(01, 0x00C6)
        {
            //1.0.1406
            //SCChatMessagePacket
            //                chat_id -2                                                          W e l c o m e !
            // 2D00 DD01 C600 FEFF         0000 00000000 000000 00000000 00 00 00000000 0000 0800 57656C636F6D6521 0000000000000000
            /*
             * chat_id
             * The answer is -5
               Whisper -3
               the system is -2 ???
                       - 1 ???
               Near 0
               Scream 1
               Trade 2
               The search unit 3
               Unit 4
               RAID 5
               Fraction 6
               Guild 7
               Family 9
               Chapter RAID 10
               Court 11
               Role play 13
               Union 14
             */

            ns.Write((short)chatId); //chat_id h
            ns.Write((short)0x00); //unk h
            ns.Write((int)0x00);   //chat_obj d
            ns.Write((Uint24)0x00);//gameObjectId d3
            ns.Write((int)0x00);   //objectId d
            ns.Write((byte)0x00);  //LanguageType c
            //ns.Write((byte)net.CurrentAccount.Character.CharRace);  //CharRace c
            ns.Write((byte)0x00);  //CharRace c
            ns.Write((int)0x00);   //type d
            ns.WriteUTF8Fixed(msg, msg.Length);   //name SS
            ns.WriteUTF8Fixed(msg2, msg2.Length); //name SS
            ns.Write((int)0x00);   //ability d
            ns.Write((int)0x00);   //option d
        }
    }
}