namespace Tomacco.Source.Models
{
    public interface IBarPoints
    {
        int Current { get; set; }
        int Max { get; set; }

        static abstract IBarPoints CreateFullBarPoints(int max);

        bool Add(int amount);
        bool Remove(int amount);

        bool Refill();
        bool Clear();
    }
    public class BarPoints : IBarPoints
    {
        public int Current { get; set; }
        public int Max { get; set; }

        public static IBarPoints CreateFullBarPoints(int max) => new BarPoints { Current = max, Max = max };
        public bool Add(int amount)
        {
            if (amount < 0) throw new ArgumentException(nameof(amount));
            if (Current == Max) return false;

            if (Current + amount > Max)
            {
                Max = amount;
                return true;
            }
            Current += amount;
            return true;
        }

        public bool Remove(int amount)
        {
            if (amount < 0) throw new ArgumentException(nameof(amount));
            if (Current == 0) return false;

            if (Current - amount < 0)
            {
                Current = 0;
                return true;
            } 
            Current -= amount;
            return true;
        }

        public bool Refill()
        {
            if (Current == Max) return false;
            Current = Max;
            return true;
        }

        public bool Clear()
        {
            if (Current == 0) return false;
            Current = 0;
            return true;
        }
    }
}
