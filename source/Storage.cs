namespace SteelCustom
{
    public class Storage
    {
        public int Level { get; private set; } = 1;
        public int Capacity => GetCapacityForLevel(Level);

        public bool CanUpgrade => Level < 5;

        public int GetUpgradePrice()
        {
            // Wood
            switch (Level)
            {
                case 1:
                    return 2;
                case 2:
                    return 4;
                case 3:
                    return 6;
                case 4:
                    return 8;
            }

            return -1;
        }

        public int GetCapacityForLevel(int level)
        {
            switch (level)
            {
                case 1:
                    return 7;
                case 2:
                    return 9;
                case 3:
                    return 12;
                case 4:
                    return 15;
                case 5:
                    return 20;
            }

            return -1;
        }

        public void Upgrade()
        {
            Level++;
        }
    }
}