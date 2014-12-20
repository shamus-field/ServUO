/*
Special thanks to Ryan.
With RunUO we now have the ability to become our own Richard Garriott.
All Spells System created by x-SirSly-x, Admin of Land of Obsidian.
All Spells System 4.0 created & supported by Lucid Nagual, Admin of The Conjuring.
All Spells System 5.0 created by A_li_N.
    _________________________________
 -=(_)_______________________________)=-
   /   .   .   . ____  . ___      _/
  /~ /    /   / /     / /   )2005 /
 (~ (____(___/ (____ / /___/     (
  \ ----------------------------- \
   \     lucidnagual@gmail.com     \
    \_     ===================      \
     \   -Admin of "The Conjuring"-  \
      \_     ===================     ~\
       )       All Spells System       )
      /~      Version [5].0.1        _/
    _/_______________________________/
 -=(_)_______________________________)=-
 */
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ReagentVendBag : Bag
	{
		[Constructable]
		public ReagentVendBag() : this( 100 )
		{
			Movable = true;
			Hue = 0x4F1;
			Name = "Reagent Vendor Bag";
		}

		[Constructable]
		public ReagentVendBag( int amount )
		{
			DropItem( new BlackPearl   ( amount ) );
			DropItem( new Bloodmoss    ( amount ) );
			DropItem( new Garlic       ( amount ) );
			DropItem( new Ginseng      ( amount ) );
			DropItem( new MandrakeRoot ( amount ) );
			DropItem( new Nightshade   ( amount ) );
			DropItem( new SulfurousAsh ( amount ) );
			DropItem( new SpidersSilk  ( amount ) );
		}

		public ReagentVendBag( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}