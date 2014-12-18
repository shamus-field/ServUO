using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.SkillHandlers;

namespace Server.Items
{
	public class ItemIdentification
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.ItemID].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile from )
		{
			from.SendLocalizedMessage( 500343 ); // What do you wish to appraise and identify?
			from.Target = new InternalTarget();

			return TimeSpan.FromSeconds( 1.0 );
		}

		[PlayerVendorTarget]
		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 8, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile from, object o )
			{
                // = [SF Imbuing] - Examine Item to show Intesity and Unravelling Ingredient =
                if (Core.AOS)
                {
                    int oInt = 0; int oMods = 0;
                    double oDo = 0;

                    Item m_obj = o as Item;
                   
                    if (o is BaseWeapon || o is BaseArmor || o is BaseJewel || o is BaseHat)
                    {
                        if (from.Skills[SkillName.ItemID].Base >= 100.0)
                        {
                            if (o is BaseWeapon)
                            {
                                oDo = ImbuingGump.ItemIntensity_Weapon(m_obj);

                                oDo = Math.Round(oDo, 1);
                                oInt = Convert.ToInt32(oDo);
                            }
                            else if (o is BaseArmor)
                            {
                                oDo = ImbuingGump.ItemIntensity_Armor(m_obj);

                                oDo = Math.Round(oDo, 1);
                                oInt = Convert.ToInt32(oDo);
                            }
                            else if (o is BaseJewel)
                            {
                                oDo = ImbuingGump.ItemIntensity_Jewelery(m_obj);

                                oDo = Math.Round(oDo, 1);
                                oInt = Convert.ToInt32(oDo);
                            }
                            else if (o is BaseHat)
                            {
                                oDo = ImbuingGump.ItemIntensity_Hat(m_obj);

                                oDo = Math.Round(oDo, 1);
                                oInt = Convert.ToInt32(oDo);
                            }

                            // == Send Imbuing/ID Messages ==
                            // Magical Residue
                            if (oInt > 0 && oInt <= 200)
                            {
                                from.LocalOverheadMessage(MessageType.Regular, 2304, false, "You conclude that item will magically unravel into: Magical Residue");
                                if (from.Skills[SkillName.Imbuing].Base >= 100.0)
                                    from.LocalOverheadMessage(MessageType.Regular, 2304, false, String.Format("Item Intensity: {0}", oInt));
                            }
                            // Enchanted Essence
                            else if (oInt > 200 && oInt < 480)
                            {
                                if (from.Skills[SkillName.Imbuing].Base >= 45.0)
                                {
                                    from.LocalOverheadMessage(MessageType.Regular, 2304, false, "You conclude that item will magically unravel into: Enchanted Essence");
                                    if (from.Skills[SkillName.Imbuing].Base >= 100.0)
                                        from.LocalOverheadMessage(MessageType.Regular, 2304, false, String.Format("Item Intensity: {0}", oInt));
                                }
                                else
                                {
                                    from.LocalOverheadMessage(MessageType.Regular, 2304, false, "Your Imbuing skill is not high enough to identify the imbuing ingredient.");
                                }
                            }
                            // Relic Fragment
                            else if (oInt >= 480)
                            {
                                if (from.Skills[SkillName.Imbuing].Base >= 95.0)
                                {
                                    from.LocalOverheadMessage(MessageType.Regular, 2304, false, "You conclude that item will magically unravel into: Relic Fragment");
                                    if (from.Skills[SkillName.Imbuing].Base >= 100.0)
                                        from.LocalOverheadMessage(MessageType.Regular, 2304, false, String.Format("Item Intensity: {0}", oInt));
                                }
                                else
                                {
                                    from.LocalOverheadMessage(MessageType.Regular, 2304, false, "Your Imbuing skill is not high enough to identify the imbuing ingredient.");
                                }
                            }
                            // Cannot be Unravelled
                            else
                            {
                                from.LocalOverheadMessage(MessageType.Regular, 2304, false, "You conclude that item cannot be magically unraveled. It appears to possess little to no magic.");
                            }
                        }
                        // Skill level not high enough
                        else
                        {
                            from.LocalOverheadMessage(MessageType.Regular, 2304, false, "You are uncertain.. your Item Identification skill isn't high enougth");
                        }

                        bool m_Success = from.CheckSkill(SkillName.ItemID, 0.0, 100.0);
                    }
                    else if (o is Mobile)
                    {
                        ((Mobile)o).OnSingleClick(from);
                    }
                    else
                    {
                        from.LocalOverheadMessage(MessageType.Regular, 2304, false, "You conclude that item cannot be magically unraveled.");
                    }
                }
                // ===== Pre-AOS ItemID =====
                else
                {
                    if (o is Item)
                    {
                        if (from.CheckTargetSkill(SkillName.ItemID, o, 0, 100))
                        {
                            if (o is BaseWeapon)
                                ((BaseWeapon)o).Identified = true;
                            else if (o is BaseArmor)
                                ((BaseArmor)o).Identified = true;

                            if (!Core.AOS)
                                ((Item)o).OnSingleClick(from);
                        }
                        else
                        {
                            from.SendLocalizedMessage(500353); // You are not certain...
                        }
                    }
                    else if (o is Mobile)
                    {
                        ((Mobile)o).OnSingleClick(from);
                    }
                    else
                    {
                        from.SendLocalizedMessage(500353); // You are not certain...
                    }

Server.Engines.XmlSpawner2.XmlAttach.RevealAttachments(from, o);
                }
			}
		}
	}
}