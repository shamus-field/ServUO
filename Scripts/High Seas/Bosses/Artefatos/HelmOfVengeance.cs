using System;
using Server;

namespace Server.Items
{
    public class HelmOfVengeance : NorseHelm, ITokunoDyable
	{
		public override int LabelNumber{ get{ return 1116621; } } // Helm of Vengeance
		
		public override int BasePhysicalResistance{ get{ return 11; } }
		public override int BaseFireResistance{ get{ return 10; } }
		public override int BaseColdResistance{ get{ return 14; } }
		public override int BasePoisonResistance{ get{ return 7; } }
		public override int BaseEnergyResistance{ get{ return 8; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public HelmOfVengeance() : base()
		{
			Hue = 1355;
			
			Attributes.RegenMana = 3;
			Attributes.ReflectPhysical = 30;
			Attributes.AttackChance = 7;
			Attributes.WeaponDamage = 50;//should be damage increase 10
			Attributes.LowerManaCost = 8;

		}

		public HelmOfVengeance( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}