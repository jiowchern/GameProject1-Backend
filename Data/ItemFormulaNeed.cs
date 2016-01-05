namespace Regulus.Project.ItIsNotAGame1.Data
{
    public struct ItemFormulaNeed
    {
        public string Item;

        public int Min;

        public int Max;
    }

    [ProtoBuf.ProtoContract]
    public struct ItemFormulaNeedLite
    {
        [ProtoBuf.ProtoMember(1)]
        public string Item;
        [ProtoBuf.ProtoMember(2)]
        public int Min;
    }
}