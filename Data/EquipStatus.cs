using System;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoBuf.ProtoContract][Serializable]
    public struct EquipStatus
    {
        [ProtoBuf.ProtoMember(1)]
        public string Item { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public EQUIP_PART Part { get; set; }
    }
}