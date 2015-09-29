// AlgoCTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
using namespace std;

#define CHECK_TEST(TestMethod, TestName) if (TestMethod()) \
cout << endl << "Test " << TestName << " passed!" << endl; \
else \
cout << endl << "Test " << TestName << " failed!" << endl; \


// declare tests here
int TestMethod1();
int TestSpiralOrder();
int TestDeleteReplaceString();
int TestConvertBase();
int TestSTLContainers();
int TestPointers();
int TestRangeOfValue();


int _tmain(int argc, _TCHAR* argv[])
{
	if (argc > 1)
	{
		int testIdx = _wtoi(argv[1]);
		if (0 == testIdx) {
			// run all tests
		}
		else {
			// one test at a time
			switch (testIdx)
			{

			case 1:
				CHECK_TEST(TestRangeOfValue, "TestRangeOfValue");
				break;

			case 5:
				CHECK_TEST(TestPointers, "TestPointers");
				break;

			case 6:
				CHECK_TEST(TestSTLContainers, "TestSTLContainers");
				break;

			case 4:
				CHECK_TEST(TestConvertBase, "TestConvertBase");
				break;

			case 3:
				CHECK_TEST(TestDeleteReplaceString, "TestDeleteReplaceString");
				break;

			case 2:
				CHECK_TEST(TestSpiralOrder, "TestSpiralOrder");
				break;

			case 100:
				CHECK_TEST(TestMethod1, "BaseTest"); 
				break;

			default:
				std::cout << "Test not implemented yet!";
			}
		}
	}
	else
	{
		cout << endl << "Give 0 or test id to run tests";
	}

	cout << endl << "Enter any char" << endl;
	char c;
	cin >> c;

	return 0;
}

int TestMethod1()
{
	BaseAlgoClass testClass;

	if (1 == testClass.TestMethod())
	{
		return 1;
	}
	else
	{
		return 0;
	}
}


int TestRangeOfValue()
{
	int testPass = 1;
	ArrayTests testClass;
	vector<int> a = { 1,2,3,3,3,3,3,5,5,8,9 ,10,11,11 };
	pair<int, int> result = make_pair(-1,-1);

	result = testClass.RangeOfValue(a, 3, 0, a.size());
	if (result.first != 2 || result.second != 6)
		testPass = 0;


	result = testClass.RangeOfValue(a, 1, 0, a.size());
	if (result.first != 0 || result.second != 0)
		testPass = 0;

	result = testClass.RangeOfValue(a, 4, 0, a.size());
	if (result.first != -1 || result.second != -1)
		testPass = 0;

	return testPass;
}

int TestSpiralOrder()
{
	ArrayTests testClass;

	int lines = 5;
	int cols = 4;

	vector<vector<int>> matrix(lines, vector<int>(cols));

	int value = 1;
	for (int i = 0;i < lines; i++)
		for (int j = 0;j < cols; j++)
		{
			matrix[i][j] = value++;
		}

	auto results =
		testClass.SpiralOrder(matrix);

	cout << endl << "Results: " << endl;
	for (vector<int>::iterator iter = results.begin(); iter < results.end(); iter++) 
	{
		cout << *iter << " -> ";
	}
	cout << endl;

	return 1;
}

int TestDeleteReplaceString()
{
	string testString = "bad_dbccdasdb";
	StringTests testClass(testString);

	string result = testClass.ReplaceAndDelete('b', "xy", 'd');

	cout << "old string: " << testString << endl;
	cout << "new string:" << result << endl;

	return 1;
}

int TestConvertBase()
{
	string testString = "F0";
	string result;


	result = StringTests::ConvertBase(testString, 16, 10);
	cout << "Base: " << testString << endl;
	cout << "New base:" << result << endl;

	result = StringTests::ConvertBase(testString, 16, 2);
	cout << "Base: " << testString << endl;
	cout << "New base:" << result << endl;

	result = StringTests::ConvertBase(testString, 16, 8);
	cout << "Base: " << testString << endl;
	cout << "New base:" << result << endl;

	result = StringTests::ConvertBase(testString, 16, 7);
	cout << "Base: " << testString << endl;
	cout << "New base:" << result << endl;

	testString = "110011";
	result = StringTests::ConvertBase(testString, 2, 10);
	cout << "Base: " << testString << endl;
	cout << "New base:" << result << endl;

	result = StringTests::ConvertBase(testString, 2, 16);
	cout << "Base: " << testString << endl;
	cout << "New base:" << result << endl;

	result = StringTests::ConvertBase(testString, 2, 8);
	cout << "Base: " << testString << endl;
	cout << "New base:" << result << endl;


	return 1;
}


int _TestArray()
{
	array<int, 10> myArray = { 1,5,3,4,8,6,7,29,9,10 };

	cout << endl << "MyArray:" << myArray.size() << " last:" << myArray[myArray.size() -1];
	for (int i : myArray) {
		cout << " - " << i;
	}
	cout << endl;

	sort(myArray.begin(), myArray.end());
	cout << endl << "MyArray (sorted):" << myArray.size() << " last:" << myArray[myArray.size() - 1];
	for (int i : myArray) {
		cout << " - " << i;
	}
	cout << endl;

	return 1;
}

int _TestVector()
{
	vector<int> myVector = { 1,2,3,4,5 };
	vector<vector<int>> myMatrix = { { 0,1, 2 },{ 1, 0,4 },{ 3,2,3 } };

	cout << endl << "My vector:" << myVector.size();
	cout << endl << "My matrix:" << myMatrix.size() << " columns:" << myMatrix[0].size();
	cout << endl << "My matrix[2]:" << myMatrix[2].size() << " data" << myMatrix[2].size() << " " << myMatrix[2][2];


	myMatrix[2].assign({ 4,5,6,7 });
	cout << endl << "My matrix:" << myMatrix.size() << " columns:" + myMatrix[0].size();
	cout << endl << "My matrix[2]:" << myMatrix[2].size() << " data" << myMatrix[2].size() << " " << myMatrix[2][2];

	vector<int> subVector;
	subVector.assign(&myVector[2], &myVector[4]);
	cout << endl << "Sub vector:" << subVector.size();

	//vector<int> subVector2;
	//copy(myVector.begin + 2, myVector.begin + 4, subVector.begin);
	//cout << endl << "Sub vector2:" << subVector.size();

	return 1;
}


int _TestPair()
{

	pair<int, string> myPair(100, "bla");

	cout << endl << "MyPair: " << myPair.first << ", " << myPair.second;
	auto newPair = myPair;
	newPair.first++;

	cout << endl << "MyPair: " << myPair.first << ", " << myPair.second;
	cout << endl << "NewPair: " << newPair.first << ", " << newPair.second;

	return 1;
}


int _TestLists()
{
	list<int> myList = { 3,4,5,8,0, 1,2,7 };
	cout << endl << "MyList: " << myList.size() << endl;
	for (int it : myList)
		cout << " - " << it;

	myList.push_back(100);
	myList.push_front(200);
	cout << endl << "MyList(+2): " << myList.size() << endl;
	for (int it : myList)
		cout << " - " << it;

	myList.pop_front();
	cout << endl << "MyList(-1): " << myList.size() << endl;
	for (int it : myList)
		cout << " - " << it;

	forward_list<int> myFList = { 3, 4, 5, 8, 0, 11, 12, 13, 14 };
	cout << endl << "FList: " << endl;
	for (int it : myFList)
		cout << " - " << it;

	myFList.pop_front();
	myFList.push_front(-1);
	myFList.push_front(-2);

	cout << endl << "FList(-1, +2): " << endl;
	for (int it : myFList)
		cout << " - " << it;

	return 1;
}

int _TestStackQueue()
{
	array<int, 10> myArray = { 1,5,3,4,8,6,7,29,9,10 };
	cout << endl << "Initial array:" << myArray.size() << endl;
	for (int val : myArray)
		cout << " - " << val;


	stack<int> myStack;
	for (int val : myArray)
		myStack.push(val);

	cout << endl << "Stack:" << myStack.size() << " top:" << myStack.top() << endl;
	while (!myStack.empty()) {
		cout << " - " << myStack.top();
		myStack.pop();
	}


	queue<int> myQueue;
	for (int val : myArray)
		myQueue.push(val);


	cout << endl << "Queue:" << myQueue.size() << " front:" << myQueue.front()
		<< " back:" << myQueue.back() << endl;
	while (!myQueue.empty()) {
		cout << " - " << myQueue.front() << " (b:" << myQueue.back() << ")";
		myQueue.pop();
	}

	deque<int> myDeque;
	for (int val : myArray)
		myDeque.push_front(val);

	for (int val : myArray)
		myDeque.push_back(val);


	cout << endl << "myDeque:" << myDeque.size() << " front:" << myDeque.front()
		<< " back:" << myDeque.back() << endl;
	while (!myDeque.empty()) {
		cout << " - " << myDeque.front() << " (b:" << myDeque.back() << ")";
		myDeque.pop_front();
	}

	//// not allowed - hits assert
	//cout << endl << " EmptyStack:" << myStack.top();
	//cout << endl << " EmptyQueue:" << myQueue.front() << " (b:" << myQueue.back() << ")";

	priority_queue<int> myHeap;
	for (int val : myArray)
		myHeap.push(val);

	cout << endl << "myHeap:" << myHeap.size() << " top:" << myHeap.top() << endl;
	while (!myHeap.empty()) {
		cout << " - " << myHeap.top() ;
		myHeap.pop();
	}

	// minHeap - not default
	priority_queue<int, vector<int>, greater<int>> myMinHeap;
	for (int val : myArray)
		myMinHeap.push(val);

	cout << endl << "myMinHeap:" << myMinHeap.size() << " top:" << myMinHeap.top() << endl;
	while (!myMinHeap.empty()) {
		cout << " - " << myMinHeap.top();
		myMinHeap.pop();
	}

	return 1;
}


int _TestHashMap()
{

	array<int, 10> iArray = { 1,5,3,4,8,6,7,2,9,10 };
	array<string, 10> sArray = { "one","five","three" ,"four", "eight",
		"six", "seven", "two","nine","ten" };


	map<string, int> myMap;
	unordered_map<string, int> myHMap;

	multimap<string, int> myMultiMap;
	unordered_multimap<string, int> myHMultiMap;


	for (int i = 0;i < 10;i++) {
		myMap[sArray[i]] = iArray[i];
		myHMap[sArray[i]] = iArray[i];

		myMultiMap.insert(pair<string, int>(sArray[i], iArray[i]));
		myHMultiMap.insert(pair<string, int>(sArray[i], iArray[i]));
	}

	// duplicate keys
	myMultiMap.insert(pair<string, int>("one", 101));
	myHMultiMap.insert(pair<string, int>("one", 101));
	myMultiMap.insert(pair<string, int>("five", 501));
	myHMultiMap.insert(pair<string, int>("five", 501));
	myMultiMap.insert(pair<string, int>("one", 1001));
	myHMultiMap.insert(pair<string, int>("one", 1001));


	cout << endl << "MyMap " << endl;
	for each (auto mapIter in myMap)
	{
		cout << "(" << mapIter.first << "," << mapIter.second << ") - ";
	}

	cout << endl << "MyHMap " << endl;
	for each (auto mapIter in myHMap)
	{
		cout << "(" << mapIter.first << "," << mapIter.second << ") - ";
	}

	cout << endl << "myMultiMap " << endl;
	for each (auto mapIter in myMultiMap)
	{
		cout << "(" << mapIter.first << "," << mapIter.second << ") - ";
	}
	cout << endl << "Found: " << myMultiMap.count("one") << " duplicates in myMultiMap" << endl;


	cout << endl << "myHMultiMap " << endl;
	for each (auto mapIter in myHMultiMap)
	{
		cout << "(" << mapIter.first << "," << mapIter.second << ") - ";
	}
	cout << endl << "Found: " << myHMultiMap.count("one") << " duplicates in myHMultiMap"<< endl;
	

	return 1;
}

int _TestBitSet()
{
	bitset<32> myBitset(0xF0F0); 
	bitset<32> secondBitSet;
	bitset<32> result;

	secondBitSet = myBitset << 12;

	cout << endl << "MyBitset size: " << myBitset.size()
		<< " string:" << myBitset.to_string() 
		<< " ulong /ullong" << myBitset.to_ulong() << " - " << myBitset.to_ullong()
		<< endl;

	cout << endl << "Second bitSet:" << secondBitSet.to_string();


	cout << endl << "Result:" << result.to_string();

	result = myBitset & secondBitSet;
	cout << endl << "Result & :" << result.to_string();

	result = myBitset | secondBitSet;
	cout << endl << "Result | :" << result.to_string();

	result = myBitset ^ secondBitSet;
	cout << endl << "Result ^ :" << result.to_string();

	bool orValue = 0;
	bool andValue = 1;
	bool xorValue = 0;
	for (size_t i = 0;i < myBitset.size(); i++)
	{
		orValue = orValue || myBitset[i];
		andValue = andValue && myBitset[i];
		xorValue = xorValue ^ myBitset[i];
	}


	return 1;
}

int TestSTLContainers()
{
	int result = 1;

		
	// bitset
	result &= _TestBitSet();
	
	// stack, queue, priority queue, 
	result &= _TestStackQueue();

	// set/multiset, map/multimap
	// (unordered) hashset, hashmap 
	result &= _TestHashMap();

	// pair, vector, list, slist (single linked),  
	result &= _TestLists();
	result &= _TestArray();
	result &= _TestVector();
	result &= _TestPair();

	return result;
}

int TestPointers()
{
	int* intP = new int[100];

	shared_ptr<int> lpInt = make_shared<int>();
	*lpInt = 5;

	cout << endl << "Ref count = " << lpInt.use_count() << "  get()" << lpInt.get() << " value =" << (*lpInt);

	shared_ptr<int> newPtr = lpInt;
	shared_ptr<int> newPtr2 = lpInt;
	shared_ptr<int> newPtr3 = newPtr2;
	weak_ptr<int> wpPtr = lpInt;

	cout << endl << "Ref count = " << lpInt.use_count() << "  get()" << lpInt.get() << " value =" << (*lpInt);
	cout << endl << "1. value= " << (*newPtr);
	cout << endl << "2. value= " << (*newPtr2);
	cout << endl << "3. value= " << (*newPtr3);

	cout << endl << "Ref count (+3) = " << lpInt.use_count() << "  get()" << lpInt.get() << " value =" << (*lpInt);

	newPtr.reset();
	wpPtr.reset();
	cout << endl << "Ref count (-1) = " << lpInt.use_count() << "  get()" << lpInt.get() << " value =" << (*lpInt);

	unique_ptr<char> chPtr = make_unique<char>();
	*chPtr = 'c';

	unique_ptr<char> chPtr2 = move(chPtr);

	return 1;
}
