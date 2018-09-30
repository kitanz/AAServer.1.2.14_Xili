﻿using LocalCommons.Network;
using System;

namespace ArcheAge.ArcheAge.Network
{
    public sealed class NP_Pong_0x0013 : NetPacket
    {
        ///<summary>
        ///Response to client Ping packets
        ///</summary>
        public NP_Pong_0x0013(long tm, long when, int local) : base(2, 0x0013)
        {
            ns.Write((long)tm); //tm
            ns.Write((long)when); //when
            ns.Write((long)0x00); //elapsed
            ns.Write((long)(Environment.TickCount & int.MaxValue) * 1000); //remote
            ns.Write(local); //local
            ns.Write((uint)Environment.TickCount & int.MaxValue); //world
        }
    }
}
