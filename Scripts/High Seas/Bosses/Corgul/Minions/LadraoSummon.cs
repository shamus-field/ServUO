 /*Created on SharpDevelop.
 * Build By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 27/01/2014
 * Hour: 23:32*/
 
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an Brigand corpse" )]
	public class SpawnedBrigand : Brigand
	{
		[Constructable]
		public SpawnedBrigand()
		{
			Container pack = this.Backpack;

			if ( pack != null )
				pack.Delete();

			NoKillAwards = true;
		}

		public SpawnedBrigand( Serial serial ) : base( serial )
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
			NoKillAwards = true;
		}
	}
}