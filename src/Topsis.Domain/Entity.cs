using Topsis.Domain.Contracts;

namespace Topsis.Domain
{
    public interface IEntity<TID>
    {
        TID Id { get; }
    }

    public class Entity<TID> : IEntity<TID>
    {
        public TID Id { get; set; }
    }

    public class Entity : Entity<int>
    {
    }

    public class EntityWithTitle<TID> : Entity<TID>, IHaveATitle
    {
        public EntityWithTitle()
        {
        }

        public EntityWithTitle(TID id, string title)
        {
            Id = id;
            Title = title;
        }

        public string Title { get; set; }
    }

    public class EntityWithTitle : EntityWithTitle<int>
    {
        public EntityWithTitle() : base()
        {
        }

        public EntityWithTitle(int id, string title) : base(id, title)
        {
        }
    }
}
