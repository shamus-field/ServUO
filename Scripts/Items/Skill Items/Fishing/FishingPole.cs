 /*Created on SharpDevelop.
 * Edited By : Lucas Henrique Pena de Araújo Abreu (TacurumiN)
 * Date: 31/01/2014
 * Hour: 23:47*/

using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Mobiles;
using System.Collections;
using Server.Items;
using Server.Engines.Harvest;
using Server.Network;

namespace Server.Items
{
    public class FishingPole : Item
    {
		#region High Seas Baited
		private int m_Charges;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int Charges
		{
			get{return this.m_Charges;}
			set{this.m_Charges = value;
				this.InvalidateProperties();}
		}
		#endregion
		
        [Constructable]
        public FishingPole()
            : base(0x0DC0)
        {
			this.Baited = false;
			this.BaitedMob = null;
			this.Charges = 0;
            this.Layer = Layer.TwoHanded;
            this.Weight = 8.0;
        }

        public FishingPole(Serial serial)
            : base(serial)
        {
        }
		
        public override void OnDoubleClick(Mobile from)
        {
            Point3D loc = this.GetWorldLocation();
			Map map = this.Map;

            if (!from.InLOS(loc) || !from.InRange(loc, 2))
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1019045); // I can't reach that
            else
			{		
				Fishing.System.BeginHarvesting(from, this);
				
				// Charges Baiting
				if(this.Baited == true)
				{
					Charges--;
					if(Charges <= 0)
					{
						from.SendMessage("Your Baited Fishing Pole effect has passed, and the Fishing Pole has been turned into a normal fishing pole.");
						this.Baited = false;
						this.BaitedMob = null;
						this.Hue = 0;
					}
				}
				else
					return;
			}
		}
		
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            BaseHarvestTool.AddContextMenuEntries(from, this, list, Fishing.System);
        }

        public override bool CheckConflictingLayer(Mobile m, Item item, Layer layer)
        {
            if (base.CheckConflictingLayer(m, item, layer))
                return true;

            if (layer == Layer.OneHanded)
            {
                m.SendLocalizedMessage(500214); // You already have something in both hands.
                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
			writer.Write((Charges));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1 && this.Layer == Layer.OneHanded)
                this.Layer = Layer.TwoHanded;
			
			Charges = reader.ReadInt( );
        }
    }
}