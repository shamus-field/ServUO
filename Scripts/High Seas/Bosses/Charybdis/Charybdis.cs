 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 23:32*/
 
using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network; 
using Server.Misc;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a Charybdis corpse" )]
	public class Charybdis : BaseCreature
	{
		private Mobile m_Fisher;

		public Mobile Fisher
		{
			get{ return m_Fisher; }
			set{ m_Fisher = value; }
		}

		[Constructable]
		public Charybdis() : this( null )
		{
		}

		[Constructable]
		public Charybdis( Mobile fisher ) : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			m_Fisher = fisher;

			Name = "Charybdis";

			Body = 1244;
			BaseSoundID = 0x117;

			SetStr( 533, 586 );
			SetDex( 113, 131 );
			SetInt( 110, 155 );

			SetHits( 100000 );
			SetStam( 113, 131);
			SetMana( 110, 155 );

			SetDamage( 24, 33 );

			SetDamageType( ResistanceType.Physical, 50 );
			//SetDamageType( ResistanceType.Fire, 10 );
			//SetDamageType( ResistanceType.Cold, 30 );
			//SetDamageType( ResistanceType.Poison, 40);
			SetDamageType( ResistanceType.Energy, 50);
			

			SetResistance( ResistanceType.Physical, 70, 80 );
			SetResistance( ResistanceType.Fire, 70, 80 );
			SetResistance( ResistanceType.Cold, 45, 55 );
			SetResistance( ResistanceType.Poison, 80, 90 );
			SetResistance( ResistanceType.Energy, 60, 70 );

			SetSkill( SkillName.Magery, 134.6, 140.6 );
			SetSkill( SkillName.EvalInt, 141.8, 143.6 );
			SetSkill( SkillName.Tactics, 120.1, 123.1 );
			SetSkill(SkillName.MagicResist, 165.2, 178.7);
			SetSkill( SkillName.Wrestling, 120.1, 121.2 );

			Fame = 50000;
			Karma = -24000;
			
			CanSwim = true;
			CantWalk = true;

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

		public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
		
		public override bool BardImmune{ get{ return true; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 5 );
			AddLoot( LootPack.AosSuperBoss, 8 );
		}

		public override int TreasureMapLevel{ get{ return 7; } }
		
		
		public override void OnDamagedBySpell( Mobile caster )
		{
			if ( caster == this )
				return;

			TentacleSpawn( caster );
		}
		
		public void TentacleSpawn( Mobile target )
		{
			Map map = target.Map;

			if ( map == null )
				return;

			int tentacles = 0;

			foreach ( Mobile m in this.GetMobilesInRange( 5 ) )
			{
				if ( m is CharybdisTentacles )
					++tentacles;
			}

			if ( tentacles < 5 )
			{
				int chance = Utility.Random(100);
				
				if(chance > 45)
				{
					BaseCreature CharybdisTentacles = new CharybdisTentacles();

					CharybdisTentacles.Team = this.Team;

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

					CharybdisTentacles.MoveToWorld( loc, map );

					CharybdisTentacles.Combatant = target;
				}
				else
					return;
			}
		}
		
		public Charybdis( Serial serial ) : base( serial )
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
			typeof( EnchantedCoralBracelet ),
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
				m.SendMessage( "As a reward for slaying Charybdis, an artifact has been placed in your backpack." );
			else
				m.SendMessage( "As your backpack is full, your reward for destroying the legendary Charybdis has been placed at your feet." );
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