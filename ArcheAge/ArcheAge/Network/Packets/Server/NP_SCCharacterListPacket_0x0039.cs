using ArcheAge.ArcheAge.Holders;
using ArcheAge.ArcheAge.Network.Connections;
using ArcheAge.ArcheAge.Structuring;
using LocalCommons.Network;
using System.Collections.Generic;

namespace ArcheAge.ArcheAge.Network
{
    public sealed class NP_CharacterListPacket_0x0039 : NetPacket
    {
        /*private void WriteItem(int itemId)
        {
            ns.Write((int)itemId);
            if (itemId <= 0)
            {
                return;
            }

            ns.Write((int)0x01);
            for (int i = 0; i < 6; i++)
            {
                ns.Write((byte)0x00);
            }

            ns.Write((byte)0x01);
            for (int i = 0; i < 3; i++)
            {
                ns.Write((byte)0x00);
            }

            ns.Write((byte)0x01);
            for (int i = 0; i < 4; i++)
            {
                ns.Write((byte)0x00);
            }

            ns.Write((byte)0x55);
            for (int i = 0; i < 62; i++)
            {
                ns.Write((byte)0x00);
            }

            ns.Write((byte)0x0B);
            for (int i = 0; i < 16; i++)
            {
                ns.Write((byte)0x00);
            }
        }*/
        private void WriteItem(int itemId)
        {
            ns.Write((int)itemId);
            switch (itemId)
            {
                case 0:
                    break;
                default:
                    ns.Write((long)Program.ObjectUid.Next()); //id[1] Q //TODO: сделать у вещей постоянные UID
                    ns.Write((byte)0); //type[1] c
                    ns.Write((byte)0); //flags[1] c
                    ns.Write((int)0x01); //stackSize[1] d
                    byte detailType = 1;
                    ns.Write((byte)detailType); //detailType c
                    switch (detailType)
                    {
                        case 0:
                            break;
                        case 1:
                            ns.WriteHex("000000005500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"); //detail 51 b
                            break;
                        case 2:
                            ns.WriteHex("0000000000000000000000000000000000000000000000000000000000"); //detail 29 b
                            break;
                        case 3:
                            ns.WriteHex("000000000000"); //detail 6 b
                            break;
                        case 4:
                            ns.WriteHex("000000000000000000"); //detail 9 b
                            break;
                        case 5:
                            ns.Write((long)0); //type d
                            ns.Write((long)0); //x Q
                            ns.Write((long)0); //y Q
                            ns.Write((float)0); //z f
                            break;
                        case 6:
                            ns.WriteHex("00000000000000000000000000000000"); //detail 16 b
                            break;
                        case 7:
                            ns.WriteHex("00000000000000000000000000000000"); //detail 16 b
                            break;
                        case 8:
                            ns.WriteHex("0000000000000000"); //detail 8 b
                            break;
                    }

                    ns.Write((long)0); //creationTime[1] Q
                    ns.Write((int)0); //lifespanMins[1] d
                    ns.Write((int)0); //type[1] d
                    ns.Write((byte)0x0b); //worldId c"
                    ns.Write((long)0); //unsecureDateTime Q
                    ns.Write((long)0); //unpackDateTime Q
                    break;
            }
        }

        /// <summary>
        /// the package for the entrance to the Lobby
        /// CharacterListPacket01_0x0039
        /// author: NLObP
        /// </summary>
        /// <param name="net"></param>
        /// <param name="num">the sequential number of the character (of the total), which we derive in the package</param>
        /// <param name="last">0 - more are expected to pack Character List, 1 - latest package will not exist</param>
        public NP_CharacterListPacket_0x0039(ClientConnection net, int num, int last) : base(01, 0x0039)
        {
            var accountId = net.CurrentAccount.AccountId;
            List<Character> charList = CharacterHolder.LoadCharacterData(accountId);
            var totalChars = CharacterHolder.GetCount();

            ns.Write((byte)last); //last c
            if (totalChars == 0)
            {
                ns.Write((byte)0); //totalChars); //count c
                return; //if the list is empty, we finish the work
            }
            else
            {
                ns.Write((byte)1); //totalChars); //count c
            }
            int aa = 0;
            foreach (Character chr in charList)
            {
                if (num == aa) //the NUM parameter corresponds, which Chara to display in the package (can be from 0 to 2)
                {
                    CharacterHolder.LoadEquipPacksData(chr, chr.Ability[0]); //read more- NewbieClothPackId, NewbieWeaponPackId из таблицы character_equip_packs
                    CharacterHolder.LoadClothsData(chr, chr.NewbieClothPackId); //read more- Head,Chest,Legs,Gloves,Feet из таблицы equip_pack_cloths
                    CharacterHolder.LoadWeaponsData(chr, chr.NewbieWeaponPackId); //read more- Weapon,WeaponExtra,WeaponRanged,Instrument из таблицы equip_pack_weapons
                    CharacterHolder.LoadCharacterBodyCoord(chr, chr.CharRace, chr.CharGender); //read more- body, x, y, z из таблицы charactermodel
                    CharacterHolder.LoadZoneFaction(chr, chr.CharRace, chr.CharGender); //read more- FactionId,StartingZoneId из таблицы characters

                    ns.Write((int)chr.CharacterId); //type d
                    ns.WriteUTF8Fixed(chr.CharName, chr.CharName.Length); //name S
                    ns.Write((byte)chr.CharRace); //CharRace c
                    ns.Write((byte)chr.CharGender); //CharGender c
                    ns.Write((byte)chr.Level); //level c
                    ns.Write((int)0x001C4); //health d
                    ns.Write((int)0x001CE); //mana d
                    ns.Write((int)chr.StartingZoneId); //zid d
                    ns.Write((int)chr.FactionId); //faction_id d
                    string factionName = ""; //factionName SS
                    ns.WriteUTF8Fixed(factionName, factionName.Length);
                    //-----------------------------
                    ns.Write((int)0x00); //type d
                    ns.Write((int)0x00); //family d
                    //------------------------------------
                    // the inventory of the character
                    //------------------------------------
                    // 1.suit
                    // 2.Helmet (fabric, leather, armor)
                    // 3.Bib (fabric, leather, armor)
                    // 4.Belt (fabric, leather, armor)
                    // 5.Bracers (cloth, leather, plate)
                    // 6.Gloves (fabric, leather, armor) 
                    // 7.Cape
                    // 8.Greaves (fabric, leather, armor)
                    // 9.Shoes (fabric, leather, armor)
                    //10.Necklace
                    //11.Earring
                    //12.Earring
                    //13.Ring
                    //14.Ring
                    //15.Weapon
                    //16.Additional weapon
                    //17.Onion
                    //18.Muses'. instrument (string, wind, percussion)
                    //19.Cargo (glider, commercial cargo, local goods)
                    //for (int i = 0; i < 19; i++)
                    //{
                    //equip_slot
                    WriteItem(chr.Head); //ES_HEAD
                    WriteItem(0); //ES_NECK
                    WriteItem(chr.Chest); //ES_CHEST Chest (fabric, leather, armor)	23387
                    WriteItem(0); //ES_WAIST
                    WriteItem(chr.Legs); //ES_LEGS Greaves (fabric, leather, armor) 23388
                    WriteItem(chr.Gloves); //ES_HANDS
                    WriteItem(chr.Feet); //ES_FEET Shoes (fabric, leather, armor) 23390
                    WriteItem(0); //ES_ARMS
                    WriteItem(0); //ES_BACK	
                    WriteItem(0); //ES_EAR_1
                    WriteItem(0); //ES_EAR_2
                    WriteItem(0); //ES_FINGER_1
                    WriteItem(0); //ES_FINGER_2
                    WriteItem(0); //ES_UNDERSHIRT
                    WriteItem(0); //ES_UNDERPANTS
                    WriteItem(chr.Weapon); //ES_MAINHAND Weapon
                    WriteItem(chr.WeaponExtra); //ES_OFFHAND Additional weapon
                    WriteItem(chr.WeaponRanged); //ES_RANGED Onion
                    WriteItem(chr.Instrument); //ES_MUSICAL Muses'. instrument (string, wind, percussion) - Some stupid translation from russian lmao
                    //}
                    //for (int i = 0; i < 7; i++)
                    //{
                    ns.Write((int)chr.Type[0]); //type[somehow_special] d 19839 face
                    ns.Write((int)chr.Type[1]); //type[somehow_special] d 25372 hair_id
                    ns.Write((int)chr.Type[2]); //type[somehow_special] d 
                    ns.Write((int)chr.Type[3]); //type[somehow_special] d
                    ns.Write((int)chr.Type[4]); //type[somehow_special] d

                    ns.Write((int)chr.CharBody); //type[somehow_special] d 539   body
                    ns.Write((int)chr.Type[6]); //type[somehow_special] d
                    //}

                    //for (int i = 0; i < 2; i++)
                    //{
                    //equip_slot
                    WriteItem(0); //ES_BACKPACK
                    WriteItem(0); //ES_COSPLAY
                    //}

                    //for (int i = 0; i < 3; i++)
                    //{
                    ns.Write((byte)chr.Ability[0]); //specialization: 1-FIGHTER attack 7-MAGIC magic 6-WILD healing,
                                                    //10-LOVE harassment, 5-DEATH mysticism, 8-VOCATION stealth
                    ns.Write((byte)chr.Ability[1]); //the effect of class 1
                    ns.Write((byte)chr.Ability[2]); //the effect of class 2
                                                    //}

                    //ns.WriteHex("00000000A8B7CF03");
                    ns.Write((int)0);
                    ns.Write((float) 867.27);  //x Q 03cfb7a8 00000000 = 0.0
                    //ns.WriteHex("000000006090A603");
                    ns.Write((int)0);
                    ns.Write((float)779.11);  //y Q 03a69060 00000000 = 
                    //ns.WriteHex("EFFC1043");
                    ns.Write((float)247.8); //z f  4310fcef = 144.988

                    ns.Write((byte)chr.Ext); //ext c
                    switch (chr.Ext)
                    {
                        case 0:
                            break;
                        case 1:
                            ns.Write((int)chr.Type[7]); //type d
                            break;
                        case 2:
                            ns.Write((int)chr.Type[7]); //type d
                            ns.Write((int)chr.Type[8]); //type d
                            ns.Write((int)chr.Type[9]); //type d
                            break;
                        default:
                            ns.Write((int)chr.Type[7]); //type d        4299 hair_color_id
                            ns.Write((int)chr.Type[8]); //type d        4    skin_color_id
                            ns.Write((int)chr.Type[9]); //type d        0
                            ns.Write((int)chr.Type[10]); //type d       0
                            ns.Write((float)chr.Weight[10]); //weight f 1
                            ns.Write((float)chr.Scale); //scale f       1
                            ns.Write((float)chr.Rotate); //rotate f     0
                            ns.Write((short)chr.MoveX); //moveX h       0
                            ns.Write((short)chr.MoveY); //moveY h       0
                            //for (int i = 11; i < 15; i++)
                            //{
                            //ns.Write((int)chr.Type[i]); //type d
                            //ns.Write((float)chr.Weight[i]); //weight f
                            ns.Write((int)chr.Type[11]); //type d          0    face_fixed_decal_asset_0_id
                            ns.Write((float)chr.Weight[11]); //weight f    1    face_fixed_decal_asset_0_weight
                            ns.Write((int)chr.Type[12]); //type d          444  face_fixed_decal_asset_1_id
                            ns.Write((float)chr.Weight[12]); //weight f    1    face_fixed_decal_asset_1_weight
                            ns.Write((int)chr.Type[13]); //type d          170  face_fixed_decal_asset_2_id
                            ns.Write((float)chr.Weight[13]); //weight f    1    face_fixed_decal_asset_2_weight)
                            ns.Write((int)chr.Type[14]); //type d          0    face_fixed_decal_asset_3_id
                            ns.Write((float)chr.Weight[14]); //weight f    0.71 face_fixed_decal_asset_3_weight
                            //}

                            ns.Write((int)chr.Type[15]); //type d             0
                            ns.Write((int)chr.Type[16]); //type d             0 face_normal_map_id
                            ns.Write((int)chr.Type[17]); //type d             0
                            ns.Write((float)chr.Weight[17]); //weight f       1
                            ns.Write((int)chr.Lip); //lip d                   0
                            ns.Write((int)chr.LeftPupil); //leftPupil d       left_pupil_color
                            ns.Write((int)chr.RightPupil); //rightPupil d     right_pupil_color
                            ns.Write((int)chr.Eyebrow); //eyebrow d           eyebrow_color
                            ns.Write((int)chr.Decor); //decor d               deco_color
                            //the following instruction writes: len.stringHex
                            //---ns.Write((short)0x00); //modifiers_len h
                            string subString = chr.Modifiers.Substring(0, 256); //it is necessary to cut at the end of two characters \0\0
                            ns.WriteHex(subString, subString.Length); //modifiers b"
                            break;
                    }

                    ns.Write((short)0x36); //laborPower h  //LaborPoints = 5000
                    ns.Write((long)0x532F427F); //lastLaborPowerModified Q
                    ns.Write((short)0x00); //deadCount h
                    ns.Write((long)0x532B300C); //deadTime Q
                    ns.Write((int)0x00); //rezWaitDuration d
                    ns.Write((long)0x532B300C); //rezTime Q
                    ns.Write((int)0x00); //rezPenaltyDuration d
                    ns.Write((long)0x532F41B4); //lastWorldLeaveTime Q
                    ns.Write((long)0xC2); //moneyAmount Q  Number of copper coins Automatic 1:100:10000 Convert gold coins  //серебро, золото и платина (начало)
                    ns.Write((long)0x00); //moneyAmount Q //silver, gold and platinum (continued)
                    ns.Write((short)0x00); //crimePoint h
                    ns.Write((int)0x00); //crimeRecord d
                    ns.Write((short)0x00); //crimeScore h
                    ns.Write((long)0x00); //deleteRequestedTime Q
                    ns.Write((long)0x00); //transferRequestedTime Q
                    ns.Write((long)0x00); //deleteDelay Q
                    //ns.Write((int) 0x07); //consumedLp d
                    ns.Write((long)0x1E); //bmPoint Q  //монеты дару = 30
                    ns.Write((int)0x00); //consumedLp d ?
                    ns.Write((int)0x00); //? had to be inserted to align the packet length
                    ns.Write((long)0x00); //moneyAmount Q
                    ns.Write((long)0x00); //moneyAmount Q
                    ns.Write((byte)0x00); //autoUseAApoint A"
                    ns.Write((int)0x00); //point d
                    ns.Write((int)0x00); //gift d
                    ns.Write((long)0x00532F3FB3); //updated Q
                }
                ++aa;
            }
        }
    }
}
