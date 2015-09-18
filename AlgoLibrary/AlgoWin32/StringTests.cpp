#include "stdafx.h"
#include "StringTests.h"


StringTests::StringTests(string testString)
{
	m_String = testString;
}


StringTests::~StringTests()
{
}


string StringTests::ReplaceAndDelete(char replaceCh, string replaceStr, char delCh)
{
	size_t oldSize = m_String.length();
	int cntDelete = 0;
	int cntReplace = 0;
	size_t replaceSize = replaceStr.size();

	for (size_t i = 0; i < oldSize; i++)
	{
		if (m_String[i] == replaceCh)
			cntReplace++;
		if (m_String[i] == delCh)
			cntDelete++;
	}

	size_t newSize = oldSize - cntDelete + (cntReplace * (replaceStr.length()-1));

	char* newString = new char[newSize+1];
	int idx = 0;

	for (char c: m_String)
	{
		if (c == replaceCh)
		{
			strncpy_s( newString + idx, (newSize - idx+1), replaceStr.c_str(), replaceSize);
			idx += replaceSize;
		}
		else if (c != delCh)
		{
			newString[idx++] = c;
		}
	}

	string result(newString, idx);
	return result;

}

int value(char c)
{
	int result = 0;
	if (c >= '0' && c <= '9')
		return c - '0';
	if (c >= 'a' && c <= 'f')
		return 10 + c - 'a';
	if (c >= 'A' && c <= 'F')
		return 10 + c - 'A';

	return result;
}


string StringTests::ConvertBase(const string& testString, int b1, int b2)
{
	string digits = "0123456789ABCDF";
	register int i;

	long long number = 0;
	for (char ch : testString) {
		number = number * b1 + value(ch);
	}

	vector<char> convert;
	unsigned digit;
	while (number > 0)
	{
		digit = number % b2;
		number = number / b2;
		convert.emplace_back(digits[digit]);
	}

	char* stringResult = new char[convert.size()+1];
	for (i = 0; i < convert.size(); i++)
	{
		stringResult[convert.size() - i - 1] = convert[i];
	}
	stringResult[convert.size()] = 0;

	string result(stringResult);
	return result;
}