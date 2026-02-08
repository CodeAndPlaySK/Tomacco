namespace Domain.Models
{
    public interface IEventStrategy
    {
        string Name { get; set; }
    }

    public class EventStrategy : IEventStrategy
    {
        public string Name { get; set; }
    }

    public interface IFamilyResourceManagingEventStrategy : IEventStrategy
    {
        FamilyResourceEnum Resource { get; set; }
        Func<int> ResourceAmount { get; set; }
    }

    public class FamilyResourceManagingEventStrategy : EventStrategy, IFamilyResourceManagingEventStrategy
    {
        public FamilyResourceEnum Resource { get; set; }
        public Func<int> ResourceAmount { get; set; }
    }
}
