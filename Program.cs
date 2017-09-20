using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    class Program
    {
        static BitArray GetDottedMatrix(int width, int height)
        {
            BitArray matrix = new BitArray(height * width, false);

            for (int j = 1; j < height; j+=2)
                for (int i = 1; i < width; i+=2)
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
                         if (maze[i - 1, j] == MazeCell.Wall  && maze[i + 1, j] == MazeCell.Empty &&
                        maze[i, j - 1] != MazeCell.Wall && maze[i, j + 1] != MazeCell.Wall)
                        maze[i, j] = MazeCell.SideLeft;

                    else if (maze[i - 1, j] == MazeCell.Empty && maze[i + 1, j] == MazeCell.Wall &&
                        maze[i, j - 1] != MazeCell.Wall && maze[i, j + 1] != MazeCell.Wall)
                        maze[i, j] = MazeCell.SideRight;

                    else if (maze[i - 1, j] != MazeCell.Wall  && maze[i + 1, j] != MazeCell.Wall &&
                        maze[i, j - 1] == MazeCell.Wall && maze[i, j + 1] == MazeCell.Empty)
                        maze[i, j] = MazeCell.SideTop;

                    else if (maze[i - 1, j] != MazeCell.Wall  && maze[i + 1, j] != MazeCell.Wall &&
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
            /*Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();*/
            Console.SetCursorPosition(0, 0);
            for (int j = 0; j < height; j ++)
            {
                for (int i = 0; i < width; i ++)
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

        static void GenerateMaze(BitArray matrix, int width, int height, Cell start, bool visualize = true, 
            ConsoleColor currentColor = ConsoleColor.Green, ConsoleColor visitedColor = ConsoleColor.DarkGreen, 
            ConsoleColor stepBackColor = ConsoleColor.DarkCyan, ConsoleColor currentStepBackColor = ConsoleColor.Cyan,
            ConsoleColor deadEndColor = ConsoleColor.Magenta, ConsoleColor forkColor = ConsoleColor.DarkBlue)
        {
            BitArray visited = new BitArray(height * width);
            Stack<Cell> stack = new Stack<Cell>();
            Random rnd = new Random();

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
                        Back = true;
                    }
                }
                else
                    break;
            }
            while (true);

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
        }

        static void Main(string[] args)
        {
            int Height = 120, Width = 24;
            int height = Height * 2 + 1, width = Width * 2 + 1;
            
            var matrix = GetDottedMatrix(width, height);

            DrawBitArray(matrix, width, height);

            GenerateMaze(matrix, width, height, new Cell(1, 1), true);

            DrawBitArray(matrix, width, height);

            Task.Delay(250).Wait();

            var maze = ScaleMatrixAndConvertToMaze(matrix, ref width, ref height);

            Task.Delay(250).Wait();

            DrawMaze(maze, width, height);

            Task.Delay(250).Wait();

            MergeEmptyAndWalls(maze, width, height, true);

            Task.Delay(300).Wait();

            AddWalls(maze, width, height);

            DrawMaze(maze, width, height);

            Task.Delay(500).Wait();

            AddCorners(maze, width, height);

            DrawMaze(maze, width, height);

            Task.Delay(500).Wait();

            AddInnerCorners(maze, width, height);

            DrawMaze(maze, width, height);

            while(true)
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
