namespace eShopAnalysis.CouponSaleItemAPI.Utilities.Prototype
{
    public abstract class ClonableObject
    {
        public ClonableObject() { }
        protected ClonableObject(ClonableObject toCloneObj) {
            //implement how to clone in the subclass
        }
        public abstract ClonableObject Clone();
    }
}
