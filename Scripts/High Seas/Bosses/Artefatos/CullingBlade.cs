using System;
using Server;

namespace Server.Items
{
	public class CullingBlade : BoneHarvester, ITokunoDyable
	{
		public override int LabelNumber{ get{ return 1116630; } } // The Culling Blade
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public CullingBlade()
		{
			Hue = 988;
			WeaponAttributes.HitLeechMana = 30;
			WeaponAttributes.HitLeechStam = 30;
			WeaponAttributes.HitLowerDefend = 40;
			Attributes.RegenHits = 3;
			Attributes.WeaponSpeed = 20;
			Attributes.WeaponDamage = 50;
			Attributes.SpellDamage = 5;
			Attributes.WeaponDamage = 50;
		}
		
		#region Mondain's Legacy
		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = pois = fire = cold = nrgy = direct = 0;
			chaos = 100;
		}
		#endregion

		public CullingBlade( Serial serial ) : base( serial )
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