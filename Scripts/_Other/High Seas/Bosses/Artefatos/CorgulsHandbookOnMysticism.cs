using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class CorgulsHandbookOnMysticism : Spellbook
	{
		public override int LabelNumber{ get{ return 1149779; } } // Corgul's Handbook on Mysticism
	
		[Constructable]
		public CorgulsHandbookOnMysticism() : base()
		{
			Name = "Corgul's Handbook on Mysticism";
			Hue = 988;
			
			Attributes.RegenMana = 3;
			Attributes.DefendChance = 5;
			Attributes.SpellDamage = 25;
			Attributes.LowerManaCost = 10;
			Attributes.LowerRegCost = 20;
		}

		public CorgulsHandbookOnMysticism( Serial serial ) : base( serial )
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
		
		public override int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			double magery = from.Skills.Magery.Value - 100;
			
			if ( magery < 0 )
				magery = 0;
					
			int count = (int) Math.Round( magery * Utility.RandomDouble() / 5 );
			
			if ( count > 2 )
				count = 2;
				
			if ( Utility.RandomDouble() < 0.5 )
				count = 0;
			else
				BaseRunicTool.ApplyAttributesTo( this, false, 0, count, 70, 80 );
				
			Attributes.SpellDamage = 25;
			Attributes.LowerManaCost = 10;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
			
			if ( makersMark )
				Crafter = from;
				
			return quality;
		}
	}
}

