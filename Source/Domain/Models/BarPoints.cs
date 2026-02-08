namespace Domain.Models
{
    public class BarPoints
    {
        public int Current { get; set; }
        public int Max { get; set; }

        public static BarPoints CreateFullBarPoints(int max) => new BarPoints { Current = max, Max = max };
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
