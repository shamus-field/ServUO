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
    public class ImbuingGumpC : Gump
    {
        public const int i_MaxProps = 5;

        private static int i_Mod, m_Intensity = 0, modvalue, i_Inc = 0;
        private static int m_Imax, s_Weight, m_Desc, IWmax;
        private static string s_Mod, RepModName = "";
        private static object i_Item;

        private static int m_Gem_no = 0, m_A_no = 0, m_B_no = 0;
        private static string m_Gem = "", m_A = "", m_B = "";

        private static double i_Success = 0, i_Diff;
       
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

                    if (isTMSForge)
                    {
                        m.Imbue_SFBonus = 5;
                    }

                    if (sforge)
                        break;
                }
            }
        }*/

        public ImbuingGumpC(Mobile from, object item, int mod, int value) : base(520, 340)
        {
            PlayerMobile m = from as PlayerMobile;

            from.CloseGump(typeof(ImbuingGump));
            from.CloseGump(typeof(ImbuingGumpB));

            // SoulForge Check
            bool sforge = false;
            ImbuingGump.CheckSoulForge(from, 1, out sforge);
            if (sforge != true)
            {
                from.SendLocalizedMessage(1079787);
                from.CloseGump(typeof(ImbuingGumpC));
                return;
            }

            i_Item = m.Imbue_Item;
            i_Mod = m.Imbue_Mod;
            modvalue = m.Imbue_ModInt;

            // = Check Type of Ingredients Needed 
            ImbuingGumpC.GetMaterials(i_Mod);

            // = Attribute max values & ranges
            m.ImbMenu_ModInc = i_Inc;
            m.Imbue_ModVal = m_Imax;
            if (m.Imbue_ModInt < i_Inc) { m.Imbue_ModInt = i_Inc; }
            if (m.Imbue_ModInt > m_Imax) { m.Imbue_ModInt = m_Imax; }
            modvalue = m.Imbue_ModInt;

            // = Times Item has been Imbued
            int i_Imbued = 0;
            if (i_Item is BaseWeapon) { BaseWeapon Ti = i_Item as BaseWeapon; i_Imbued = Ti.TimesImbued; }
            if (i_Item is BaseArmor) { BaseArmor Ti = i_Item as BaseArmor; i_Imbued = Ti.TimesImbued; }
            if (i_Item is BaseJewel) { BaseJewel Ti = i_Item as BaseJewel; i_Imbued = Ti.TimesImbued; }
            if (i_Item is BaseHat) { BaseHat Ti = i_Item as BaseHat; i_Imbued = Ti.TimesImbued; }

            // = Check Ingredients needed at the current Intensity
            m_Gem_no = GetMGemNo(m_Imax, i_Inc, modvalue);
            m_A_no = GetMANo(m_Imax, i_Inc, modvalue);
            m_B_no = GetMBNo(m_Imax, i_Inc, modvalue);

            // ------------------------------ Gump Menu -------------------------------------------------------------
            AddPage(0);
            this.AddBackground(0, 0, 540, 450, 9270);
            this.AddAlphaRegion(17, 17, 503, 20);
            this.AddAlphaRegion(17, 45, 245, 140);
            this.AddAlphaRegion(275, 45, 245, 140);
            this.AddAlphaRegion(17, 195, 245, 140);
            this.AddAlphaRegion(275, 195, 245, 140);
            this.AddAlphaRegion(17, 345, 503, 60);
            this.AddAlphaRegion(17, 415, 503, 20);

            this.AddLabel(187, 18, 1359, "IMBUING CONFIRMATION");
            this.AddLabel(57, 49, 1359, "PROPERTY INFORMATION");

            // - Attribute to Imbue
            AddHtmlLocalized(30, 80, 80, 17, 1114270, 0xFFFFFF, false, false);
            AddHtml(100, 80, 150, 17, String.Format("<BASEFONT COLOR=#FFFFFF> {0}", s_Mod), false, false);

            // - Weight Modifier
            AddHtmlLocalized(30, 120, 80, 17, 1114272, 0xFFFFFF, false, false);
            if (s_Weight == 100) { AddHtml(100, 120, 80, 17, "<BASEFONT COLOR=#CCCCFF> 1.0x", false, false); }
            if (s_Weight == 110) { AddHtml(100, 120, 80, 17, "<BASEFONT COLOR=#CCCCFF> 1.1x", false, false); }
            if (s_Weight == 120) { AddHtml(100, 120, 80, 17, "<BASEFONT COLOR=#CCCCFF> 1.2x", false, false); }
            if (s_Weight == 130) { AddHtml(100, 120, 80, 17, "<BASEFONT COLOR=#CCCCFF> 1.3x", false, false); }
            if (s_Weight == 140) { AddHtml(100, 120, 80, 17, "<BASEFONT COLOR=#CCCCFF> 1.4x", false, false); }
            if (s_Weight == 150) { AddHtml(100, 120, 80, 17, "<BASEFONT COLOR=#CCCCFF> 1.5x", false, false); }

            double c_modv = modvalue;
            double c_max = m_Imax;
            double cur_int = (c_modv / c_max) * 100;
            cur_int = Math.Round(cur_int, 1);
            m_Intensity = Convert.ToInt32(cur_int);

            AddHtmlLocalized(30, 140, 80, 17, 1114273, 0xFFFFFF, false, false);
            AddHtml(100, 140, 80, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}%", m_Intensity), false, false);

            // - Materials needed
            this.AddLabel(96, 199, 1359, "MATERIALS");
            AddHtml(40, 230, 180, 17, String.Format("<BASEFONT COLOR=#FFFFFF> {0}", m_A), false, false);
            AddHtml(210, 230, 40, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}", m_A_no), false, false);
            AddHtml(40, 255, 180, 17, String.Format("<BASEFONT COLOR=#FFFFFF> {0}", m_Gem), false, false);
            AddHtml(210, 255, 40, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}", m_Gem_no), false, false);
            if (m_B_no > 0)
            {
                AddHtml(40, 280, 180, 17, String.Format("<BASEFONT COLOR=#FFFFFF> {0}", m_B), false, false);
                AddHtml(210, 280, 40, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}", m_B_no), false, false);
            }

            // - Mod Description
            AddHtmlLocalized(290, 65, 215, 110, m_Desc, 0xFFFFFF, false, false); 

            this.AddLabel(365, 199, 1359, "RESULTS");
                                  
            // - Replaces Attribute
            RepModName = "";
            if (RepModName == "M'KAY!") 
                RepModName = s_Mod;

            // - Current Mod Weight
            int i_TpropW = GetTotalWeight(i_Item); int i_TMods = GetTotalMods(i_Item);
            if (m_Imax <= 1) { m_Intensity = 100; }
            double c_i = m_Intensity; double c_w = s_Weight;
            double cur_wei = (s_Weight / m_Imax) * m.Imbue_ModInt; 
            cur_wei = Math.Round(cur_wei);

            // - Maximum allowed Property Weight & Item Mod Count
            IWmax = GetMaxWeight(m.Imbue_Item);
            m.Imbue_IWmax = IWmax;
            AddHtmlLocalized(288, 230, 150, 17, 1113645, 0xFFFFFF, false, false);
            AddHtml(458, 230, 80, 17, String.Format("<BASEFONT COLOR=#CCFFCC> {0}/5", i_TMods + 1), false, false);
            AddHtmlLocalized(288, 250, 150, 17, 1113646, 0xFFFFFF, false, false);
            AddHtml(458, 250, 80, 17, String.Format("<BASEFONT COLOR=#CCFFCC> {0}/{1}", i_TpropW + Convert.ToInt32(cur_wei), IWmax), false, false);

            // - Times Imbued
            AddHtmlLocalized(288, 270, 150, 17, 1113647, 0xFFFFFF, false, false);
            AddHtml(458, 270, 80, 17, String.Format("<BASEFONT COLOR=#CCFFCC> {0}/20", i_Imbued), false, false);

            // - Name of Attribute to be Replaced
            string ReplaceMod = WhatReplacesWhat(m.Imbue_Mod, m.Imbue_Item);
            AddHtmlLocalized(30, 100, 80, 17, 1114271, 0xFFFFFF, false, false);  
            if (ReplaceMod != "") { AddHtml(100, 100, 150, 17, String.Format("<BASEFONT COLOR=#FFFFFF> {0}", ReplaceMod), false, false); }

            // ===== CALCULATE DIFFICULTY =====
            i_Diff = 0;    // = Actual difficulty
            i_Success = 0; // = Display difficulty
            // - Skill Bonus -
            double iBonus = m.Skills[SkillName.Imbuing].Base / 500;

            // - Racial Bonus - SA ONLY -
            //if (m.Race == Race.Gargoyle) { iBonus += 0.05; }

            // - Normal / Exceptional Quality Bonus -
            double Q_Factor = (IWmax / 100) - 0.25;
            // ===== Difficulty Equation =====
            i_Diff = (((i_TpropW + cur_wei) / Q_Factor) * (1.65 - iBonus)) + ((i_TpropW + cur_wei) / 25);

            // - Add TerMur Soulforge bonus
            if (m.Imbue_SFBonus > 0) { i_Diff = (i_Diff / 100); i_Diff = (i_Diff * 95); }

            // = Calculate Difficulty as Percentage %
            i_Success = (m.Skills[SkillName.Imbuing].Base - (i_Diff - 25)) * 2; // = %
            double iX = (m.Skills[SkillName.Imbuing].Base - (i_Diff - 25)) * 200; // = Decimal Probability ( i.e [0.1 = 10%] )

            if ((i_Diff - 25) > m.Skills[SkillName.Imbuing].Base) { iX = 0; i_Success = 0; } // = No Chance at skill Level
            if (iX < 0.005) { iX = 0; i_Success = 0; } // = No Chance at skill Level
            i_Success = Math.Round(i_Success, 2);

            int Succ = Convert.ToInt32(i_Success);

            // = Imbuing Success Chance % 
            AddHtmlLocalized(305, 300, 150, 17, 1044057, 0xFFFFFF, false, false);
            if (Succ <= 1) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#FF5511>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ > 1 && Succ < 10) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#EE6611>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ >= 10 && Succ < 20) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#DD7711>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ >= 20 && Succ < 30) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#CC8811>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ >= 30 && Succ < 40) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#BB9911>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ >= 40 && Succ < 50) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#AAAA11>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ >= 50 && Succ < 60) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#99BB11>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ >= 60 && Succ < 70) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#88CC11>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ >= 70 && Succ < 80) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#77DD11>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ >= 80 && Succ < 90) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#66EE11>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ >= 90 && Succ < 100) { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#55FF11>\t{0}%", Succ.ToString()), false, false); }
            else if (Succ >= 100) { AddHtml(445, 300, 80, 17, "<BASEFONT COLOR=#01FF01>\t100%", false, false); }
            else { AddHtml(445, 300, 80, 17, String.Format("<BASEFONT COLOR=#FFFFFF>\t{0}%", Succ.ToString()), false, false); }

            // - Attribute Level
            int ModValue_plus = 0;
            if (m_Imax > 1)
            {
                // - Set Intesity to Minimum
                if (modvalue <= 0) { modvalue = i_Inc; }

                // - Armor = Natural Resistance + Intensity
                int Iref = ImbuingGump.GetItemRef(i_Item);
                if (Iref == 3)
                {
                    if (i_Mod == 51) { BaseArmor Ti = i_Item as BaseArmor; ModValue_plus = Ti.BasePhysicalResistance; }
                    if (i_Mod == 52) { BaseArmor Ti = i_Item as BaseArmor; ModValue_plus = Ti.BaseFireResistance; }
                    if (i_Mod == 53) { BaseArmor Ti = i_Item as BaseArmor; ModValue_plus = Ti.BaseColdResistance; }
                    if (i_Mod == 54) { BaseArmor Ti = i_Item as BaseArmor; ModValue_plus = Ti.BasePoisonResistance; }
                    if (i_Mod == 55) { BaseArmor Ti = i_Item as BaseArmor; ModValue_plus = Ti.BaseEnergyResistance; }
                }

                // = New Value:
                AddHtmlLocalized(235, 350, 100, 17, 1062300, 0xFFFFFF, false, false); 
                // - Mage Weapon Value ( i.e [Mage Weapon -25] )
                if (i_Mod == 41) { AddHtml(254, 374, 40, 17, String.Format("<BASEFONT COLOR=#CCCCFF> -{0}", (30 - modvalue)), false, false); }
                // - Show Property Value as % ( i.e [Hit Fireball 25%] )
                else if (i_Mod == 41) { AddHtml(254, 374, 40, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}%", (modvalue + ModValue_plus)), false, false); }
                // - Show Property Value as just Number ( i.e [Mana Regen 2] )
                else { AddHtml(254, 374, 40, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}", (modvalue + ModValue_plus)), false, false); }

                // == Buttons ==
                AddButton(230, 376, 5223, 5223, 10051, GumpButtonType.Reply, 0); // To Minimum Value
                AddImageTiled(232, 377, 11, 15, 5603);
                AddButton(211, 376, 5223, 5223, 10052, GumpButtonType.Reply, 0); // Dec Value by %
                AddImageTiled(212, 376, 13, 15, 5603);
                AddButton(192, 376, 5223, 5223, 10053, GumpButtonType.Reply, 0); // Dec Value by 1
                AddImageTiled(190, 376, 17, 15, 5603);
                AddImageTiled(195, 376, 17, 15, 5603);
                AddButton(308, 376, 5224, 5224, 10054, GumpButtonType.Reply, 0); // To Maximum Value
                AddImageTiled(305, 377, 11, 15, 5601);
                AddImageTiled(308, 376, 7, 16, 5224);
                AddButton(327, 376, 5224, 5224, 10055, GumpButtonType.Reply, 0); // Inc Value by %
                AddImageTiled(325, 376, 11, 15, 5601);
                AddImageTiled(327, 376, 6, 16, 5224);
                AddButton(346, 376, 5224, 5224, 10056, GumpButtonType.Reply, 0); // Inc Value by 1
                AddImageTiled(345, 376, 17, 15, 5601);
                AddImageTiled(350, 376, 17, 15, 5601);
            }
            AddButton(19, 415, 4017, 4018, 10099, GumpButtonType.Reply, 0);
            this.AddLabel(58, 416, 1359, "CANCEL");
            AddButton(400, 415, 4017, 4018, 10100, GumpButtonType.Reply, 0);
            this.AddLabel(439, 416, 1359, "IMBUE ITEM");
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            PlayerMobile pm = from as PlayerMobile;

            // = Check Type of Ingredients Needed 
            ImbuingGumpC.GetMaterials(pm.Imbue_Mod);

            int buttonNum = 0;
            if (info.ButtonID > 0 && info.ButtonID < 10000)
                buttonNum = 0;
            else if (info.ButtonID > 20004)
                buttonNum = 30000;
            else
                buttonNum = info.ButtonID;

            switch (buttonNum)
            {
                case 0:
                    {
                        //Close
                        break;
                    }
                case 10051: // = Decrease Mod Value [<]
                    {
                        if (pm.Imbue_ModInt > pm.ImbMenu_ModInc) { pm.Imbue_ModInt -= pm.ImbMenu_ModInc; }
                        from.CloseGump(typeof(ImbuingGumpC));
                        from.SendGump(new ImbuingGumpC(from, pm.Imbue_Item, pm.Imbue_Mod, pm.Imbue_ModInt));
                        break;
                    }
                case 10052:// = Decrease Mod Value [<<]
                    {
                        if (i_Mod == 42 || i_Mod == 24) { pm.Imbue_ModInt -= 20; }
                        if (i_Mod == 13 || i_Mod == 20 || i_Mod == 21) { pm.Imbue_ModInt -= 10; }
                        else { pm.Imbue_ModInt -= 5; }
                        from.CloseGump(typeof(ImbuingGumpC));
                        from.SendGump(new ImbuingGumpC(from, pm.Imbue_Item, pm.Imbue_Mod, pm.Imbue_ModInt));
                        break;
                    }
                case 10053:// = Minimum Mod Value [<<<]
                    {
                        pm.Imbue_ModInt -= 100;
                        from.CloseGump(typeof(ImbuingGumpC));
                        from.SendGump(new ImbuingGumpC(from, pm.Imbue_Item, pm.Imbue_Mod, pm.Imbue_ModInt));
                        break;
                    }
                case 10054: // = Increase Mod Value [>]
                    {
                        pm.Imbue_ModInt += pm.ImbMenu_ModInc;
                        from.CloseGump(typeof(ImbuingGumpC));
                        from.SendGump(new ImbuingGumpC(from, pm.Imbue_Item, pm.Imbue_Mod, pm.Imbue_ModInt));
                        break;
                    }
                case 10055: // = Increase Mod Value [>>]
                    {
                        if (i_Mod == 42 || i_Mod == 24) { pm.Imbue_ModInt += 20; }
                        if (i_Mod == 13 || i_Mod == 20 || i_Mod == 21) { pm.Imbue_ModInt += 10; }
                        else { pm.Imbue_ModInt += 5; }
                        from.CloseGump(typeof(ImbuingGumpC));
                        from.SendGump(new ImbuingGumpC(from, pm.Imbue_Item, pm.Imbue_Mod, pm.Imbue_ModInt));
                        break;
                    }
                case 10056: // = Maximum Mod Value [>>>]
                    {
                        pm.Imbue_ModInt += 100;
                        from.CloseGump(typeof(ImbuingGumpC));
                        from.SendGump(new ImbuingGumpC(from, pm.Imbue_Item, pm.Imbue_Mod, pm.Imbue_ModInt));
                        break;
                    }

                case 10099: // - Cancel
                    {
                        break;
                    }
                case 10100:  // = Imbue the Item
                    {
                        int i_TpropW = GetTotalWeight(pm.Imbue_Item); int i_TMods = GetTotalMods(pm.Imbue_Item);
                        double c_i = m_Intensity;
                        double c_w = s_Weight;

                        double cur_wei = c_i * (c_w / 100); cur_wei = Math.Round(cur_wei);

                        // = Check if Item Props or Weight aren't > Maximum 
                        if (i_TMods >= i_MaxProps || i_TpropW > pm.Imbue_IWmax)
                        {
                            from.SendLocalizedMessage(1079772); // You cannot imbue this item with any more item properties.
                            from.CloseGump(typeof(ImbuingGumpC));
                            return;
                        }

                        from.CloseGump(typeof(ImbuingGumpC));
                        ImbuingGumpC.ImbueItem(from, pm.Imbue_Item, pm.Imbue_Mod, pm.Imbue_ModInt);
                        from.SendGump(new ImbuingGump(from));
                        break;
                    }
            }
            return;
        }

        // =========== Check if Choosen Attribute Replaces Another =================
        public static string WhatReplacesWhat(int r_mod, object r_item)
        {
            string Replaces = "";

            if (r_item is BaseWeapon)
            {
                BaseWeapon i = r_item as BaseWeapon;

                // Slayers replace Slayers
                if (r_mod >= 101 && r_mod <= 126)
                {
                    if (i.Slayer != SlayerName.None)
                    {
                        if (i.Slayer == SlayerName.Silver) { Replaces = "Silver"; }
                        else if (i.Slayer == SlayerName.Repond) { Replaces = "Repond"; }
                        else if (i.Slayer == SlayerName.ReptilianDeath) { Replaces = "Reptilian Death"; }
                        else if (i.Slayer == SlayerName.Exorcism) { Replaces = "Exorcism"; }
                        else if (i.Slayer == SlayerName.ArachnidDoom) { Replaces = "Arachnid Doom"; }
                        else if (i.Slayer == SlayerName.ElementalBan) { Replaces = "Elemental Ban"; }
                        else if (i.Slayer == SlayerName.Fey) { Replaces = "Fey"; }
                        else if (i.Slayer == SlayerName.OrcSlaying) { Replaces = "Orc Slaying"; }
                        else if (i.Slayer == SlayerName.TrollSlaughter) { Replaces = "Troll Slaughter"; }
                        else if (i.Slayer == SlayerName.OgreTrashing) { Replaces = "Ogre Trashing"; }
                        else if (i.Slayer == SlayerName.DragonSlaying) { Replaces = "Dragon Slaying"; }
                        else if (i.Slayer == SlayerName.Terathan) { Replaces = "Terathan"; }
                        else if (i.Slayer == SlayerName.SnakesBane) { Replaces = "Snakes Bane"; }
                        else if (i.Slayer == SlayerName.LizardmanSlaughter) { Replaces = "Lizardman Slaughter"; }
                        else if (i.Slayer == SlayerName.GargoylesFoe) { Replaces = "Gargoyles Foe"; }
                        else if (i.Slayer == SlayerName.Ophidian) { Replaces = "Ophidian"; }
                        else if (i.Slayer == SlayerName.SpidersDeath) { Replaces = "Spiders Death"; }
                        else if (i.Slayer == SlayerName.ScorpionsBane) { Replaces = "Scorpions Bane"; }
                        else if (i.Slayer == SlayerName.FlameDousing) { Replaces = "Flame Dousing"; }
                        else if (i.Slayer == SlayerName.WaterDissipation) { Replaces = "Water Dissipation"; }
                        else if (i.Slayer == SlayerName.Vacuum) { Replaces = "Vacuum"; }
                        else if (i.Slayer == SlayerName.ElementalHealth) { Replaces = "Elemental Health"; }
                        else if (i.Slayer == SlayerName.EarthShatter) { Replaces = "Earth Shatter"; }
                        else if (i.Slayer == SlayerName.BloodDrinking) { Replaces = "Blood Drinking"; }
                        else if (i.Slayer == SlayerName.SummerWind) { Replaces = "Summer Wind"; }
                    }
                }
                // OnHitEffect replace OnHitEffect
                if (r_mod >= 35 && r_mod <= 39)
                {
                    if (i.WeaponAttributes.HitMagicArrow > 0) { Replaces = "Hit Magic Arrow"; }
                    else if (i.WeaponAttributes.HitHarm > 0) { Replaces = "Hit Harm"; }
                    else if (i.WeaponAttributes.HitFireball > 0) { Replaces = "Hit Fireball"; }
                    else if (i.WeaponAttributes.HitLightning > 0) { Replaces = "Hit Lightning"; }
                    else if (i.WeaponAttributes.HitDispel > 0) { Replaces = "Hit Dispel"; }
                }
                // OnHitArea replace OnHitArea
                if (r_mod >= 30 && r_mod <= 34)
                {
                    if (i.WeaponAttributes.HitPhysicalArea > 0) { Replaces = "Hit Physical Area"; }
                    else if (i.WeaponAttributes.HitColdArea > 0) { Replaces = "Hit Cold Area"; }
                    else if (i.WeaponAttributes.HitFireArea > 0) { Replaces = "Hit Fire Area"; }
                    else if (i.WeaponAttributes.HitPoisonArea > 0) { Replaces = "Hit Poison Area"; }
                    else if (i.WeaponAttributes.HitEnergyArea > 0) { Replaces = "Hit Energy Area"; }                     
                }
                // OnHitLeech replace OnHitLeech ??& HitLower??
                if (r_mod >= 25 && r_mod <= 27)
                {
                    if (i.WeaponAttributes.HitLeechHits > 0) { Replaces = "Hit Life Leech"; }
                    else if (i.WeaponAttributes.HitLeechStam > 0) { Replaces = "Hit Stamina Leech"; }
                    else if (i.WeaponAttributes.HitLeechMana > 0) { Replaces = "Hit Mana Area"; }
                }
                // HitLower replace HitLower 
                if (r_mod >= 28 && r_mod <= 29)
                {
                    if (i.WeaponAttributes.HitLowerAttack > 0) { Replaces = "Hit Lower Attack"; }
                    else if (i.WeaponAttributes.HitLowerDefend > 0) { Replaces = "Hit Lower Defend"; }
                }
            }
            if (r_item is BaseJewel)
            {
                BaseJewel i = r_item as BaseJewel;

                // SkillGroup1 replace SkillGroup1
                if (r_mod >= 151 && r_mod <= 155)
                {
                    if (i.SkillBonuses.GetBonus(0) > 0)
                    {
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Fencing) { Replaces = "Fencing"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Macing) { Replaces = "Mace Fighting"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Swords) { Replaces = "Swordsmanship"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Musicianship) { Replaces = "Musicianship"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Magery) { Replaces = "Magery"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Wrestling) { Replaces = "Wrestling"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.AnimalTaming) { Replaces = "Animal Taming"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.SpiritSpeak) { Replaces = "Spirit Speak"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Tactics) { Replaces = "Tactics"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Provocation) { Replaces = "Provocation"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Focus) { Replaces = "Focus"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Parry) { Replaces = "Parrying"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Stealth) { Replaces = "Stealth"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Meditation) { Replaces = "Meditation"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.AnimalLore) { Replaces = "Animal Lore"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Discordance) { Replaces = "Discordance"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Bushido) { Replaces = "Bushido"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Necromancy) { Replaces = "Necromancy"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Veterinary) { Replaces = "Veterinary"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Stealing) { Replaces = "Stealing"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.EvalInt) { Replaces = "Evaluating Intelligence"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Anatomy) { Replaces = "Anatomy"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Peacemaking) { Replaces = "Peacemaking"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Ninjitsu) { Replaces = "Ninjitsu"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Chivalry) { Replaces = "Chivalry"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Archery) { Replaces = "Archery"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.MagicResist) { Replaces = "Resisting Spells"; }
                        if (i.SkillBonuses.GetSkill(0) == SkillName.Healing) { Replaces = "Healing"; }
                    }
                }
                // SkillGroup2 replace SkillGroup2
                if (r_mod >= 156 && r_mod <= 160)
                {
                    if (i.SkillBonuses.GetBonus(1) > 0)
                    {
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Fencing) { Replaces = "Fencing"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Macing) { Replaces = "Mace Fighting"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Swords) { Replaces = "Swordsmanship"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Musicianship) { Replaces = "Musicianship"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Magery) { Replaces = "Magery"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Wrestling) { Replaces = "Wrestling"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.AnimalTaming) { Replaces = "Animal Taming"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.SpiritSpeak) { Replaces = "Spirit Speak"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Tactics) { Replaces = "Tactics"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Provocation) { Replaces = "Provocation"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Focus) { Replaces = "Focus"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Parry) { Replaces = "Parrying"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Stealth) { Replaces = "Stealth"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Meditation) { Replaces = "Meditation"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.AnimalLore) { Replaces = "Animal Lore"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Discordance) { Replaces = "Discordance"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Bushido) { Replaces = "Bushido"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Necromancy) { Replaces = "Necromancy"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Veterinary) { Replaces = "Veterinary"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Stealing) { Replaces = "Stealing"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.EvalInt) { Replaces = "Evaluating Intelligence"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Anatomy) { Replaces = "Anatomy"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Peacemaking) { Replaces = "Peacemaking"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Ninjitsu) { Replaces = "Ninjitsu"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Chivalry) { Replaces = "Chivalry"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Archery) { Replaces = "Archery"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.MagicResist) { Replaces = "Resisting Spells"; }
                        if (i.SkillBonuses.GetSkill(1) == SkillName.Healing) { Replaces = "Healing"; }
                    }
                }
                // SkillGroup3 replace SkillGroup3
                if (r_mod >= 161 && r_mod <= 166)
                {
                    if (i.SkillBonuses.GetBonus(2) > 0)
                    {
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Fencing) { Replaces = "Fencing"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Macing) { Replaces = "Mace Fighting"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Swords) { Replaces = "Swordsmanship"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Musicianship) { Replaces = "Musicianship"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Magery) { Replaces = "Magery"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Wrestling) { Replaces = "Wrestling"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.AnimalTaming) { Replaces = "Animal Taming"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.SpiritSpeak) { Replaces = "Spirit Speak"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Tactics) { Replaces = "Tactics"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Provocation) { Replaces = "Provocation"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Focus) { Replaces = "Focus"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Parry) { Replaces = "Parrying"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Stealth) { Replaces = "Stealth"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Meditation) { Replaces = "Meditation"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.AnimalLore) { Replaces = "Animal Lore"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Discordance) { Replaces = "Discordance"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Bushido) { Replaces = "Bushido"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Necromancy) { Replaces = "Necromancy"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Veterinary) { Replaces = "Veterinary"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Stealing) { Replaces = "Stealing"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.EvalInt) { Replaces = "Evaluating Intelligence"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Anatomy) { Replaces = "Anatomy"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Peacemaking) { Replaces = "Peacemaking"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Ninjitsu) { Replaces = "Ninjitsu"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Chivalry) { Replaces = "Chivalry"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Archery) { Replaces = "Archery"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.MagicResist) { Replaces = "Resisting Spells"; }
                        if (i.SkillBonuses.GetSkill(2) == SkillName.Healing) { Replaces = "Healing"; }
                    }
                }
                // SkillGroup4 replace SkillGroup4
                if (r_mod >= 167 && r_mod <= 172)
                {
                    if (i.SkillBonuses.GetBonus(3) > 0)
                    {
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Fencing) { Replaces = "Fencing"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Macing) { Replaces = "Mace Fighting"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Swords) { Replaces = "Swordsmanship"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Musicianship) { Replaces = "Musicianship"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Magery) { Replaces = "Magery"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Wrestling) { Replaces = "Wrestling"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.AnimalTaming) { Replaces = "Animal Taming"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.SpiritSpeak) { Replaces = "Spirit Speak"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Tactics) { Replaces = "Tactics"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Provocation) { Replaces = "Provocation"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Focus) { Replaces = "Focus"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Parry) { Replaces = "Parrying"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Stealth) { Replaces = "Stealth"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Meditation) { Replaces = "Meditation"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.AnimalLore) { Replaces = "Animal Lore"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Discordance) { Replaces = "Discordance"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Bushido) { Replaces = "Bushido"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Necromancy) { Replaces = "Necromancy"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Veterinary) { Replaces = "Veterinary"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Stealing) { Replaces = "Stealing"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.EvalInt) { Replaces = "Evaluating Intelligence"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Anatomy) { Replaces = "Anatomy"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Peacemaking) { Replaces = "Peacemaking"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Ninjitsu) { Replaces = "Ninjitsu"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Chivalry) { Replaces = "Chivalry"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Archery) { Replaces = "Archery"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.MagicResist) { Replaces = "Resisting Spells"; }
                        if (i.SkillBonuses.GetSkill(3) == SkillName.Healing) { Replaces = "Healing"; }
                    }
                }
                // SkillGroup5 replace SkillGroup5
                if (r_mod >= 173 && r_mod <= 178)
                {
                    if (i.SkillBonuses.GetBonus(4) > 0)
                    {
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Fencing) { Replaces = "Fencing"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Macing) { Replaces = "Mace Fighting"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Swords) { Replaces = "Swordsmanship"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Musicianship) { Replaces = "Musicianship"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Magery) { Replaces = "Magery"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Wrestling) { Replaces = "Wrestling"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.AnimalTaming) { Replaces = "Animal Taming"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.SpiritSpeak) { Replaces = "Spirit Speak"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Tactics) { Replaces = "Tactics"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Provocation) { Replaces = "Provocation"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Focus) { Replaces = "Focus"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Parry) { Replaces = "Parrying"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Stealth) { Replaces = "Stealth"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Meditation) { Replaces = "Meditation"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.AnimalLore) { Replaces = "Animal Lore"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Discordance) { Replaces = "Discordance"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Bushido) { Replaces = "Bushido"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Necromancy) { Replaces = "Necromancy"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Veterinary) { Replaces = "Veterinary"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Stealing) { Replaces = "Stealing"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.EvalInt) { Replaces = "Evaluating Intelligence"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Anatomy) { Replaces = "Anatomy"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Peacemaking) { Replaces = "Peacemaking"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Ninjitsu) { Replaces = "Ninjitsu"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Chivalry) { Replaces = "Chivalry"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Archery) { Replaces = "Archery"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.MagicResist) { Replaces = "Resisting Spells"; }
                        if (i.SkillBonuses.GetSkill(4) == SkillName.Healing) { Replaces = "Healing"; }
                    }
                }
            }

            return Replaces;
        }

        // ============== Calculate Total & Check for Mod Replacement ============
        public static int GetTotalWeight(object itw)
        {
            double oDo = 0;

            if (itw is BaseWeapon)
            {
                BaseWeapon i = itw as BaseWeapon;

                // - Ranged Weapons
                if (i is BaseRanged)
                {
                    BaseRanged r = i as BaseRanged;
                    if (r.Velocity > 0) { if (i_Mod != 60) { oDo += (130 / 50) * r.Velocity; } else { RepModName = "M'KAY!"; } }
                    if (r.Balanced == true) { if (i_Mod != 61) { oDo += 150; } else { RepModName = "M'KAY!"; } }
                    if (r.Attributes.DefendChance > 0) { if (i_Mod != 1) { oDo += (130 / 25) * r.Attributes.DefendChance; } else { RepModName = "M'KAY!"; } }
                    if (r.Attributes.AttackChance > 0) { if (i_Mod != 2) { oDo += (130 / 25) * r.Attributes.AttackChance; } else { RepModName = "M'KAY!"; } }
                    if (r.Attributes.Luck > 0) { if (i_Mod != 21) { oDo += (100 / 120) * r.Attributes.Luck; } else { RepModName = "M'KAY!"; } }
                    if (r.WeaponAttributes.ResistPhysicalBonus > 0) { if (i_Mod != 43) { oDo += (100 / 18) * r.WeaponAttributes.ResistPhysicalBonus; } else { RepModName = "M'KAY!"; } }
                    if (r.WeaponAttributes.ResistFireBonus > 0) { if (i_Mod != 44) { oDo += (100 / 18) * r.WeaponAttributes.ResistFireBonus; } else { RepModName = "M'KAY!"; } }
                    if (r.WeaponAttributes.ResistColdBonus > 0) { if (i_Mod != 45) { oDo += (100 / 18) * r.WeaponAttributes.ResistColdBonus; } else { RepModName = "M'KAY!"; } }
                    if (r.WeaponAttributes.ResistPoisonBonus > 0) { if (i_Mod != 46) { oDo += (100 / 18) * r.WeaponAttributes.ResistPoisonBonus; } else { RepModName = "M'KAY!"; } }
                    if (r.WeaponAttributes.ResistEnergyBonus > 0) { if (i_Mod != 47) { oDo += (100 / 18) * r.WeaponAttributes.ResistEnergyBonus; } else { RepModName = "M'KAY!"; } }
                }
                // - Melee Weapons
                else if (i is BaseMeleeWeapon)
                {
                    if (i.Attributes.DefendChance > 0) { if (i_Mod != 1) { oDo += (130 / 15) * i.Attributes.DefendChance; } else { RepModName = "M'KAY!"; } }
                    if (i.Attributes.AttackChance > 0) { if (i_Mod != 2) { oDo += (130 / 15) * i.Attributes.AttackChance; } else { RepModName = "M'KAY!"; } }
                    if (i.Attributes.Luck > 0) { if (i_Mod != 21) { oDo += i.Attributes.Luck; } else { RepModName = "M'KAY!"; } }
                    if (i.WeaponAttributes.ResistPhysicalBonus > 0) { if (i_Mod != 43) { oDo += (100 / 15) * i.WeaponAttributes.ResistPhysicalBonus; } else { RepModName = "M'KAY!"; } }
                    if (i.WeaponAttributes.ResistFireBonus > 0) { if (i_Mod != 44) { oDo += (100 / 15) * i.WeaponAttributes.ResistFireBonus; } else { RepModName = "M'KAY!"; } }
                    if (i.WeaponAttributes.ResistColdBonus > 0) { if (i_Mod != 45) { oDo += (100 / 15) * i.WeaponAttributes.ResistColdBonus; } else { RepModName = "M'KAY!"; } }
                    if (i.WeaponAttributes.ResistPoisonBonus > 0) { if (i_Mod != 46) { oDo += (100 / 15) * i.WeaponAttributes.ResistPoisonBonus; } else { RepModName = "M'KAY!"; } }
                    if (i.WeaponAttributes.ResistEnergyBonus > 0) { if (i_Mod != 47) { oDo += (100 / 15) * i.WeaponAttributes.ResistEnergyBonus; } else { RepModName = "M'KAY!"; } }
                }
                // - All Weapon Types
                if (i.Attributes.RegenHits > 0) { if (i_Mod != 3) { oDo += (50 * i.Attributes.RegenHits); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenStam > 0) { if (i_Mod != 4) { oDo += (100 / 3) * i.Attributes.RegenStam; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenMana > 0) { if (i_Mod != 5) { oDo += (50 * i.Attributes.RegenMana); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusStr > 0) { if (i_Mod != 6) { oDo += (110 / 8) * i.Attributes.BonusStr; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusDex > 0) { if (i_Mod != 7) { oDo += (110 / 8) * i.Attributes.BonusDex; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusInt > 0) { if (i_Mod != 8) { oDo += (110 / 8) * i.Attributes.BonusInt; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusHits > 0) { if (i_Mod != 9) { oDo += 22 * i.Attributes.BonusHits; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusStam > 0) { if (i_Mod != 10) { oDo += (100 / 8) * i.Attributes.BonusStam; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusMana > 0) { if (i_Mod != 11) { oDo += (110 / 8) * i.Attributes.BonusMana; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.WeaponDamage > 0) { if (i_Mod != 12) { oDo += (2 * i.Attributes.WeaponDamage); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.WeaponSpeed > 0) { if (i_Mod != 13) { oDo += (110 / 30) * i.Attributes.WeaponSpeed; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.SpellDamage > 0) { if (i_Mod != 14) { oDo += (100 / 12) * i.Attributes.SpellDamage; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.CastRecovery > 0) { if (i_Mod != 15) { oDo += (40 * i.Attributes.CastRecovery); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.LowerManaCost > 0) { if (i_Mod != 17) { oDo += (110 / 8) * i.Attributes.LowerManaCost; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.LowerRegCost > 0) { if (i_Mod != 18) { oDo += 5 * i.Attributes.LowerRegCost; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.ReflectPhysical > 0) { if (i_Mod != 19) { oDo += (100 / 15) * i.Attributes.ReflectPhysical; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.EnhancePotions > 0) { if (i_Mod != 20) { oDo += (4 * i.Attributes.EnhancePotions); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.NightSight > 0) { if (i_Mod != 23) { oDo += 50; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.SpellChanneling > 0)
                {
                    if (i_Mod != 22) { oDo += 100; } else { RepModName = "M'KAY!"; }
                    if (i.Attributes.CastSpeed == 0) { if (i_Mod != 16) { oDo += 140; } else { RepModName = "M'KAY!"; } }
                    if (i.Attributes.CastSpeed == 1) { if (i_Mod != 16) { oDo += 280; } else { RepModName = "M'KAY!"; } }
                }
                else if (i.Attributes.CastSpeed > 0) { if (i_Mod != 16) { oDo += (140 * i.Attributes.CastSpeed); } else { RepModName = "M'KAY!"; } }
               
                if (i.SkillBonuses.GetBonus(0) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(0)); }
                if (i.SkillBonuses.GetBonus(1) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(1)); }
                if (i.SkillBonuses.GetBonus(2) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(2)); }
                if (i.SkillBonuses.GetBonus(3) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(3)); }
                if (i.SkillBonuses.GetBonus(4) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(4)); }
                
                if (i.WeaponAttributes.LowerStatReq > 0) { if (i_Mod != 24) { oDo += i.WeaponAttributes.LowerStatReq; } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitLeechHits > 0) { if (i_Mod != 25 && i_Mod != 26 && i_Mod != 27) { oDo += ((110 / 50) * i.WeaponAttributes.HitLeechHits); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitLeechStam > 0) { if (i_Mod != 25 && i_Mod != 26 && i_Mod != 27) { oDo += (2 * i.WeaponAttributes.HitLeechStam); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitLeechMana > 0) { if (i_Mod != 25 && i_Mod != 26 && i_Mod != 27) { oDo += ((110 / 50) * i.WeaponAttributes.HitLeechMana); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitLowerAttack > 0) { if (i_Mod != 28 && i_Mod != 29) { oDo += ((110 / 50) * i.WeaponAttributes.HitLowerAttack); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitLowerDefend > 0) { if (i_Mod != 28 && i_Mod != 29) { oDo += ((130 / 50) * i.WeaponAttributes.HitLowerDefend); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitColdArea > 0) { if (i_Mod != 30 && i_Mod != 31 && i_Mod != 32 && i_Mod != 33 && i_Mod != 34) { oDo += (2 * i.WeaponAttributes.HitColdArea); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitFireArea > 0) { if (i_Mod != 30 && i_Mod != 31 && i_Mod != 32 && i_Mod != 33 && i_Mod != 34) { oDo += (2 * i.WeaponAttributes.HitFireArea); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitPoisonArea > 0) { if (i_Mod != 30 && i_Mod != 31 && i_Mod != 32 && i_Mod != 33 && i_Mod != 34) { oDo += (2 * i.WeaponAttributes.HitPoisonArea); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitEnergyArea > 0) { if (i_Mod != 30 && i_Mod != 31 && i_Mod != 32 && i_Mod != 33 && i_Mod != 34) { oDo += (2 * i.WeaponAttributes.HitEnergyArea); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitPhysicalArea > 0) { if (i_Mod != 30 && i_Mod != 31 && i_Mod != 32 && i_Mod != 33 && i_Mod != 34) { oDo += (2 * i.WeaponAttributes.HitPhysicalArea); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitMagicArrow > 0) { if (i_Mod != 35 && i_Mod != 36 && i_Mod != 37 && i_Mod != 38 && i_Mod != 39) { oDo += (2.4 * i.WeaponAttributes.HitMagicArrow); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitHarm > 0) { if (i_Mod != 35 && i_Mod != 36 && i_Mod != 37 && i_Mod != 38 && i_Mod != 39) { oDo += (2.2 * i.WeaponAttributes.HitHarm); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitFireball > 0) { if (i_Mod != 35 && i_Mod != 36 && i_Mod != 37 && i_Mod != 38 && i_Mod != 39) { oDo += (2.8 * i.WeaponAttributes.HitFireball); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitLightning > 0) { if (i_Mod != 35 && i_Mod != 36 && i_Mod != 37 && i_Mod != 38 && i_Mod != 39) { oDo += (2.8 * i.WeaponAttributes.HitLightning); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.HitDispel > 0) { if (i_Mod != 35 && i_Mod != 36 && i_Mod != 37 && i_Mod != 38 && i_Mod != 39) { oDo += (2 * i.WeaponAttributes.HitDispel); } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.UseBestSkill > 0) { if (i_Mod != 40) { oDo += 150; } else { RepModName = "M'KAY!"; } }
                if (i.WeaponAttributes.MageWeapon > 0) { if (i_Mod != 41) { oDo += 20 * i.WeaponAttributes.MageWeapon; } else { RepModName = "M'KAY!"; } }

                // SUPER Slayers
                if (i.Slayer == SlayerName.Silver) { if (i_Mod < 101 || i_Mod > 127) { oDo += 130; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.Repond) { if (i_Mod < 101 || i_Mod > 127) { oDo += 130; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.ReptilianDeath) { if (i_Mod < 101 || i_Mod > 127) { oDo += 130; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.Exorcism) { if (i_Mod < 101 || i_Mod > 127) { oDo += 130; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.ArachnidDoom) { if (i_Mod < 101 || i_Mod > 127) { oDo += 130; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.ElementalBan) { if (i_Mod < 101 || i_Mod > 127) { oDo += 130; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.Fey) {oDo += 130; }
                // Slayers
                else if (i.Slayer == SlayerName.OrcSlaying) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.TrollSlaughter) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.OgreTrashing) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.DragonSlaying) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.Terathan) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.SnakesBane) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.LizardmanSlaughter) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.GargoylesFoe) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.Ophidian) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.SpidersDeath) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.ScorpionsBane) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.FlameDousing) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.WaterDissipation) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.Vacuum) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.ElementalHealth) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.EarthShatter) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.BloodDrinking) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }
                else if (i.Slayer == SlayerName.SummerWind) { if (i_Mod < 101 || i_Mod > 127) { oDo += 110; } else { RepModName = "M'KAY!"; } }

                // SUPER Slayers (Slot 2)
                if (i.Slayer2 == SlayerName.Silver) { oDo += 130; }
                else if (i.Slayer2 == SlayerName.Repond) { oDo += 130; }
                else if (i.Slayer2 == SlayerName.ReptilianDeath) { oDo += 130; }
                else if (i.Slayer2 == SlayerName.Exorcism) { oDo += 130; }
                else if (i.Slayer2 == SlayerName.ArachnidDoom) { oDo += 130; }
                else if (i.Slayer2 == SlayerName.ElementalBan) { oDo += 130; }
                else if (i.Slayer2 == SlayerName.Fey) {oDo += 130; }
                // Slayers (Slot 2)
                else if (i.Slayer2 == SlayerName.OrcSlaying) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.TrollSlaughter) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.OgreTrashing) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.DragonSlaying) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.Terathan) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.SnakesBane) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.LizardmanSlaughter) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.GargoylesFoe) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.Ophidian) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.SpidersDeath) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.ScorpionsBane) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.FlameDousing) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.WaterDissipation) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.Vacuum) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.ElementalHealth) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.EarthShatter) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.BloodDrinking) { oDo += 110; }
                else if (i.Slayer2 == SlayerName.SummerWind) { oDo += 110; }
            }
            else if (itw is BaseArmor)
            {
                BaseArmor i = itw as BaseArmor;

                if (i.Attributes.DefendChance > 0) { if (i_Mod != 1) { oDo += ((130 / 15) * i.Attributes.DefendChance); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.AttackChance > 0) { if (i_Mod != 2) { oDo += ((130 / 15) * i.Attributes.AttackChance); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenHits > 0) { if (i_Mod != 3) { oDo += (50 * i.Attributes.RegenHits); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenStam > 0) { if (i_Mod != 4) { oDo += ((100 / 3) * i.Attributes.RegenStam); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenMana > 0) { if (i_Mod != 5) { oDo += (50 * i.Attributes.RegenMana); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusStr > 0) { if (i_Mod != 6) { oDo += ((110 / 8) * i.Attributes.BonusStr); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusDex > 0) { if (i_Mod != 7) { oDo += ((110 / 8) * i.Attributes.BonusDex); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusInt > 0) { if (i_Mod != 8) { oDo += ((110 / 8) * i.Attributes.BonusInt); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusHits > 0) { if (i_Mod != 9) { oDo += (22 * i.Attributes.BonusHits); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusStam > 0) { if (i_Mod != 10) { oDo += ((100 / 8) * i.Attributes.BonusStam); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusMana > 0) { if (i_Mod != 11) { oDo += ((110 / 8) * i.Attributes.BonusMana); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.WeaponDamage > 0) { if (i_Mod != 12) { oDo += (2 * i.Attributes.WeaponDamage); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.WeaponSpeed > 0) { if (i_Mod != 13) { oDo += ((110 / 30) * i.Attributes.WeaponSpeed); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.SpellDamage > 0) { if (i_Mod != 14) { oDo += ((100 / 12) * i.Attributes.SpellDamage); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.CastRecovery > 0) { if (i_Mod != 15) { oDo += (40 * i.Attributes.CastRecovery); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.LowerManaCost > 0) { if (i_Mod != 17) { oDo += ((110 / 8) * i.Attributes.LowerManaCost); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.LowerRegCost > 0) { if (i_Mod != 18) { oDo += (5 * i.Attributes.LowerRegCost); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.ReflectPhysical > 0) { if (i_Mod != 19) { oDo += ((100 / 15) * i.Attributes.ReflectPhysical); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.EnhancePotions > 0) { if (i_Mod != 20) { oDo += (4 * i.Attributes.EnhancePotions); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.Luck > 0) { if (i_Mod != 21) { oDo += i.Attributes.Luck; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.NightSight > 0) { if (i_Mod != 23) { oDo += 50; } else { RepModName = "M'KAY!"; } }
                if (i.ArmorAttributes.MageArmor > 0) { if (i_Mod != 49) { oDo += 140; } else { RepModName = "M'KAY!"; } }
                if (i.ArmorAttributes.DurabilityBonus > 0) { oDo += i.ArmorAttributes.DurabilityBonus; }
                if (i.ArmorAttributes.LowerStatReq > 0) { oDo += i.ArmorAttributes.LowerStatReq; }              
                if (i.Attributes.SpellChanneling > 0)
                {
                    if (i_Mod != 22) { oDo += 100; } else { RepModName = "M'KAY!"; }
                    if (i.Attributes.CastSpeed == 0) { if (i_Mod != 16) { oDo += 140; } else { RepModName = "M'KAY!"; } }
                    if (i.Attributes.CastSpeed == 1) { if (i_Mod != 16) { oDo += 280; } else { RepModName = "M'KAY!"; } }
                }
                else if (i.Attributes.CastSpeed > 0) { if (i_Mod != 16) { oDo += (140 * i.Attributes.CastSpeed); } else { RepModName = "M'KAY!"; } }
               
                if (i.SkillBonuses.GetBonus(0) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(0)); }
                if (i.SkillBonuses.GetBonus(1) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(1)); }
                if (i.SkillBonuses.GetBonus(2) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(2)); }
                if (i.SkillBonuses.GetBonus(3) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(3)); }
                if (i.SkillBonuses.GetBonus(4) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(4)); }

                if (i.Quality != ArmorQuality.Exceptional)
                {
                    if (i.PhysicalBonus > 0) { if (i_Mod != 51) { oDo += ((100 / 15) * i.PhysicalBonus); } else { RepModName = "M'KAY!"; } }
                    if (i.FireBonus > 0) { if (i_Mod != 52) { oDo += ((100 / 15) * i.FireBonus); } else { RepModName = "M'KAY!"; } }
                    if (i.ColdBonus > 0) { if (i_Mod != 53) { oDo += ((100 / 15) * i.ColdBonus); } else { RepModName = "M'KAY!"; } }
                    if (i.PoisonBonus > 0) { if (i_Mod != 54) { oDo += ((100 / 15) * i.PoisonBonus); } else { RepModName = "M'KAY!"; } }
                    if (i.EnergyBonus > 0) { if (i_Mod != 55) { oDo += ((100 / 15) * i.EnergyBonus); } else { RepModName = "M'KAY!"; } }
                }
                else if (i.Quality == ArmorQuality.Exceptional)
                {
                    if (i.Physical_Modded && i.PhysicalBonus > 0) { if (i_Mod != 51) { oDo += ((100 / 15) * i.PhysicalBonus); } else { RepModName = "M'KAY!"; } }
                    if (i.Fire_Modded && i.FireBonus > 0) { if (i_Mod != 52) { oDo += ((100 / 15) * i.FireBonus); } else { RepModName = "M'KAY!"; } }
                    if (i.Cold_Modded && i.ColdBonus > 0) { if (i_Mod != 53) { oDo += ((100 / 15) * i.ColdBonus); } else { RepModName = "M'KAY!"; } }
                    if (i.Poison_Modded && i.PoisonBonus > 0) { if (i_Mod != 54) { oDo += ((100 / 15) * i.PoisonBonus); } else { RepModName = "M'KAY!"; } }
                    if (i.Energy_Modded && i.EnergyBonus > 0) { if (i_Mod != 55) { oDo += ((100 / 15) * i.EnergyBonus); } else { RepModName = "M'KAY!"; } }
                }
            }
            else if (itw is BaseJewel)
            {
                BaseJewel i = itw as BaseJewel;

                if (i.Attributes.DefendChance > 0) { if (i_Mod != 1) { oDo += ((130 / 15) * i.Attributes.DefendChance); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.AttackChance > 0) { if (i_Mod != 2) { oDo += ((130 / 15) * i.Attributes.AttackChance); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenHits > 0) { if (i_Mod != 3) { oDo += (50 * i.Attributes.RegenHits); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenStam > 0) { if (i_Mod != 4) { oDo += ((100 / 3) * i.Attributes.RegenStam); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenMana > 0) { if (i_Mod != 5) { oDo += (50 * i.Attributes.RegenMana); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusStr > 0) { if (i_Mod != 6) { oDo += ((110 / 8) * i.Attributes.BonusStr); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusDex > 0) { if (i_Mod != 7) { oDo += ((110 / 8) * i.Attributes.BonusDex); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusInt > 0) { if (i_Mod != 8) { oDo += ((110 / 8) * i.Attributes.BonusInt); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusHits > 0) { if (i_Mod != 9) { oDo += (22 * i.Attributes.BonusHits); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusStam > 0) { if (i_Mod != 10) { oDo += ((100 / 8) * i.Attributes.BonusStam); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusMana > 0) { if (i_Mod != 11) { oDo += ((110 / 8) * i.Attributes.BonusMana); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.WeaponDamage > 0) { if (i_Mod != 12) { oDo += (2 * i.Attributes.WeaponDamage); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.WeaponSpeed > 0) { if (i_Mod != 13) { oDo += ((110 / 30) * i.Attributes.WeaponSpeed); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.SpellDamage > 0) { if (i_Mod != 14) { oDo += ((100 / 12) * i.Attributes.SpellDamage); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.CastRecovery > 0) { if (i_Mod != 15) { oDo += (40 * i.Attributes.CastRecovery); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.CastSpeed > 0) { if (i_Mod != 16) { oDo += (140 * i.Attributes.CastSpeed); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.LowerManaCost > 0) { if (i_Mod != 17) { oDo += ((110 / 8) * i.Attributes.LowerManaCost); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.LowerRegCost > 0) { if (i_Mod != 18) { oDo += (5 * i.Attributes.LowerRegCost); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.ReflectPhysical > 0) { if (i_Mod != 19) { oDo += ((100 / 15) * i.Attributes.ReflectPhysical); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.EnhancePotions > 0) { if (i_Mod != 20) { oDo += (4 * i.Attributes.EnhancePotions); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.Luck > 0) { if (i_Mod != 21) { oDo += i.Attributes.Luck; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.SpellChanneling > 0) { if (i_Mod != 22) { oDo += 100; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.NightSight > 0) { if (i_Mod != 23) { oDo += 50; } else { RepModName = "M'KAY!"; } }
                
                if (i.SkillBonuses.GetBonus(0) > 0) { if (i_Mod < 151 || i_Mod > 155) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(0)); } else { RepModName = "M'KAY!"; } }
                if (i.SkillBonuses.GetBonus(1) > 0) { if (i_Mod < 156 || i_Mod > 160) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(1)); } else { RepModName = "M'KAY!"; } }
                if (i.SkillBonuses.GetBonus(2) > 0) { if (i_Mod < 161 || i_Mod > 166) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(2)); } else { RepModName = "M'KAY!"; } }
                if (i.SkillBonuses.GetBonus(3) > 0) { if (i_Mod < 167 || i_Mod > 172) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(3)); } else { RepModName = "M'KAY!"; } }
                if (i.SkillBonuses.GetBonus(4) > 0) { if (i_Mod < 173 || i_Mod > 178) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(4)); } else { RepModName = "M'KAY!"; } }

                if (i.Resistances.Physical > 0) { if (i_Mod != 51) { oDo += ((100 / 15) * i.Resistances.Physical); } else { RepModName = "M'KAY!"; } }
                if (i.Resistances.Fire > 0) { if (i_Mod != 52) { oDo += ((100 / 15) * i.Resistances.Fire); } else { RepModName = "M'KAY!"; } }
                if (i.Resistances.Cold > 0) { if (i_Mod != 53) { oDo += ((100 / 15) * i.Resistances.Cold); } else { RepModName = "M'KAY!"; } }
                if (i.Resistances.Poison > 0) { if (i_Mod != 54) { oDo += ((100 / 15) * i.Resistances.Poison); } else { RepModName = "M'KAY!"; } }
                if (i.Resistances.Energy > 0) { if (i_Mod != 55) { oDo += ((100 / 15) * i.Resistances.Energy); } else { RepModName = "M'KAY!"; } }      
            }
            else if (itw is BaseHat)
            {
                BaseHat i = itw as BaseHat;

                if (i.Attributes.DefendChance > 0) { if (i_Mod != 1) { oDo += ((130 / 15) * i.Attributes.DefendChance); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.AttackChance > 0) { if (i_Mod != 2) { oDo += ((130 / 15) * i.Attributes.AttackChance); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenHits > 0) { if (i_Mod != 3) { oDo += (50 * i.Attributes.RegenHits); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenStam > 0) { if (i_Mod != 4) { oDo += ((100 / 3) * i.Attributes.RegenStam); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.RegenMana > 0) { if (i_Mod != 5) { oDo += (50 * i.Attributes.RegenMana); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusStr > 0) { if (i_Mod != 6) { oDo += ((110 / 8) * i.Attributes.BonusStr); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusDex > 0) { if (i_Mod != 7) { oDo += ((110 / 8) * i.Attributes.BonusDex); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusInt > 0) { if (i_Mod != 8) { oDo += ((110 / 8) * i.Attributes.BonusInt); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusHits > 0) { if (i_Mod != 9) { oDo += (22 * i.Attributes.BonusHits); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusStam > 0) { if (i_Mod != 10) { oDo += ((100 / 8) * i.Attributes.BonusStam); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.BonusMana > 0) { if (i_Mod != 11) { oDo += ((110 / 8) * i.Attributes.BonusMana); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.WeaponDamage > 0) { if (i_Mod != 12) { oDo += (2 * i.Attributes.WeaponDamage); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.WeaponSpeed > 0) { if (i_Mod != 13) { oDo += ((110 / 30) * i.Attributes.WeaponSpeed); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.SpellDamage > 0) { if (i_Mod != 14) { oDo += ((100 / 12) * i.Attributes.SpellDamage); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.CastRecovery > 0) { if (i_Mod != 15) { oDo += (40 * i.Attributes.CastRecovery); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.CastSpeed > 0) { if (i_Mod != 16) { oDo += (140 * i.Attributes.CastSpeed); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.LowerManaCost > 0) { if (i_Mod != 17) { oDo += ((110 / 8) * i.Attributes.LowerManaCost); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.LowerRegCost > 0) { if (i_Mod != 18) { oDo += (5 * i.Attributes.LowerRegCost); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.ReflectPhysical > 0) { if (i_Mod != 19) { oDo += ((100 / 15) * i.Attributes.ReflectPhysical); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.EnhancePotions > 0) { if (i_Mod != 20) { oDo += (4 * i.Attributes.EnhancePotions); } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.Luck > 0) { if (i_Mod != 21) { oDo += i.Attributes.Luck; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.SpellChanneling > 0) { if (i_Mod != 22) { oDo += 100; } else { RepModName = "M'KAY!"; } }
                if (i.Attributes.NightSight > 0) { if (i_Mod != 23) { oDo += 50; } else { RepModName = "M'KAY!"; } }
                if (i.ClothingAttributes.DurabilityBonus > 0) { oDo += i.ClothingAttributes.DurabilityBonus; }
                if (i.ClothingAttributes.LowerStatReq > 0) { oDo += i.ClothingAttributes.LowerStatReq; }
              
                if (i.SkillBonuses.GetBonus(0) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(0)); }
                if (i.SkillBonuses.GetBonus(1) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(1)); }
                if (i.SkillBonuses.GetBonus(2) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(2)); }
                if (i.SkillBonuses.GetBonus(3) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(3)); }
                if (i.SkillBonuses.GetBonus(4) > 0) { oDo += ((140 / 15) * i.SkillBonuses.GetBonus(4)); }

                if (i.Quality != ClothingQuality.Exceptional)
                {
                    if (i.Resistances.Physical > 0) { if ( i_Mod != 51) { oDo += (100 / 15) * i.Resistances.Physical; } else { RepModName = "M'KAY!"; } }
                    if (i.Resistances.Fire > 0) { if (i_Mod != 52) { oDo += (100 / 15) * i.Resistances.Fire; } else { RepModName = "M'KAY!"; } }
                    if (i.Resistances.Cold > 0) { if ( i_Mod != 53) { oDo += (100 / 15) * i.Resistances.Cold; } else { RepModName = "M'KAY!"; } }
                    if (i.Resistances.Poison > 0) { if ( i_Mod != 54) { oDo += (100 / 15) * i.Resistances.Poison; } else { RepModName = "M'KAY!"; } }
                    if (i.Resistances.Energy > 0) { if ( i_Mod != 55) { oDo += (100 / 15) * i.Resistances.Energy; } else { RepModName = "M'KAY!"; } }
                }
                else if (i.Quality == ClothingQuality.Exceptional)
                {
                    if (i.Physical_Modded && i.Resistances.Physical > 0) { if ( i_Mod != 51) { oDo += ((100 / 15) * i.Resistances.Physical); } else { RepModName = "M'KAY!"; } }
                    if (i.Fire_Modded && i.Resistances.Fire > 0) { if ( i_Mod != 52) { oDo += ((100 / 15) * i.Resistances.Fire); } else { RepModName = "M'KAY!"; } }
                    if (i.Cold_Modded && i.Resistances.Cold > 0) { if ( i_Mod != 53) { oDo += ((100 / 15) * i.Resistances.Cold); } else { RepModName = "M'KAY!"; } }
                    if (i.Poison_Modded && i.Resistances.Poison > 0) { if ( i_Mod != 54) { oDo += ((100 / 15) * i.Resistances.Poison); } else { RepModName = "M'KAY!"; } }
                    if (i.Energy_Modded && i.Resistances.Energy > 0) { if (i_Mod != 55) { oDo += ((100 / 15) * i.Resistances.Energy); } else { RepModName = "M'KAY!"; } }
                }
            }
            oDo = Math.Round(oDo);
            int oInt = Convert.ToInt32(oDo);
            return oInt;
        }

        // ========== Count Item Attributes =========================================
        public static int GetTotalMods(object itw)
        {
            int oMods = 0;

            if (itw is BaseWeapon)
            {
                BaseWeapon i = itw as BaseWeapon;

                // - Ranged Weapon
                if (itw is BaseRanged)
                {
                    BaseRanged r = itw as BaseRanged;
                    if (r.Velocity > 0) { oMods += 1; }
                    if (r.Balanced == true) { oMods += 1; }
                }
                // - All Weapon Types
                if (i.Attributes.DefendChance > 0 && i_Mod != 1) { oMods += 1; }
                if (i.Attributes.AttackChance > 0 && i_Mod != 2) { oMods += 1; }
                if (i.Attributes.RegenHits > 0 && i_Mod != 3) { oMods += 1; }
                if (i.Attributes.RegenStam > 0 && i_Mod != 4) { oMods += 1; }
                if (i.Attributes.RegenMana > 0 && i_Mod != 5) { oMods += 1; }
                if (i.Attributes.BonusStr > 0 && i_Mod != 6) { oMods += 1; }
                if (i.Attributes.BonusDex > 0 && i_Mod != 7) { oMods += 1; }
                if (i.Attributes.BonusInt > 0 && i_Mod != 8) { oMods += 1; }
                if (i.Attributes.BonusHits > 0 && i_Mod != 9) { oMods += 1; }
                if (i.Attributes.BonusStam > 0 && i_Mod != 10) { oMods += 1; }
                if (i.Attributes.BonusMana > 0 && i_Mod != 11) { oMods += 1; }
                if (i.Attributes.WeaponDamage > 0 && i.DImodded == false && i_Mod != 12) { oMods += 1; }
                if (i.Attributes.WeaponSpeed > 0 && i_Mod != 13) { oMods += 1; }
                if (i.Attributes.SpellDamage > 0 && i_Mod != 14) { oMods += 1; }
                if (i.Attributes.CastRecovery > 0 && i_Mod != 15) { oMods += 1; }
                if (i.Attributes.LowerManaCost > 0 && i_Mod != 17) { oMods += 1; }
                if (i.Attributes.LowerRegCost > 0 && i_Mod != 18) { oMods += 1; }
                if (i.Attributes.ReflectPhysical > 0 && i_Mod != 19) { oMods += 1; }
                if (i.Attributes.EnhancePotions > 0 && i_Mod != 20) { oMods += 1; }
                if (i.Attributes.Luck > 0 && i_Mod != 21) { oMods += 1; }
                if (i.Attributes.NightSight > 0 && i_Mod != 23) { oMods += 1; }

                if (i.Attributes.SpellChanneling > 0 && i_Mod != 22)
                {
                    oMods += 1;
                    if (i.Attributes.CastSpeed == 0 && i_Mod != 16) { oMods += 1; }
                    if (i.Attributes.CastSpeed == 1 && i_Mod != 16) { oMods += 1; }
                }
                else if (i.Attributes.CastSpeed > 0 && i_Mod != 16) { oMods += 1; }
                               
                if (i.SkillBonuses.GetBonus(0) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(1) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(2) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(3) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(4) > 0) { oMods += 1; }
                
                if (i.WeaponAttributes.LowerStatReq > 0 && i_Mod != 24) { oMods += 1; }
                if (i.WeaponAttributes.HitLeechHits > 0 && (i_Mod != 25 && i_Mod != 26 && i_Mod != 27)) { oMods += 1; }
                if (i.WeaponAttributes.HitLeechStam > 0 && (i_Mod != 25 && i_Mod != 26 && i_Mod != 27)) { oMods += 1; }
                if (i.WeaponAttributes.HitLeechMana > 0 && (i_Mod != 25 && i_Mod != 26 && i_Mod != 27)) { oMods += 1; }
                if (i.WeaponAttributes.HitLowerAttack > 0 && (i_Mod != 28 && i_Mod != 29)) { oMods += 1; }
                if (i.WeaponAttributes.HitLowerDefend > 0 && (i_Mod != 28 && i_Mod != 29)) { oMods += 1; }
                if (i.WeaponAttributes.HitColdArea > 0 && (i_Mod != 30 && i_Mod != 31 && i_Mod != 32 && i_Mod != 33 && i_Mod != 34)) { oMods += 1; }
                if (i.WeaponAttributes.HitFireArea > 0 && (i_Mod != 30 && i_Mod != 31 && i_Mod != 32 && i_Mod != 33 && i_Mod != 34)) { oMods += 1; }
                if (i.WeaponAttributes.HitPoisonArea > 0 && (i_Mod != 30 && i_Mod != 31 && i_Mod != 32 && i_Mod != 33 && i_Mod != 34)) { oMods += 1; }
                if (i.WeaponAttributes.HitEnergyArea > 0 && (i_Mod != 30 && i_Mod != 31 && i_Mod != 32 && i_Mod != 33 && i_Mod != 34)) { oMods += 1; }
                if (i.WeaponAttributes.HitPhysicalArea > 0 && (i_Mod != 30 && i_Mod != 31 && i_Mod != 32 && i_Mod != 33 && i_Mod != 34)) { oMods += 1; }
                if (i.WeaponAttributes.HitMagicArrow > 0 && (i_Mod != 35 && i_Mod != 36 && i_Mod != 37 && i_Mod != 38 && i_Mod != 39)) { oMods += 1; }
                if (i.WeaponAttributes.HitHarm > 0 && (i_Mod != 35 && i_Mod != 36 && i_Mod != 37 && i_Mod != 38 && i_Mod != 39)) { oMods += 1; }
                if (i.WeaponAttributes.HitFireball > 0 && (i_Mod != 35 && i_Mod != 36 && i_Mod != 37 && i_Mod != 38 && i_Mod != 39)) { oMods += 1; }
                if (i.WeaponAttributes.HitLightning > 0 && (i_Mod != 35 && i_Mod != 36 && i_Mod != 37 && i_Mod != 38 && i_Mod != 39)) { oMods += 1; }
                if (i.WeaponAttributes.HitDispel > 0 && (i_Mod != 35 && i_Mod != 36 && i_Mod != 37 && i_Mod != 38 && i_Mod != 39)) { oMods += 1; }
                if (i.WeaponAttributes.UseBestSkill > 0 && i_Mod != 40) { oMods += 1; }
                if (i.WeaponAttributes.MageWeapon > 0 && i_Mod != 41) { oMods += 1; }
                if (i.WeaponAttributes.ResistPhysicalBonus > 0 && i_Mod != 51) { oMods += 1; }
                if (i.WeaponAttributes.ResistFireBonus > 0 && i_Mod != 52) { oMods += 1; }
                if (i.WeaponAttributes.ResistColdBonus > 0 && i_Mod != 53) { oMods += 1; }
                if (i.WeaponAttributes.ResistPoisonBonus > 0 && i_Mod != 54) { oMods += 1; }
                if (i.WeaponAttributes.ResistEnergyBonus > 0 && i_Mod != 55) { oMods += 1; }
                if (i.Slayer != SlayerName.None && (i_Mod < 101 || i_Mod > 127)) { oMods += 1; }
                if (i.Slayer2 != SlayerName.None) { oMods += 1; }
            }
            else if (i_Item is BaseArmor)
            {
                BaseArmor i = i_Item as BaseArmor;

                if (i.Attributes.DefendChance > 0 && i_Mod != 1) { oMods += 1; }
                if (i.Attributes.AttackChance > 0 && i_Mod != 2) { oMods += 1; }
                if (i.Attributes.RegenHits > 0 && i_Mod != 3) { oMods += 1; }
                if (i.Attributes.RegenStam > 0 && i_Mod != 4) { oMods += 1; }
                if (i.Attributes.RegenMana > 0 && i_Mod != 5) { oMods += 1; }
                if (i.Attributes.BonusStr > 0 && i_Mod != 6) { oMods += 1; }
                if (i.Attributes.BonusDex > 0 && i_Mod != 7) { oMods += 1; }
                if (i.Attributes.BonusInt > 0 && i_Mod != 8) { oMods += 1; }
                if (i.Attributes.BonusHits > 0 && i_Mod != 9) { oMods += 1; }
                if (i.Attributes.BonusStam > 0 && i_Mod != 10) { oMods += 1; }
                if (i.Attributes.BonusMana > 0 && i_Mod != 11) { oMods += 1; }
                if (i.Attributes.WeaponDamage > 0 && i_Mod != 12) { oMods += 1; }
                if (i.Attributes.WeaponSpeed > 0 && i_Mod != 13) { oMods += 1; }
                if (i.Attributes.SpellDamage > 0 && i_Mod != 14) { oMods += 1; }
                if (i.Attributes.CastRecovery > 0 && i_Mod != 15) { oMods += 1; }
                if (i.Attributes.LowerManaCost > 0 && i_Mod != 17) { oMods += 1; }
                if (i.Attributes.LowerRegCost > 0 && i_Mod != 18) { oMods += 1; }
                if (i.Attributes.ReflectPhysical > 0 && i_Mod != 19) { oMods += 1; }
                if (i.Attributes.EnhancePotions > 0 && i_Mod != 20) { oMods += 1; }
                if (i.Attributes.Luck > 0 && i_Mod != 21) { oMods += 1; }
                if (i.Attributes.NightSight > 0 && i_Mod != 23) { oMods += 1; }
                if (i.ArmorAttributes.LowerStatReq > 0) { oMods += 1; }
                if (i.ArmorAttributes.MageArmor > 0) { oMods += 1; }
                if (i.Attributes.SpellChanneling > 0 && i_Mod != 22)
                {
                    oMods += 1;
                    if (i.Attributes.CastSpeed == 0 && i_Mod != 16) { oMods += 1; }
                    if (i.Attributes.CastSpeed == 1 && i_Mod != 16) { oMods += 1; }
                }
                else if (i.Attributes.CastSpeed > 0) { oMods += 1; }
                               
                if (i.SkillBonuses.GetBonus(0) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(1) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(2) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(3) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(4) > 0) { oMods += 1; }
                              
                if (i.Quality != ArmorQuality.Exceptional)
                {
                    if (i.PhysicalBonus > 0 && i_Mod != 51) { oMods += 1; }
                    if (i.FireBonus > 0 && i_Mod != 52) { oMods += 1; }
                    if (i.ColdBonus > 0 && i_Mod != 53) { oMods += 1; }
                    if (i.PoisonBonus > 0 && i_Mod != 54) { oMods += 1; }
                    if (i.EnergyBonus > 0 && i_Mod != 55) { oMods += 1; }
                }
                else if (i.Quality == ArmorQuality.Exceptional)
                {
                    if (i.Physical_Modded && i.PhysicalBonus > 0 && i_Mod != 51) { oMods += 1; }
                    if (i.Fire_Modded && i.FireBonus > 0 && i_Mod != 52) { oMods += 1; }
                    if (i.Cold_Modded && i.ColdBonus > 0 && i_Mod != 53) { oMods += 1; }
                    if (i.Poison_Modded && i.PoisonBonus > 0 && i_Mod != 54) { oMods += 1; }
                    if (i.Energy_Modded && i.EnergyBonus > 0 && i_Mod != 55) { oMods += 1; }
                }
            }
            else if (i_Item is BaseJewel)
            {
                BaseJewel i = i_Item as BaseJewel;

                if (i.Attributes.DefendChance > 0 && i_Mod != 1) { oMods += 1; }
                if (i.Attributes.AttackChance > 0 && i_Mod != 2) { oMods += 1; }
                if (i.Attributes.RegenHits > 0 && i_Mod != 3) { oMods += 1; }
                if (i.Attributes.RegenStam > 0 && i_Mod != 4) { oMods += 1; }
                if (i.Attributes.RegenMana > 0 && i_Mod != 5) { oMods += 1; }
                if (i.Attributes.BonusStr > 0 && i_Mod != 6) { oMods += 1; }
                if (i.Attributes.BonusDex > 0 && i_Mod != 7) { oMods += 1; }
                if (i.Attributes.BonusInt > 0 && i_Mod != 8) { oMods += 1; }
                if (i.Attributes.BonusHits > 0 && i_Mod != 9) { oMods += 1; }
                if (i.Attributes.BonusStam > 0 && i_Mod != 10) { oMods += 1; }
                if (i.Attributes.BonusMana > 0 && i_Mod != 11) { oMods += 1; }
                if (i.Attributes.WeaponDamage > 0 && i_Mod != 12) { oMods += 1; }
                if (i.Attributes.WeaponSpeed > 0 && i_Mod != 13) { oMods += 1; }
                if (i.Attributes.SpellDamage > 0 && i_Mod != 14) { oMods += 1; }
                if (i.Attributes.CastRecovery > 0 && i_Mod != 15) { oMods += 1; }
                if (i.Attributes.CastSpeed > 0 && i_Mod != 16) { oMods += 1; }
                if (i.Attributes.LowerManaCost > 0 && i_Mod != 17) { oMods += 1; }
                if (i.Attributes.LowerRegCost > 0 && i_Mod != 18) { oMods += 1; }
                if (i.Attributes.ReflectPhysical > 0 && i_Mod != 19) { oMods += 1; }
                if (i.Attributes.EnhancePotions > 0 && i_Mod != 20) { oMods += 1; }
                if (i.Attributes.Luck > 0 && i_Mod != 21) { oMods += 1; }
                if (i.Attributes.SpellChanneling > 0 && i_Mod != 22) { oMods += 1; }
                if (i.Attributes.NightSight > 0 && i_Mod != 23) { oMods += 1; }
                
                if (i.SkillBonuses.GetBonus(0) > 0 && (i_Mod < 151 || i_Mod > 155)) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(1) > 0 && (i_Mod < 156 || i_Mod > 160)) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(2) > 0 && (i_Mod < 161 || i_Mod > 166)) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(3) > 0 && (i_Mod < 167 || i_Mod > 172)) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(4) > 0 && (i_Mod < 173 || i_Mod > 178)) { oMods += 1; }

                if (i.Resistances.Physical > 0 && i_Mod != 51) { oMods += 1; }
                if (i.Resistances.Fire > 0 && i_Mod != 52) { oMods += 1; }
                if (i.Resistances.Cold > 0 && i_Mod != 53) { oMods += 1; }
                if (i.Resistances.Poison > 0 && i_Mod != 54) { oMods += 1; }
                if (i.Resistances.Energy > 0 && i_Mod != 55) { oMods += 1; }
            }
            else if (i_Item is BaseHat)
            {
                BaseHat i = i_Item as BaseHat;

                if (i.Attributes.DefendChance > 0 && i_Mod != 1) { oMods += 1; }
                if (i.Attributes.AttackChance > 0 && i_Mod != 2) { oMods += 1; }
                if (i.Attributes.RegenHits > 0 && i_Mod != 3) { oMods += 1; }
                if (i.Attributes.RegenStam > 0 && i_Mod != 4) { oMods += 1; }
                if (i.Attributes.RegenMana > 0 && i_Mod != 5) { oMods += 1; }
                if (i.Attributes.BonusStr > 0 && i_Mod != 6) { oMods += 1; }
                if (i.Attributes.BonusDex > 0 && i_Mod != 7) { oMods += 1; }
                if (i.Attributes.BonusInt > 0 && i_Mod != 8) { oMods += 1; }
                if (i.Attributes.BonusHits > 0 && i_Mod != 9) { oMods += 1; }
                if (i.Attributes.BonusStam > 0 && i_Mod != 10) { oMods += 1; }
                if (i.Attributes.BonusMana > 0 && i_Mod != 11) { oMods += 1; }
                if (i.Attributes.WeaponDamage > 0 && i_Mod != 12) { oMods += 1; }
                if (i.Attributes.WeaponSpeed > 0 && i_Mod != 13) { oMods += 1; }
                if (i.Attributes.SpellDamage > 0 && i_Mod != 14) { oMods += 1; }
                if (i.Attributes.CastRecovery > 0 && i_Mod != 15) { oMods += 1; }
                if (i.Attributes.CastSpeed > 0 && i_Mod != 16) { oMods += 1; }
                if (i.Attributes.LowerManaCost > 0 && i_Mod != 17) { oMods += 1; }
                if (i.Attributes.LowerRegCost > 0 && i_Mod != 18) { oMods += 1; }
                if (i.Attributes.ReflectPhysical > 0 && i_Mod != 19) { oMods += 1; }
                if (i.Attributes.EnhancePotions > 0 && i_Mod != 20) { oMods += 1; }
                if (i.Attributes.Luck > 0 && i_Mod != 21) { oMods += 1; }
                if (i.Attributes.SpellChanneling > 0 && i_Mod != 22) { oMods += 1; }
                if (i.Attributes.NightSight > 0 && i_Mod != 23) { oMods += 1; }
                
                if (i.SkillBonuses.GetBonus(0) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(1) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(2) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(3) > 0) { oMods += 1; }
                if (i.SkillBonuses.GetBonus(4) > 0) { oMods += 1; }

                if (i.Quality != ClothingQuality.Exceptional)
                {
                    if (i.Resistances.Physical > 0 && i_Mod != 51) { oMods += 1; }
                    if (i.Resistances.Fire > 0 && i_Mod != 52) { oMods += 1; }
                    if (i.Resistances.Cold > 0 && i_Mod != 53) { oMods += 1; }
                    if (i.Resistances.Poison > 0 && i_Mod != 54) { oMods += 1; }
                    if (i.Resistances.Energy > 0 && i_Mod != 55) { oMods += 1; }
                }
                else if (i.Quality == ClothingQuality.Exceptional)
                {
                    if (i.Physical_Modded && i.Resistances.Physical > 0 && i_Mod != 51) { oMods += 1; }
                    if (i.Fire_Modded && i.Resistances.Fire > 0 && i_Mod != 52) { oMods += 1; }
                    if (i.Cold_Modded && i.Resistances.Cold > 0 && i_Mod != 53) { oMods += 1; }
                    if (i.Poison_Modded && i.Resistances.Poison > 0 && i_Mod != 54) { oMods += 1; }
                    if (i.Energy_Modded && i.Resistances.Energy > 0 && i_Mod != 55) { oMods += 1; }
                }
            }

            return oMods;
        }

        // ======== Item Maximum Weight [Exeptional = 500 / Normal = 450] ===========
        public static int GetMaxWeight(object itw)
        {
            int MaxW = 450;

            if (itw is BaseWeapon)
            {
                BaseWeapon tit = itw as BaseWeapon;
                if (tit.Quality == WeaponQuality.Exceptional)
                    MaxW = 500;
                else if (tit.Quality == WeaponQuality.Regular)
                    MaxW = 450;
                else
                    MaxW = 400;
            }
            else if (itw is BaseArmor)
            {
                BaseArmor tit = itw as BaseArmor;
                if (tit.Quality == ArmorQuality.Exceptional)
                    MaxW = 500;
                else if (tit.Quality == ArmorQuality.Regular)
                    MaxW = 450;
                else
                    MaxW = 400;
            }
            else if (itw is BaseHat)
            {
                BaseHat tit = itw as BaseHat;
                if (tit.Quality == ClothingQuality.Exceptional)
                    MaxW = 500;
                else if (tit.Quality == ClothingQuality.Regular)
                    MaxW = 450;
                else
                    MaxW = 400;
            }
            else if (itw is BaseJewel)
            {
                BaseJewel tit = itw as BaseJewel;
                if (tit.Quality == ArmorQuality.Exceptional)
                    MaxW = 500;
                else if (tit.Quality == ArmorQuality.Regular)
                    MaxW = 450;
                else
                    MaxW = 400;
            }

            return MaxW;
        }

        // ============== Imbue Item with selected Properties ========================
        public static void ImbueItem(Mobile from, object i, int mod, int Mvalue)
        {
            PlayerMobile pm = from as PlayerMobile;
            pm.Imbue_Item = i; pm.Imbue_Mod = mod; pm.Imbue_ModInt = Mvalue;

            i_Item = pm.Imbue_Item;
            i_Mod = pm.Imbue_Mod;
            modvalue = pm.Imbue_ModInt;

            // = Check Type and Quantity of Ingredients Needed 
            Type res_gem = null; Type res_a = null; Type res_b = null;
            ImbuingGumpC.GetMaterials(i_Mod);
            pm.Imbue_ModVal = s_Weight;

            // = Ingredient List
            if (m_Gem == "Diamond") { res_gem = typeof(Diamond); }
            if (m_Gem == "Emerald") { res_gem = typeof(Emerald); }
            if (m_Gem == "Ruby") { res_gem = typeof(Ruby); }
            if (m_Gem == "Citrine") { res_gem = typeof(Citrine); }
            if (m_Gem == "Tourmaline") { res_gem = typeof(Tourmaline); }
            if (m_Gem == "Amber") { res_gem = typeof(Amber); }
            if (m_Gem == "Amethyst") { res_gem = typeof(Amethyst); }
            if (m_Gem == "Sapphire") { res_gem = typeof(Sapphire); }
            if (m_Gem == "Star Sapphire") { res_gem = typeof(StarSapphire); }
            if (m_A == "Magical Residue") { res_a = typeof(MagicalResidue); }
            if (m_A == "Enchanted Essence") { res_a = typeof(EnchantEssence); }
            if (m_A == "Relic Fragment") { res_a = typeof(RelicFragment); }
            if (m_B == "Essence of Persistence") { res_b = typeof(EssencePersistence); }
            if (m_B == "Essence of Singularity") { res_b = typeof(EssenceSingularity); }
            if (m_B == "Essence of Precision") { res_b = typeof(EssencePrecision); }
            if (m_B == "Essence of Diligence") { res_b = typeof(EssenceDiligence); }
            if (m_B == "Essence of Achievement") { res_b = typeof(EssenceAchievement); }
            if (m_B == "Essence of Order") { res_b = typeof(EssenceOrder); }
            if (m_B == "Essence of Feeling") { res_b = typeof(EssenceFeeling); }
            if (m_B == "Essence of Passion") { res_b = typeof(EssencePassion); }
            if (m_B == "Essence of Direction") { res_b = typeof(EssenceDirection); }
            if (m_B == "Essence of Balance") { res_b = typeof(EssenceBalance); }
            if (m_B == "Essence of Control") { res_b = typeof(EssenceControl); }
            if (m_B == "Fire Ruby") { res_b = typeof(FireRuby); }
            if (m_B == "Blue Diamond") { res_b = typeof(BlueDiamond); }
            if (m_B == "Turquoise") { res_b = typeof(Turquoise); }
            if (m_B == "Delicate Scales") { res_b = typeof(DelicateScales); }
            if (m_B == "White Pearl") { res_b = typeof(WhitePearl); }
            if (m_B == "Luminescent Fungi") { res_b = typeof(LuminescentFungi); }
            if (m_B == "Parasitic Plant") { res_b = typeof(ParasiticPlant); }
            if (m_B == "Crystalline Blackrock") { res_b = typeof(CrystallineBlackrock); }
            if (m_B == "Vial of Vitriol") { res_b = typeof(VialVitirol); }
            if (m_B == "Spider Carapace") { res_b = typeof(SpiderCarapace); }
            if (m_B == "Daemon Claw") { res_b = typeof(DaemonClaw); }
            if (m_B == "Lava Serpent Crust") { res_b = typeof(LavaSerpenCrust); }
            if (m_B == "Goblin Blood") { res_b = typeof(GoblinBlood); }
            if (m_B == "Undying Flesh") { res_b = typeof(UndyingFlesh); }
            if (m_B == "Boura Pelt") { res_b = typeof(BouraPelt); }
            if (m_B == "Abyssal Cloth") { res_b = typeof(AbyssalCloth); }
            if (m_B == "Powdered Iron") { res_b = typeof(PowderedIron); }
            if (m_B == "Arcanic Rune Stone") { res_b = typeof(ArcanicRuneStone); }
            if (m_B == "Slith Tongue") { res_b = typeof(SlithTongue); }
            if (m_B == "Raptor Teeth") { res_b = typeof(RaptorTeeth); }
            if (m_B == "Void Orb") { res_b = typeof(VoidOrb); }
            if (m_B == "Elven Fletching") { res_b = typeof(ElvenFletchings); }
            if (m_B == "Bottle of Ichor") { res_b = typeof(BottleIchor); }
            if (m_B == "Silver Snake Skin") { res_b = typeof(SilverSnakeSkin); }
            if (m_B == "Chaga Mushroom") { res_b = typeof(ChagaMushroom); }
            if (m_B == "Crushed Glass") { res_b = typeof(CrushedGlass); }
            if (m_B == "Reflective Wolf Eye") { res_b = typeof(ReflectiveWolfEye); }
            if (m_B == "Faery Dust") { res_b = typeof(FaeryDust); }
            if (m_B == "Crystal Shards") { res_b = typeof(CrystalShards); }
            if (m_B == "Seed of Renewal") { res_b = typeof(SeedRenewal); }

            // = Item Reference Number
            int Iref = 0;
            if (i_Item is BaseWeapon) { Iref = 1; }
            if (i_Item is BaseRanged) { Iref = 2; }
            if (i_Item is BaseArmor) { Iref = 3; }
            if (i_Item is BaseShield) { Iref = 4; }
            if (i_Item is BaseHat) { Iref = 5; }
            if (i_Item is BaseJewel) { Iref = 6; }

            // = Check for Ingredients - Not Enougth
            if (from.Backpack == null || from.Backpack.GetAmount(res_gem) < m_Gem_no || from.Backpack.GetAmount(res_a) < m_A_no || from.Backpack.GetAmount(res_b) < m_B_no)
            {
                from.SendLocalizedMessage(1079773);

                if (from.Backpack.GetAmount(res_gem) < m_Gem_no)
                {
                    from.SendMessage(String.Format("You need more {0}", m_Gem));
                }
                if (from.Backpack.GetAmount(res_a) < m_A_no)
                {
                    from.SendMessage(String.Format("You need more {0}", m_A));
                }
                if (from.Backpack.GetAmount(res_b) < m_B_no)
                {
                    from.SendMessage(String.Format("You need more {0}", m_B));
                }
            }
            // = Check for Ingredients - All Available
            else
            {
                // - Delete Used Ingredients
                from.Backpack.ConsumeTotal(res_gem, m_Gem_no);
                from.Backpack.ConsumeTotal(res_a, m_A_no);
                from.Backpack.ConsumeTotal(res_b, m_B_no);

                // Get Items Maximum Weight
                IWmax = GetMaxWeight(pm.Imbue_Item);
                pm.Imbue_IWmax = IWmax;

                // = Get Total Item Mods & Weight
                int i_TpropW = GetTotalWeight(pm.Imbue_Item);
                int i_TMods = GetTotalMods(pm.Imbue_Item);

                // = Current Mod Weight
                double c_i = pm.Imbue_ModInt; double c_w = pm.Imbue_ModVal;                
                double cur_wei = (s_Weight / m_Imax) * c_i; 
                cur_wei = Math.Round(cur_wei, 1);

                // = Item has too many Props or weight is to great
                if ((i_TpropW + cur_wei) >= pm.Imbue_IWmax)
                {
                    from.SendLocalizedMessage(1079772); // You cannot imbue this item with any more item properties.
                    from.CloseGump(typeof(ImbuingGumpC));
                    return;
                }

                // ===== CALCULATE DIFFICULTY =====
                bool m_Success = false;
                i_Diff = 0;    // = Actual difficulty
                i_Success = 0; // = Display difficulty                
                // - Skill Bonus -
                double iBonus = pm.Skills[SkillName.Imbuing].Base / 500;

                // - Racial Bonus - SA ONLY -
                //if (pm.Race == Race.Gargoyle) { iBonus += 0.05; }

                // = Item Quality Bonus
                double Q_Factor = (pm.Imbue_IWmax / 100) - 0.25; // = Item Quality Bonus
                // ===== Difficulty Equation =====
                i_Diff = (((i_TpropW + cur_wei) / Q_Factor) * (1.65 - iBonus)) + ((i_TpropW + cur_wei) / 25);

                // - Add TerMur Soulforge bonus
                if (pm.Imbue_SFBonus > 0) { i_Diff = (i_Diff / 100); i_Diff = (i_Diff * 95); }

                i_Success = (pm.Skills[SkillName.Imbuing].Base - (i_Diff - 25)) * 2;
                m_Success = from.CheckSkill(SkillName.Imbuing, i_Diff - 25, i_Diff + 25);

                //from.SendMessage(String.Format("i_Diff = {0} || i_Success = {1}", i_Diff, i_Success)); // Debug Message

                // = SUCCESS - Item is Imbued =
                if (m_Success)
                {
                    from.SendLocalizedMessage(1079775); // Success
                    // Imbuing FX
                    from.PlaySound(0x5D1);
                    Effects.SendLocationParticles(
                    EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x373A,
                          10, 30, 0, 4, 0, 0);
                    // == Add Attribute ==
                    if (i_Item is BaseWeapon)
                    {
                        BaseWeapon it = i_Item as BaseWeapon;
                        it.TimesImbued += 1;
                        it.Attributes.Brittle = 1;
                        it.WeaponAttributes.DurabilityBonus = 0;
                        it.WeaponAttributes.SelfRepair = 0;

                        if (i_Mod == 1) { it.Attributes.DefendChance = modvalue; }
                        if (i_Mod == 2) { it.Attributes.AttackChance = modvalue; }
                        if (i_Mod == 3) { it.Attributes.RegenHits = modvalue; }
                        if (i_Mod == 4) { it.Attributes.RegenStam = modvalue; }
                        if (i_Mod == 5) { it.Attributes.RegenMana = modvalue; }
                        if (i_Mod == 6) { it.Attributes.BonusStr = modvalue; }
                        if (i_Mod == 7) { it.Attributes.BonusDex = modvalue; }
                        if (i_Mod == 8) { it.Attributes.BonusInt = modvalue; }
                        if (i_Mod == 9) { it.Attributes.BonusHits = modvalue; }
                        if (i_Mod == 10) { it.Attributes.BonusStam = modvalue; }
                        if (i_Mod == 11) { it.Attributes.BonusMana = modvalue; }
                        if (i_Mod == 12) 
                        { 
                            it.Attributes.WeaponDamage = modvalue;
                            if (it.DImodded == false) { it.DImodded = true; }
                        }
                        if (i_Mod == 13) { it.Attributes.WeaponSpeed = modvalue; }
                        if (i_Mod == 14) { it.Attributes.SpellDamage = modvalue; }
                        if (i_Mod == 16)
                        {
                            if (it.Attributes.CastSpeed < 0) { it.Attributes.CastSpeed = 0; }
                            else if (it.Attributes.CastSpeed == 0 || it.Attributes.CastSpeed == 1) { it.Attributes.CastSpeed = 1; }
                        }
                        if (i_Mod == 17) { it.Attributes.LowerManaCost = modvalue; }
                        if (i_Mod == 18) { it.Attributes.LowerRegCost = modvalue; }
                        if (i_Mod == 19) { it.Attributes.ReflectPhysical = modvalue; }
                        if (i_Mod == 20) { it.Attributes.EnhancePotions = modvalue; }
                        if (i_Mod == 21) { it.Attributes.Luck = modvalue; }
                        if (i_Mod == 22)
                        {
                            it.Attributes.SpellChanneling = 1;
                            it.Attributes.CastSpeed -= 1;
                        }
                        if (i_Mod == 23) { it.Attributes.NightSight = 1; }
                        if (i_Mod == 25) 
                        { 
                            it.WeaponAttributes.HitLeechHits = modvalue;
                            it.WeaponAttributes.HitLeechStam = 0;
                            it.WeaponAttributes.HitLeechMana = 0;
                        }
                        if (i_Mod == 26) 
                        { 
                            it.WeaponAttributes.HitLeechStam = modvalue;
                            it.WeaponAttributes.HitLeechHits = 0;
                            it.WeaponAttributes.HitLeechMana = 0;
                        }
                        if (i_Mod == 27) 
                        { 
                            it.WeaponAttributes.HitLeechMana = modvalue;
                            it.WeaponAttributes.HitLeechHits = 0;
                            it.WeaponAttributes.HitLeechStam = 0;
                        }
                        if (i_Mod == 28) 
                        { 
                            it.WeaponAttributes.HitLowerAttack = modvalue;
                            it.WeaponAttributes.HitPhysicalArea = 0;                       
                        }
                        if (i_Mod == 29) 
                        { 
                            it.WeaponAttributes.HitLowerDefend = modvalue;
                            it.WeaponAttributes.HitLowerAttack = 0;                       
                        }
                        if (i_Mod == 30) 
                        { 
                            it.WeaponAttributes.HitPhysicalArea = modvalue;
                            it.WeaponAttributes.HitFireArea = 0;
                            it.WeaponAttributes.HitColdArea = 0;
                            it.WeaponAttributes.HitPoisonArea = 0;
                            it.WeaponAttributes.HitEnergyArea = 0;
                        }
                        if (i_Mod == 31)
                        {
                            it.WeaponAttributes.HitPhysicalArea = 0;
                            it.WeaponAttributes.HitFireArea = modvalue;
                            it.WeaponAttributes.HitColdArea = 0;
                            it.WeaponAttributes.HitPoisonArea = 0;
                            it.WeaponAttributes.HitEnergyArea = 0;
                        }
                        if (i_Mod == 32)
                        {
                            it.WeaponAttributes.HitPhysicalArea = 0;
                            it.WeaponAttributes.HitFireArea = 0;
                            it.WeaponAttributes.HitColdArea = modvalue;
                            it.WeaponAttributes.HitPoisonArea = 0;
                            it.WeaponAttributes.HitEnergyArea = 0;
                        }
                        if (i_Mod == 33)
                        {
                            it.WeaponAttributes.HitPhysicalArea = 0;
                            it.WeaponAttributes.HitFireArea = 0;
                            it.WeaponAttributes.HitColdArea = 0;
                            it.WeaponAttributes.HitPoisonArea = modvalue;
                            it.WeaponAttributes.HitEnergyArea = 0;
                        }
                        if (i_Mod == 34)
                        {
                            it.WeaponAttributes.HitPhysicalArea = 0;
                            it.WeaponAttributes.HitFireArea = 0;
                            it.WeaponAttributes.HitColdArea = 0;
                            it.WeaponAttributes.HitPoisonArea = 0;
                            it.WeaponAttributes.HitEnergyArea = modvalue;
                        }
                        if (i_Mod == 35) 
                        { 
                            it.WeaponAttributes.HitMagicArrow = modvalue;
                            it.WeaponAttributes.HitHarm = 0;
                            it.WeaponAttributes.HitFireball = 0;
                            it.WeaponAttributes.HitLightning = 0;
                            it.WeaponAttributes.HitDispel = 0;
                        }
                        if (i_Mod == 36)
                        {
                            it.WeaponAttributes.HitMagicArrow = 0;
                            it.WeaponAttributes.HitHarm = modvalue;
                            it.WeaponAttributes.HitFireball = 0;
                            it.WeaponAttributes.HitLightning = 0;
                            it.WeaponAttributes.HitDispel = 0;
                        }
                        if (i_Mod == 37)
                        {
                            it.WeaponAttributes.HitMagicArrow = 0;
                            it.WeaponAttributes.HitHarm = 0;
                            it.WeaponAttributes.HitFireball = modvalue;
                            it.WeaponAttributes.HitLightning = 0;
                            it.WeaponAttributes.HitDispel = 0;
                        }
                        if (i_Mod == 38)
                        {
                            it.WeaponAttributes.HitMagicArrow = 0;
                            it.WeaponAttributes.HitHarm = 0;
                            it.WeaponAttributes.HitFireball = 0;
                            it.WeaponAttributes.HitLightning = modvalue;
                            it.WeaponAttributes.HitDispel = 0;
                        }
                        if (i_Mod == 39)
                        {
                            it.WeaponAttributes.HitMagicArrow = 0;
                            it.WeaponAttributes.HitHarm = 0;
                            it.WeaponAttributes.HitFireball = 0;
                            it.WeaponAttributes.HitLightning = 0;
                            it.WeaponAttributes.HitDispel = modvalue;
                        }
                        if (i_Mod == 40) { it.WeaponAttributes.UseBestSkill = 1; }
                        if (i_Mod == 41) { it.WeaponAttributes.MageWeapon = modvalue; }
                        if (i_Mod == 51) { it.WeaponAttributes.ResistPhysicalBonus = modvalue; }
                        if (i_Mod == 52) { it.WeaponAttributes.ResistFireBonus = modvalue; }
                        if (i_Mod == 53) { it.WeaponAttributes.ResistColdBonus = modvalue; }
                        if (i_Mod == 54) { it.WeaponAttributes.ResistPoisonBonus = modvalue; }
                        if (i_Mod == 55) { it.WeaponAttributes.ResistEnergyBonus = modvalue; }
                        if (i_Mod == 60) { BaseRanged rg = it as BaseRanged; rg.Velocity = modvalue; }
                        if (i_Mod == 61) { BaseRanged rg = it as BaseRanged; rg.Balanced = true; }
                        if (i_Mod == 101) { it.Slayer = SlayerName.OrcSlaying; }
                        if (i_Mod == 102) { it.Slayer = SlayerName.TrollSlaughter; }
                        if (i_Mod == 103) { it.Slayer = SlayerName.OgreTrashing; }
                        if (i_Mod == 104) { it.Slayer = SlayerName.DragonSlaying; }
                        if (i_Mod == 105) { it.Slayer = SlayerName.Terathan; }
                        if (i_Mod == 106) { it.Slayer = SlayerName.SnakesBane; }
                        if (i_Mod == 107) { it.Slayer = SlayerName.LizardmanSlaughter; }
                        if (i_Mod == 109) { it.Slayer = SlayerName.GargoylesFoe; }
                        if (i_Mod == 111) { it.Slayer = SlayerName.Ophidian; }
                        if (i_Mod == 112) { it.Slayer = SlayerName.SpidersDeath; }
                        if (i_Mod == 113) { it.Slayer = SlayerName.ScorpionsBane; }
                        if (i_Mod == 114) { it.Slayer = SlayerName.FlameDousing; }
                        if (i_Mod == 115) { it.Slayer = SlayerName.WaterDissipation; }
                        if (i_Mod == 116) { it.Slayer = SlayerName.Vacuum; }
                        if (i_Mod == 117) { it.Slayer = SlayerName.ElementalHealth; }
                        if (i_Mod == 118) { it.Slayer = SlayerName.EarthShatter; }
                        if (i_Mod == 119) { it.Slayer = SlayerName.BloodDrinking; }
                        if (i_Mod == 120) { it.Slayer = SlayerName.SummerWind; }
                        if (i_Mod == 121) { it.Slayer = SlayerName.Silver; }
                        if (i_Mod == 122) { it.Slayer = SlayerName.Repond; }
                        if (i_Mod == 123) { it.Slayer = SlayerName.ReptilianDeath; }
                        if (i_Mod == 124) { it.Slayer = SlayerName.Exorcism; }
                        if (i_Mod == 125) { it.Slayer = SlayerName.ArachnidDoom; }
                        if (i_Mod == 126) { it.Slayer = SlayerName.ElementalBan; }
                    }
                    else if (Iref == 3)
                    {
                        BaseArmor it = i_Item as BaseArmor;
                        it.TimesImbued += 1;
                        it.Attributes.Brittle = 1;
                        it.ArmorAttributes.DurabilityBonus = 0;
                        it.ArmorAttributes.SelfRepair = 0;

                        if (i_Mod == 3) { it.Attributes.RegenHits = modvalue; }
                        if (i_Mod == 4) { it.Attributes.RegenStam = modvalue; }
                        if (i_Mod == 5) { it.Attributes.RegenMana = modvalue; }
                        if (i_Mod == 9) { it.Attributes.BonusHits = modvalue; }
                        if (i_Mod == 10) { it.Attributes.BonusStam = modvalue; }
                        if (i_Mod == 11) { it.Attributes.BonusMana = modvalue; }
                        if (i_Mod == 17) { it.Attributes.LowerManaCost = modvalue; }
                        if (i_Mod == 18) { it.Attributes.LowerRegCost = modvalue; }
                        if (i_Mod == 19) { it.Attributes.ReflectPhysical = modvalue; }
                        if (i_Mod == 21) { it.Attributes.Luck = modvalue; }
                        if (i_Mod == 23) { it.Attributes.NightSight = 1; }
                        if (i_Mod == 22)
                        {
                            it.Attributes.SpellChanneling = 1;
                            if (it.Attributes.CastSpeed == 0) { it.Attributes.CastSpeed = -1; }
                            if (it.Attributes.CastSpeed == 1) { it.Attributes.CastSpeed = 0; }
                        }
                        if (i_Mod == 16)
                        {
                            if (it.Attributes.CastSpeed < 0) { it.Attributes.CastSpeed = 0; }
                            else if (it.Attributes.CastSpeed == 0 || it.Attributes.CastSpeed == 1) { it.Attributes.CastSpeed = 1; }
                        }
                        if (i_Mod == 51) { it.PhysicalBonus = modvalue; it.Physical_Modded = true; }
                        if (i_Mod == 52) { it.FireBonus = modvalue; it.Fire_Modded = true; }
                        if (i_Mod == 53) { it.ColdBonus = modvalue; it.Cold_Modded = true; }
                        if (i_Mod == 54) { it.PoisonBonus = modvalue; it.Poison_Modded = true; }
                        if (i_Mod == 55) { it.EnergyBonus = modvalue; it.Energy_Modded = true; }
                    }
                    else if (Iref == 4)
                    {
                        BaseShield it = i_Item as BaseShield;
                        it.TimesImbued += 1;

                        it.ArmorAttributes.DurabilityBonus = 0;
                        it.ArmorAttributes.SelfRepair = 0;

                        if (i_Mod == 1) { it.Attributes.DefendChance = modvalue; }
                        if (i_Mod == 2) { it.Attributes.AttackChance = modvalue; }
                        if (i_Mod == 19) { it.Attributes.ReflectPhysical = modvalue; }
                        if (i_Mod == 16)
                        {
                            if (it.Attributes.CastSpeed < 0) { it.Attributes.CastSpeed = 0; }
                            else if (it.Attributes.CastSpeed == 0 || it.Attributes.CastSpeed == 1) { it.Attributes.CastSpeed = 1; }
                        }
                        if (i_Mod == 22)
                        {
                            it.Attributes.SpellChanneling = 1;
                            if (it.Attributes.CastSpeed == 0) { it.Attributes.CastSpeed = -1; }
                            if (it.Attributes.CastSpeed == 1) { it.Attributes.CastSpeed = 0; }
                        }
                        if (i_Mod == 24) { it.ArmorAttributes.LowerStatReq = modvalue; }
                        if (i_Mod == 42) { it.ArmorAttributes.DurabilityBonus = modvalue; }
                    }
                    else if (i_Item is BaseHat)
                    {
                        BaseHat it = i_Item as BaseHat;
                        it.TimesImbued += 1;
                        it.Attributes.Brittle = 1;

                        if (i_Mod == 3) { it.Attributes.RegenHits = modvalue; }
                        if (i_Mod == 4) { it.Attributes.RegenStam = modvalue; }
                        if (i_Mod == 5) { it.Attributes.RegenMana = modvalue; }
                        if (i_Mod == 9) { it.Attributes.BonusHits = modvalue; }
                        if (i_Mod == 10) { it.Attributes.BonusStam = modvalue; }
                        if (i_Mod == 11) { it.Attributes.BonusMana = modvalue; }
                        if (i_Mod == 17) { it.Attributes.LowerManaCost = modvalue; }
                        if (i_Mod == 18) { it.Attributes.LowerRegCost = modvalue; }
                        if (i_Mod == 19) { it.Attributes.ReflectPhysical = modvalue; }
                        if (i_Mod == 21) { it.Attributes.Luck = modvalue; }
                        if (i_Mod == 23) { it.Attributes.NightSight = 1; }
                        if (i_Mod == 51) { it.Resistances.Physical = modvalue; it.Physical_Modded = true; }
                        if (i_Mod == 52) { it.Resistances.Fire = modvalue; it.Fire_Modded = true; }
                        if (i_Mod == 53) { it.Resistances.Cold = modvalue; it.Cold_Modded = true; }
                        if (i_Mod == 54) { it.Resistances.Poison = modvalue; it.Poison_Modded = true; }
                        if (i_Mod == 55) { it.Resistances.Energy = modvalue; it.Energy_Modded = true; }
                    }
                    else if (i_Item is BaseJewel)
                    {
                        BaseJewel it = i_Item as BaseJewel;
                        it.TimesImbued += 1;
                        it.Attributes.Brittle = 1;

                        if (it.MaxHitPoints <= 0)
                        {
                            it.MaxHitPoints = Utility.RandomMinMax(50, 75);
                            it.HitPoints = it.MaxHitPoints;
                        }

                        if (i_Mod == 1) { it.Attributes.DefendChance = modvalue; }
                        if (i_Mod == 2) { it.Attributes.AttackChance = modvalue; }
                        if (i_Mod == 6) { it.Attributes.BonusStr = modvalue; }
                        if (i_Mod == 7) { it.Attributes.BonusDex = modvalue; }
                        if (i_Mod == 8) { it.Attributes.BonusInt = modvalue; }
                        if (i_Mod == 12) { it.Attributes.WeaponDamage = modvalue; }
                        if (i_Mod == 14) { it.Attributes.SpellDamage = modvalue; }
                        if (i_Mod == 15) { it.Attributes.CastRecovery = modvalue; }
                        if (i_Mod == 16) { it.Attributes.CastSpeed = 1; }
                        if (i_Mod == 17) { it.Attributes.LowerManaCost = modvalue; }
                        if (i_Mod == 18) { it.Attributes.LowerRegCost = modvalue; }
                        if (i_Mod == 20) { it.Attributes.EnhancePotions = modvalue; }
                        if (i_Mod == 21) { it.Attributes.Luck = modvalue; }
                        if (i_Mod == 23) { it.Attributes.NightSight = 1; }
                        if (i_Mod == 51) { it.Resistances.Physical = modvalue; }
                        if (i_Mod == 52) { it.Resistances.Fire = modvalue; }
                        if (i_Mod == 53) { it.Resistances.Cold = modvalue; }
                        if (i_Mod == 54) { it.Resistances.Poison = modvalue; }
                        if (i_Mod == 55) { it.Resistances.Energy = modvalue; }
                        if (i_Mod == 151) { it.SkillBonuses.SetSkill(0, SkillName.Fencing); it.SkillBonuses.SetBonus(0, modvalue); }
                        if (i_Mod == 152) { it.SkillBonuses.SetSkill(0, SkillName.Macing); it.SkillBonuses.SetBonus(0, modvalue); }
                        if (i_Mod == 153) { it.SkillBonuses.SetSkill(0, SkillName.Swords); it.SkillBonuses.SetBonus(0, modvalue); }
                        if (i_Mod == 154) { it.SkillBonuses.SetSkill(0, SkillName.Musicianship); it.SkillBonuses.SetBonus(0, modvalue); }
                        if (i_Mod == 155) { it.SkillBonuses.SetSkill(0, SkillName.Magery); it.SkillBonuses.SetBonus(0, modvalue); }
                        if (i_Mod == 156) { it.SkillBonuses.SetSkill(1, SkillName.Wrestling); it.SkillBonuses.SetBonus(1, modvalue); }
                        if (i_Mod == 157) { it.SkillBonuses.SetSkill(1, SkillName.AnimalTaming); it.SkillBonuses.SetBonus(1, modvalue); }
                        if (i_Mod == 158) { it.SkillBonuses.SetSkill(1, SkillName.SpiritSpeak); it.SkillBonuses.SetBonus(1, modvalue); }
                        if (i_Mod == 159) { it.SkillBonuses.SetSkill(1, SkillName.Tactics); it.SkillBonuses.SetBonus(1, modvalue); }
                        if (i_Mod == 160) { it.SkillBonuses.SetSkill(1, SkillName.Provocation); it.SkillBonuses.SetBonus(1, modvalue); }
                        if (i_Mod == 161) { it.SkillBonuses.SetSkill(2, SkillName.Focus); it.SkillBonuses.SetBonus(2, modvalue); }
                        if (i_Mod == 162) { it.SkillBonuses.SetSkill(2, SkillName.Parry); it.SkillBonuses.SetBonus(2, modvalue); }
                        if (i_Mod == 163) { it.SkillBonuses.SetSkill(2, SkillName.Stealth); it.SkillBonuses.SetBonus(2, modvalue); }
                        if (i_Mod == 164) { it.SkillBonuses.SetSkill(2, SkillName.Meditation); it.SkillBonuses.SetBonus(2, modvalue); }
                        if (i_Mod == 165) { it.SkillBonuses.SetSkill(2, SkillName.AnimalLore); it.SkillBonuses.SetBonus(2, modvalue); }
                        if (i_Mod == 166) { it.SkillBonuses.SetSkill(2, SkillName.Discordance); it.SkillBonuses.SetBonus(2, modvalue); }
                        if (i_Mod == 167) { it.SkillBonuses.SetSkill(3, SkillName.Bushido); it.SkillBonuses.SetBonus(3, modvalue); }
                        if (i_Mod == 168) { it.SkillBonuses.SetSkill(3, SkillName.Necromancy); it.SkillBonuses.SetBonus(3, modvalue); }
                        if (i_Mod == 169) { it.SkillBonuses.SetSkill(3, SkillName.Veterinary); it.SkillBonuses.SetBonus(3, modvalue); }
                        if (i_Mod == 170) { it.SkillBonuses.SetSkill(3, SkillName.Stealing); it.SkillBonuses.SetBonus(3, modvalue); }
                        if (i_Mod == 171) { it.SkillBonuses.SetSkill(3, SkillName.EvalInt); it.SkillBonuses.SetBonus(3, modvalue); }
                        if (i_Mod == 172) { it.SkillBonuses.SetSkill(3, SkillName.Anatomy); it.SkillBonuses.SetBonus(3, modvalue); }
                        if (i_Mod == 173) { it.SkillBonuses.SetSkill(4, SkillName.Peacemaking); it.SkillBonuses.SetBonus(4, modvalue); }
                        if (i_Mod == 174) { it.SkillBonuses.SetSkill(4, SkillName.Ninjitsu); it.SkillBonuses.SetBonus(4, modvalue); }
                        if (i_Mod == 175) { it.SkillBonuses.SetSkill(4, SkillName.Chivalry); it.SkillBonuses.SetBonus(4, modvalue); }
                        if (i_Mod == 176) { it.SkillBonuses.SetSkill(4, SkillName.Archery); it.SkillBonuses.SetBonus(4, modvalue); }
                        if (i_Mod == 177) { it.SkillBonuses.SetSkill(4, SkillName.MagicResist); it.SkillBonuses.SetBonus(4, modvalue); }
                        if (i_Mod == 178) { it.SkillBonuses.SetSkill(4, SkillName.Healing); it.SkillBonuses.SetBonus(4, modvalue); }
                    }
                }
                // == FAILURE == 
                else
                {
                    from.SendLocalizedMessage(1079774); // Fail
                    from.PlaySound(0x1E5);
                }
            }
        }

        // ======================= Attribute Data ===================================
        public static void GetMaterials( int Mod )
        {           
            if (Mod == 1) { s_Mod = "Defense Chance Increase"; s_Weight = 130; m_Gem = "Tourmaline"; m_A = "Relic Fragment"; m_B = "Essence of Singularity"; m_Imax = 15; i_Inc = 1; m_Desc = 1111947; }
            if (Mod == 2) { s_Mod = "Hit Chance Increase"; s_Weight = 130; m_Gem = "Amber"; m_A = "Relic Fragment"; m_B = "Essence of Precision"; m_Imax = 15; i_Inc = 1; m_Desc = 1111958; }
            if (Mod == 3) { s_Mod = "Regen Hitpoints"; s_Weight = 100; m_Gem = "Tourmaline"; m_A = "Enchanted Essence"; m_B = "Seed of Renewal"; m_Imax = 2; i_Inc = 1; m_Desc = 1111994; }
            if (Mod == 4) { s_Mod = "Regen Stamina"; s_Weight = 100; m_Gem = "Diamond"; m_A = "Enchanted Essence"; m_B = "Seed of Renewal"; m_Imax = 3; i_Inc = 1; m_Desc = 1112043; }
            if (Mod == 5) { s_Mod = "Regen Mana"; s_Weight = 100; m_Gem = "Sapphire"; m_A = "Enchanted Essence"; m_B = "Seed of Renewal"; m_Imax = 2; i_Inc = 1; m_Desc = 1112003; }
            if (Mod == 6) { s_Mod = "Strength Bonus"; s_Weight = 110; m_Gem = "Diamond"; m_A = "Enchanted Essence"; m_B = "Fire Ruby"; m_Imax = 8; i_Inc = 1; m_Desc = 1112044; }
            if (Mod == 7) { s_Mod = "Dexterity Bonus"; s_Weight = 110; m_Gem = "Ruby"; m_A = "Enchanted Essence"; m_B = "Blue Diamond"; m_Imax = 8; i_Inc = 1; m_Desc = 1111948; }
            if (Mod == 8) { s_Mod = "Intelligence Bonus"; s_Weight = 110; m_Gem = "Tourmaline"; m_A = "Enchanted Essence"; m_B = "Turquoise"; m_Imax = 8; i_Inc = 1; m_Desc = 1111995; }
            if (Mod == 9) { s_Mod = "Hitpoint Increase"; s_Weight = 110; m_Gem = "Ruby"; m_A = "Enchanted Essence"; m_B = "Luminescent Fungi"; m_Imax = 5; i_Inc = 1; m_Desc = 1111993; }
            if (Mod == 10) { s_Mod = "Stamina Increase"; s_Weight = 100; m_Gem = "Diamond"; m_A = "Enchanted Essence"; m_B = "Luminescent Fungi"; m_Imax = 8; i_Inc = 1; m_Desc = 1112042; }
            if (Mod == 11) { s_Mod = "Mana Increase"; s_Weight = 110; m_Gem = "Sapphire"; m_A = "Enchanted Essence"; m_B = "Luminescent Fungi"; m_Imax = 8; i_Inc = 1; m_Desc = 1112002; }
            if (Mod == 12) { s_Mod = "Damage Increase"; s_Weight = 100; m_Gem = "Citrine"; m_A = "Enchanted Essence"; m_B = "Crystal Shards"; m_Imax = 50; i_Inc = 1; m_Desc = 1112005; }
            if (Mod == 13) { s_Mod = "Swing Speed Increase"; s_Weight = 110; m_Gem = "Tourmaline"; m_A = "Relic Fragment"; m_B = "Essence of Control"; m_Imax = 40; i_Inc = 5; m_Desc = 1112045; }
            if (Mod == 14) { s_Mod = "Spell Damage Increase"; s_Weight = 100; m_Gem = "Emerald"; m_A = "Enchanted Essence"; m_B = "Crystal Shards"; m_Imax = 12; i_Inc = 1; m_Desc = 1112041; }
            if (Mod == 15) { s_Mod = "Faster Cast Recovery"; s_Weight = 120; m_Gem = "Amethyst"; m_A = "Relic Fragment"; m_B = "Essence of Diligence"; m_Imax = 3; i_Inc = 1; m_Desc = 1111952; }
            if (Mod == 16) { s_Mod = "Faster Casting"; s_Weight = 140; m_Gem = "Ruby"; m_A = "Relic Fragment"; m_B = "Essence of Achievement"; m_Imax = 1; i_Inc = 0; m_Desc = 1111951; }
            if (Mod == 17) { s_Mod = "Lower Mana Cost"; s_Weight = 110; m_Gem = "Tourmaline"; m_A = "Relic Fragment"; m_B = "Essence of Order"; m_Imax = 8; i_Inc = 1; m_Desc = 1111996; }
            if (Mod == 18) { s_Mod = "Lower Reagent Cost"; s_Weight = 100; m_Gem = "Amber"; m_A = "Magical Residue"; m_B = "Faery Dust"; m_Imax = 20; i_Inc = 1; m_Desc = 1111997; }
            if (Mod == 19) { s_Mod = "Reflect Physical Damage"; s_Weight = 100; m_Gem = "Citrine"; m_A = "Magical Residue"; m_B = "Reflective Wolf Eye"; m_Imax = 15; i_Inc = 1; m_Desc = 1112006; }
            if (Mod == 20) { s_Mod = "Enhance Potions"; s_Weight = 100; m_Gem = "Citrine"; m_A = "Enchanted Essence"; m_B = "Crushed Glass"; m_Imax = 30; i_Inc = 5; m_Desc = 1111950; }
            if (Mod == 21) { s_Mod = "Luck"; s_Weight = 100; m_Gem = "Citrine"; m_A = "Magical Residue"; m_B = "Chaga Mushroom"; m_Imax = 100; i_Inc = 1; m_Desc = 1111999; }
            if (Mod == 22) { s_Mod = "Spell Channeling"; s_Weight = 100; m_Gem = "Diamond"; m_A = "Magical Residue"; m_B = "Silver Snake Skin"; m_Imax = 1; i_Inc = 0; m_Desc = 1112040; }
            if (Mod == 23) { s_Mod = "Night Sight"; s_Weight = 50; m_Gem = "Tourmaline"; m_A = "Magical Residue"; m_B = "Bottle of Ichor"; m_Imax = 1; i_Inc = 0; m_Desc = 1112004; }

            if (Mod == 24) { s_Mod = "Lower Requirements"; s_Weight = 100; m_Gem = "Amethyst"; m_A = "Enchanted Essence"; m_B = "Elven Fletching"; m_Imax = 100; i_Inc = 10; m_Desc = 1111998; }
            if (Mod == 25) { s_Mod = "Hit Life Leech"; s_Weight = 110; m_Gem = "Ruby"; m_A = "Magical Residue"; m_B = "Void Orb"; m_Imax = 50; i_Inc = 2; m_Desc = 1111964; }
            if (Mod == 26) { s_Mod = "Hit Stamina Leech"; s_Weight = 110; m_Gem = "Diamond"; m_A = "Magical Residue"; m_B = "Void Orb"; m_Imax = 50; i_Inc = 2; m_Desc = 1111992; }
            if (Mod == 27) { s_Mod = "Hit Mana Leech"; s_Weight = 100; m_Gem = "Sapphire"; m_A = "Magical Residue"; m_B = "Void Orb"; m_Imax = 50; i_Inc = 2; m_Desc = 1111967; }
            if (Mod == 28) { s_Mod = "Hit Lower Attack"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Enchanted Essence"; m_B = "Parasitic Plant"; m_Imax = 50; i_Inc = 2; m_Desc = 1111965; }
            if (Mod == 29) { s_Mod = "Hit Lower Defense"; s_Weight = 130; m_Gem = "Tourmaline"; m_A = "Enchanted Essence"; m_B = "Parasitic Plant"; m_Imax = 50; i_Inc = 2; m_Desc = 1111966; }
            if (Mod == 30) { s_Mod = "Hit Physical Area"; s_Weight = 100; m_Gem = "Diamond"; m_A = "Magical Residue"; m_B = "Raptor Teeth"; m_Imax = 50; i_Inc = 2; m_Desc = 1111956; }
            if (Mod == 31) { s_Mod = "Hit Fire Area"; s_Weight = 100; m_Gem = "Ruby"; m_A = "Magical Residue"; m_B = "Raptor Teeth"; m_Imax = 50; i_Inc = 2; m_Desc = 1111955; }
            if (Mod == 32) { s_Mod = "Hit Cold Area"; s_Weight = 100; m_Gem = "Sapphire"; m_A = "Magical Residue"; m_B = "Raptor Teeth"; m_Imax = 50; i_Inc = 2; m_Desc = 1111953; }
            if (Mod == 33) { s_Mod = "Hit Poison Area"; s_Weight = 100; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "Raptor Teeth"; m_Imax = 50; i_Inc = 2; m_Desc = 1111957; }
            if (Mod == 34) { s_Mod = "Hit Energy Area"; s_Weight = 100; m_Gem = "Amethyst"; m_A = "Magical Residue"; m_B = "Raptor Teeth"; m_Imax = 50; i_Inc = 2; m_Desc = 1111954; }
            if (Mod == 35) { s_Mod = "Hit Magic Arrow"; s_Weight = 120; m_Gem = "Amber"; m_A = "Relic Fragment"; m_B = "Essence of Feeling"; m_Imax = 50; i_Inc = 2; m_Desc = 1111963; }
            if (Mod == 36) { s_Mod = "Hit Harm"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Enchanted Essence"; m_B = "Parasitic Plant"; m_Imax = 50; i_Inc = 2; m_Desc = 1111961; }
            if (Mod == 37) { s_Mod = "Hit Fireball"; s_Weight = 140; m_Gem = "Ruby"; m_A = "Enchanted Essence"; m_B = "Fire Ruby"; m_Imax = 50; i_Inc = 2; m_Desc = 1111960; }
            if (Mod == 38) { s_Mod = "Hit Lightning"; s_Weight = 140; m_Gem = "Amethyst"; m_A = "Relic Fragment"; m_B = "Essence of Passion"; m_Imax = 50; i_Inc = 2; m_Desc = 1111962; }
            if (Mod == 39) { s_Mod = "Hit Dispel"; s_Weight = 100; m_Gem = "Amber"; m_A = "Magical Residue"; m_B = "Slith Tongue"; m_Imax = 50; i_Inc = 2; m_Desc = 1111959; }
            if (Mod == 40) { s_Mod = "Use Best Weapon Skill"; s_Weight = 150; m_Gem = "Amber"; m_A = "Enchanted Essence"; m_B = "Delicate Scales"; m_Imax = 1; i_Inc = 0; m_Desc = 1111946; }
            if (Mod == 41) { s_Mod = "Mage Weapon"; s_Weight = 100; m_Gem = "Emerald"; m_A = "Enchanted Essence"; m_B = "Arcanic Rune Stone"; m_Imax = 10; i_Inc = 1; m_Desc = 1112001; }
            if (Mod == 42) { s_Mod = "Durability"; s_Weight = 100; m_Gem = "Diamond"; m_A = "Enchanted Essence"; m_B = "Powdered Iron"; m_Imax = 100; i_Inc = 10; m_Desc = 1111949; }

            if (Mod == 49) { s_Mod = "Mage Armor"; s_Weight = 100; m_Gem = "Diamond"; m_A = "Enchanted Essence"; m_B = "Abyssal Cloth"; m_Imax = 1; i_Inc = 0; m_Desc = 1112000; }

            if (Mod == 51) { s_Mod = "Physical Resist Bonus"; s_Weight = 100; m_Gem = "Diamond"; m_A = "Magical Residue"; m_B = "Boura Pelt"; m_Imax = 15; i_Inc = 1; m_Desc = 1112010; }
            if (Mod == 52) { s_Mod = "Fire Resist Bonus"; s_Weight = 100; m_Gem = "Ruby"; m_A = "Magical Residue"; m_B = "Boura Pelt"; m_Imax = 15; i_Inc = 1; m_Desc = 1112009; }
            if (Mod == 53) { s_Mod = "Cold Resist Bonus"; s_Weight = 100; m_Gem = "Sapphire"; m_A = "Magical Residue"; m_B = "Boura Pelt"; m_Imax = 15; i_Inc = 1; m_Desc = 1112007; }
            if (Mod == 54) { s_Mod = "Poison Resist Bonus"; s_Weight = 100; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "Boura Pelt"; m_Imax = 15; i_Inc = 1; m_Desc = 1112011; }
            if (Mod == 55) { s_Mod = "Energy Resist Bonus"; s_Weight = 100; m_Gem = "Amethyst"; m_A = "Magical Residue"; m_B = "Boura Pelt"; m_Imax = 15; i_Inc = 1; m_Desc = 1112008; }

            if (Mod == 60) { s_Mod = "Velocity"; s_Weight = 150; m_Gem = "Tourmaline"; m_A = "Relic Fragment"; m_B = "Essence of Direction"; m_Imax = 50; i_Inc = 2; m_Desc = 1112048; }
            if (Mod == 61) { s_Mod = "Balanced"; s_Weight = 100; m_Gem = "Amber"; m_A = "Relic Fragment"; m_B = "Essence of Balance"; m_Imax = 1; i_Inc = 0; m_Desc = 1112047; }

            if (Mod == 101) { s_Mod = "Orc Slaying"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111977; }
            if (Mod == 102) { s_Mod = "Troll Slaughter"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111990; }
            if (Mod == 103) { s_Mod = "Ogre Trashing"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111975; }
            if (Mod == 104) { s_Mod = "Dragon Slaying"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111970; }
            if (Mod == 105) { s_Mod = "Terathan"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111989; }
            if (Mod == 106) { s_Mod = "Snakes Bane"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111980; }
            if (Mod == 107) { s_Mod = "Lizardman Slaughter"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111973; }
            if (Mod == 108) { s_Mod = "Daemon Dismissal"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1112001; }
            if (Mod == 109) { s_Mod = "Gargoyles Foe"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111973; }
            if (Mod == 110) { s_Mod = "Balron Damnation"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1112001; }
            if (Mod == 111) { s_Mod = "Ophidian"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111976; }
            if (Mod == 112) { s_Mod = "Spiders Death"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111982; }
            if (Mod == 113) { s_Mod = "Scorpions Bane"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111979; }
            if (Mod == 114) { s_Mod = "Flame Dousing"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111972; }
            if (Mod == 115) { s_Mod = "Water Dissipation"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111991; }
            if (Mod == 116) { s_Mod = "Vacuum"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111968; }
            if (Mod == 117) { s_Mod = "Elemental Health"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111978; }
            if (Mod == 118) { s_Mod = "Earth Shatter"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111971; }
            if (Mod == 119) { s_Mod = "Blood Drinking"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111969; }
            if (Mod == 120) { s_Mod = "Summer Wind"; s_Weight = 110; m_Gem = "Emerald"; m_A = "Magical Residue"; m_B = "White Pearl"; m_Imax = 1; i_Inc = 0; m_Desc = 1111981; }

            if (Mod == 121) { s_Mod = "Silver"; s_Weight = 130; m_Gem = "Ruby"; m_A = "Relic Fragment"; m_B = "Undying Flesh"; m_Imax = 1; i_Inc = 0; m_Desc = 1111988; }
            if (Mod == 122) { s_Mod = "Repond"; s_Weight = 130; m_Gem = "Ruby"; m_A = "Relic Fragment"; m_B = "Goblin Blood"; m_Imax = 1; i_Inc = 0; m_Desc = 1111986; }
            if (Mod == 123) { s_Mod = "Reptilian Death"; s_Weight = 130; m_Gem = "Ruby"; m_A = "Relic Fragment"; m_B = "Lava Serpent Crust"; m_Imax = 1; i_Inc = 0; m_Desc = 1111987; }
            if (Mod == 124) { s_Mod = "Exorcism"; s_Weight = 130; m_Gem = "Ruby"; m_A = "Relic Fragment"; m_B = "Daemon Claw"; m_Imax = 1; i_Inc = 0; m_Desc = 1111984; }
            if (Mod == 125) { s_Mod = "Arachnid Doom"; s_Weight = 130; m_Gem = "Ruby"; m_A = "Relic Fragment"; m_B = "Spider Carapace"; m_Imax = 1; i_Inc = 0; m_Desc = 1111983; }
            if (Mod == 126) { s_Mod = "Elemental Ban"; s_Weight = 130; m_Gem = "Ruby"; m_A = "Relic Fragment"; m_B = "Vial of Vitriol"; m_Imax = 1; i_Inc = 0; m_Desc = 1111985; }

            if (Mod == 151) { s_Mod = "Fencing"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112012; }
            if (Mod == 152) { s_Mod = "Mace Fighting"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112013; }
            if (Mod == 153) { s_Mod = "Swordsmanship"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112016; }
            if (Mod == 154) { s_Mod = "Musicianship"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112015; }
            if (Mod == 155) { s_Mod = "Magery"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112014; }

            if (Mod == 156) { s_Mod = "Wrestling"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112021; }
            if (Mod == 157) { s_Mod = "Animal Taming"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112017; }
            if (Mod == 158) { s_Mod = "Spirit Speak"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112019; }
            if (Mod == 159) { s_Mod = "Tactics"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112020; }
            if (Mod == 160) { s_Mod = "Provocation"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 11120018; }

            if (Mod == 161) { s_Mod = "Focus"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112024; }
            if (Mod == 162) { s_Mod = "Parrying"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112026; }
            if (Mod == 163) { s_Mod = "Stealth"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112027; }
            if (Mod == 164) { s_Mod = "Meditation"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112025; }
            if (Mod == 165) { s_Mod = "Animal Lore"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112022; }
            if (Mod == 166) { s_Mod = "Discordance"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112023; }

            if (Mod == 167) { s_Mod = "Bushido"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112029; }
            if (Mod == 168) { s_Mod = "Necromancy"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112031; }
            if (Mod == 169) { s_Mod = "Veterinary"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112033; }
            if (Mod == 170) { s_Mod = "Stealing"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112032; }
            if (Mod == 171) { s_Mod = "Evaluating Intelligence"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112030; }
            if (Mod == 172) { s_Mod = "Anatomy"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112028; }

            if (Mod == 173) { s_Mod = "Peacemaking"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112038; }
            if (Mod == 174) { s_Mod = "Ninjitsu"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112037; }
            if (Mod == 175) { s_Mod = "Chivalry"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112035; }
            if (Mod == 176) { s_Mod = "Archery"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112034; }
            if (Mod == 177) { s_Mod = "Resisting Spells"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112039; }
            if (Mod == 178) { s_Mod = "Healing"; s_Weight = 140; m_Gem = "Star Sapphire"; m_A = "Enchanted Essence"; m_B = "Crystalline Blackrock"; m_Imax = 15; i_Inc = 1; m_Desc = 1112036; }

            return;
        }
        // ========== Ingredient Quantity Needed ======= Gems ===================
        public int GetMGemNo(int Max, int Inc, double Mv)
        {
            double mno = 0;
            if (Max == 100) { mno = Mv / 10; }
            if (Max == 50) { mno = 1 * (Mv / 5); }
            if (Max == 40 && Inc == 5) { mno = 1 * (Mv / 4); }
            if (Max == 30 && Inc == 5) { mno = 1 * (Mv / 3); }
            if (Max == 20 && Inc == 1) { mno = 1 * (Mv / 2); }
            if (Max == 15 && Inc == 1) { mno = 1 * (Mv / 1.5); }
            if (Max == 12 && Inc == 1) { mno = Mv; }
            if (Max == 10 && Inc == 1) { mno = Mv; }
            if (Max == 8 && Inc == 1) { mno = Mv; }
            if (Max == 5 && Inc == 1) { mno = Mv * 2; }
            if (Max == 3 && Inc == 1) { mno = Mv * 3; }
            if (Max == 2 && Inc == 1) { mno = Mv * 5; }
            if (i_Mod == 16 || i_Mod == 22 || i_Mod == 23 || i_Mod == 40 || i_Mod == 41 || i_Mod == 49) { mno = 10; }
            if (i_Mod >= 100 && i_Mod <= 126) { mno = 10; }

            if (mno < 1) { mno = 1; }
            mno = Math.Round(mno);
            int oInt = Convert.ToInt32(mno);
            return oInt;
        }
        // ========== Ingredient Quantity Needed ===== Minor Ingredients ===========
        public int GetMANo(int Max, int Inc, double Mv)
        {
            double mno = 0;
            if (Max == 100) { mno = 1 * (Mv / 20); }
            if (Max == 50) { mno = 1 * (Mv / 10); }
            if (Max == 40 && Inc == 5) { mno = 1 * (Mv / 8); }
            if (Max == 30 && Inc == 5) { mno = 1 * (Mv / 6); }
            if (Max == 20 && Inc == 1) { mno = 1 * (Mv / 4); }
            if (Max == 15 && Inc == 1) { mno = 1 * (Mv / 3); }
            if (Max == 12 && Inc == 1) { mno = 1 * (Mv / 2.4); }
            if (Max == 10 && Inc == 1) { mno = 1 * (Mv / 2); }
            if (Max == 8 && Inc == 1) { mno = Mv * 0.625; }
            if (Max == 5 && Inc == 1) { mno = Mv; }
            if (Max == 3 && Inc == 1) { mno = Mv * 1.6; }
            if (Max == 2 && Inc == 1) { mno = Mv * 2.5; }
            if (i_Mod == 16 || i_Mod == 22 || i_Mod == 23 || i_Mod == 40 || i_Mod == 41 || i_Mod == 49) { mno = 5; }
            if (i_Mod >= 100 && i_Mod <= 126) { mno = 5; }
            if (mno < 1) { mno = 1; }
            mno = Math.Round(mno);
            int oInt = Convert.ToInt32(mno);
            return oInt;
        }
        // ========== Ingredient Quantity Needed ===== Major Ingredients ===========
        public int GetMBNo(int Max, int Inc, double Mv)
        {
            double mno = 0;
            if (Max == 100) { mno = (Mv - 90); }
            if (Max == 50) { mno = (Mv - 45) * 2; }
            if (Max == 40 && Inc == 5)
            {
                if (Mv == 30) { mno = 3; }
                if (Mv == 35) { mno = 6; }
                if (Mv == 40) { mno = 10; }
            }
            if (Max == 30 && Inc == 5)
            {
                if (Mv == 30) { mno = 4; }
            }
            if (Max == 20 && Inc == 1) { mno = (Mv - 18) * 5; }
            if (Max == 15 && Inc == 1) { mno = (Mv - 12) * 3.3; }
            if (Max == 12 && Inc == 1) { mno = (Mv - 10) * 5; }
            if (Max == 10 && Inc == 1) { mno = (Mv - 8) * 5; }
            if (Max == 8 && Inc == 1) { mno = (Mv - 7) * 10; }
            if (Max == 5 && Inc == 1) { mno = (Mv - 4) * 10; }
            if (Max == 3 && Inc == 1) { mno = (Mv - 2) * 10; }
            if (Max == 2 && Inc == 1) { mno = (Mv - 1) * 10; }
            if (i_Mod == 16 || i_Mod == 22 || i_Mod == 23 || i_Mod == 40 || i_Mod == 41 || i_Mod == 49) { mno = 10; }
            if (i_Mod >= 100 && i_Mod <= 126) { mno = 10; }

            if (mno < 0) { mno = 0; }
            mno = Math.Round(mno);
            int oInt = Convert.ToInt32(mno);
            return oInt;
        }
    }                
}