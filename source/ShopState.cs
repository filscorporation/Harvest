using Steel;

namespace SteelCustom
{
    public struct ShopState
    {
        public const int MAX_PRICE = 8;
        
        public int[] BuyPrice { get; private set; }
        public int[] SellPrice { get; private set; }
        
        public static ShopState GenerateRandom()
        {
            return new ShopState()
            {
                BuyPrice = new []
                {
                     3, // Wood
                    -1, // Tobacco
                     3, // Corn
                    -1, // Cotton
                    -1, // Spices
                },
                SellPrice = new []
                {
                    Random.NextInt(1, 1),         // Wood
                    Random.NextInt(1, MAX_PRICE), // Tobacco
                    Random.NextInt(1, 1),         // Corn
                    Random.NextInt(1, MAX_PRICE), // Cotton
                    Random.NextInt(1, MAX_PRICE), // Spices
                },
            };
        }
    }
}