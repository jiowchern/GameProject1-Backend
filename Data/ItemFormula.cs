using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public class ItemFormula
    {
        public ItemFormulaNeed[] NeedItems;
        public ItemEffect[] Effects;

        public string Name;
        public string Item;

        public int NeedLimit;

        public ItemFormula()
        {
            NeedItems = new ItemFormulaNeed[0];
            Effects = new ItemEffect[0];
        }
    }

    [ProtoBuf.ProtoContract]
    public class ItemFormulaLite
    {
        [ProtoBuf.ProtoMember(1)]
        public string Name;
        [ProtoBuf.ProtoMember(2)]
        public string Item;

        [ProtoBuf.ProtoMember(3)]
        public ItemFormulaNeedLite[] NeedItems;
        [ProtoBuf.ProtoMember(4)]
        public int NeedLimit;

        public ItemFormulaLite()
        {
            NeedItems = new ItemFormulaNeedLite[0];            
        }
    }
}
