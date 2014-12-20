 /*Created on SharpDevelop.
 * Build By : koluch from RunUO community
 * Forum Page : http://www.runuo.com/community/threads/7-0-19-scalis-and-artis-not-100-osi.505081/
 * Edited By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 26/01/2014
 * Hour: 15:38*/
 
using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network; 
using Server.Misc;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a scalis corpse" )]
	public class Scalis : BaseCreature
	{
		private Mobile m_Fisher;

		public Mobile Fisher
		{
			get{ return m_Fisher; }
			set{ m_Fisher = value; }
		}

		[Constructable]
		public Scalis() : this( null )
		{
		}

		[Constructable]
		public Scalis( Mobile fisher ) : base( Utility.RandomBool() ? AIType.ScalisAI : AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			m_Fisher = fisher;

			Name = "Osiredon";
			Title = "the Scalis Enforcer";
			Body = 0x42C;
			BaseSoundID = 278;

			SetStr( 805, 900 );
			SetDex( 121, 165 );
			SetInt( 125, 137 );

			SetHits( 100000 );
			SetStam( 121, 165 );
			SetMana( 4000, 4000 );

			SetDamage( 19, 26 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 30 );
			SetDamageType( ResistanceType.Energy, 30 );

			SetResistance( ResistanceType.Physical, 80 );
			SetResistance( ResistanceType.Fire, 80, 90 );
			SetResistance( ResistanceType.Cold, 85, 95 );
			SetResistance( ResistanceType.Poison, 80, 90 );
			SetResistance( ResistanceType.Energy, 80, 90 );

			SetSkill( SkillName.Anatomy, 20.6, 35.7 );
			SetSkill( SkillName.Magery, 30.6, 40.5 );
			SetSkill( SkillName.EvalInt, 20.1, 30.1 );
			SetSkill( SkillName.Necromancy, 90.1, 100.0 );
			SetSkill( SkillName.SpiritSpeak, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 101.6, 111.5 );
			SetSkill( SkillName.Meditation, 67.6, 72.5 );
			SetSkill( SkillName.Tactics, 117.6, 127.3 );
			SetSkill( SkillName.Wrestling, 119.6, 129.6 );

			Fame = 24000;
			Karma = -24000;

			VirtualArmor = 50;

			CanSwim = true;
			CantWalk = true;

			PackItem( new MessageInABottle() );
			PackItem( new FabledFishingNet() );

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

			EelSpawn( caster );
		}
		
		public void EelSpawn( Mobile target )
		{
			Map map = target.Map;

			if ( map == null )
				return;

			int eels = 0;

			foreach ( Mobile m in this.GetMobilesInRange( 5 ) )
			{
				if ( m is ParasiticEel )
					++eels;
			}

			if ( eels < 5 )
			{
				int chance = Utility.Random(100);
				
				if(chance > 45)
				{
					BaseCreature eel = new SpawnedParasiticEel();

					eel.Team = this.Team;

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

					eel.MoveToWorld( loc, map );

					eel.Combatant = target;
				}
				else
					return;
			}
		}
		
		public Scalis( Serial serial ) : base( serial )
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
				m.SendMessage( "As a reward for slaying the mighty Scalis, an artifact has been placed in your backpack." );
			else
				m.SendMessage( "As your backpack is full, your reward for destroying the legendary Scalis has been placed at your feet." );
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