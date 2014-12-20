using System;
using Server;

namespace Server.Items
{
	public class RuneEngravedPegLeg : Club
	{
		public override int LabelNumber{ get{ return 1116622; } }// Rune Engraved Peg Leg

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public RuneEngravedPegLeg()
		{
			Hue = 1256;
			
			WeaponAttributes.HitLightning = 40;
			WeaponAttributes.HitLowerDefend = 40;
			Attributes.RegenHits = 3;
			Attributes.WeaponSpeed = 30;
			Attributes.WeaponDamage = 50;
		}
		
		#region Mondain's Legacy
		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = pois = fire = cold = nrgy = direct = 0;
			chaos = 100;
		}
		#endregion

		public RuneEngravedPegLeg( Serial serial ) : base( serial )
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