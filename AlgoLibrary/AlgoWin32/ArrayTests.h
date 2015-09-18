#pragma once

using namespace std;
class ArrayTests
{
public:
	ArrayTests();
	~ArrayTests();

	vector<int> SpiralOrder(
		const vector<vector<int>>& matrix
		);


	pair<int, int> RangeOfValue(const vector<int>& a, int value, int from, int to);

};

