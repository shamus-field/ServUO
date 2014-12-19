using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Network;

namespace Server.Items
{
    public interface IOracle
    {
        bool Oracle(Mobile from, BaseOracle oracle);
    }

    public abstract class BaseOracle : Item
    {
        private int m_UsesRemaining;
		private string m_Attuned;
		
        public BaseOracle(int itemID): base(itemID)
        {
        }

        public BaseOracle(Serial serial): base(serial)
        {
        }
		
		#region Gets and Sets Uses Remaining, Attuned and Name Clilocs
        public override int LabelNumber
        {
            get
            {
                return 1150184;
            }
        }// Oracle of the Sea
        public virtual bool ForceShowName
        {
            get
            {
                return false;
            }
        }// used to override default name
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get{return this.m_UsesRemaining;}
            set{this.m_UsesRemaining = value;
                this.InvalidateProperties();}
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public string Attuned
        {
            get{return this.m_Attuned;}
            set{this.m_Attuned = value;
                this.InvalidateProperties();}
        }
		#endregion
		
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
			if(this.m_UsesRemaining >= 0)
				list.Add("Charges: {0}/5", m_UsesRemaining);
				
			if (this.m_Attuned != null)
                list.Add(m_Attuned); 
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)this.m_UsesRemaining);	
			writer.Write((string)this.m_Attuned);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			m_UsesRemaining = reader.ReadInt( );
			m_Attuned = reader.ReadString( );
        }
    }
}