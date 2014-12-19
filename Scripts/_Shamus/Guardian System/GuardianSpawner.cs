using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Commands.Generic;


namespace Server.Mobiles
{
    public class GuardianSpawner : Spawner
    {

        private int m_GuardLevel = 1;

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int GuardLevel 
        {
            get { return m_GuardLevel; }
            set { m_GuardLevel = value; }
        }
                
        
        [Constructable]
        public GuardianSpawner(): base(1, 5, 5, 50, 10, "warriorguardian1")
        {
            this.Weight = 1.0;
            this.Movable = true;
            this.Name = "guardian spawner";
            this.Stop();
            this.Visible = true;
            this.ItemID = 0x1f1c;
        }

        public GuardianSpawner(Serial serial): base(serial)
        {
        }
  
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            writer.Write(m_GuardLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                {
                    m_GuardLevel = reader.ReadInt();
                    goto case 0;
                }
                case 0:
                {
                    break;
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (this.IsChildOf(pm.Backpack)) 
            {
                from.SendMessage("You cannot use that in your inventory.");
                return;
            }
        
            GuardianGump g = new GuardianGump(this);
            from.SendGump(g);
        }
        
        public int GetLevel() 
        {
            return this.m_GuardLevel;
        }
        
        public int GetMaxCount() 
        {
            return this.m_Count;
        }
        
        public void AddLevel()
        {
            this.m_GuardLevel += 1;
            this.m_SpawnNames = new List<string>();
            this.m_SpawnNames.Add("warriorguardian" + this.m_GuardLevel);
            this.m_MinDelay = TimeSpan.FromMinutes(5 - this.m_GuardLevel);
            this.m_MaxDelay = TimeSpan.FromMinutes(5 - this.m_GuardLevel);
            this.Respawn();
        }
        
        public void IncCount()
        {
            this.m_Count += 1;
        }

        public class GuardianGump : Gump
        {
            private readonly GuardianSpawner m_Spawner;
            public GuardianGump(GuardianSpawner spawner): base(50, 50)
            {
                this.m_Spawner = spawner;
                this.AddPage(0);

                this.AddBackground(0, 0, 410, 371, 5054);
                this.AddLabel(160, 1, 0, "Guardian Upgrades");
                this.AddLabel(20, 22, 0, "To level the guardian stone, click the icon next to an attribute.");
                
                int level = this.m_Spawner.GetLevel();
                int count = this.m_Spawner.GetMaxCount();
                
                if (level < 4) 
                {
                    this.AddButton(5, 44, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                }
                
                if (count < 6)
                {
                    this.AddButton(5, 66, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
                }
                
                this.AddLabel(35, 44, 0, "Guardian Level: " + level);
                this.AddLabel(35, 66, 0, "Max Guardians : " + count);
                
                this.AddButton(5, 347, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
                this.AddLabel(35, 347, 0x384, "Lock/Start Spawning");
                
                this.AddButton(220, 347, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
                this.AddLabel(255, 347, 0x384, "Unlock/Stop Spawning");
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (this.m_Spawner.Deleted)
                    return;
                    
                PlayerMobile player = state.Mobile as PlayerMobile;
                if (player == null)
                    return;

                switch ( info.ButtonID )
                {
                    case 0:
                        {
                            return;
                        }                       
                    case 1:
                        {
                            if (player.Backpack == null)
                                break;
                                
                            if (this.m_Spawner.GetLevel() == 1)
                            {
                                if (player.Backpack.ConsumeTotal(typeof(Gold), 2000))
                                {
                                    this.m_Spawner.AddLevel();
                                } 
                                else
                                {
                                    state.Mobile.SendMessage("You need 2000 gold to upgrade that.");
                                }
                            }
                            else if (this.m_Spawner.GetLevel() == 2) 
                            {
                                if (player.Backpack.ConsumeTotal(typeof(Gold), 5000))
                                {
                                    this.m_Spawner.AddLevel();
                                } 
                                else
                                {
                                    state.Mobile.SendMessage("You need 5000 gold to upgrade that.");
                                }
                            } 
                            else if (this.m_Spawner.GetLevel() == 3) 
                            {
                                if (player.Backpack.ConsumeTotal(typeof(Gold), 10000))
                                {
                                    this.m_Spawner.AddLevel();
                                } 
                                else
                                {
                                    state.Mobile.SendMessage("You need 10000 gold to upgrade that.");
                                }
                            }
                            
                            break;
                        }
                    case 2:
                        {
                            if (player.Backpack.ConsumeTotal(typeof(Gold), 5000))
                            {
                                this.m_Spawner.IncCount();
                            } 
                            else
                            {
                                state.Mobile.SendMessage("You need 5000 gold to upgrade that.");
                            }

                            break;
                        }
                    case 3:
                        {
                            this.m_Spawner.Movable = false;
                            this.m_Spawner.Running = true;
                            this.m_Spawner.Start();
                            break;                         
                        }
                    case 4:
                        {
                            this.m_Spawner.Movable = true;
                            this.m_Spawner.Running = false;
                            this.m_Spawner.Stop();
                            this.m_Spawner.RemoveSpawned();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                state.Mobile.SendGump(new GuardianGump(this.m_Spawner));
            }
        }
    }
}