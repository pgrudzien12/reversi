namespace reversi
{
    public struct MoveDescriptor
    {
        public MoveDescriptor(byte pos)
        {
            Position = pos;
        }

        public MoveDescriptor(int col, int row)
        {
            Position = (byte)((row << 4) + col);
        }

        public byte X => (byte)(Position & 7);
        public byte Y => (byte)(Position >> 4 & 7);
        public byte Position { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is MoveDescriptor))
            {
                return false;
            }

            var descriptor = (MoveDescriptor)obj;
            return Position == descriptor.Position;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public static bool operator ==(MoveDescriptor descriptor1, MoveDescriptor descriptor2)
        {
            return descriptor1.Equals(descriptor2);
        }

        public static bool operator !=(MoveDescriptor descriptor1, MoveDescriptor descriptor2)
        {
            return !(descriptor1 == descriptor2);
        }
    }
}