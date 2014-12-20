using System;
using Server.Items;

namespace Server.Items
{
	public class RingOfTheSoulbinder : GoldRing
	{
		public override int LabelNumber{ get{ return 1116620; } } // Ring of the Soulbinder
		
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public RingOfTheSoulbinder() : base()
		{

			Hue = 1266;  
			Attributes.RegenMana = 2;
			Attributes.DefendChance = 15;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 3;
			Attributes.SpellDamage = 10;
			Attributes.LowerRegCost = 10;

		}

		public RingOfTheSoulbinder( Serial serial ) : base( serial )
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
