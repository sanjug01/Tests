using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{
    public class Vertex
    {
        public Vertex(int info)
        {
            Label = info;
        }
        public int Label { get; set; }
    }

    public class Edge: IComparable<Edge>
    {
        public Edge(Vertex start, Vertex end, int cost = 0)
        {
            Start = start;
            End = end;
            Cost = cost;
        }

        public Vertex Start { get; set; }
        public Vertex End { get; set; }
        public int Cost { get; set; }

        public int CompareTo(Edge other)
        {
            return this.Cost.CompareTo(other.Cost);
        }
    }


    public class VertexANode
    {
        public VertexANode(Vertex v, List<Vertex> neighbors)
        {
            V = v;
            Neighbors = neighbors;
        }
        public Vertex V { get; private set; }
        public List<Vertex> Neighbors { get; private set; }
    }

    public class VertexINode
    {
        public VertexINode(Vertex v, List<Edge> edges)
        {
            V = v;
            Edges = edges;
            Neighbors = new List<Vertex>();
            foreach(Edge e in edges)
            {
                Neighbors.Add(e.End);
            }
        }
        public Vertex V { get; private set; }

        public List<Edge> Edges{ get; private set; }
        public List<Vertex> Neighbors { get; private set; }
    }

    public class GraphAList
    {
        public List<VertexANode> Vertices { get; private set; }

        public List<Edge> Edges{ get; private set; }

        public GraphAList(int[][] matrix)
        {
            int vCnt = matrix.Length;
            Vertices = new List<VertexANode>();
            Edges = new List<Edge>();

            Vertex[] nodes = new Vertex[vCnt];
            for (int i=0; i< vCnt;i++)
            {
                nodes[i] = new Vertex(i);
            }

            for(int i = 0; i< vCnt; i++)
            {
                List<Vertex> neighbors = new List<Vertex>();
                for (int j=0; j < vCnt; j++)
                {
                    if (matrix[i][j] > 0)
                    {
                        neighbors.Add(nodes[j]);
                        Edges.Add(new Edge(nodes[i], nodes[j], matrix[i][j]));
                    }
                }

                Vertices.Add(new VertexANode(nodes[i], neighbors));
            }

        }
    }


    public class GraphAMatrix
    {
        int[,] _adiacencyMatrix;
        public GraphAMatrix(int[,] matrix)
        {
            _adiacencyMatrix = matrix;
        }
    }

    public class GraphIMatrix
    {

        int[,] _incidenceMatrix;
        public GraphIMatrix(int[,] matrix)
        {
            _incidenceMatrix = matrix;
        }
    }


    public class GraphIList
    {

        public List<VertexINode> Vertices { get; private set; }
        public List<Edge> Edges{ get; private set; }


        public GraphIList(int[][] matrix)
        {
            int vCnt = matrix.Length;
            int eCnt = ( vCnt > 0)? matrix[0].Length : 0;
            
            Vertices = new List<VertexINode>();

            Vertex[] nodes = new Vertex[vCnt];
            Edge[] edges = new Edge[eCnt];
            for (int i = 0; i < vCnt; i++)
            {
                nodes[i] = new Vertex(i);
            }
            for (int j = 0; j< eCnt; j++)
            {
                // will assign start/end when parsing the matrix
                edges[j] = new Edge(null, null, 0);
            }

            for (int i = 0; i < vCnt; i++)
            {
                List<Edge> edgesList = new List<Edge>();
                for (int j = 0; j < eCnt; j++)
                {
                    if (matrix[i][j] > 0)
                    {
                        Edge crtEdge = edges[j];
                        crtEdge.Cost = matrix[i][j]; // usually 1
                        if (null == crtEdge.Start)
                            crtEdge.Start = nodes[i];
                        else if (null == crtEdge.End)
                            crtEdge.End = nodes[i];
                        else
                        {
                            // should not happen, max 2 nodes per edge's column
                        }
                        edgesList.Add(crtEdge);
                    }
                }

                Vertices.Add(new VertexINode(nodes[i], edgesList));
            }

        }
    }



    public class GraphAlgorithms
    {
        public GraphAlgorithms() { }
        public int BaseMethod()
        {
            return 1;
        }


        /*****
          Too messy, should use a single struct to hold all info -  IComparable by cost
        *****/

        public class VertexNode
        {
            VertexNode(int node)
            {
                Node = node;
            }

            public int Node { get; set; } // index of the node
            public int Prev { get; set; } // store previous
            
            public int DistFromStart { get; set; }
            public int DistToTarget { get; set; } 
        }

        public int Dijkstra(int[][] costMatrix, int start, int target)
        {
            int nodeCnt = costMatrix.Length;
            int[] previous = new int[nodeCnt];
            int[] distance = new int[nodeCnt];
            CostObject<int>[] costNodes = new CostObject<int>[nodeCnt];
            SortedSet<CostObject<int>> unvisited = new SortedSet<CostObject<int>>();

            for(int i=0; i<nodeCnt; i++)
            {
                distance[i] = int.MaxValue;
                previous[i] = -1; // none
                costNodes[i] = new CostObject<int>(i, distance[i]);
                unvisited.Add(costNodes[i]);
            }

            distance[start] = 0;
            // List<int> neighbors = new List<int>()

            int crt = start;            

            while (unvisited.Count > 0 && crt != target)
            {
                // select crt as the one having min distance
                // may be optimized with a heap

                crt = unvisited.Min.Instance;

                for (int j = 0; j < costMatrix[crt].Length; j++)
                {
                    if (costMatrix[crt][j] > 0 && unvisited.Contains(costNodes[j]))
                    {
                        if (distance[j] > costMatrix[crt][j] + distance[crt])
                        {
                            distance[j] = costMatrix[crt][j] + distance[crt];
                            // refresh cost and reorder unvisited
                            unvisited.Remove(costNodes[j]);
                            costNodes[j].Cost = distance[j];
                            unvisited.Add(costNodes[j]);

                            previous[j] = crt;
                        }
                    }
                }

                unvisited.Remove(costNodes[crt]);
            }

            // results: distance and previous

            return 1;
        }

        /// <summary>
        ///  AStar algorithm
        /// </summary>
        /// <param name="costMatrix"></param>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <param name="minDist"> heuristic estimation of min distance</param>
        /// <returns></returns>
        public int AStar(int[][] costMatrix, int start, int target, int[][] minDist)
        {
            int nodeCnt = costMatrix.Length;
            int[] previous = new int[nodeCnt];
            int[] distToTarget = new int[nodeCnt];
            int[] distFromStart = new int[nodeCnt];
            CostObject<int>[] costNodes = new CostObject<int>[nodeCnt];
            SortedSet<CostObject<int>> unvisited = new SortedSet<CostObject<int>>();

            HashSet<int> visited = new HashSet<int>();

            for (int i = 0; i < nodeCnt; i++)
            {
                distToTarget[i] = distFromStart[i] = int.MaxValue;
                previous[i] = -1; // none

                costNodes[i] = new CostObject<int>(i, distToTarget[i]);
                unvisited.Add(costNodes[i]);
            }

            distFromStart[start] = 0;
            distToTarget[start] = distFromStart[start] + minDist[start][target];
            unvisited.Add(costNodes[start]);

            int crt = start;
            while (unvisited.Count > 0 && crt != target)
            {
                // select crt as the one having min distance
                // may be optimized with a heap
                crt = unvisited.Min.Instance;

                unvisited.Remove(unvisited.Min);
                visited.Add(crt);

                // unvisited.

                for (int j = 0; j < costMatrix[crt].Length; j++)
                {
                    if (costMatrix[crt][j] > 0 && !visited.Contains(j))
                    {
                        int possibleFromDist = distFromStart[crt] + costMatrix[crt][j];

                        // if score can be improved - TBD
                        if (unvisited.Contains(costNodes[j]) || possibleFromDist < distFromStart[j])
                        {
                            previous[j] = crt;
                            distFromStart[j] = possibleFromDist;
                            distToTarget[j] = distFromStart[j] + minDist[j][target];
                        }

                        // add j to unvisited if needed
                        unvisited.Add(costNodes[j]);
                    }
                }
            }
            return 1;
        }

        public int BellmanFord()
        {
            // return 1 if negative cycles, 0 otherwise
            return 1;
        }

        public int WarshallFloyd()
        {
            return 1;
        }

    }
}
