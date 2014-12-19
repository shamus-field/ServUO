 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 26/01/2014
 * Hour: 21:32*/

using System;

namespace Server.Items
{
    public class CorgulMap : MapItem
    {
        [Constructable]
        public CorgulMap()
        {
            this.SetDisplay(6272, 1088, 6591, 1407, 400, 400);
			this.Name = "SoulBlinder Island Map";
			this.Hue = 2075;
        }

        public CorgulMap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}