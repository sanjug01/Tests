#pragma once

using namespace std;
class GraphTests
{
public:
	GraphTests();
	~GraphTests();
};


class Vertex
{
public:
	Vertex(int label)
	{
		m_Info = label;
	};
private:
	int m_Info;
};

class GraphEdge
{
public:
	GraphEdge() { m_Start = NULL; m_End = NULL; m_Cost = 0; }
	GraphEdge(Vertex* s, Vertex* e, int cost = 0)
	{
		m_Start = s;
		m_End = e;
		m_Cost = cost;
	};
private:
	Vertex* m_Start;
	Vertex* m_End;
	int m_Cost;
};

class GraphAMatrix
{
public:
	GraphAMatrix(vector<vector<int>> iMatrix)
	{
		m_Matrix = iMatrix;
	};
	~GraphAMatrix();
	vector<int> const BFS();
	vector<int> const DFS();

private:
	vector<vector<int>> m_Matrix;
};

class GraphAList
{
public:
	GraphAList(const GraphAMatrix& matrix);
	vector<int> const BFS();
	vector<int> const DFS();

private:
};

class GraphIMatrix
{
public:
	GraphIMatrix(vector<vector<int>> iMatrix)
	{
		m_Matrix = iMatrix;
		m_vCnt = m_Matrix.size();
		if (0 < m_vCnt)
			m_eCnt = m_Matrix[0].size();
	};
	vector<int> const BFS();
	vector<int> const DFS();
private:
	vector<vector<int>> m_Matrix;
	int m_vCnt;
	int m_eCnt;
};

class GraphIList
{
public:
	GraphIList(const GraphIMatrix& gMatrix);
	vector<int> const BFS();
	vector<int> const DFS();
private:
	int m_vCnt;
	int m_eCnt;
};