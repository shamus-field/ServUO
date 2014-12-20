using System;
using Server;

namespace Server.Items
{
	public class FieldOfBlades : Item 
	{
		public override int LabelNumber { get { return 1034240; } } // field of blades

		[Constructable]
		public FieldOfBlades( )
			: base( 0x37A0 )
		{
			LootType = LootType.Blessed;
		}

		public FieldOfBlades( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}