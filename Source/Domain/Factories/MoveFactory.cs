using Domain.Models;

namespace Domain.Factories
{
    public class MoveFactory
    {
        public Move CreateSimpleMove(
            string name, string description, int damage, int manaCost, TargetType target, HeroStatsEnumeration statToHit, int numberTargets = 1, int range = 1)
        {
            return new Move
            {
                Name = name,
                Description = description,
                MoveType = MoveType.Simple,
                Damage = damage,
                ManaCost = manaCost,
                SimpleTargetMove = target,
                StatToHit = statToHit,
                NumberTargets = numberTargets,
                Range = range
            };
        }

        public Move CreateAreaMove(string name, string description, int damage, int manaCost, TargetType target)
        {
            return new Move
            {
                Name = name,
                Description = description,
                MoveType = MoveType.Area,
                Damage = damage,
                ManaCost = manaCost,
                AreaTargetMove = target
            };
        }

        public Move CreateSelfMove(string name, string description, int damage, int manaCost)
        {
            return new Move
            {
                Name = name,
                Description = description,
                MoveType = MoveType.Self,
                Damage = damage,
                ManaCost = manaCost
            };
        }
    }
}
