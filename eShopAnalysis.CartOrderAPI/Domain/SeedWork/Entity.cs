namespace eShopAnalysis.CartOrderAPI.Domain.SeedWork
{
    public abstract class Entity //if the primary key is for one single id, composite key cannot use it
    {
        public Guid Id { get; init; } //once in the constructor
        protected Entity(Guid id) { Id = id; }

        public override bool Equals(object? other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;
            if (other is not Entity entity) return false;

            return entity.Id == Id ? true : false;
        }

        public bool Equals(Entity otherEntity)
        {
            if (otherEntity == null) return false;
            if (ReferenceEquals(this, otherEntity)) return true;

            return otherEntity.Id == Id ? true : false;
        }

        public static bool operator == (Entity? first, Entity? second)
        {
            return first is not null && second is not null && first.Equals(second);
        }

        public static bool operator !=(Entity? first, Entity? second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ 31; //fingerprint base on the id
        }
    }
}
