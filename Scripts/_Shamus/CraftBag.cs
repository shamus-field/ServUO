using System;
using Server;

namespace Server.Items
{
	public class CraftBag : StrongBackpack
	{
		public override int DefaultMaxWeight{ get{ return 60000; } }

		[Constructable]
		public CraftBag() : base()
		{
			Weight = 0.0;
			Hue = 91;
			Name = "Craft Bag";
		}

		public CraftBag(Serial serial) : base(serial)
        {
        }
        
		public override void Serialize( GenericWriter writer ) 
        {
            base.Serialize( writer ); writer.Write( (int) 0 );
        }
        
		public override void Deserialize( GenericReader reader ) 
        { 
            base.Deserialize( reader ); int version = reader.ReadInt();
        }
	}
}