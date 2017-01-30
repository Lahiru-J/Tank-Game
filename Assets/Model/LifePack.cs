namespace Assets.Model
{
    public class LifePack
    {
        public int Lifetime;
        public int X;
        public int Y;

        public LifePack(int x, int y, int lifetime)
        {
            X = x;
            Y = y;
            Lifetime = lifetime;
        }

        public override string ToString()
        {
            return string.Format("LifePack@" + X + "," + Y);
        }
    }
}