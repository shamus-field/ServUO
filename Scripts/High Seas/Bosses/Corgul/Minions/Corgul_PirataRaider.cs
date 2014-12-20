 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 23:32*/
 
using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Corgul_PirataRaider : BaseCreature
    {
        [Constructable]
        public Corgul_PirataRaider()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			this.Name = "Soulbound Pirate Raider";
            this.Body = 400;

            this.SetStr(150);
            this.SetDex(140);
            this.SetInt(68);
			
			this.SetHits( 450 );
			this.SetStam( 250 );
			this.SetMana( 200 );

            this.SetDamage(18, 23);

            this.SetSkill(SkillName.MagicResist, 99.6);
			this.SetSkill(SkillName.Swords, 96.7);
			this.SetSkill(SkillName.Anatomy, 81.9);
            this.SetSkill(SkillName.Tactics, 77.1);
            this.SetSkill(SkillName.Wrestling, 45.7);

			SetDamageType( ResistanceType.Physical, 65 );
			SetDamageType( ResistanceType.Fire, 23 );
			SetDamageType( ResistanceType.Cold, 20);
			SetDamageType( ResistanceType.Poison, 20);
			SetDamageType( ResistanceType.Energy, 20);
			

			SetResistance( ResistanceType.Physical, 70 );
			SetResistance( ResistanceType.Fire, 54 );
			SetResistance( ResistanceType.Cold, 45 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 30 );
			
            this.Fame = 2000;
            this.Karma = -1000;
			
			this.AddItem(new TricorneHat());
            this.AddItem(new StuddedChest());
            this.AddItem(new ThighBoots());
			this.AddItem(new BoneHarvester());
			this.AddItem(new Cloak());

        }

        public Corgul_PirataRaider(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}