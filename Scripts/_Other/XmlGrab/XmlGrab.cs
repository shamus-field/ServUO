// Author: XmlGrab 1.0, by Oak
// Date: 7/1/2006
// Version: 1.0
// Requirements: Runuo 2.0, XmlSpawner2
// History: 
//	Used "Claim System version 1.6, by Xanthos" as a base
//	added XmlAttachment to allow player specific looting options, 
//	removed external config file and other dependencies.
//	simplified code and made it grab/claim corpses only.
//
using System;
using System.Collections;
using System.Reflection;
using Server;
using Server.Items;
using Server.Misc;
using Server.Spells;
using Server.Guilds;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Network;


namespace Server.Commands
{
	public class XmlGrab
	{
		//============= Configuration ================================//
		/* How many tiles around the player that 'grab' will search	*/
		private const int GrabRadius = 5;

		/*  Set to a positive value to make players manually scavenge
			when other players are near.
			Set to zero to always allow grabbing in proximity of
			other players.	*/
		private const  int CompetitiveGrabRadius = 0;

		/* allow player corpses to be looted */
		private const  bool LootPlayers = false;

		/* Mobs with fame below this require no looting rights */
		private const  int FreelyLootableFame = 1000;

		/* minimum reward for corpes */
		private const  int MinimumReward = 100;
		
		/* Reward = (((mob fame + |mob karma| + player fame + |player karma|)/1.5) / FameDivisor)
			Lower divisor yeilds higher reward.  In this formula, it pays to keep your fame
			and karma high.	 */
		private  const int FameDivisor = 100;


		public static void Initialize()
		{
				CommandSystem.Register( "Grab", AccessLevel.Player, new CommandEventHandler( Grab_OnCommand ) );
		}

		public static bool IsArtifact( Item item )
		{
			if ( null == item )
				return false;
		
			Type t = item.GetType();
			PropertyInfo prop = null;

			try { prop = t.GetProperty( "ArtifactRarity" ); }
			catch {}

			if ( null == prop || (int)(prop.GetValue( item, null )) <= 0 )
				return false;			

			return true;
		}

		[Usage( "Grab" )]
		[Description( "Grab lootable items off of the ground and claim nearby corpses" )]
		public static void Grab_OnCommand( CommandEventArgs e )
		{
            e.Mobile.PublicOverheadMessage(MessageType.Regular, 0, false, "*yoink*");
			//   Get LootData attachment 
			LootData lootoptions = new LootData();
			// does player already have a lootdata attachment?
			if (XmlAttach.FindAttachment(e.Mobile, typeof(LootData))==null)
			{
				XmlAttach.AttachTo(e.Mobile, lootoptions);
				// give them one free lootbag
				e.Mobile.AddToBackpack ( new LootBag());
			}
			else
			{
				// they have the attachment, just load their options
				lootoptions=(LootData)XmlAttach.FindAttachment(e.Mobile,typeof(LootData));
			}

		//   Check args to see if they want to change loot options
		// if we have args after  "grab"
			if ( e.Length != 0 )
			{
				// we need to set the loot bag
				if (e.GetString(0) != "options")
				{
					e.Mobile.SendMessage ("Typing the command [grab  by itself loots corpses of your victims. [grab options will allow you to decide what you want to loot.");
				}
				// show loot options gump
				else if (e.GetString(0) == "options")
				{
				   e.Mobile.SendGump( new LootGump(e.Mobile));
				}
			}

			//   Check loot legalities
			Mobile from = e.Mobile;

			if ( from.Alive == false )
			{
				from.PlaySound( 1069 ); //hey
				from.SendMessage( "You cannot do that while you are dead!" );
				return;
			}
			else if ( 0 != CompetitiveGrabRadius && BlockingMobilesInRange( from, GrabRadius ))
			{
				from.PlaySound( 1069 ); //hey
				from.SendMessage( "You are too close to another player to do that!" );
				return;
			}

			ArrayList grounditems = new ArrayList();
			ArrayList lootitems = new ArrayList();
			ArrayList corpses = new ArrayList();
			Container lootBag = GetLootBag( from );

			// Gather lootable corpses and items into lists
			foreach ( Item item in from.GetItemsInRange( GrabRadius ))
			{
				if ( !from.InLOS( item ) || !item.IsAccessibleTo( from ) || !(item.Movable || item is Corpse) )
					continue;

				// add to corpse list if corpse
				if ( item is Corpse && CorpseIsLootable( from, item as Corpse, false ))
					corpses.Add( item );
	
				// otherwise add to ground items list if loot options indicate
				else
					if(lootoptions.GetGroundItems)
						if (!(item is Corpse))
							grounditems.Add( item );
			}

			// see if we really want any of the junk lying on the ground
			GetItems(lootoptions, from, grounditems);

			grounditems.Clear();

			// now inspect and loot appropriate items in corpses
			foreach ( Item corpse in corpses )
			{
				Corpse bod = corpse as Corpse;
				
				// if we are looting hides/scales/meat then carve the corpse
				if ( lootoptions.GetHides && !(bod.Owner is PlayerMobile))
						bod.Carve( from, null );

				//rummage through the corpse for good stuff
				foreach ( Item item in bod.Items )
					lootitems.Add( item );

				//  now see if we really want any of this junk
				GetItems(lootoptions, from,lootitems);

				// alrighty then, we have all the items we want, now award gold for this corpse, delete it and increment the body count
				//AwardGold( from, bod,lootBag);
				//bod.Delete();

				// empty lootitems arraylist
				lootitems.Clear();
			}
		}
		private static void GetItems(LootData lootoptions, Mobile from, ArrayList loothopefuls)
		{
			ArrayList itemstoloot = new ArrayList();

			Container lootBag = GetLootBag( from );

			foreach(Item item in loothopefuls)
			{
				if (item is Server.Items.Gold)
					itemstoloot.Add( item );
				else if ( (item is Server.Items.BaseWeapon || item is Server.Items.Arrow || item is Server.Items.Bolt) && lootoptions.GetWeapons &&  !lootoptions.GetArtifacts)
					itemstoloot.Add( item );
				else if (item is Server.Items.BaseArmor && lootoptions.GetArmor  && !lootoptions.GetArtifacts)
					itemstoloot.Add( item );
				else if (item is Server.Items.BaseJewel && lootoptions.GetJewelry )
					itemstoloot.Add( item );
				else if (item is Server.Items.BasePotion && lootoptions.GetPotions )
					itemstoloot.Add( item );
				else if (item is Server.Items.BaseReagent && lootoptions.GetRegs )
					itemstoloot.Add( item );
				else if ( (item is Server.Items.BaseClothing || item is Server.Items.BaseShoes) && lootoptions.GetClothes )
					itemstoloot.Add( item );
				else if ( (item is Server.Items.BaseIngot || item is Server.Items.Shaft  || item is Server.Items.Board || item is Server.Items.BaseOre  || item is Server.Items.Log  || item is Server.Items.Feather) && lootoptions.GetResources )
					itemstoloot.Add( item );
				else if (
					(item is Server.Items.Diamond ||
					 item is Server.Items.Amber   ||
					 item is Server.Items.Amethyst   ||
					 item is Server.Items.Citrine   ||
					 item is Server.Items.Emerald   ||
					 item is Server.Items.Ruby   ||
					 item is Server.Items.Sapphire   ||
					 item is Server.Items.StarSapphire   ||
					 item is Server.Items.Tourmaline )	&& lootoptions.GetGems )
					itemstoloot.Add( item );
				else if (IsArtifact( item ) && lootoptions.GetArtifacts )
					itemstoloot.Add( item );
				else if  ( (item is Server.Items.BaseHides || item is Server.Items.BaseScales))
						itemstoloot.Add( item );
				else if ( (item is Server.Items.CookableFood || item is Server.Items.Food) && lootoptions.GetFood)
					itemstoloot.Add( item );
				// add whatever it is if getall is true
				else if (lootoptions.GetAll)
					itemstoloot.Add( item );
				
				// if we have any items
				if(itemstoloot != null)
				{
					// Drop all of the items into player's bag/pack
					foreach ( Item itemx in itemstoloot )
					{
						if ( !lootBag.TryDropItem( from, itemx, false ) )
							lootBag.DropItem( itemx );
						from.PlaySound( 0x2E6 ); // drop gold sound
					}
					itemstoloot.Clear();
				}
			}
		}

		//    determine if the corpse is ok to loot
		private static bool CorpseIsLootable( Mobile from, Corpse corpse, bool notify )
		{
			if ( null == corpse )
				return false;

			bool result = false;
			string notification = "";

			if ( corpse.Owner == from )
				notification = "You may not claim your own corpses.";
			else if ( corpse.Owner is PlayerMobile && !LootPlayers )
				notification = "You may not loot player corpses.";
			else
			{
				BaseCreature creature = corpse.Owner as BaseCreature;

				if ( null != creature && creature.IsBonded )
					notification = "You may not loot the corpses of bonded pets.";
				else if ( null != creature && creature.Fame <= FreelyLootableFame )
					result = true;
				else
					result = corpse.CheckLoot( from, null ) && !( corpse.IsCriminalAction( from ) );
			}

			if ( false == result && notify )
			{
				from.PlaySound( 1074 );		// no
				from.SendMessage( notification );
			}

			return result;
		}

// ================================  make sure we have a loot bag
		public static Container GetLootBag( Mobile from )
		{
			Container lootBag = from.Backpack.FindItemByType( typeof(LootBag) ) as Container;
			return ( null == lootBag ) ? from.Backpack : lootBag;
		}

// ================================  reward the player for not leaving those stinking corpses lying about
		public static void AwardGold( Mobile from, Corpse corpse, Container lootbag )
		{
			BaseCreature mob = corpse.Owner as BaseCreature;
			int mobBasis = ( mob == null ? MinimumReward : mob.Fame + Math.Abs( mob.Karma ) );
			int playerBasis = ( from.Fame + Math.Abs( from.Karma ) );
			int amount = Math.Max( (int)((mobBasis + playerBasis) / 1.5) / FameDivisor, MinimumReward );
			if ( !lootbag.TryDropItem( from, new Gold(amount), false ) )	// Attempt to stack it
				lootbag.DropItem( new Gold(amount) );
			from.PlaySound( 0x2E6 ); // drop gold sound

		}

// ================================   Is someone too close for comfort?
		public static bool BlockingMobilesInRange( Mobile from, int range )
		{
			foreach ( Mobile mobile in from.GetMobilesInRange( range ) )
			{
				if ( mobile is PlayerMobile && IsBlockingMobile( from, mobile ) )
					return true;
			}
			return false;
		}

		public static bool IsBlockingMobile( Mobile looter, Mobile other )
		{
			// Self and hidden staff dont count
			if ( looter == other || ( other.Hidden && other.AccessLevel > AccessLevel.Player ) )
				return false;

			Guild looterGuild = SpellHelper.GetGuildFor( looter );
			Guild otherGuild = SpellHelper.GetGuildFor( other );

			if ( null != looterGuild && null != otherGuild && ( looterGuild == otherGuild || looterGuild.IsAlly( otherGuild ) ) )
				return false;

			Party party = Party.Get( looter );

			return !( null != party && party.Contains( other ) );
		}
	}
	
// ================================  Loot Options gump

public class LootGump : Gump
{
	public LootGump(Mobile m) : base( 0, 0 )
	{
		PlayerMobile mm = m as PlayerMobile;
		LootData lootoptions = (LootData)XmlAttach.FindAttachment(m, typeof(LootData));

		this.Closable=true;
		this.Disposable=true;
		this.Dragable=true;
		this.Resizable=false;
		this.AddPage(0);
		this.AddBackground(0, 0, 368, 284, 9380);

		this.AddLabel(126, 7, 0, @"Oak's Loot Options");
		this.AddLabel(81, 34, 0, @"All Corpse Items");
		this.AddLabel(81, 64, 0, @"Hides/Scales");
		this.AddLabel(81, 94, 0, @"Just Artifacts");
		this.AddLabel(81, 124, 0, @"All Weapons");
		this.AddLabel(81, 154, 0, @"All Armor");
		this.AddLabel(81, 184, 0, @"Get Ground Items");

		this.AddLabel(259,  34, 0, @"Jewelry");
		this.AddLabel(259,  64, 0, @"Potions");
		this.AddLabel(259, 94, 0, @"Gems");
		this.AddLabel(259, 124, 0, @"Clothes");
		this.AddLabel(259, 154, 0, @"Food");
		this.AddLabel(259, 184, 0, @"Resources");

		this.AddLabel(113, 210, 0, @"*Gold is always looted*");

		this.AddCheck(60, 34, 210, 211, lootoptions.GetAll,  1);
		this.AddCheck(60, 64, 210, 211, lootoptions.GetHides,  2);
		this.AddCheck(60, 94, 210, 211, lootoptions.GetArtifacts, 3);
		this.AddCheck(60, 124, 210, 211, lootoptions.GetWeapons, 4);
		this.AddCheck(60, 154, 210, 211, lootoptions.GetArmor, 5);
		this.AddCheck(60, 184, 210, 211, lootoptions.GetGroundItems, 11);

		this.AddCheck(229, 34, 210, 211, lootoptions.GetJewelry,  6);
		this.AddCheck(229, 64, 210, 211, lootoptions.GetPotions,  7);
		this.AddCheck(229, 94, 210, 211, lootoptions.GetGems,  8);
		this.AddCheck(229, 124, 210, 211, lootoptions.GetClothes, 9);
		this.AddCheck(229, 154, 210, 211, lootoptions.GetFood, 10);
		this.AddCheck(229, 184, 210, 211, lootoptions.GetResources, 12);

		this.AddButton(67, 228, 247, 248, 1, GumpButtonType.Reply, 0);
		this.AddButton(244, 228, 241, 248, 0, GumpButtonType.Reply, 0);
	}
	public override void OnResponse(Server.Network.NetState sender, RelayInfo info )
	{

		PlayerMobile mm = sender.Mobile as PlayerMobile;
		LootData lootoptions = (LootData)XmlAttach.FindAttachment(mm, typeof(LootData));
		switch( info.ButtonID ) 
	    { 
			case 0: // Closed or Cancel
	        {
				return;
	        }
	        default: 
	        { 
				// Make sure that the OK, button was pressed
			    if( info.ButtonID == 1 )
	            {
					// see what types of loot the user wants to loot and toggle the property
				    ArrayList Selections = new ArrayList( info.Switches );

					if( Selections.Contains( 1 ) == true ){	lootoptions.GetAll=true;}
					else{ lootoptions.GetAll=false;}
					if( Selections.Contains( 2 ) == true ){ lootoptions.GetHides=true;}
					else{	lootoptions.GetHides=false;}
					if( Selections.Contains( 3 ) == true ){	lootoptions.GetArtifacts=true;}
					else{	lootoptions.GetArtifacts=false;}
					if( Selections.Contains( 4 ) == true ){	lootoptions.GetWeapons=true;}
					else{	lootoptions.GetWeapons=false;}
					if( Selections.Contains( 5 ) == true ){	lootoptions.GetArmor=true;}
					else{	lootoptions.GetArmor=false;}
					if( Selections.Contains( 6 ) == true ){	lootoptions.GetJewelry=true;}
					else{	lootoptions.GetJewelry=false;}
					if( Selections.Contains( 7 ) == true ){	lootoptions.GetPotions=true;}
					else{	lootoptions.GetPotions=false;}
					if( Selections.Contains( 8 ) == true ){	lootoptions.GetGems=true;}
					else{	lootoptions.GetGems=false;}
					if( Selections.Contains( 9 ) == true ){	lootoptions.GetClothes=true;}
					else{	lootoptions.GetClothes=false;}
					if( Selections.Contains( 10 ) == true ){ lootoptions.GetFood=true;}
					else{	lootoptions.GetFood=false;}
					if( Selections.Contains( 11 ) == true ){ lootoptions.GetGroundItems=true;}
					else{	lootoptions.GetGroundItems=false;}
					if( Selections.Contains( 12 ) == true ){ lootoptions.GetResources=true;}
					else{	lootoptions.GetResources=false;}

				}
			break;
			}
		}
	}
}
}
