namespace Assets.Model
{
    public class Cell
    {
        public CellDamage Damage;
        public CellType Type;
        public int X;
        public int Y;

        public Cell(int x, int y, int type, int damage)
        {
            X = x;
            Y = y;
            Type = (CellType) type;
            Damage = (CellDamage) damage;
        }

        public override string ToString()
        {
            return string.Format("Cell@" + X + "," + Y);
        }
    }
}