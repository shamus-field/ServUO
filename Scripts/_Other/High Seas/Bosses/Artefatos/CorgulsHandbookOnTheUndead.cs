using System;

namespace Server.Items
{
	public class CorgulsHandbookOnTheUndead : NecromancerSpellbook, ITokunoDyable
	{
		public override int LabelNumber { get { return 1149780; } } // Corgul's Handbook on the Undead

		[Constructable]
		public CorgulsHandbookOnTheUndead() : base()
		{
			Hue = 988;
			LootType = LootType.Blessed;

			Attributes.RegenMana = 3;
			Attributes.DefendChance = 5;
			Attributes.LowerManaCost = 10;
			Attributes.LowerRegCost = 20;
		}

		public CorgulsHandbookOnTheUndead( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
