#pragma once

using namespace std;

class StringTests
{
public:
	StringTests(string testString);
	~StringTests();
	string ReplaceAndDelete(char replaceCh, string replaceStr, char delChar);
	static string ConvertBase(const string& testString, int b1, int b2);
	
	string GetString() {
		return m_String;
	};


private:
	string m_String;

};

