using System;
using System.Collections.Generic;
using System.Text;

namespace SlidingPuzzle
{
    public enum Direction
    {
        Up = 2,
        Down = 3,
        Left = 0,
        Right = 1
    }
    public class Table
    {
        private byte[,] representation;
        private byte zeroRow, zeroCol;

        public Table(byte[,] representation, Table lastTable, Direction? lastDirection, int pathLength)
        {
            this.LastTable = lastTable;
            this.CameFrom = lastDirection;
            this.PathLength = pathLength;
            this.representation = representation;
            (this.zeroRow, this.zeroCol) = this.FindTargetCoordinatesInMattrix(0, representation);
        }

        public Table Move(Direction direction)
        {
            if (direction == Direction.Up &&
                (!this.CameFrom.HasValue || this.CameFrom.Value != Direction.Down) &&
                this.zeroRow < this.representation.GetLength(0) - 1)
            {
                byte[,] newRepr = (byte[,])this.representation.Clone();
                Swap(ref newRepr[this.zeroRow, this.zeroCol], ref newRepr[this.zeroRow + 1, this.zeroCol]);
                return new Table(newRepr, this, direction, this.PathLength + 1);
            }

            if (direction == Direction.Down &&
                (!this.CameFrom.HasValue || this.CameFrom.Value != Direction.Up) &&
                this.zeroRow > 0)
            {
                byte[,] newRepr = (byte[,])this.representation.Clone();
                Swap(ref newRepr[this.zeroRow, this.zeroCol], ref newRepr[this.zeroRow - 1, this.zeroCol]);
                return new Table(newRepr, this, direction, this.PathLength + 1);
            }

            if (direction == Direction.Left &&
                (!this.CameFrom.HasValue || this.CameFrom.Value != Direction.Right) &&
                this.zeroCol < this.representation.GetLength(1) - 1)
            {
                byte[,] newRepr = (byte[,])this.representation.Clone();
                Swap(ref newRepr[this.zeroRow, this.zeroCol], ref newRepr[this.zeroRow, this.zeroCol + 1]);
                return new Table(newRepr, this, direction, this.PathLength + 1);
            }

            if (direction == Direction.Right &&
                (!this.CameFrom.HasValue || this.CameFrom.Value != Direction.Left) &&
                this.zeroCol > 0)
            {
                byte[,] newRepr = (byte[,])this.representation.Clone();
                Swap(ref newRepr[this.zeroRow, this.zeroCol], ref newRepr[this.zeroRow, this.zeroCol - 1]);
                return new Table(newRepr, this, direction, this.PathLength + 1);
            }
            return null;
        }

        public int PathLength { get; }

        public Table LastTable { get; }

        public Direction? CameFrom { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < representation.GetLength(0); i++)
            {
                for (int j = 0; j < representation.GetLength(1); j++)
                {
                    sb.Append($"{representation[i, j]} ");
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }

        public int CalcManhattanDistance(Table target)
        {
            int total = 0;
            int kx, ky = 0;
            int expX, expY = 0;
            var colsInRepr = this.representation.GetLength(1);
            for (byte k = 1; k < this.representation.Length; k++)
            {
                (expX, expY) = FindTargetCoordinatesInMattrix(k, target.representation);
                (kx, ky) = FindTargetCoordinatesInMattrix(k, this.representation);
                total = total + Math.Abs(kx - expX) + Math.Abs(ky - expY);
            }
            return total;
        }

        private (byte, byte) FindTargetCoordinatesInMattrix(byte target, byte[,] mattrix)
        {
            for (byte i = 0; i < mattrix.GetLength(0); i++)
            {
                for (byte j = 0; j < mattrix.GetLength(1); j++)
                {
                    if (mattrix[i, j] == target)
                    {
                        return (i, j);
                    }
                }
            }
            return (0, 0);
        }

        private static void Swap(ref byte x, ref byte y)
        {
            var temp = x;
            x = y;
            y = temp;
        }
    }
}
