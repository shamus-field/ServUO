 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 20:21*/

using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network; 
using Server.Misc;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a Corgul corpse" )]
	public class Corgul : BaseCreature
	{
		private Mobile m_Fisher;

		public Mobile Fisher
		{
			get{ return m_Fisher; }
			set{ m_Fisher = value; }
		}

		[Constructable]
		public Corgul() : this( null )
		{
		}

		[Constructable]
		public Corgul( Mobile fisher ) : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			m_Fisher = fisher;

			Name = "Corgul";
			Title = "the Soulbinder";
			Body = 0x4C;
			Hue = 2076;
			BaseSoundID = 609;

			SetStr( 882, 882 );
			SetDex( 126, 126 );
			SetInt( 323, 323 );

			SetHits( 75000 );
			SetStam( 126, 126);
			SetMana( 4000, 4000 );

			SetDamage( 19, 24 );

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Fire, 10 );
			SetDamageType( ResistanceType.Cold, 30 );
			SetDamageType( ResistanceType.Poison, 40);
			SetDamageType( ResistanceType.Energy, 10);
			

			SetResistance( ResistanceType.Physical, 85 );
			SetResistance( ResistanceType.Fire, 88 );
			SetResistance( ResistanceType.Cold, 91 );
			SetResistance( ResistanceType.Poison, 80 );
			SetResistance( ResistanceType.Energy, 81 );

			SetSkill( SkillName.Magery, 110.5 );
			SetSkill( SkillName.EvalInt, 111.9 );
			SetSkill( SkillName.Meditation, 36.8 );
			SetSkill( SkillName.Tactics, 112.3 );
			SetSkill( SkillName.Wrestling, 115.5 );

			Fame = 24000;
			Karma = -24000;

			VirtualArmor = 50;

			Rope rope = new Rope();
			rope.ItemID = 0x14F8;
			PackItem( rope );

			rope = new Rope();
			rope.ItemID = 0x14FA;
			PackItem( rope );
			
			if ( Paragon.ChestChance > Utility.RandomDouble() )
				PackItem( new ParagonChest( Name, TreasureMapLevel ) );
		}


		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 5 );
			AddLoot( LootPack.AosSuperBoss, 8 );
		}

		public override int TreasureMapLevel{ get{ return 5; } }
		
		
		public override void OnDamagedBySpell( Mobile caster )
		{
			if ( caster == this )
				return;

			PirateSpawn( caster );
		}
		
		public void PirateSpawn( Mobile target )
		{
			Map map = target.Map;

			if ( map == null )
				return;

			int brigands = 0;

			foreach ( Mobile m in this.GetMobilesInRange( 5 ) )
			{
				if ( m is Brigand )
					++brigands;
			}
			
			if ( brigands < 5 )
			{
				int chance = Utility.Random(100);
				
				if(chance > 36)
				{
					BaseCreature Brigand = new SpawnedBrigand();

					Brigand.Team = this.Team;

					Point3D loc = target.Location;
					bool validLocation = false;

					for ( int j = 0; !validLocation && j < 5; ++j )
					{
						int x = target.X + Utility.Random( 3 ) - 1;
						int y = target.Y + Utility.Random( 3 ) - 1;
						int z = map.GetAverageZ( x, y );

						if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
							loc = new Point3D( x, y, Z );
						else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
							loc = new Point3D( x, y, z );
					}

					Brigand.MoveToWorld( loc, map );

					Brigand.Combatant = target;
				}
				else
					return;
			}
		}
		
		public Corgul( Serial serial ) : base( serial )
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

		public static Type[] Artifacts { get { return m_Artifacts; } }

		private static Type[] m_Artifacts = new Type[]
		{
			// Decorations
			typeof( CorgulsHandbookOnTheUndead ),
			typeof( CorgulsHandbookOnMysticism ),
			typeof( EnchantedCoralBracelet ),
			typeof( RingOfTheSoulbinder ),
			typeof( CorgulsEnchantedSash ),
			typeof( HelmOfVengeance ),
			typeof( LeviathanHideBracers ),

			// Equipment
			typeof( SmilingMoonBlade ),
			typeof( IllustriousWandOfThunderingGlory ),
			typeof( RuneEngravedPegLeg ),
			typeof( FieldOfBlades ),
			typeof( CullingBlade )
		};

		public static void GiveArtifactTo( Mobile m )
		{
			Item item = Loot.Construct( m_Artifacts );

			if ( item == null )
				return;

			// TODO: Confirm messages
			if ( m.AddToBackpack( item ) )
				m.SendMessage( "As a reward for slaying Corgul the Soulblinder, an artifact has been placed in your backpack." );
			else
				m.SendMessage( "As your backpack is full, your reward for destroying the legendary Corgul the Soulblinder has been placed at your feet." );
		}

		public override void OnKilledBy( Mobile mob )
		{
			base.OnKilledBy( mob );

			if ( Paragon.CheckArtifactChance( mob, this ) )
			{
				GiveArtifactTo( mob );

				if ( mob == m_Fisher )
					m_Fisher = null;
			}
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			if ( m_Fisher != null )//&& 25 > Utility.Random( 100 ) )
				GiveArtifactTo( m_Fisher );

			m_Fisher = null;
		}
	}
}