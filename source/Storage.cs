namespace SteelCustom
{
    public class Storage
    {
        public int Level { get; private set; } = 1;
        public int Capacity => GetCapacityForLevel(Level);

        public bool CanUpgrade => Level < 8;

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
                    return 8;
                case 4:
                    return 12;
                case 5:
                    return 16;
                case 6:
                    return 20;
                case 7:
                    return 30;
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
                    return 10;
                case 3:
                    return 15;
                case 4:
                    return 20;
                case 5:
                    return 30;
                case 6:
                    return 40;
                case 7:
                    return 60;
                case 8:
                    return 100;
            }

            return -1;
        }

        public void Upgrade()
        {
            Level++;
        }
    }
}