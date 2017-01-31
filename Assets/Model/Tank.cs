using UnityEngine;

namespace Assets.Model
{
    public class Tank
    {
        public int Coins;
        public Direction Direction;
        public int Health;
        public int Id;
        public bool IsPlayer;
        public bool IsShot;
        public int Points;
        public int X;
        public int Y;

        public Vector3 Possition { get { return new Vector3(X,Y,0);} }

        public Tank(int id, bool isPlayer, int x, int y, int direction, bool isShot = false, int health = 0, int coins = 0,
            int points = 0)
        {
            Id = id;
            IsPlayer = isPlayer;
            X = x;
            Y = y;
            Direction = (Direction) direction;
            IsShot = isShot;
            Health = health;
            Coins = coins;
            Points = points;
        }

        public override string ToString()
        {
            return string.Format("Tank#" + Id);
        }
    }
}