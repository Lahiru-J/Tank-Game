using UnityEngine;

namespace Assets.Model
{
    public class CoinPack
    {
        public int Lifetime;
        public int Value;
        public int X;
        public int Y;

        public CoinPack(int x, int y, int lifetime, int value)
        {
            X = x;
            Y = y;
            Lifetime = lifetime;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("CoinPack@" + X + "," + Y);
        }

        public bool IsAvailable { get { return Lifetime > 0; } }
        public Vector3 Possition { get {return new Vector3(X,Y,0);} }
    }
}