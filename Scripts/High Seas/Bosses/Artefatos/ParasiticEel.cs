using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "an eel corpse" )]
	public class ParasiticEel : BaseCreature
	{

		[Constructable]
		public ParasiticEel() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a parasitic eel";
			Body = 52;
			Hue = Utility.RandomGreenHue();
			BaseSoundID = 0xDB;

			SetStr( 80, 90 );
			SetDex( 150, 160 );
			SetInt( 20, 30 );

			SetHits( 80, 90 );

			SetDamage( 4, 12 );

			SetDamageType( ResistanceType.Physical, 25 );
			SetDamageType( ResistanceType.Cold, 25 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 80, 90 );
			SetResistance( ResistanceType.Poison, 90, 100 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 15.1, 25.0 );
			SetSkill( SkillName.Tactics, 75.1, 90.0 );
			SetSkill( SkillName.Wrestling, 70.1, 90.0 );

			Fame = 300;
			Karma = -300;
			
			CanSwim = true;
			CantWalk = false;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );

		}

		public override int Meat{ get{ return 1; } }


		public ParasiticEel( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}