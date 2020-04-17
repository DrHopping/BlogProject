namespace DAL.Entities.Base
{
    public abstract class EntityBase<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}