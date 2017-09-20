using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator
{
    /// <summary>
    /// Граф лабиринта
    /// </summary>
    class Graph
    {
        private List<Node> nodes = new List<Node>();

        public Node AddNode()
        {
            var node = new Node();
            nodes.Add(node);
            return node;
        }

        public Node AddNodePos(Cell cell)
        {
            var node = AddNode();
            node.pos = cell;
            return node;
        }

        public void Associate(Node a, Node b, int distance)
        {
            a.AddNeighbour(b, distance);
            b.AddNeighbour(a, distance);
        }

        public void AddNextNodeAndDistanceToIt(Cell cell, int distance)
        {
            var last = nodes.Last();
            var newnode = AddNodePos(cell);
            Associate(last, newnode, distance);
        }

        private int[,] GetDistanceMatrix()
        {
            var n = nodes.Count;
            int[,] matrix = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    matrix[i, j] = nodes[i].GetDistance(nodes[j]);
                }
            }

            return matrix;
        }

        public Tuple<Cell,Cell> GetFarest()
        {
            var dist = GetDistanceMatrix();
            var n = nodes.Count;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.Clear();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(string.Format(" {0,2}", dist[i, j]));
                }
                Console.WriteLine();
            }
            Console.ReadKey();

            // FIXME
            for (int l = 0; l < n; l++)
            {
                Console.Clear();
                for (int i = 0; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        if (dist[i, j] == -1)
                        {
                            int? min = null;
                            for (int k = 0; k < n; k++)
                            {
                                if (k == i || k == j)
                                    continue;

                                var a1 = Math.Min(i, k);
                                var a2 = Math.Max(i, k);
                                var b1 = Math.Min(j, k);
                                var b2 = Math.Max(j, k);

                                if (dist[a1, a2] != -1 && dist[b1, b2] != -1)
                                {    
                                    //if (dist[a1, a2] + dist[b1, b2] < dist[i, j])
                                    if (!min.HasValue || min.Value > dist[a1, a2] + dist[b1, b2])
                                    min = dist[a1, a2] + dist[b1, b2];
                                    break;
                                }
                            }
                            dist[i, j] = min ?? -1;
                        }
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Console.Write(string.Format(" {0,2}", dist[i,j]));
                    }
                    Console.WriteLine();
                }
                Console.ReadKey();
            }

            int x = 0, y = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (dist[i,j] > dist[x,y])
                    {
                        x = i; y = j;
                    }
                }
            }

            Console.Clear();



            return new Tuple<Cell, Cell>(nodes[x].pos.Value, nodes[y].pos.Value);
        }

        public Node First
        {
            get
            {
                return nodes.First();
            }
        }

        public Node Last
        {
            get
            {
                return nodes.Last();
            }
        }

        public Node PreLast
        {
            get
            {
                return nodes[nodes.Count - 2];
            }
        }

        internal bool IsNode(Cell cell)
        {
            return nodes.Any(n => n.pos.HasValue && n.pos.Value.x == cell.x && n.pos.Value.y == cell.y);
        }

        public Node this[Cell cell]
        {
            get
            {
                if (IsNode(cell))
                    return nodes.First(n => n.pos.HasValue && n.pos.Value.x == cell.x && n.pos.Value.y == cell.y);
                else
                    return null;
            }
        }
    }

    /// <summary>
    /// Вершина графа лабиринта (тупик или развилка)
    /// </summary>
    class Node
    {
        Dictionary<Node, int> Edges = new Dictionary<Node, int>();

        public Cell? pos { get; set; } = null;

        public void AddNeighbour(Node neighbour, int distance)
        {
            if (Edges.ContainsKey(neighbour))
                return;

            //neighbour.AddNeighbour(this, distance, reversed = false);
            Edges.Add(neighbour, distance);
        }

        public int GetDistance(Node node, bool recursive = false)
        {
            if (recursive)
                throw new NotImplementedException();

            if (node == this)
                return 0;

            if (!Edges.ContainsKey(node))
                return -1;
            else
                return Edges[node];
        }

        public override string ToString()
        {
            if (pos.HasValue)
                return pos.ToString();
            return base.ToString();
        }
    }
}
