#include "stdafx.h"
#include "ArrayTests.h"

typedef enum _SpiralDirection
{
	Left,
	Down,
	Right,
	Up
} SpiralDirection;

ArrayTests::ArrayTests()
{
}


ArrayTests::~ArrayTests()
{
}

// already sorted vector
pair<int, int> ArrayTests::RangeOfValue(const vector<int>& a, int value, int from, int to)
{
	pair<int, int> result = make_pair(-1, -1);

	if (from > to)
	{
		// it may happen if mid-1 or mid+1 outside of range
		return result;
	}

	if (from == to)
	{
		if (value == a[from]) {
			result.first = result.second = from;
		}

		return result;
	}

	int mid = (from + to) / 2;
	if(a[mid] < value)
	{
		result = RangeOfValue(a, value, mid+1, to);
	}
	else if (a[mid] > value)
	{
		result = RangeOfValue(a, value, from, mid-1);
	}
	else
	{
		// one value found
		result.first = mid;
		result.second = mid;
		pair<int, int> leftResult = RangeOfValue(a, value, from, mid - 1);
		pair<int, int> rightResult = RangeOfValue(a, value, mid + 1, to);
		
		if (leftResult.first > 0) result.first = leftResult.first;
		if (rightResult.second > 0) result.second = rightResult.second;
	}

	return result;
}

vector<int> ArrayTests::SpiralOrder(
	const vector<vector<int>>& matrix
	)
{
	size_t size = 0;
	if(matrix.size() > 0)
		size = matrix.size() * matrix[0].size();

	vector<int> line(size, 0);

	int minLin = 0, minCol = 0;
	int maxLin = matrix.size() -1, maxCol = matrix[0].size() - 1;

	SpiralDirection dir = SpiralDirection::Right;
	int lin = 0, col = 0;
	size_t idx = 0;

	while (idx < size)
	{
		switch (dir)
		{
		case SpiralDirection::Right:
			// copy line A[lin,minCol:maxCol]
			for (int i = minCol; i <= maxCol; i++)
				line[idx++] = matrix[lin][i];

			lin ++;
			col = maxCol;
			minLin++;
			dir = SpiralDirection::Down;

		case SpiralDirection::Down:
			// copy col A[minLin:maxLin,col]
			for (int i = minLin; i <= maxLin; i++)
				line[idx++] = matrix[i][col];

			maxCol--;
			col--;
			lin = maxLin;
			dir = SpiralDirection::Left;

		case SpiralDirection::Left:
			// copy line A[lin,maxCol:minCol]
			for (int i = maxCol; i >= minCol; i--)
				line[idx++] = matrix[lin][i];

			maxLin--;
			lin--;
			col = minCol;
			dir = SpiralDirection::Up;

		case SpiralDirection::Up:
			// copy col A[maxLin:minLin,col]
			for (int i = maxLin; i >= minLin; i--)
				line[idx++] = matrix[i][col];

			minCol++;
			col++;
			lin = minLin;
			dir = SpiralDirection::Right;
			break;
		}

	}

	return line;
}