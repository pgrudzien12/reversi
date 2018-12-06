using System;

namespace reversi
{
    public struct Vector128 : IEquatable<Vector128>
    {
        ulong half1;
        ulong half2;
        public byte this[int pos]
        {
            get
            {
                if (pos >= 64)
                {
                    pos -= 64;
                    if ((half2 & (1UL << pos)) > 0UL)
                        return 1;
                    else
                        return 0;
                }
                else
                {
                    if ((half1 & (1UL << pos)) > 0UL)
                        return 1;
                    else
                        return 0;
                }
            }
            set
            {
                if (pos >= 64)
                {
                    pos -= 64;
                    if (value == 0)
                        half2 &= ~(1UL << pos);
                    else
                        half2 |= (1UL << pos);
                }
                else
                {
                    if (value == 0)
                        half1 &= ~(1UL << pos);
                    else
                        half1 |= (1UL << pos);
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Vector128 && Equals((Vector128)obj);
        }

        public bool Equals(Vector128 other)
        {
            return half1 == other.half1 &&
                   half2 == other.half2;
        }

        public override int GetHashCode()
        {
            var hashCode = 187066377;
            hashCode = hashCode * -1521134295 + half1.GetHashCode();
            hashCode = hashCode * -1521134295 + half2.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Vector128 vector1, Vector128 vector2)
        {
            return vector1.half1 == vector2.half1 &&
                   vector1.half2 == vector2.half2;
        }

        public static bool operator !=(Vector128 vector1, Vector128 vector2)
        {
            return !(vector1 == vector2);
        }
    }
}