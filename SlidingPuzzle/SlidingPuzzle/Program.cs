using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;
using Priority_Queue;

namespace SlidingPuzzle
{
    public class Program
    {
        public enum Direction
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
        }

        public class Table
        {
            private int? manhattanDistance;
            private byte[,] representation;
            private string cachedStringRepr;
            private Table cameFrom;
            private byte zeroRow, zeroCol;

            public Table(byte[,] representation, Table cameFrom)
            {
                this.cameFrom = cameFrom;
                this.representation = representation;
                (this.zeroRow, this.zeroCol) = this.FindTargetCoordinatesInMattrix(0, representation);
            }

            public Table Move(Direction direction)
            {
                if (direction == Direction.Up && this.zeroRow < this.representation.GetLength(0) - 1)
                {
                    byte[,] newRepr = (byte[,])this.representation.Clone();
                    this.Swap(ref newRepr[this.zeroRow, this.zeroCol], ref newRepr[this.zeroRow + 1, this.zeroCol]);
                    return new Table(newRepr, this);
                }

                if (direction == Direction.Down && this.zeroRow > 0)
                {
                    byte[,] newRepr = (byte[,])this.representation.Clone();
                    this.Swap(ref newRepr[this.zeroRow, this.zeroCol], ref newRepr[this.zeroRow - 1, this.zeroCol]);
                    return new Table(newRepr, this);
                }

                if (direction == Direction.Left && this.zeroCol < this.representation.GetLength(1) - 1)
                {
                    byte[,] newRepr = (byte[,])this.representation.Clone();
                    this.Swap(ref newRepr[this.zeroRow, this.zeroCol], ref newRepr[this.zeroRow, this.zeroCol + 1]);
                    return new Table(newRepr, this);
                }

                if (direction == Direction.Right && this.zeroCol > 0)
                {
                    byte[,] newRepr = (byte[,])this.representation.Clone();
                    this.Swap(ref newRepr[this.zeroRow, this.zeroCol], ref newRepr[this.zeroRow, this.zeroCol - 1]);
                    return new Table(newRepr, this);
                }
                return null;
            }

            public Table LastTable => this.cameFrom;

            //public int ManhattanDistance
            //{
            //    get
            //    {
            //        if (!this.manhattanDistance.HasValue)
            //        {
            //            this.manhattanDistance = this.CalcManhattanDistance();
            //        }
            //        return this.manhattanDistance.Value;
            //    }
            //}

            public override string ToString()
            {
                if (string.IsNullOrEmpty(this.cachedStringRepr))
                {
                    var stringRepr = new StringBuilder();
                    for (int i = 0; i < representation.GetLength(0); i++)
                    {
                        for (int j = 0; j < representation.GetLength(1); j++)
                        {
                            stringRepr.Append($"{representation[i, j]} ");
                        }
                        stringRepr.Append('\n');
                    }
                    this.cachedStringRepr = stringRepr.ToString();
                }

                return this.cachedStringRepr;
            }

            public int CalcManhattanDistance(Table table)
            {
                int total = 0;
                int kx, ky = 0;
                int expX, expY = 0;
                var colsInRepr = this.representation.GetLength(1);
                for (byte k = 1; k < this.representation.Length; k++)
                {

                    (expX, expY) = FindTargetCoordinatesInMattrix(k, table.representation);
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

            private void Swap(ref byte x, ref byte y)
            {
                var temp = x;
                x = y;
                y = temp;
            }
        }

        public class TableEqualityComparer : IEqualityComparer<Table>
        {
            public bool Equals([AllowNull] Table x, [AllowNull] Table y)
            {
                return x.ToString() == y.ToString();
            }

            public int GetHashCode([DisallowNull] Table table)
            {
                return table.ToString().GetHashCode();
            }
        }

        static void Main(string[] args)
        {
            byte[,] testTable =
            {
                {15, 14, 1, 6},
                {9, 11, 4, 12},
                {0, 10, 7, 3},
                {13, 8, 5, 2},
            };

            var targetTable = new Table(new byte[,]{
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 15, 0},
            }, null);



            var startState = new Table(testTable, null);
            Console.WriteLine("initial manhattan distance: " + startState.CalcManhattanDistance(targetTable));
            var itemEqualityComparer = new TableEqualityComparer();

            var priorityQueue = new SimplePriorityQueue<Table, int>(itemEqualityComparer);
            var visitedStates = new List<string>();


            for (int threshold = 100; threshold < 10000000; threshold *= 2)
            {
                Console.WriteLine($"Trying with threshold: {threshold}");
                var pathLength = 0;
                priorityQueue.Enqueue(startState, startState.CalcManhattanDistance(targetTable));
                visitedStates.Add(startState.ToString());
                while (priorityQueue.Count > 0 && pathLength <= threshold)
                {
                    var cur = priorityQueue.Dequeue();
                    //Console.WriteLine(cur);

                    if (cur.ToString() == targetTable.ToString())
                    {
                        Console.WriteLine("WIN!!");

                        Console.WriteLine("========= AND NOW ============");
                        Console.WriteLine("========= THE SOLUTION ============");
                        Console.WriteLine(cur);
                        var last = cur.LastTable;
                        while (last != null)
                        {
                            Console.WriteLine(last);
                            last = last.LastTable;
                        }
                        break;
                    }

                    pathLength++;
                    for (int i = 0; i < 4; i++)
                    {
                        var neighbour = cur.Move((Direction)i);
                        if (neighbour != null && !visitedStates.Contains(neighbour.ToString()))
                        {
                            //Console.WriteLine(
                            //    $"neighbour in direction {(Direction)i} has value {neighbour.CalcManhattanDistance(targetTable)} + {pathLength}");
                            priorityQueue.Enqueue(neighbour, neighbour.CalcManhattanDistance(targetTable) + pathLength);
                            visitedStates.Add(neighbour.ToString());
                        }
                    }
                }
                pathLength = 0;
                priorityQueue.Clear();
                visitedStates.Clear();
            }
            Console.ReadKey();
        }
    }


}
