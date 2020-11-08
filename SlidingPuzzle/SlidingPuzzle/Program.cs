using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;
using Priority_Queue;
using System.Diagnostics;

namespace SlidingPuzzle
{
    public class Program
    {
        static void Main(string[] args)
        {
            var (startArr, targetArr) = ParseInitialAndTargetTables();
            var startState = new Table(startArr, null, null, 0);
            var targetTable = new Table(targetArr, null, null, 0);
            var stack = new Stack<Table>();
            Console.WriteLine("Begin processing...");
            var sw = new Stopwatch();
            sw.Start();
            var curBound = startState.CalcManhattanDistance(targetTable);
            while (!IdsSearch(startState, targetTable, curBound, out var nextBound, stack))
            {
                curBound = nextBound;
            }
            sw.Stop();
            Console.WriteLine($"Finished for {sw.ElapsedMilliseconds}ms");
        }

        public static bool IdsSearch(Table initialState, Table targetState, int bound, out int nextBound, Stack<Table> stack)
        {
            //Console.WriteLine($"Trying with bound: {bound}");
            nextBound = int.MaxValue;
            stack.Push(initialState);

            while (stack.Count > 0)
            {
                var cur = stack.Pop();
                var curManhattan = cur.CalcManhattanDistance(targetState);
                if (curManhattan == 0)
                {
                    PrintSolution(cur);
                    return true;
                }

                var fn = curManhattan + cur.PathLength;
                if (bound < fn)
                {
                    if (nextBound > fn)
                    {
                        nextBound = fn;
                    }
                    continue;
                }

                for (int i = 0; i < 4; i++)
                {
                    var successor = cur.Move((Direction)i);
                    if (successor != null)
                    {
                        stack.Push(successor);
                    }
                }
            }
            return false;
        }

        private static (byte[,], byte[,]) ParseInitialAndTargetTables()
        {
            var tableLength = int.Parse(Console.ReadLine());
            var tableDim = (int)Math.Sqrt(tableLength + 1);
            byte[,] startArr = new byte[tableDim, tableDim];
            byte[,] targetArr = new byte[tableDim, tableDim];
            var pos = int.Parse(Console.ReadLine());

            for (int i = 0; i < tableDim; i++)
            {
                var curLine = Console.ReadLine();
                byte[] integers = curLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => byte.Parse(s)).ToArray();
                for (int j = 0; j < tableDim; j++)
                {
                    startArr[i, j] = integers[j];
                }
            }

            var posZero = (tableLength + 1 + pos) % (tableLength + 1);
            byte counter = 1;
            for (int i = 0; i < tableDim; i++)
            {
                for (int j = 0; j < tableDim; j++)
                {
                    if (i == posZero / tableDim && j == posZero % tableDim)
                    {
                        continue;
                    }
                    targetArr[i, j] = counter;
                    counter++;
                }
            }

            return (startArr, targetArr);
        }

        private static void PrintSolution(Table current)
        {
            var results = new List<string>();
            results.Add(current.CameFrom.ToString());
            var last = current.LastTable;
            while (last != null && last.CameFrom != null)
            {
                results.Add(last.CameFrom.ToString());
                last = last.LastTable;
            }
            results.Reverse();
            Console.WriteLine(results.Count);
            Console.WriteLine(string.Join('\n', results));
        }
    }
}
