using System;
using Server;

namespace Server.Items
{
    public class CorgulsEnchantedSash : BodySash
    {
        public override int LabelNumber { get { return 1149781; } } // Corgul's Enchanted Sash

        public override int AosStrReq { get { return 10; } }

        [Constructable]
        public CorgulsEnchantedSash()
        {
            Hue = 988;
			Attributes.DefendChance = 5;
			Attributes.BonusStam = 1;
        }

        public CorgulsEnchantedSash(Serial serial)
            : base(serial)
        {
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