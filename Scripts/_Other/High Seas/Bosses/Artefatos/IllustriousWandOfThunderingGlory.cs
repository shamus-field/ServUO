using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x26BC, 0x26C6 )]
	public class IllustriousWandOfThunderingGlory : BaseBashing
	{
		public override int LabelNumber{ get{ return 1116623; } } // Illustrious Wand of Thundering Glory
		
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int AosStrengthReq{ get{ return 40; } }
		public override int AosMinDamage{ get{ return 14; } }
		public override int AosMaxDamage{ get{ return 17; } }
		public override int AosSpeed{ get{ return 30; } }
		
		#region Mondain's Legacy
		public override float MlSpeed{ get{ return 3.50f; } }
		#endregion

		public override int OldStrengthReq{ get{ return 40; } }
		public override int OldMinDamage{ get{ return 14; } }
		public override int OldMaxDamage{ get{ return 17; } }
		public override int OldSpeed{ get{ return 30; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public IllustriousWandOfThunderingGlory() : base( 0x26BC )
		{
			
			Attributes.SpellChanneling = 1;
			WeaponAttributes.HitLightning = 40;
			WeaponAttributes.HitLowerDefend = 5;
			Attributes.WeaponSpeed = 10;
			Attributes.WeaponDamage = 50;
		}
		#region Mondain's Legacy
		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = pois = fire = cold = nrgy = direct = 0;
			chaos = 100;
		}
		#endregion


		public IllustriousWandOfThunderingGlory( Serial serial ) : base( serial )
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