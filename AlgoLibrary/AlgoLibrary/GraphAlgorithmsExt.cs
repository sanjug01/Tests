using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{
    public class SimpleEdge: IComparable<Edge>
    {
        public SimpleEdge(int start, int end, int cost = 0)
        {
            Start = start;
            End = end;
            Weight = cost;
        }

        public int Start { get; set; }
        public int End { get; set; }
        public int Weight { get; set; }

        public int CompareTo(Edge other)
        {
            return this.Weight.CompareTo(other.Cost);
        }
    }

    public class EdgeGraph
    {
        public EdgeGraph(int vertexCnt)
        {
            VCount = vertexCnt;
            Edges = new List<SimpleEdge>();
        }

        public EdgeGraph(int[][] matrix)
        {
            Edges = new List<SimpleEdge>();
            LoadMatrix(matrix);
        }

        public int VCount { get; private set; }
        public int ECount { get { return Edges.Count; } }

        public List<SimpleEdge> Edges { get; private set; }

        /// <summary>
        /// load a cost matrix - 0 or INFINITE cost means no edge
        /// </summary>
        /// <param name="matrix"></param>
        private void LoadMatrix(int[][] matrix)
        {
            VCount = matrix.Length;
            for(int i=0;i<VCount;i++)
            {
                for(int j=0;j<matrix[i].Length;j++) // partial matrix possible
                {
                    if(0 != matrix[i][j] && int.MaxValue != matrix[i][j])
                    {
                        Edges.Add(new SimpleEdge(i, j, matrix[i][j]));
                    }
                }
            }
        }
    }



    public class GraphAlgorithmsExt
    {
        public GraphAlgorithmsExt() { }

        public bool BellmanFord(EdgeGraph graph, int src, out int[] dist)
        {

            if (null == graph)
            {
                dist = null;
                return true;
            }

            // distance vector
            dist = new int[graph.VCount];

            bool hasNegCycles = false;

            for (int i= 0; i< graph.VCount; i++ )
            {
                dist[i] = int.MaxValue;
            }
            dist[src] = 0;

            // max V-1 relaxation
            for(int i = 0; i< graph.VCount - 1; i++)
            {
                foreach(var edge in graph.Edges)
                {
                    int u = edge.Start;
                    int v = edge.End;
                    int w = edge.Weight;

                    if(dist[u] != int.MaxValue && (dist[u] + w) < dist[v])
                    {
                        dist[v] = dist[u] + w;
                        // could save predecesor here: prev[u] = v;
                    }
                }
            }

            // check cycles
            foreach (var edge in graph.Edges)
            {
                int u = edge.Start;
                int v = edge.End;
                int w = edge.Weight;

                if (dist[u] != int.MaxValue && (dist[u] + w) < dist[v])
                {
                    hasNegCycles = true;
                }
            }


            // return 1 if negative cycles, 0 otherwise
            return hasNegCycles;
        }

        /// <summary>
        /// Finds all distances
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public int WarshallFloyd(EdgeGraph graph, out int[,] distance)
        {
            int V = graph.VCount;
            int[,] dist = new int[graph.VCount, graph.VCount];

            // init dist matrix
            for (int i = 0; i< V; i++)
                for(int j = 0;j <V; j++)
                {
                    if (i == j) dist[i, j] = 0;
                    else dist[i, j] = int.MaxValue;
                }

            foreach(var edge in graph.Edges)
            {
                dist[edge.Start, edge.End] = edge.Weight;
            }

            //
            for(int k =0; k< V; k++)
            {
                // all edges start at i
                for(int i=0; i<V; i++)
                {
                    // all edges finishing in j
                    for (int j = 0; j< V; j++)
                    {
                        if(int.MaxValue != dist[i,k] &&
                            int.MaxValue != dist[k,j] &&
                            (dist[i,k] + dist[k,j]) < dist[i,j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            // can do next[i,j] = next[i,k] - to find next in any minimal path
                        }
                    }
                }
            }

            distance = dist;
            return 1;
        }

    }
}
