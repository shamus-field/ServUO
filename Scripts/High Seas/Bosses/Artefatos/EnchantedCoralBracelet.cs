using System;
using Server.Items;

namespace Server.Items
{
	public class EnchantedCoralBracelet : GoldBracelet
	{
		public override int LabelNumber{ get{ return 1116624; } } // Enchanted Coral Bracelet
		
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }
		
		[Constructable]
		public EnchantedCoralBracelet() : base()
		{
			SetHue = 2693;
			
			Attributes.BonusHits = 5;
			Attributes.RegenMana = 1;
			Attributes.AttackChance = 5;
			SetAttributes.DefendChance = 15;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 3;
			Attributes.SpellDamage = 10;
			
		}

		public EnchantedCoralBracelet( Serial serial ) : base( serial )
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
