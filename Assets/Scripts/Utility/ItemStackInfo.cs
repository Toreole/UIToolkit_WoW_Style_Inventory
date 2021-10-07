namespace WoW_Inventory
{
    public struct ItemStackInfo
    {
        public Item item;
        public int amount;
        
        public static readonly ItemStackInfo Empty = new ItemStackInfo(){item = null, amount = -1};

        public static bool operator ==(ItemStackInfo a, ItemStackInfo b) => a.item == b.item && a.amount == b.amount;
        public static bool operator !=(ItemStackInfo a, ItemStackInfo b) => a.item != b.item || a.amount != b.amount;

        //dont care
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        //What the hell is a HashCode???
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}