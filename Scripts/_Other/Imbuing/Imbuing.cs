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
using Server.Commands;

namespace Server.SkillHandlers
{
    public class Imbuing
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Imbuing].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile from)
        {
            if (!from.Alive)
            {
                from.SendMessage(2499, "It occurs to you that you cannot do much imbuing when you are dead..");
            }
            else
            {
                from.CloseGump(typeof(ImbuingGump));
                from.SendGump(new ImbuingGump(from));
            }

            return TimeSpan.FromSeconds(1.0);
        }
    }

    public class ImbuingGump : Gump
    {
        // == SoulForge Check ==
        public static void CheckSoulForge(Mobile from, int range, out bool sforge)
        {
            sforge = false;
            PlayerMobile m = from as PlayerMobile;

            m.Imbue_SFBonus = 0;

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

                    // = Termur Soulforge Bonus
                    if (isTMSForge)
                    {
                        m.Imbue_SFBonus = 5;
                    }
                    m.Imbue_SFBonus = 5;

                    if (sforge)
                        break;
                }
            }
        }

        public ImbuingGump(Mobile from) : base(520, 340)
        {
            Mobile m = from;
            PlayerMobile pm = from as PlayerMobile;

            from.CloseGump(typeof(ImbuingGumpB));
            from.CloseGump(typeof(ImbuingGumpC));

            pm.Imbue_ModVal = 0;
            pm.ImbMenu_Cat = 0;

            AddPage(0);
            this.AddBackground(0, 0, 540, 340, 9270);
            this.AddAlphaRegion(17, 17, 486, 20);
            this.AddAlphaRegion(17, 45, 486, 247);
            this.AddAlphaRegion(17, 299, 486, 25);
            this.AddLabel(221, 18, 1359, "IMBUING MENU");

            AddButton(25, 66, 4017, 4018, 10005, GumpButtonType.Reply, 0);
            AddHtml(66, 68, 430, 18, "<BASEFONT COLOR=#FFFFFF>Imbue Item - Adds or modifies an item property on an item", false, false);
            AddButton(25, 95, 4017, 4018, 10006, GumpButtonType.Reply, 0);
            AddHtml(66, 97, 430, 18, "<BASEFONT COLOR=#FFFFFF>Reimbue Last - Repeats the last imbuing attempt", false, false);
            AddButton(25, 124, 4017, 4018, 10007, GumpButtonType.Reply, 0);
            AddHtml(66, 126, 430, 18, "<BASEFONT COLOR=#FFFFFF>Imbue Last Item - Auto targets the last imbued item", false, false);
            AddButton(25, 153, 4017, 4018, 10008, GumpButtonType.Reply, 0);
            AddHtml(66, 155, 430, 18, "<BASEFONT COLOR=#FFFFFF>Imbue Last Property - Imbues a new item with the last property", false, false);

            AddButton(25, 184, 4017, 4018, 10010, GumpButtonType.Reply, 0);
            AddHtml(66, 186, 430, 18, "<BASEFONT COLOR=#FFFFFF>Unravel Item - Extracts magical ingredients from an item, destroying it", false, false);
            AddButton(25, 213, 4017, 4018, 10011, GumpButtonType.Reply, 0);
            AddHtml(66, 215, 430, 18, "<BASEFONT COLOR=#FFFFFF>Unravel Container - Unravels all items in a container", false, false);

            //AddButton(25, 242, 4017, 4018, 10012, GumpButtonType.Reply, 0);
            //AddHtml(66, 244, 430, 18, "<BASEFONT COLOR=#FFFFFF>Soul Reinforcement - Fortify a cursed artifact", false, false);

            AddButton(19, 301, 4017, 4018, 10002, GumpButtonType.Reply, 0);
            this.AddLabel(58, 302, 1359, "CANCEL");

        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            PlayerMobile pm = from as PlayerMobile;

            int buttonNum = 0;

            if (info.ButtonID > 0 && info.ButtonID < 10000)
                buttonNum = 1;
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
                case 1:
                    {
                        break;
                    }
                case 10002:  // = Cancel button
                    {
                        break;
                    }
                case 10005:  // = Imbue Item
                    {
                        from.CloseGump(typeof(ImbuingGump));

                        bool sforge = false;
                        CheckSoulForge(from, 1, out sforge);

                        if (sforge != true)
                        {
                            from.SendLocalizedMessage(1079787); // You must be near a soulforge to imbue an item.
                            break;
                        }

                        from.SendLocalizedMessage(1079589);
                        from.Target = new InternalTargetC();
                      
                        break;
                    }
                case 10006:  // = ReImbue Last ( Mod & Item )
                    {
                        object o = pm.Imbue_Item;
                        int mod = pm.Imbue_Mod;
                        int modint = pm.Imbue_ModInt;
                       
                        Item it = pm.Imbue_Item as Item;

                        from.CloseGump(typeof(ImbuingGump));
                        bool sforge = false;
                        CheckSoulForge(from, 1, out sforge);

                        if (sforge != true)
                        {
                            from.SendLocalizedMessage(1079787); // You must be near a soulforge to imbue an item.
                            break;
                        }
                        if (o == null || mod == 0 || modint == 0)
                        {
                            from.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            break;
                        }

                        // Item has been imbued 20 or more times
                        if (o is BaseWeapon) { BaseWeapon Ti = o as BaseWeapon; if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); break; } }
                        if (o is BaseArmor) { BaseArmor Ti = o as BaseArmor; if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); break; } }
                        if (o is BaseJewel) { BaseJewel Ti = o as BaseJewel; if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); break; } }
                        if (o is BaseHat) { BaseHat Ti = o as BaseHat; if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); break; } }

                        if ( it.LootType == LootType.Blessed)
                        {
                            from.SendLocalizedMessage(1080438); // You cannot imbue a blessed item.
                            break;
                        }
                        else
                        {
                            ImbuingGumpC.ImbueItem(from, o, mod, modint);
                            from.SendGump(new ImbuingGump(from));
                        }
                        break;
                    }
                  
                case 10007:  // = Imbue Last ( Select Last imbued Item )
                    {
                        object o = pm.Imbue_Item;
                        int mod = pm.Imbue_Mod;
                        int modint = pm.Imbue_ModInt;

                        from.CloseGump(typeof(ImbuingGump));
                        bool sforge = false;
                        CheckSoulForge(from, 1, out sforge);

                        if (sforge != true)
                        {
                            from.SendLocalizedMessage(1079787); // You must be near a soulforge to imbue an item.
                            break;
                        }
                        if (pm.Imbue_Item == null)
                        {
                            from.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            break;
                        }
                        else
                        {
                            ImbuingGump.ImbueStep1(from, o);
                        }
                        break;
                    }
                case 10008:  // = Imbue Last Mod( To target Item )
                    {
                        pm.Imbue_Item = null;
                        int mod = pm.Imbue_Mod;
                        int modint = pm.Imbue_ModInt;

                        from.CloseGump(typeof(ImbuingGump));
                        bool sforge = false;
                        CheckSoulForge(from, 1, out sforge);

                        if (sforge != true)
                        {
                            from.SendLocalizedMessage(1079787); // You must be near a soulforge to imbue an item.
                            break;
                        }
                        if (pm.Imbue_Mod == 0 || pm.Imbue_ModInt == 0)
                        {
                            from.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            break;
                        }
                        else
                            ImbuingGump.ImbueLastProp(from, mod, modint);

                        break;
                    }
                case 10010:  // = Unravel Item
                    {
                        from.CloseGump(typeof(ImbuingGump));
                        bool sforge = false;
                        CheckSoulForge(from, 1, out sforge);

                        if (sforge != true)
                        {
                            from.SendLocalizedMessage(1080433); // You must be near a soulforge to imbue an item.
                            break;
                        }

                        from.SendLocalizedMessage(1080422); // What item do you wish to unravel?
                        from.Target = new InternalTargetA();
                        break;
                    }
                case 10011:  // = Unravel Container
                    {
                        from.CloseGump(typeof(ImbuingGump));
                        bool sforge = false;
                        CheckSoulForge(from, 1, out sforge);

                        if (sforge != true)
                        {
                            from.SendLocalizedMessage(1080433); // You must be near a soulforge to imbue an item.
                            break;
                        }
                        from.SendMessage("Which Container do you wish to unravel the contents of?");

                        from.Target = new InternalTargetB();
                        break;
                    }
            }
            return;
        }

        // ===== Targetting ===== Unravel Single Item =====
        private class InternalTargetA : Target
        {
            public bool m_Success;

            public InternalTargetA() : base(8, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                Mobile m_mob = from;
                PlayerMobile pm = from as PlayerMobile;

                Item m_obj = o as Item;                
                int oIntense = 0;
                double oDo = 0;

                // ==== Calculate Item Intensity ====
                if (o is BaseWeapon || o is BaseArmor || o is BaseJewel || o is BaseHat)
                {
                    if (m_obj.RootParent == m_mob)
                    {
                        // = Cannot Unravel - Blessed Item
                        if (m_obj.LootType == LootType.Blessed)
                        {
                            from.SendLocalizedMessage(1080421); // You cannot unravel the magic of a blessed item.
                            return;
                        }

                        if (o is BaseWeapon)
                        {
                            oDo = ItemIntensity_Weapon(m_obj);

                            oDo = Math.Round(oDo);
                            oIntense = Convert.ToInt32(oDo);
                        }
                        else if (o is BaseArmor)
                        {
                            oDo = ItemIntensity_Armor(m_obj);

                            oDo = Math.Round(oDo);
                            oIntense = Convert.ToInt32(oDo);
                        }
                        else if (o is BaseJewel)
                        {
                            oDo = ItemIntensity_Jewelery(m_obj);

                            oDo = Math.Round(oDo);
                            oIntense = Convert.ToInt32(oDo);
                        }
                        else if (o is BaseHat)
                        {
                            oDo = ItemIntensity_Hat(m_obj);

                            oDo = Math.Round(oDo);
                            oIntense = Convert.ToInt32(oDo);
                        }

                        int URavBonus = pm.Imbue_SFBonus;
                        if (oIntense > 0)
                        {
                            m_Success = false;

                            // == Magical Residue ==
                            if (oIntense <= (200 - URavBonus)) { m_Success = m_mob.CheckSkill(SkillName.Imbuing, 0.0, 45.0); }
                            // == Enchanted Essence ==
                            else if (oIntense > (200 - URavBonus) && oIntense < (480 - URavBonus))
                            {
                                if (m_mob.Skills[SkillName.Imbuing].Base >= 45.0)
                                    m_Success = m_mob.CheckSkill(SkillName.Imbuing, 45.0, 95.0);                                
                                else
                                {
                                    m_mob.SendLocalizedMessage(1080434); // Your Imbuing skill is not high enough to magically unravel this item.
                                    return;
                                }
                            }
                            // == Relic Fragment ==
                            else if (oIntense >= (480 - URavBonus))
                            {
                                if (m_mob.Skills[SkillName.Imbuing].Base >= 45.0)
                                    m_Success = m_mob.CheckSkill(SkillName.Imbuing, 95.0, 120.0);
                                else
                                {
                                    m_mob.SendLocalizedMessage(1080434); // Your Imbuing skill is not high enough to magically unravel this item.
                                    return;
                                }
                            }
                            // ===== Unravelling Attempt FAILURE =====
                            if (!m_Success)
                            {
                                Effects.PlaySound(m_mob.Location, m_mob.Map, 0x3BF);
                                m_mob.SendLocalizedMessage(1080428); // Fail
                                return;
                            }
                            // ===== Unravelling Attempt SUCCESS =====
                            else
                            {
                                // - Give Ingredient to Unraveller
                                if (oIntense <= (200 - URavBonus)) { m_obj.Delete(); m_mob.AddToBackpack(new MagicalResidue()); }
                                if (oIntense > (200 - URavBonus) && oIntense < (480 - URavBonus)) { m_obj.Delete(); m_mob.AddToBackpack(new EnchantEssence()); }
                                if (oIntense >= (480 - URavBonus)) { m_obj.Delete(); m_mob.AddToBackpack(new RelicFragment()); }
                                // - Unravelling FX
                                Effects.PlaySound(m_mob.Location, m_mob.Map, 0x1ED);
                                Effects.SendLocationParticles(
                                    EffectItem.Create(m_mob.Location, m_mob.Map, EffectItem.DefaultDuration), 0x373A,
                                    10, 30, 0, 4, 0, 0);
                                m_mob.SendLocalizedMessage(1080429); // Unravelled :P

                                return;
                            }
                        }
                        else
                        {
                            m_mob.SendLocalizedMessage(1080437); // You cannot magically unravel this item. It appears to possess little or no magic.
                            return;
                        }
                    }
                    else
                    {
                        m_mob.SendLocalizedMessage(1080424); // The item must be in your backpack to magically unravel it.
                        return;
                    }
                }
                else
                {
                    m_mob.SendLocalizedMessage(1080425); // You cannot magically unravel this item.
                    return;
                }
            }
        }

        // ===== Targetting ===== Unravel Items in Container =====
        private class InternalTargetB : Target
        {
            public bool m_Success = false;

            public InternalTargetB() : base(8, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }
            protected override void OnTarget(Mobile from, object o)
            {                               
                Mobile m_mob = from;
                PlayerMobile pm = from as PlayerMobile;
                
                int oIntense = 0;
                double oDo = 0;

                if (o is BaseContainer)
                {
                    Item m_obj = o as Item;

                    if (m_obj.RootParent == m_mob)
                    {
                        BaseContainer sBag = o as BaseContainer;

                        object[] stuffs = sBag.FindItemsByType(typeof(object));

                        // - Cycle through each Item in the Container
                        foreach (object item in stuffs)
                        {
                            m_obj = item as Item;
                            oDo = 0; oIntense = 0;

                            if (item is BaseWeapon)
                            {
                                oDo = ItemIntensity_Weapon(m_obj);

                                oDo = Math.Round(oDo);
                                oIntense = Convert.ToInt32(oDo);
                            }
                            else if (item is BaseArmor)
                            {
                                oDo = ItemIntensity_Armor(m_obj);

                                oDo = Math.Round(oDo);
                                oIntense = Convert.ToInt32(oDo);
                            }
                            else if (item is BaseJewel)
                            {
                                oDo = ItemIntensity_Jewelery(m_obj);

                                oDo = Math.Round(oDo);
                                oIntense = Convert.ToInt32(oDo);
                            }
                            else if (item is BaseHat)
                            {
                                oDo = ItemIntensity_Hat(m_obj);

                                oDo = Math.Round(oDo);
                                oIntense = Convert.ToInt32(oDo);
                            }

                            int URavBonus = pm.Imbue_SFBonus;

                            if (oIntense > 0)
                            {
                                m_Success = false;

                                // == Magical Residue ==
                                if (oIntense <= (200 - URavBonus)) { m_Success = m_mob.CheckSkill(SkillName.Imbuing, 0.0, 45.0); }
                                // == Enchanted Essence ==
                                else if (oIntense > (200 - URavBonus) && oIntense < (480 - URavBonus))
                                {
                                    if (m_mob.Skills[SkillName.Imbuing].Base >= 45.0)
                                        m_Success = m_mob.CheckSkill(SkillName.Imbuing, 45.0, 95.0);
                                    else
                                    {
                                        m_mob.SendLocalizedMessage(1080434); // Your Imbuing skill is not high enough to magically unravel this item.
                                        return;
                                    }
                                }
                                // == Relic Fragment ==
                                else if (oIntense >= (480 - URavBonus))
                                {
                                    if (m_mob.Skills[SkillName.Imbuing].Base >= 45.0)
                                        m_Success = m_mob.CheckSkill(SkillName.Imbuing, 95.0, 120.0);
                                    else
                                    {
                                        m_mob.SendLocalizedMessage(1080434); // Your Imbuing skill is not high enough to magically unravel this item.
                                        return;
                                    }
                                }
                                // = FAILURE
                                if (!m_Success)
                                {
                                    Effects.PlaySound(m_mob.Location, m_mob.Map, 0x3BF);
                                    m_mob.SendLocalizedMessage(1080428); // Fail
                                }
                                // = SUCCESS
                                else
                                {                 
                                    // Give Ingredient
                                    if (oIntense <= (200 - URavBonus)) { m_obj.Delete(); m_mob.AddToBackpack(new MagicalResidue()); }
                                    if (oIntense > (200 - URavBonus) && oIntense < (480 - URavBonus)) { m_obj.Delete(); m_mob.AddToBackpack(new EnchantEssence()); }
                                    if (oIntense >= (480 - URavBonus)) { m_obj.Delete(); m_mob.AddToBackpack(new RelicFragment()); }
                                    // - Unravelling FX
                                    Effects.PlaySound(m_mob.Location, m_mob.Map, 0x1ED);
                                    Effects.SendLocationParticles(
                                        EffectItem.Create(m_mob.Location, m_mob.Map, EffectItem.DefaultDuration), 0x373A,
                                        10, 30, 0, 4, 0, 0);
                                    m_mob.SendLocalizedMessage(1080429); // Unravelled :P
                                    oDo = 0; oIntense = 0;
                                }
                            }
                            else
                            {
                                m_mob.SendLocalizedMessage(1080437); // You cannot magically unravel this item. It appears to possess little or no magic.
                            }
                        }
                    }
                    else
                    {
                        m_mob.SendMessage(2499, "You must target a container you are holding..");
                        return;
                    }
                }
                else
                {
                    m_mob.SendMessage(2499, "That is not a container..");
                    return;
                }
                return;
            }
        } 

        // ===== Targetting ===== Imbue Item =====
        private class InternalTargetC : Target
        {
            public InternalTargetC() : base(1, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                PlayerMobile pm = from as PlayerMobile;

                int Irf = ImbuingGump.GetItemRef( o );

                if (Irf == 0)
                {
                    from.SendLocalizedMessage(1079576);
                    return;
                }
                else
                {
                    pm.Imbue_Item = o;
                }

                if (pm.Imbue_Item != null)
                    ImbuingGump.ImbueStep1(from, pm.Imbue_Item);

                return;
            }
        }

        public static int GetItemRef(object i)
        {
            int Ir = 0;
            if (i is BaseWeapon) { Ir = 1; }
            if (i is BaseRanged) { Ir = 2; }
            if (i is BaseArmor) { Ir = 3; }
            if (i is BaseShield) { Ir = 4; }
            if (i is BaseHat) { Ir = 5; }
            if (i is BaseJewel) { Ir = 6; }

            return Ir;
        }

        // === Choose Target and Check ===
        public static void ImbueStep1(Mobile from, object o)
        {
            PlayerMobile pm = from as PlayerMobile;

            Item it = o as Item;
            if (it.LootType == LootType.Blessed)
            {
                from.SendLocalizedMessage(1080444);
                return;
            }

            if (o is BaseWeapon) { BaseWeapon Ti = o as BaseWeapon; if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); return; } }
            if (o is BaseArmor)  { BaseArmor Ti = o as BaseArmor;   if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); return; } }
            if (o is BaseJewel)  { BaseJewel Ti = o as BaseJewel;   if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); return; } }
            if (o is BaseHat)    { BaseHat Ti = o as BaseHat;       if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); return; } }

            pm.Imbue_Item = o;

            from.CloseGump(typeof(ImbuingGump));
            from.SendGump(new ImbuingGumpB(from, o));
            return;
        }

        // ===== Imbue Target with Last Prop ====================
        public static void ImbueLastProp(Mobile from, int Mod, int Mint)
        {
            from.Target = new InternalTargetD();
            return;
        }

        // ===== Targetting ===== Imbue Last Mod =====
        private class InternalTargetD : Target
        {
            public InternalTargetD() : base(2, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }
            protected override void OnTarget(Mobile from, object o)
            {
                PlayerMobile pm = from as PlayerMobile;
                int Imod = pm.Imbue_Mod;
                int ImodInt = pm.Imbue_ModInt;

                Item it = o as Item;

                // = Item has been Imbued 20 or more times
                if (o is BaseWeapon) { BaseWeapon Ti = o as BaseWeapon; if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); return; } }
                if (o is BaseArmor)  { BaseArmor Ti = o as BaseArmor;   if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); return; } }
                if (o is BaseJewel)  { BaseJewel Ti = o as BaseJewel;   if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); return; } }
                if (o is BaseHat)    { BaseHat Ti = o as BaseHat;       if (Ti.TimesImbued >= 20) { from.SendMessage("This item has been modified too many times and cannot be imbued any further."); return; } }
                
                if (it.LootType == LootType.Blessed)
                {
                    from.SendLocalizedMessage(1080438); // You cannot imbue a blessed item.
                    return;
                }

                // = Check Last Mod can be applied to Targeted Item Type
                if (o is BaseMeleeWeapon)
                {
                    if (Imod == 1 || Imod == 2 || Imod == 12 || Imod == 13 || Imod == 16 || Imod == 21 || Imod == 22 || (Imod >= 25 && Imod <= 41) || Imod >= 101)
                    {
                        ImbuingGumpC.ImbueItem(from, o, Imod, ImodInt);
                        from.SendGump(new ImbuingGump(from));
                        return;
                    }
                    else
                        from.SendMessage("The selected item cannot be Imbued with the last Property..");
                }
                else if (o is BaseRanged)
                {
                    if (Imod == 1 || Imod == 2 || Imod == 12 || Imod == 13 || Imod == 16 || Imod == 21 || Imod == 22 || Imod == 60 || Imod == 61 || (Imod >= 25 && Imod <= 41) || Imod >= 101)
                    {
                        ImbuingGumpC.ImbueItem(from, o, Imod, ImodInt);
                        from.SendGump(new ImbuingGump(from));
                        return;
                    }
                    else
                        from.SendMessage("The selected item cannot be Imbued with the last Property..");
                }
                else if (o is BaseShield)
                {
                    if (Imod == 1 || Imod == 2 || Imod == 19 || Imod == 16 || Imod == 22 || Imod == 24 || Imod == 42)
                    {
                        ImbuingGumpC.ImbueItem(from, o, Imod, ImodInt);
                        from.SendGump(new ImbuingGump(from));
                        return;
                    }
                    else
                        from.SendMessage("The selected item cannot be Imbued with the last Property..");
                }
                else if (o is BaseArmor)
                {
                    if (Imod == 3 || Imod == 4 || Imod == 5 || Imod == 9 || Imod == 10 || Imod == 11 || Imod == 21 || Imod == 23 || (Imod >= 17 && Imod <= 19))
                    {
                        ImbuingGumpC.ImbueItem(from, o, Imod, ImodInt);
                        from.SendGump(new ImbuingGump(from));
                        return;
                    }
                    else
                        from.SendMessage("The selected item cannot be Imbued with the last Property..");
                }
                else if (o is BaseHat)
                {
                    if (Imod == 3 || Imod == 4 || Imod == 5 || Imod == 9 || Imod == 10 || Imod == 11 || Imod == 21 || Imod == 23 || (Imod >= 17 && Imod <= 19))
                    {
                        ImbuingGumpC.ImbueItem(from, o, Imod, ImodInt);
                        from.SendGump(new ImbuingGump(from));
                        return;
                    }
                    else
                        from.SendMessage("The selected item cannot be Imbued with the last Property..");
                }
                else if (o is BaseJewel)
                {
                    if (Imod == 1 || Imod == 2 || Imod == 6 || Imod == 7 || Imod == 8 || Imod == 12 || Imod == 10 || Imod == 11 || Imod == 20 || Imod == 21 || Imod == 23 || Imod == 21 || (Imod >= 14 && Imod <= 18) || (Imod >= 51 && Imod <= 55) || Imod >= 151)
                    {
                        ImbuingGumpC.ImbueItem(from, o, Imod, ImodInt);
                        from.SendGump(new ImbuingGump(from));
                        return;
                    }
                    else
                        from.SendMessage("The selected item cannot be Imbued with the last Property..");
                }
                else
                    from.SendMessage("The selected item cannot be Imbued with the last Property..");

                return;
            }
        }

        // =========== Calculate Items Total Attribute Weight ======================
        public static double ItemIntensity_Weapon(Item item)
        {
            double oDo = 0;
            BaseWeapon w = item as BaseWeapon;

            // - Ranged Weapons
            if (item is BaseRanged)
            {
                BaseRanged r = item as BaseRanged;
                if (r.Velocity > 0) { oDo += (130 / 50) * r.Velocity;  }
                if (r.Balanced == true) { oDo += 150;  }
                if (w.Attributes.DefendChance > 0) { oDo += (130 / 25) * w.Attributes.DefendChance;  }
                if (w.Attributes.AttackChance > 0) { oDo += (130 / 25) * w.Attributes.AttackChance;  }
                if (w.Attributes.Luck > 0) { oDo += (100 / 120) * w.Attributes.Luck;  }
                if (w.WeaponAttributes.ResistPhysicalBonus > 0) { oDo += (100 / 18) * w.WeaponAttributes.ResistPhysicalBonus;  }
                if (w.WeaponAttributes.ResistFireBonus > 0) { oDo += (100 / 18) * w.WeaponAttributes.ResistFireBonus;  }
                if (w.WeaponAttributes.ResistColdBonus > 0) { oDo += (100 / 18) * w.WeaponAttributes.ResistColdBonus;  }
                if (w.WeaponAttributes.ResistPoisonBonus > 0) { oDo += (100 / 18) * w.WeaponAttributes.ResistPoisonBonus;  }
                if (w.WeaponAttributes.ResistEnergyBonus > 0) { oDo += (100 / 18) * w.WeaponAttributes.ResistEnergyBonus;  }
            }
            // - Melee Weapons
            else 
            {
                if (w.Attributes.DefendChance > 0) { oDo += (130 / 15) * w.Attributes.DefendChance;  }
                if (w.Attributes.AttackChance > 0) { oDo += (130 / 15) * w.Attributes.AttackChance;  }
                if (w.Attributes.Luck > 0) { oDo += w.Attributes.Luck;  }
                if (w.WeaponAttributes.ResistPhysicalBonus > 0) { oDo += (100 / 15) * w.WeaponAttributes.ResistPhysicalBonus;  }
                if (w.WeaponAttributes.ResistFireBonus > 0) { oDo += (100 / 15) * w.WeaponAttributes.ResistFireBonus;  }
                if (w.WeaponAttributes.ResistColdBonus > 0) { oDo += (100 / 15) * w.WeaponAttributes.ResistColdBonus;  }
                if (w.WeaponAttributes.ResistPoisonBonus > 0) { oDo += (100 / 15) * w.WeaponAttributes.ResistPoisonBonus;  }
                if (w.WeaponAttributes.ResistEnergyBonus > 0) { oDo += (100 / 15) * w.WeaponAttributes.ResistEnergyBonus;  }
            }
            // - All Weapon Types
            if (w.Attributes.RegenHits > 0) { oDo += (50 * w.Attributes.RegenHits);  }
            if (w.Attributes.RegenStam > 0) { oDo += ((100 / 3) * w.Attributes.RegenStam);  }
            if (w.Attributes.RegenMana > 0) { oDo += (50 * w.Attributes.RegenMana);  }
            if (w.Attributes.BonusStr > 0) { oDo += (110 / 8) * w.Attributes.BonusStr;  }
            if (w.Attributes.BonusDex > 0) { oDo += (110 / 8) * w.Attributes.BonusDex;  }
            if (w.Attributes.BonusInt > 0) { oDo += (110 / 8) * w.Attributes.BonusInt;  }
            if (w.Attributes.BonusHits > 0) { oDo += 22 * w.Attributes.BonusHits;  }
            if (w.Attributes.BonusStam > 0) { oDo += (100 / 8) * w.Attributes.BonusStam;  }
            if (w.Attributes.BonusMana > 0) { oDo += (110 / 8) * w.Attributes.BonusMana;  }
            if (w.Attributes.WeaponDamage > 0 && w.DImodded == false) { oDo += (2 * w.Attributes.WeaponDamage); }
            if (w.Attributes.WeaponSpeed > 0) { oDo += (110 / 30) * w.Attributes.WeaponSpeed;  }
            if (w.Attributes.SpellDamage > 0) { oDo += (100 / 12) * w.Attributes.SpellDamage;  }
            if (w.Attributes.CastRecovery > 0) { oDo += (40 * w.Attributes.CastRecovery);  }
            if (w.Attributes.LowerManaCost > 0) { oDo += (110 / 8) * w.Attributes.LowerManaCost;  }
            if (w.Attributes.LowerRegCost > 0) { oDo += (5 * w.Attributes.LowerRegCost);  }
            if (w.Attributes.ReflectPhysical > 0) { oDo += (100 / 15) * w.Attributes.ReflectPhysical;  }
            if (w.Attributes.EnhancePotions > 0) { oDo += (4 * w.Attributes.EnhancePotions);  }
            if (w.Attributes.NightSight > 0) { oDo += 50;  }

            if (w.Attributes.SpellChanneling > 0)
            {
                oDo += 100; 
                if (w.Attributes.CastSpeed == 0) { oDo += 140;  }
                if (w.Attributes.CastSpeed == 1) { oDo += 280;  }
            }
            else if (w.Attributes.CastSpeed > 0) { oDo += (140 * w.Attributes.CastSpeed);  }

            if (w.WeaponAttributes.HitLeechHits > 0) { oDo += (110 / 50) * w.WeaponAttributes.HitLeechHits;  }
            if (w.WeaponAttributes.HitLeechStam > 0) { oDo += 2 * w.WeaponAttributes.HitLeechStam;  }
            if (w.WeaponAttributes.HitLeechMana > 0) { oDo += (110 / 50) * w.WeaponAttributes.HitLeechMana;  }
            if (w.WeaponAttributes.HitLowerAttack > 0) { oDo += (110 / 50) * w.WeaponAttributes.HitLowerAttack;  }
            if (w.WeaponAttributes.HitLowerDefend > 0) { oDo += (130 / 50) * w.WeaponAttributes.HitLowerDefend;  }
            if (w.WeaponAttributes.HitColdArea > 0) { oDo += (2 * w.WeaponAttributes.HitColdArea);  }
            if (w.WeaponAttributes.HitFireArea > 0) { oDo += (2 * w.WeaponAttributes.HitFireArea);  }
            if (w.WeaponAttributes.HitPoisonArea > 0) { oDo += (2 * w.WeaponAttributes.HitPoisonArea);  }
            if (w.WeaponAttributes.HitEnergyArea > 0) { oDo += (2 * w.WeaponAttributes.HitEnergyArea);  }
            if (w.WeaponAttributes.HitPhysicalArea > 0) { oDo += (2 * w.WeaponAttributes.HitPhysicalArea);  }
            if (w.WeaponAttributes.HitMagicArrow > 0) { oDo += 2.4 * w.WeaponAttributes.HitMagicArrow;  }
            if (w.WeaponAttributes.HitHarm > 0) { oDo += (110 / 50) * w.WeaponAttributes.HitHarm;  }
            if (w.WeaponAttributes.HitFireball > 0) { oDo += 2.4 * w.WeaponAttributes.HitFireball;  }
            if (w.WeaponAttributes.HitLightning > 0) { oDo += 2.4 * w.WeaponAttributes.HitLightning;  }
            if (w.WeaponAttributes.HitDispel > 0) { oDo += (2 * w.WeaponAttributes.HitDispel);  }
            if (w.WeaponAttributes.UseBestSkill > 0) { oDo += 150;  }
            if (w.WeaponAttributes.MageWeapon > 0) { oDo += (10 * w.WeaponAttributes.MageWeapon);  }
            if (w.WeaponAttributes.DurabilityBonus > 0) { oDo += w.WeaponAttributes.DurabilityBonus;  }
            if (w.WeaponAttributes.LowerStatReq > 0) { oDo += w.WeaponAttributes.LowerStatReq;  }
                        
            if (w.Slayer == SlayerName.Silver || w.Slayer == SlayerName.Repond || w.Slayer == SlayerName.ReptilianDeath || w.Slayer == SlayerName.Exorcism || w.Slayer == SlayerName.ArachnidDoom || w.Slayer == SlayerName.ElementalBan || w.Slayer == SlayerName.Fey) { oDo += 130;  }
            else if (w.Slayer != SlayerName.None) { oDo += 110;  }
            if (w.Slayer2 == SlayerName.Silver || w.Slayer2 == SlayerName.Repond || w.Slayer2 == SlayerName.ReptilianDeath || w.Slayer2 == SlayerName.Exorcism || w.Slayer2 == SlayerName.ArachnidDoom || w.Slayer2 == SlayerName.ElementalBan || w.Slayer2 == SlayerName.Fey) { oDo += 130;  }
            else if (w.Slayer2 != SlayerName.None) { oDo += 110;  }

            if (w.SkillBonuses.GetBonus(0) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(0);  }
            if (w.SkillBonuses.GetBonus(1) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(1);  }
            if (w.SkillBonuses.GetBonus(2) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(2);  }
            if (w.SkillBonuses.GetBonus(3) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(3);  }
            if (w.SkillBonuses.GetBonus(4) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(4);  }

            return oDo;
        }

        public static double ItemIntensity_Armor(Item item)
        {
            double oDo = 0;
            BaseArmor w = item as BaseArmor;

            if (w.Attributes.DefendChance > 0) { oDo += (130 / 15) * w.Attributes.DefendChance;  }
            if (w.Attributes.AttackChance > 0) { oDo += (130 / 15) * w.Attributes.AttackChance;  }
            if (w.Attributes.Luck > 0) { oDo += w.Attributes.Luck;  }
            if (w.Attributes.RegenHits > 0) { oDo += (50 * w.Attributes.RegenHits);  }
            if (w.Attributes.RegenStam > 0) { oDo += ((100 / 3) * w.Attributes.RegenStam);  }
            if (w.Attributes.RegenMana > 0) { oDo += (50 * w.Attributes.RegenMana);  }
            if (w.Attributes.BonusStr > 0) { oDo += (110 / 8) * w.Attributes.BonusStr;  }
            if (w.Attributes.BonusDex > 0) { oDo += (110 / 8) * w.Attributes.BonusDex;  }
            if (w.Attributes.BonusInt > 0) { oDo += (110 / 8) * w.Attributes.BonusInt;  }
            if (w.Attributes.BonusHits > 0) { oDo += 22 * w.Attributes.BonusHits;  }
            if (w.Attributes.BonusStam > 0) { oDo += (100 / 8) * w.Attributes.BonusStam;  }
            if (w.Attributes.BonusMana > 0) { oDo += (110 / 8) * w.Attributes.BonusMana;  }
            if (w.Attributes.WeaponDamage > 0) { oDo += (2 * w.Attributes.WeaponDamage);  }
            if (w.Attributes.WeaponSpeed > 0) { oDo += (110 / 30) * w.Attributes.WeaponSpeed;  }
            if (w.Attributes.SpellDamage > 0) { oDo += (100 / 12) * w.Attributes.SpellDamage;  }
            if (w.Attributes.CastRecovery > 0) { oDo += (40 * w.Attributes.CastRecovery);  }
            if (w.Attributes.LowerManaCost > 0) { oDo += (110 / 8) * w.Attributes.LowerManaCost;  }
            if (w.Attributes.LowerRegCost > 0) { oDo += (5 * w.Attributes.LowerRegCost);  }
            if (w.Attributes.ReflectPhysical > 0) { oDo += (100 / 15) * w.Attributes.ReflectPhysical;  }
            if (w.Attributes.EnhancePotions > 0) { oDo += (4 * w.Attributes.EnhancePotions);  }
            if (w.Attributes.NightSight > 0) { oDo += 50;  }
            if (w.ArmorAttributes.LowerStatReq > 0) { oDo += w.ArmorAttributes.LowerStatReq;  }
            if (w.ArmorAttributes.MageArmor > 0) { oDo += 140;  }
            if (w.ArmorAttributes.DurabilityBonus > 0) { oDo += w.ArmorAttributes.DurabilityBonus;  }

            if (w.Attributes.SpellChanneling > 0)
            {
                oDo += 100; 
                if (w.Attributes.CastSpeed == 0) { oDo += 140;  }
                if (w.Attributes.CastSpeed == 1) { oDo += 280;  }
            }
            else if (w.Attributes.CastSpeed > 0) { oDo += (140 * w.Attributes.CastSpeed);  }
                                 
            if (w.Quality == ArmorQuality.Exceptional)
            {
                if (w.Physical_Modded && w.PhysicalBonus > 0) { oDo += ((100 / 15) * w.PhysicalBonus); }
                if (w.Fire_Modded && w.FireBonus > 0) { oDo += ((100 / 15) * w.FireBonus); }
                if (w.Cold_Modded && w.ColdBonus > 0) { oDo += ((100 / 15) * w.ColdBonus); }
                if (w.Poison_Modded && w.PoisonBonus > 0) { oDo += ((100 / 15) * w.PoisonBonus); }
                if (w.Energy_Modded && w.EnergyBonus > 0) { oDo += ((100 / 15) * w.EnergyBonus); }
            }
            else
            {
                if (w.PhysicalBonus > 0) { oDo += (100 / 15) * w.PhysicalBonus;  }
                if (w.FireBonus > 0) { oDo += (100 / 15) * w.FireBonus;  }
                if (w.ColdBonus > 0) { oDo += (100 / 15) * w.ColdBonus;  }
                if (w.PoisonBonus > 0) { oDo += (100 / 15) * w.PoisonBonus;  }
                if (w.EnergyBonus > 0) { oDo += (100 / 15) * w.EnergyBonus;  }
            }
            if (w.SkillBonuses.GetBonus(0) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(0);  }
            if (w.SkillBonuses.GetBonus(1) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(1);  }
            if (w.SkillBonuses.GetBonus(2) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(2);  }
            if (w.SkillBonuses.GetBonus(3) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(3);  }
            if (w.SkillBonuses.GetBonus(4) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(4);  }

            return oDo;
        }

        public static double ItemIntensity_Jewelery(Item item)
        {
            double oDo = 0;
            BaseJewel w = item as BaseJewel;

            if (w.Attributes.DefendChance > 0) { oDo += (130 / 15) * w.Attributes.DefendChance;  }
            if (w.Attributes.AttackChance > 0) { oDo += (130 / 15) * w.Attributes.AttackChance;  }
            if (w.Attributes.Luck > 0) { oDo += w.Attributes.Luck;  }
            if (w.Attributes.RegenHits > 0) { oDo += (50 * w.Attributes.RegenHits);  }
            if (w.Attributes.RegenStam > 0) { oDo += ((100 / 3) * w.Attributes.RegenStam);  }
            if (w.Attributes.RegenMana > 0) { oDo += (50 * w.Attributes.RegenMana);  }
            if (w.Attributes.BonusStr > 0) { oDo += (110 / 8) * w.Attributes.BonusStr;  }
            if (w.Attributes.BonusDex > 0) { oDo += (110 / 8) * w.Attributes.BonusDex;  }
            if (w.Attributes.BonusInt > 0) { oDo += (110 / 8) * w.Attributes.BonusInt;  }
            if (w.Attributes.BonusHits > 0) { oDo += 22 * w.Attributes.BonusHits;  }
            if (w.Attributes.BonusStam > 0) { oDo += (100 / 8) * w.Attributes.BonusStam;  }
            if (w.Attributes.BonusMana > 0) { oDo += (110 / 8) * w.Attributes.BonusMana;  }
            if (w.Attributes.WeaponDamage > 0) { oDo += (2 * w.Attributes.WeaponDamage);  }
            if (w.Attributes.WeaponSpeed > 0) { oDo += (110 / 30) * w.Attributes.WeaponSpeed;  }
            if (w.Attributes.SpellDamage > 0) { oDo += (100 / 12) * w.Attributes.SpellDamage;  }
            if (w.Attributes.CastRecovery > 0) { oDo += (40 * w.Attributes.CastRecovery);  }
            if (w.Attributes.LowerManaCost > 0) { oDo += (110 / 8) * w.Attributes.LowerManaCost;  }
            if (w.Attributes.LowerRegCost > 0) { oDo += (5 * w.Attributes.LowerRegCost);  }
            if (w.Attributes.ReflectPhysical > 0) { oDo += (100 / 15) * w.Attributes.ReflectPhysical;  }
            if (w.Attributes.EnhancePotions > 0) { oDo += (4 * w.Attributes.EnhancePotions);  }
            if (w.Attributes.NightSight > 0) { oDo += 50;  }
            
            if (w.Attributes.SpellChanneling > 0)
            {
                oDo += 100; 
                if (w.Attributes.CastSpeed == 0) { oDo += 140;  }
                if (w.Attributes.CastSpeed == 1) { oDo += 280;  }
            }
            else if (w.Attributes.CastSpeed > 0) { oDo += (140 * w.Attributes.CastSpeed);  }
           
            if (w.Resistances.Physical > 0) { oDo += (100 / 15) * w.Resistances.Physical;  }
            if (w.Resistances.Fire > 0) { oDo += (100 / 15) * w.Resistances.Fire;  }
            if (w.Resistances.Cold > 0) { oDo += (100 / 15) * w.Resistances.Cold;  }
            if (w.Resistances.Poison > 0) { oDo += (100 / 15) * w.Resistances.Poison;  }
            if (w.Resistances.Energy > 0) { oDo += (100 / 15) * w.Resistances.Energy;  }

            if (w.SkillBonuses.GetBonus(0) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(0);  }
            if (w.SkillBonuses.GetBonus(1) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(1);  }
            if (w.SkillBonuses.GetBonus(2) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(2);  }
            if (w.SkillBonuses.GetBonus(3) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(3);  }
            if (w.SkillBonuses.GetBonus(4) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(4);  }

            return oDo;
        }

        public static double ItemIntensity_Hat(Item item)
        {
            double oDo = 0;
            BaseHat w = item as BaseHat;

            if (w.Attributes.DefendChance > 0) { oDo += (130 / 15) * w.Attributes.DefendChance;  }
            if (w.Attributes.AttackChance > 0) { oDo += (130 / 15) * w.Attributes.AttackChance;  }
            if (w.Attributes.Luck > 0) { oDo += w.Attributes.Luck;  }
            if (w.Attributes.RegenHits > 0) { oDo += (50 * w.Attributes.RegenHits);  }
            if (w.Attributes.RegenStam > 0) { oDo += ((100 / 3) * w.Attributes.RegenStam);  }
            if (w.Attributes.RegenMana > 0) { oDo += (50 * w.Attributes.RegenMana);  }
            if (w.Attributes.BonusStr > 0) { oDo += (110 / 8) * w.Attributes.BonusStr;  }
            if (w.Attributes.BonusDex > 0) { oDo += (110 / 8) * w.Attributes.BonusDex;  }
            if (w.Attributes.BonusInt > 0) { oDo += (110 / 8) * w.Attributes.BonusInt;  }
            if (w.Attributes.BonusHits > 0) { oDo += 22 * w.Attributes.BonusHits;  }
            if (w.Attributes.BonusStam > 0) { oDo += (100 / 8) * w.Attributes.BonusStam;  }
            if (w.Attributes.BonusMana > 0) { oDo += (110 / 8) * w.Attributes.BonusMana;  }
            if (w.Attributes.WeaponDamage > 0) { oDo += (2 * w.Attributes.WeaponDamage);  }
            if (w.Attributes.WeaponSpeed > 0) { oDo += (110 / 30) * w.Attributes.WeaponSpeed;  }
            if (w.Attributes.SpellDamage > 0) { oDo += (100 / 12) * w.Attributes.SpellDamage;  }
            if (w.Attributes.CastRecovery > 0) { oDo += (40 * w.Attributes.CastRecovery);  }
            if (w.Attributes.LowerManaCost > 0) { oDo += (110 / 8) * w.Attributes.LowerManaCost;  }
            if (w.Attributes.LowerRegCost > 0) { oDo += (5 * w.Attributes.LowerRegCost);  }
            if (w.Attributes.ReflectPhysical > 0) { oDo += (100 / 15) * w.Attributes.ReflectPhysical;  }
            if (w.Attributes.EnhancePotions > 0) { oDo += (4 * w.Attributes.EnhancePotions);  }
            if (w.Attributes.NightSight > 0) { oDo += 50;  }
            if (w.ClothingAttributes.LowerStatReq > 0) { oDo += w.ClothingAttributes.LowerStatReq;  }
          
            if (w.Attributes.SpellChanneling > 0)
            {
                oDo += 100; 
                if (w.Attributes.CastSpeed == 0) { oDo += 140;  }
                if (w.Attributes.CastSpeed == 1) { oDo += 280;  }
            }
            else if (w.Attributes.CastSpeed > 0) { oDo += (140 * w.Attributes.CastSpeed);  }
            
            if (w.Quality != ClothingQuality.Exceptional)
            {
                if (w.Resistances.Physical > 0) { oDo += (100 / 15) * w.Resistances.Physical;  }
                if (w.Resistances.Fire > 0) { oDo += (100 / 15) * w.Resistances.Fire;  }
                if (w.Resistances.Cold > 0) { oDo += (100 / 15) * w.Resistances.Cold;  }
                if (w.Resistances.Poison > 0) { oDo += (100 / 15) * w.Resistances.Poison;  }
                if (w.Resistances.Energy > 0) { oDo += (100 / 15) * w.Resistances.Energy;  }
            }
            else if (w.Quality >= ClothingQuality.Exceptional)
            {
                if (w.Physical_Modded && w.Resistances.Physical > 0) { oDo += ((100 / 15) * w.Resistances.Physical);  }
                if (w.Fire_Modded && w.Resistances.Fire > 0) { oDo += ((100 / 15) * w.Resistances.Fire);  }
                if (w.Cold_Modded && w.Resistances.Cold > 0) { oDo += ((100 / 15) * w.Resistances.Cold);  }
                if (w.Poison_Modded && w.Resistances.Poison > 0) { oDo += ((100 / 15) * w.Resistances.Poison);  }
                if (w.Energy_Modded && w.Resistances.Energy > 0) { oDo += ((100 / 15) * w.Resistances.Energy);  }
            }

            if (w.SkillBonuses.GetBonus(0) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(0);  }
            if (w.SkillBonuses.GetBonus(1) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(1);  }
            if (w.SkillBonuses.GetBonus(2) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(2);  }
            if (w.SkillBonuses.GetBonus(3) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(3);  }
            if (w.SkillBonuses.GetBonus(4) > 0) { oDo += (140 / 15) * w.SkillBonuses.GetBonus(4);  }

            return oDo;
        }
    }
}        
