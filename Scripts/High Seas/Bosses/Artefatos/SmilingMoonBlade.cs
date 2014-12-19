using System;
using Server;

namespace Server.Items
{
	public class SmilingMoonBlade : CrescentBlade, ITokunoDyable
	{
		public override int LabelNumber{ get{ return 1116628; } } // Smiling Moon Blade
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public SmilingMoonBlade()
		{
			Hue = 1162;
			
			WeaponAttributes.HitLeechMana = 10;
			WeaponAttributes.HitFireball = 45;
			WeaponAttributes.HitLowerDefend = 40;
			Attributes.WeaponSpeed = 30;
			Attributes.WeaponDamage = 45;

		}
		
		#region Mondain's Legacy
		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = pois = fire = chaos = nrgy = direct = 0;
			cold = 100;
		}
		#endregion

		public SmilingMoonBlade( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}