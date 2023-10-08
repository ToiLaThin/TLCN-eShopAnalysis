namespace eShopAnalysis.CartOrderAPI.Domain.SeedWork
{
    public abstract class ValueObject 
    {

        protected abstract IEnumerable<object> GetMemberValues();

        public override bool Equals(object other)
        {
            if (other == null) { return false; }
            if (other.GetType() != this.GetType()) { return false; }

            var otherValueObj = (ValueObject)other; //same type confirmed

            return this.GetMemberValues().SequenceEqual(otherValueObj.GetMemberValues());
        }
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }

        public override int GetHashCode()
        {
            return GetMemberValues().GetHashCode();
        }
    }
}
