﻿using LocalCommons.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcheAgeStream.ArcheAge.Network
{
    /// <summary>
    /// Sends Information About Registration Result.
    /// </summary>
    public sealed class NET_TCJoinResponse_0x01 : NetPacket
    {
        public NET_TCJoinResponse_0x01(byte success) : base(0x0001, true)
        {
            ns.Write(success); //connected to the proxy server
        }
    }
    public sealed class NET_TCDoodadStream_0x02 : NetPacket
    {
        public NET_TCDoodadStream_0x02(int id, int next, int count, string msg) : base(0x0002, true)
        {
            ns.Write((int)id);
            ns.Write((int)next);
            ns.Write((int)count);
            ns.WriteHex(msg);
        }
    }
    //public sealed class NET_Result0x09 : NetPacket
    //{
    //    public NET_Result0x09(byte success) : base(0x09, true)
    //    {
    //        //16000900
    //        ns.WriteHex("D61900000E00D09BD0B0D180D0B5D0B9D0BAD0B0");
    //    }
    //}
    public sealed class NET_TCCharNameQueried_0x09 : NetPacket
    {
        public NET_TCCharNameQueried_0x09(int charId) : base(0x0009, true)
        {
            ns.Write(charId);
            const string name = "Mistake"; //For testing
            ns.WriteUTF8Fixed(name, name.Length);
        }
    }
}
