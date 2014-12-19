 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 23:32*/
 
using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Corgul_CapitaoPirata : BaseCreature
    {
        [Constructable]
        public Corgul_CapitaoPirata()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			this.Name = "Soulbound Pirate Captain";
            this.Body = 400;

            this.SetStr(100);
            this.SetDex(125);
            this.SetInt(68);
			
			this.SetHits( 360 );
			this.SetStam( 250 );
			this.SetMana( 200 );

            this.SetDamage(15, 20);

            this.SetSkill(SkillName.MagicResist, 83.6);
			this.SetSkill(SkillName.Fencing, 91.7);
			this.SetSkill(SkillName.EvalInt, 81.9);
            this.SetSkill(SkillName.Tactics, 77.1);
            this.SetSkill(SkillName.Wrestling, 45.7);

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Cold, 20);
			SetDamageType( ResistanceType.Poison, 20);
			SetDamageType( ResistanceType.Energy, 20);
			

			SetResistance( ResistanceType.Physical, 55 );
			SetResistance( ResistanceType.Fire, 54 );
			SetResistance( ResistanceType.Cold, 51 );
			SetResistance( ResistanceType.Poison, 50 );
			SetResistance( ResistanceType.Energy, 52 );
			
            this.Fame = 1025;
            this.Karma = -1000;
			
			this.AddItem(new TricorneHat());
            this.AddItem(new StuddedChest());
            this.AddItem(new ThighBoots());
			this.AddItem(new Pitchfork());

        }

        public Corgul_CapitaoPirata(Serial serial)
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