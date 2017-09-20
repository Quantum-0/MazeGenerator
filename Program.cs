using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * TODO:
 * - comments
 */

namespace MazeGenerator
{
    struct Cell
    {
        public int x;
        public int y;
        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"{{{x}; {y}}}";
        }
    }

    class Program
    {
        static BitArray GetDottedMatrix(int width, int height)
        {
            BitArray matrix = new BitArray(height * width, false);

            for (int j = 1; j < height; j += 2)
                for (int i = 1; i < width; i += 2)
                    matrix[j * width + i] = true;

            return matrix;
        }

        static MazeCell[,] ScaleMatrixAndConvertToMaze(BitArray matrix, ref int width, ref int height)
        {
            MazeCell[,] maze = new MazeCell[width * 2 - 1, height * 2 - 1];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    maze[2 * i, 2 * j] = matrix[j * width + i] ? MazeCell.Empty : MazeCell.Wall;

            height = height * 2 - 1;
            width = width * 2 - 1;
            return maze;
        }

        static void AddWalls(MazeCell[,] maze, int width, int height)
        {
            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    if (maze[i - 1, j] == MazeCell.Wall && maze[i + 1, j] == MazeCell.Empty &&
                   maze[i, j - 1] != MazeCell.Wall && maze[i, j + 1] != MazeCell.Wall)
                        maze[i, j] = MazeCell.SideLeft;

                    else if (maze[i - 1, j] == MazeCell.Empty && maze[i + 1, j] == MazeCell.Wall &&
                        maze[i, j - 1] != MazeCell.Wall && maze[i, j + 1] != MazeCell.Wall)
                        maze[i, j] = MazeCell.SideRight;

                    else if (maze[i - 1, j] != MazeCell.Wall && maze[i + 1, j] != MazeCell.Wall &&
                        maze[i, j - 1] == MazeCell.Wall && maze[i, j + 1] == MazeCell.Empty)
                        maze[i, j] = MazeCell.SideTop;

                    else if (maze[i - 1, j] != MazeCell.Wall && maze[i + 1, j] != MazeCell.Wall &&
                        maze[i, j - 1] == MazeCell.Empty && maze[i, j + 1] == MazeCell.Wall)
                        maze[i, j] = MazeCell.SideBottom;
                }
            }
        }

        static void AddInnerCorners(MazeCell[,] maze, int width, int height)
        {
            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    if (maze[i - 1, j - 1] == MazeCell.Empty && maze[i, j] == MazeCell.Undefined)
                        maze[i, j] = MazeCell.InnerCornerBottomRight;
                    if (maze[i + 1, j - 1] == MazeCell.Empty && maze[i, j] == MazeCell.Undefined)
                        maze[i, j] = MazeCell.InnerCornerBottomLeft;
                    if (maze[i - 1, j + 1] == MazeCell.Empty && maze[i, j] == MazeCell.Undefined)
                        maze[i, j] = MazeCell.InnerCornerTopRight;
                    if (maze[i + 1, j + 1] == MazeCell.Empty && maze[i, j] == MazeCell.Undefined)
                        maze[i, j] = MazeCell.InnerCornerTopLeft;
                }
            }
        }

        static void AddCorners(MazeCell[,] maze, int width, int height)
        {
            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    if (maze[i - 1, j - 1] == MazeCell.Wall &&
                        maze[i, j - 1] == MazeCell.SideLeft && maze[i - 1, j] == MazeCell.SideTop)
                        maze[i, j] = MazeCell.CornerTopLeft;

                    if (maze[i + 1, j - 1] == MazeCell.Wall &&
                        maze[i, j - 1] == MazeCell.SideRight && maze[i + 1, j] == MazeCell.SideTop)
                        maze[i, j] = MazeCell.CornerTopRight;

                    if (maze[i - 1, j + 1] == MazeCell.Wall &&
                        maze[i, j + 1] == MazeCell.SideLeft && maze[i - 1, j] == MazeCell.SideBottom)
                        maze[i, j] = MazeCell.CornerBottomLeft;

                    if (maze[i + 1, j + 1] == MazeCell.Wall &&
                        maze[i, j + 1] == MazeCell.SideRight && maze[i + 1, j] == MazeCell.SideBottom)
                        maze[i, j] = MazeCell.CornerBottomRight;
                }
            }
        }

        static void MergeEmptyAndWalls(MazeCell[,] maze, int width, int height, bool visualize = true)
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width - 2; i++)
                {
                    if (maze[i, j] == MazeCell.Wall)
                        if (maze[i + 2, j] == MazeCell.Wall)
                        {
                            maze[i + 1, j] = MazeCell.Wall;
                            if (visualize)
                            {
                                Console.SetCursorPosition(i + 1, j);
                                Console.Write('▓');
                                Task.Delay(3).Wait();
                            }
                        }

                    if (maze[i, j] == MazeCell.Empty)
                        if (maze[i + 2, j] == MazeCell.Empty)
                        {
                            maze[i + 1, j] = MazeCell.Empty;
                            if (visualize)
                            {
                                Console.SetCursorPosition(i + 1, j);
                                Console.Write(' ');
                                Task.Delay(3).Wait();
                            }
                        }
                }
            }

            for (int j = 0; j < height - 2; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (maze[i, j] == MazeCell.Wall)
                        if (maze[i, j + 2] == MazeCell.Wall)

                        {
                            maze[i, j + 1] = MazeCell.Wall;

                            if (visualize)
                            {
                                Console.SetCursorPosition(i, j + 1);
                                Console.Write('▓');
                                Task.Delay(3).Wait();
                            }
                        }

                    if (maze[i, j] == MazeCell.Empty)
                        if (maze[i, j + 2] == MazeCell.Empty)

                        {

                            maze[i, j + 1] = MazeCell.Empty;

                            if (visualize)
                            {
                                Console.SetCursorPosition(i, j + 1);
                                Console.Write(' ');
                                Task.Delay(3).Wait();
                            }
                        }
                }
            }
        }

        static void DrawMaze(MazeCell[,] maze, int width, int height)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    switch (maze[i, j])
                    {
                        case MazeCell.Undefined:
                            Console.Write('░');
                            break;
                        case MazeCell.Empty:
                            Console.Write(' ');
                            break;
                        case MazeCell.Wall:
                            Console.Write("▓");
                            break;
                        case MazeCell.CornerTopLeft:
                            Console.Write('╝');
                            break;
                        case MazeCell.CornerTopRight:
                            Console.Write('╚');
                            break;
                        case MazeCell.CornerBottomLeft:
                            Console.Write('╗');
                            break;
                        case MazeCell.CornerBottomRight:
                            Console.Write('╔');
                            break;
                        case MazeCell.SideLeft:
                        case MazeCell.SideRight:
                            Console.Write('║');
                            break;
                        case MazeCell.SideTop:
                        case MazeCell.SideBottom:
                            Console.Write('═');
                            break;
                        default:
                            Console.Write('?');
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        static void DrawBitArray(BitArray matrix, int width, int height, ConsoleColor wall = ConsoleColor.DarkRed, ConsoleColor empty = ConsoleColor.Black)
        {
            Console.SetCursorPosition(0, 0);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Console.BackgroundColor = !matrix[j * width + i] ? wall : empty;
                    Console.Write(' ');
                    if (i % 8 == 0)
                        Task.Delay(1).Wait();
                }
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Task.Delay(5).Wait();
            }
        }

        static Tuple<Cell, Cell> GenerateMaze(BitArray matrix, int width, int height, Cell start, bool visualize = true,
            ConsoleColor currentColor = ConsoleColor.Green, ConsoleColor visitedColor = ConsoleColor.DarkGreen,
            ConsoleColor stepBackColor = ConsoleColor.DarkCyan, ConsoleColor currentStepBackColor = ConsoleColor.Cyan,
            ConsoleColor deadEndColor = ConsoleColor.Magenta, ConsoleColor forkColor = ConsoleColor.DarkBlue)
        {
            BitArray visited = new BitArray(height * width);
            Stack<Cell> stack = new Stack<Cell>();
            Random rnd = new Random();
            Graph graph = new Graph();
            int distance = 0;

            bool Back = false;

            Cell current = start;
            visited[current.y * width + current.x] = true;

            if (visualize)
            {
                Console.BackgroundColor = currentColor;
                Console.SetCursorPosition(start.x, start.y);
                Console.Write(' ');
                Task.Delay(60).Wait();
            }

            Node curnode = null;
            do
            {
                if (visualize)
                    Task.Delay(2).Wait();

                var neighbours = GetNeighbours();
                if (neighbours.Count != 0)
                {
                    var neiId = rnd.Next(neighbours.Count);
                    var nei = neighbours[neiId];
                    stack.Push(current);
                    var wall = new Cell((nei.x + current.x) / 2, (nei.y + current.y) / 2);
                    matrix[wall.y * width + wall.x] = true;
                    visited[current.y * width + current.x] = true;

                    if (visualize)
                    {
                        Console.BackgroundColor = currentColor;
                        Console.SetCursorPosition(nei.x, nei.y);
                        Console.Write(' ');
                        Task.Delay(2).Wait();

                        Console.BackgroundColor = visitedColor;
                        Console.SetCursorPosition(wall.x, wall.y);
                        Console.Write(' ');
                        Task.Delay(2).Wait();

                        if (Back) // Развилка
                            Console.BackgroundColor = forkColor;
                        Console.SetCursorPosition(current.x, current.y);
                        Console.Write(' ');


                    }

                    if (Back)
                    {
                        graph.AddNodePos(current);
                        graph.Associate(graph.Last, graph.PreLast, distance);
                        distance = 0;
                        Back = false;
                    }

                    current = nei;
                }
                else if (stack.Count > 0)
                {
                    visited[current.y * width + current.x] = true;
                    if (visualize)
                    {
                        // Тупик / путь назад
                        Console.BackgroundColor = !Back ? deadEndColor : stepBackColor;
                        Console.SetCursorPosition(current.x, current.y);
                        Console.Write(" ");
                        Task.Delay(2).Wait();
                    }

                    if (!Back)
                    {
                        // пришли в тупик
                        graph.AddNodePos(current);
                        distance = 0;

                        Back = true;
                    }
                    else if (graph.IsNode(current))
                    {
                        curnode = graph[current];
                        graph.Associate(graph.Last, curnode, distance);
                        graph.SetLast(curnode);
                        distance = 0;
                    }

                    distance++;

                    var prevcur = current;
                    current = stack.Pop();

                    if (visualize)
                    {
                        var wall = new Cell((prevcur.x + current.x) / 2, (prevcur.y + current.y) / 2);
                        Console.BackgroundColor = stepBackColor;
                        Console.SetCursorPosition(wall.x, wall.y);
                        Console.Write(" ");
                        Task.Delay(1).Wait();
                        Console.BackgroundColor = currentStepBackColor;
                        Console.SetCursorPosition(current.x, current.y);
                        Console.Write(" ");
                        Task.Delay(2).Wait();
                    }
                }
                else
                    break;
            }
            while (true);
            graph.Associate(curnode, graph.AddNodePos(current), distance);

            if (visualize)
            {
                Task.Delay(300).Wait();
                var nds = graph.Nodes;
                for (int i = 0; i < nds.Length; i++)
                {
                    var A = (byte)'A';
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(nds[i].pos.Value.x, nds[i].pos.Value.y);
                    Console.Write((char)(A + i));
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Task.Delay(1000).Wait();
            }

            var farest = graph.GetFarest(); // FIXME

            if (visualize)
            {
                Console.BackgroundColor = deadEndColor;
                Console.SetCursorPosition(current.x, current.y);
                Console.Write(' ');
            }

            List<Cell> GetNeighbours()
            {
                var x = current.x;
                var y = current.y;
                List<Cell> result = new List<Cell>();

                if (x > 2 && !visited[y * width + (x - 2)])
                    result.Add(new Cell(x - 2, y));
                if (y > 2 && !visited[(y - 2) * width + x])
                    result.Add(new Cell(x, y - 2));
                if (x < width - 2 && !visited[y * width + (x + 2)])
                    result.Add(new Cell(x + 2, y));
                if (y < height - 2 && !visited[(y + 2) * width + x])
                    result.Add(new Cell(x, y + 2));

                return result;
            }

            return farest;
        }

        static void SelectSize(out int Width, out int Height)
        {
            int MaxWidth = Math.Min(Console.BufferWidth, Console.WindowWidth) / 4 - 1;
            int MaxHeight = Math.Min(Console.BufferHeight, Console.WindowHeight) / 4 - 1;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Выберите размер лабиринта:");
            Console.WriteLine();

            List<Tuple<int, int>> Sizes = new Tuple<int, int>[] {new Tuple<int, int>(3,3), new Tuple<int, int>(3,5),
                new Tuple<int, int>(4,4), new Tuple<int, int>(5,5), new Tuple<int, int>(8,6), new Tuple<int, int>(7,7),
                new Tuple<int, int>(8,8), new Tuple<int, int>(9,9), new Tuple<int, int>(10, 10), new Tuple<int, int>(12, 12),
                new Tuple<int, int>(15, 15), new Tuple<int, int>(16,10), new Tuple<int, int>(16,12),
                new Tuple<int, int>(16, 16), new Tuple<int, int>(20, 10), new Tuple<int, int>(20, 16), new Tuple<int, int>(24, 12),
                new Tuple<int, int>(24, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(32, 12), new Tuple<int, int>(32, 32),
                new Tuple<int, int>(40, 40), new Tuple<int, int>(MaxWidth, MaxHeight)}.ToList();
            Sizes.RemoveAll(t => t.Item1 > MaxWidth || t.Item2 > MaxHeight);

            for (int i = 0; i < Sizes.Count; i++)
            {
                Console.WriteLine($" {i}) {Sizes[i].Item1}x{Sizes[i].Item2}");
            }

            Console.WriteLine();
            Width = Height = 0;
            do
            {
                Console.Write(">");
                var str = Console.ReadLine();
                if (!int.TryParse(str, out int selected))
                {
                    Console.WriteLine("Введите число");
                    continue;
                }

                if (selected < 0 || selected >= Sizes.Count)
                {
                    Console.WriteLine($"Введите число от 0 до {Sizes.Count - 1}");
                    continue;
                }

                Width = Sizes[selected].Item1;
                Height = Sizes[selected].Item2;
            }
            while (Width == 0);
        }

        private static void SelectShowGeneration(out bool show)
        {
            Console.WriteLine("\nОтображать генерацию лабиринта? (Y/N)");
            bool? result = null;
            do
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Y)
                    result = true;
                else if (key == ConsoleKey.N)
                    result = false;
            }
            while (!result.HasValue);
            Console.WriteLine(result.Value ? " YES ": " NO ");
            show = result.Value;
        }

        private static void DrawStartEnd(Tuple<Cell, Cell> StartEnd)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(StartEnd.Item1.x * 2, StartEnd.Item1.y * 2);
            Console.Write(' ');
            Console.BackgroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(StartEnd.Item2.x * 2, StartEnd.Item2.y * 2);
            Console.Write(' ');
        }

        static void Main(string[] args)
        {
            SelectSize(out int Width, out int Height);
            SelectShowGeneration(out bool Show);

            Console.Clear();
            Console.CursorVisible = false;

            int height = Height * 2 + 1, width = Width * 2 + 1;

            BitArray Matrix;
            Tuple<Cell, Cell> StartEnd;
            MazeCell[,] Maze;
            if (Show)
            {
                Matrix = GetDottedMatrix(width, height);
                DrawBitArray(Matrix, width, height);
                StartEnd = GenerateMaze(Matrix, width, height, new Cell(1, 1), visualize: true);
                DrawBitArray(Matrix, width, height);
                Task.Delay(250).Wait();
                Maze = ScaleMatrixAndConvertToMaze(Matrix, ref width, ref height);
                DrawMaze(Maze, width, height);
                Task.Delay(250).Wait();
                MergeEmptyAndWalls(Maze, width, height, visualize: true);
                DrawMaze(Maze, width, height);
                Task.Delay(300).Wait();
                AddWalls(Maze, width, height);
                DrawMaze(Maze, width, height);
                Task.Delay(500).Wait();
                AddCorners(Maze, width, height);
                DrawMaze(Maze, width, height);
                Task.Delay(500).Wait();
                AddInnerCorners(Maze, width, height);
                DrawMaze(Maze, width, height);
                Task.Delay(100).Wait();
                DrawStartEnd(StartEnd);
            }
            else
            {
                Matrix = GetDottedMatrix(width, height);
                StartEnd = GenerateMaze(Matrix, width, height, new Cell(1, 1), visualize: false);
                Maze = ScaleMatrixAndConvertToMaze(Matrix, ref width, ref height);
                MergeEmptyAndWalls(Maze, width, height, visualize: false);
                AddWalls(Maze, width, height);
                AddCorners(Maze, width, height);
                AddInnerCorners(Maze, width, height);
                DrawMaze(Maze, width, height);
                DrawStartEnd(StartEnd);
            }
            
            while (true)
                Console.ReadKey(true);
        }
        
        enum MazeCell
        {
            Undefined = 0,
            Empty = 1,
            Wall = 2,
            CornerTopLeft = 3,
            CornerTopRight = 4,
            CornerBottomLeft = 5,
            CornerBottomRight = 6,
            SideLeft = 7,
            SideRight = 8,
            SideTop = 9,
            SideBottom = 10,
            InnerCornerTopLeft = 6,
            InnerCornerTopRight = 5,
            InnerCornerBottomLeft = 4,
            InnerCornerBottomRight = 3,
        }
    }
}
