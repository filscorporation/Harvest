using Steel;

namespace SteelCustom
{
    public class Dice
    {
        public int Value1 { get; private set; }
        public int Value2 { get; private set; }
        
        public int Value => Value1 + Value2;

        public void Roll()
        {
            Value1 = Random.NextInt(1, 6);
            Value2 = Random.NextInt(1, 6);
        }
    }
}