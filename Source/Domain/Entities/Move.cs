namespace Tomacco.Source.Entities
{
    public interface IMove
    {
        int Id { get; init; }
        string Name { get; init; }
        IList<IMoveTypeStrategy> Strategies { get; init; }
    }
    public abstract class Move : IMove
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public IList<IMoveTypeStrategy> Strategies { get; init; }
    }

    public enum TargetAreaMoveEnum { Allies, Enemies, All }

    public interface IAreaMove : IMove
    {
        TargetAreaMoveEnum TargetMove { get; init; }
    }
    public class AreaMove : Move, IAreaMove
    {
        public TargetAreaMoveEnum TargetMove { get; init; }

    }

    public interface ISelfMove : IMove
    {
    }

    public class SelfMove : Move, ISelfMove
    {
    }

    public enum TargetSimpleMoveEnum {Allies, Enemies, All}
    public interface ISimpleMove : IMove
    {
        TargetSimpleMoveEnum TargetMove { get; init; }
        HeroStatsEnumeration? StatToHit { get; init; }
        int NumberTargets { get; init; }
        int Range { get; init; }
    }

    public class SimpleMove : Move, ISimpleMove
    {
        public TargetSimpleMoveEnum TargetMove { get; init; }
        public HeroStatsEnumeration? StatToHit { get; init; }
        public int NumberTargets { get; init; }
        public int Range { get; init; }
    }
}
