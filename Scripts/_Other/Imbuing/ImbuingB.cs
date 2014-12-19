using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.SkillHandlers
{
	public class ImbuingGumpB : Gump
	{
        private static int Cat_Select;
        public static object ImIt;

        /*public static void CheckSoulForge(Mobile from, int range, out bool sforge)
        {
            sforge = false;
            PlayerMobile m = from as PlayerMobile;

            Map map = from.Map;

            if (map == null)
                return;

            IPooledEnumerable eable = map.GetItemsInRange(from.Location, 1);

            foreach (Item item in eable)
            {
                bool isTMSForge = (item.ItemID >= 17015 && item.ItemID <= 17030); // TerMur SoulForge (+5% bonus & easier unravels)
                bool isSForge = (item.ItemID >= 16995 && item.ItemID <= 17010); // Standard SoulForge
                bool isGSForge = (item.ItemID >= 17607 && item.ItemID <= 17610); // Gargoyle Mini SoulForge

                if (isSForge || isTMSForge || isGSForge)
                {
                    if ((from.Z + 16) < item.Z || (item.Z + 16) < from.Z || !from.InLOS(item))
                        continue;

                    sforge = true;
                    m.Imbue_SFBonus = 0;

                    if (isTMSForge) { m.Imbue_SFBonus = 5; }

                    if (sforge)
                        break;
                }
            }
        }*/

        public ImbuingGumpB(Mobile from, object item) : base(520, 340)
        {
            from.CloseGump(typeof(ImbuingGump));
            from.CloseGump(typeof(ImbuingGumpC));

            Mobile m = from;
            PlayerMobile pm = from as PlayerMobile;

            ImIt = pm.Imbue_Item;

            int Iref = ImbuingGump.GetItemRef(ImIt);

            AddPage(0);
            this.AddBackground(0, 0, 540, 560, 9270);
            this.AddAlphaRegion(17, 17, 503, 20);
            this.AddAlphaRegion(17, 45, 230, 465);
            this.AddAlphaRegion(256, 45, 264, 465);
            this.AddAlphaRegion(17, 518, 503, 25);

            this.AddLabel(221, 18, 1359, "IMBUING MENU");
            int by = 0;

            // ===== Attribute Catagories ========================================
            this.AddLabel(87, 50, 1359, "CATAGORIES");

            AddButton(22, 85 + (by * 25), 4017, 4018, 10001, GumpButtonType.Reply, 0);
            AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Casting", false, false); by += 1;
            if ( Iref == 1 || Iref == 2 || Iref == 4 || Iref == 6 )
            {
                AddButton(22, 85 + (by * 25), 4017, 4018, 10002, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Combat", false, false); by += 1;
            }
            if ( Iref == 1 || Iref == 2 )
            {
                AddButton(22, 85 + (by * 25), 4017, 4018, 10006, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Area Effects", false, false); by += 1;
                AddButton(22, 85 + (by * 25), 4017, 4018, 10007, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Effects", false, false); by += 1;
            }
            AddButton(22, 85 + (by * 25), 4017, 4018, 10003, GumpButtonType.Reply, 0);
            AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Misc.", false, false); by += 1;
            if (Iref == 1 || Iref == 2 || Iref == 3 || Iref == 5 || Iref == 6)
            {
                AddButton(22, 85 + (by * 25), 4017, 4018, 10004, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Resists", false, false); by += 1;
            }
            if ( Iref == 2 ||  Iref == 3  ||  Iref == 5 || Iref == 6 )
            {
                AddButton(22, 85 + (by * 25), 4017, 4018, 10005, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Stats", false, false); by += 1;
            }
            if ( Iref == 1  || Iref == 2 )
            {
                AddButton(22, 85 + (by * 25), 4017, 4018, 10008, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Slayers", false, false); by += 1;
                AddButton(22, 85 + (by * 25), 4017, 4018, 10009, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Super Slayers", false, false); by += 1;
            }
            if ( Iref == 6 )
            {
                AddButton(22, 85 + (by * 25), 4017, 4018, 10010, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Skill Group 1", false, false); by += 1;
                AddButton(22, 85 + (by * 25), 4017, 4018, 10011, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Skill Group 2", false, false); by += 1;
                AddButton(22, 85 + (by * 25), 4017, 4018, 10012, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Skill Group 3", false, false); by += 1;
                AddButton(22, 85 + (by * 25), 4017, 4018, 10013, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Skill Group 4", false, false); by += 1;
                AddButton(22, 85 + (by * 25), 4017, 4018, 10014, GumpButtonType.Reply, 0);
                AddHtml(63, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Skill Group 5", false, false); by += 1;
            }

            // ===== Attribute Catagories ========================================
            by = 0;
            this.AddLabel(347, 50, 1359, "SELECTIONS");
            
            if (pm.ImbMenu_Cat == 1) // == CASTING ==
            {
                if ( Iref == 1 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10122, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Spell Channeling", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10141, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Mage Weapon", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10116, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Faster Casting", false, false); by += 1;
                }
                else if ( Iref == 2 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10122, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Spell Channeling", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10116, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Faster Casting", false, false); by += 1;
                }
                else if ( Iref == 3 )
                {
                    BaseArmor Ar = ImIt as BaseArmor;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10117, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Lower Mana Cost", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10118, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Lower Reagent Cost", false, false); by += 1;
                    if (Ar.MeditationAllowance != ArmorMeditationAllowance.All)
                    {
                        AddButton(258, 85 + (by * 25), 4017, 4018, 10149, GumpButtonType.Reply, 0);
                        AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Mage Armor", false, false); by += 1;
                    }
                }
                else if ( Iref == 4 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10122, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Spell Channeling", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10116, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Faster Casting", false, false); by += 1;
                }
                else if ( Iref == 5 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10117, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Lower Mana Cost", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10118, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Lower Reagent Cost", false, false); by += 1;
                }
                else if ( Iref == 6 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10117, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Lower Mana Cost", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10118, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Lower Reagent Cost", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10114, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Spell Damage Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10116, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Faster Casting", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10115, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Faster Cast Recovery", false, false); by += 1;
                }
            }
            else if (pm.ImbMenu_Cat == 2) // == COMBAT ==
            {
                if ( Iref == 1 || Iref == 2 )
                {

                    AddButton(258, 85 + (by * 25), 4017, 4018, 10112, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Damage Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10101, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Defense Chance Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10102, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Chance Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10113, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Swing Speed Increase", false, false); by += 1;
                    if (Iref == 1)
                    {
                        AddButton(258, 85 + (by * 25), 4017, 4018, 10140, GumpButtonType.Reply, 0);
                        AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Use Best Weapon Skill", false, false); by += 1;
                    }
                    if (Iref == 2)
                    {
                        AddButton(258, 85 + (by * 25), 4017, 4018, 10160, GumpButtonType.Reply, 0);
                        AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Velocity", false, false); by += 1;
                        AddButton(258, 85 + (by * 25), 4017, 4018, 10161, GumpButtonType.Reply, 0);
                        AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Balanced", false, false); by += 1;

                    }
                }
                else if ( Iref == 4 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10101, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Defense Chance Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10102, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Chance Increase", false, false); by += 1;

                }                
                else if ( Iref == 6 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10112, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Damage Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10101, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Defense Chance Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10102, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Chance Increase", false, false); by += 1;
                }
            }
            else if (pm.ImbMenu_Cat == 3)  // == MISC ==
            {
                if ( Iref == 1 || Iref == 2)
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10121, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Luck", false, false); by += 1;
                }
                else if ( Iref == 3 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10119, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Reflect Physical Damage", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10121, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Luck", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10123, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Night Sight", false, false); by += 1;
                }
                else if ( Iref == 4 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10119, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Reflect Physical Damage", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10121, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Lower Requirements", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10123, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Durability", false, false); by += 1;
                }
                else if ( Iref == 5 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10119, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Reflect Physical Damage", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10121, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Luck", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10123, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Night Sight", false, false); by += 1;
                }
                else if ( Iref == 6 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10121, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Luck", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10123, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Night Sight", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10120, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Enhance Potions", false, false); by += 1;
                }
            }
            else if (pm.ImbMenu_Cat == 4) // == RESISTS ==
            {
                AddButton(258, 85 + (by * 25), 4017, 4018, 10151, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Physical Resist Bonus", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10152, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Fire Resist Bonus", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10153, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Cold Resist Bonus", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10154, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Poison Resist Bonus", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10155, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Energy Resist Bonus", false, false); by += 1;
            }
            else if (pm.ImbMenu_Cat == 5)  // == STATS ==
            {
                if ( Iref == 3 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10109, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Point Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10110, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Stamina Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10111, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Mana Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10103, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Point Regeneration", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10104, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Stamina Regeneration", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10105, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Mana Regeneration", false, false); by += 1;
                }
                else if ( Iref == 5 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10109, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Point Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10110, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Stamina Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10111, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Mana Increase", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10103, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Point Regeneration", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10104, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Stamina Regeneration", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10105, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Mana Regeneration", false, false); by += 1;
                }
                else if ( Iref == 6 )
                {
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10106, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Strength Bonus", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10107, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Dexterity Bonus", false, false); by += 1;
                    AddButton(258, 85 + (by * 25), 4017, 4018, 10108, GumpButtonType.Reply, 0);
                    AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Intelligence Bonus", false, false); by += 1;
                }
            }
            else if (pm.ImbMenu_Cat == 6)  // == HIT AREA EFFECTS ==
            {
                AddButton(258, 85 + (by * 25), 4017, 4018, 10130, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Physical Area", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10131, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Fire Area", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10132, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Cold Area", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10133, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Poison Area", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10134, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Energy Area", false, false); by += 1;
            }
            else if (pm.ImbMenu_Cat == 7)  // == ON HIT EFFECTS ==
            {
                AddButton(258, 85 + (by * 25), 4017, 4018, 10135, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Magic Arrow", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10136, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Harm", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10137, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Fireball", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10138, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Lightning", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10139, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Dispel", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10128, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Lower Attack", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10129, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Lower Defense", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10125, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Life Leech", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10126, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Stamina Leech", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10127, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Hit Mana Leech", false, false); by += 1;
            }
            else if (pm.ImbMenu_Cat == 8)  // == SLAYERS ==
            {
                AddButton(258, 85 + (by * 25), 4017, 4018, 10201, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Orc Slaying", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10202, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Troll Slaughter", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10203, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Ogre Trashing", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10204, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Dragon Slaying", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10205, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Terathan", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10206, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Snakes Bane", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10207, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Lizardman Slaughter", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10208, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Gargoyles Foe", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10211, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Ophidian", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10212, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Spiders Death", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10213, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Scorpions Bane", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10214, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Flame Dousing", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10215, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Water Dissipation", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10216, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Vacuum", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10217, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Elemental Health", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10218, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Earth Shatter", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10219, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Blood Drinking", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10220, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Summer Wind", false, false); by += 1;
            }
            else if (pm.ImbMenu_Cat == 9)  // == SUPER SLAYERS ==
            {
                AddButton(258, 85 + (by * 25), 4017, 4018, 10221, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Silver", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10222, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Repond", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10223, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Reptilian Death", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10224, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Exorcism", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10225, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Arachnid Doom", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10226, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Elemental Ban", false, false); by += 1;
            }
            else if (pm.ImbMenu_Cat == 10)  // == SKILL GROUP 1 ==
            {
                AddButton(258, 85 + (by * 25), 4017, 4018, 10251, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Fencing", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10252, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Mace Fighting", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10253, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Swordsmanship", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10254, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Musicianship", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10255, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Magery", false, false); by += 1;
            }
            else if (pm.ImbMenu_Cat == 11)  // == SKILL GROUP 2 ==
            {
                AddButton(258, 85 + (by * 25), 4017, 4018, 10256, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Wrestling", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10257, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Animal Taming", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10258, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Spirit Speak", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10259, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Tactics", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10260, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Provocation", false, false); by += 1;
            }
            else if (pm.ImbMenu_Cat == 12)  // == SKILL GROUP 3 ==
            {
                AddButton(258, 85 + (by * 25), 4017, 4018, 10261, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Focus", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10262, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Parrying", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10263, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Stealth", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10264, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Meditation", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10265, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Animal Lore", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10266, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Discordance", false, false); by += 1;
            }
            else if (pm.ImbMenu_Cat == 13)  // == SKILL GROUP 4 ==
            {
                AddButton(258, 85 + (by * 25), 4017, 4018, 10267, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Bushido", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10268, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Necromancy", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10269, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Veterinary", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10270, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Stealing", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10271, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Evaluating Intelligence", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10272, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Anatomy", false, false); by += 1;
            }
            else if (pm.ImbMenu_Cat == 14)  // == SKILL GROUP 5 ==
            {
                AddButton(258, 85 + (by * 25), 4017, 4018, 10273, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Peacemaking", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10274, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Ninjitsu", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10275, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Chivalry", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10276, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Archery", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10277, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Resisting Spells", false, false); by += 1;
                AddButton(258, 85 + (by * 25), 4017, 4018, 10278, GumpButtonType.Reply, 0);
                AddHtml(299, 87 + (by * 25), 150, 18, "<BASEFONT COLOR=#FFFFFF>Healing", false, false); by += 1;
            }

            AddButton(19, 520, 4017, 4018, 10099, GumpButtonType.Reply, 0);
            this.AddLabel(58, 521, 1359, "CANCEL");
        }                      

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
            PlayerMobile pm = from as PlayerMobile;

            ImIt = pm.Imbue_Item;

			int buttonNum = 0;
			int buttonRNum = 0;

            if ((info.ButtonID > 0 && info.ButtonID < 10000) || info.ButtonID == 10099 )
				buttonNum = 0;
			else if( info.ButtonID > 20004 )
				buttonNum = 30000;
			else if( ( info.ButtonID > 10100 ) && ( info.ButtonID < 10300 ) )
            {
				buttonNum = 10101;  
                buttonRNum = info.ButtonID - 10100;
            }
			else
				buttonNum = info.ButtonID;

			switch( buttonNum )
			{
				case 0:
				{
					// = Close/Cancel
					break;
				}	
				case 10001:
				{
                    pm.ImbMenu_Cat = 1;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));  	   
					break;
				}
				case 10002:
				{
                    pm.ImbMenu_Cat = 2;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));  	   
					break;
				}
				case 10003:
				{
                    pm.ImbMenu_Cat = 3;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));  	   
					break;
				}
				case 10004:
				{
                    pm.ImbMenu_Cat = 4;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));    	   
					break;
				}
				case 10005:
				{
                    pm.ImbMenu_Cat = 5;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));  	   
					break;
				}
				case 10006:
				{
                    pm.ImbMenu_Cat = 6;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));    	   
					break;
				}
				case 10007:
				{
                    pm.ImbMenu_Cat = 7;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));     
					break;
				}
				case 10008:
				{
                    pm.ImbMenu_Cat = 8;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));   	   
					break;
				}
				case 10009:
				{
                    pm.ImbMenu_Cat = 9;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));    	   
					break;
				}
				case 10010:
				{
                    pm.ImbMenu_Cat = 10;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));  	   
					break;
				}
				case 10011:
				{
                    pm.ImbMenu_Cat = 11;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));  	   
					break;
				}
				case 10012:
				{
                    pm.ImbMenu_Cat = 12;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));    	   
					break;
				}
				case 10013:
				{
                    pm.ImbMenu_Cat = 13;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));   	   
					break;
				}
				case 10014:
				{
                    pm.ImbMenu_Cat = 14;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpB(from, pm.Imbue_Item));   	   
					break;
				}
				case 10099:  // = Cancel
				{
 					break;
				}

                case 10101:  // = Proceed to Attribute Intensity Menu [ImbuingC.cs]
                {                   
                    pm.Imbue_Mod = buttonRNum;
                    from.CloseGump(typeof(ImbuingGumpB));
                    from.SendGump(new ImbuingGumpC(from, pm.Imbue_Item, buttonRNum, 0));
                    break;
                }				
			}
            return;
		}
    }
}