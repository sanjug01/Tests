using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterview
{

    // The Friendship class
    //
    // Feel free to modify it however you want to implement the 3-4 methods and make them work.
    // 
    public class Friendship
    {
        Dictionary<string, HashSet<string>> _friends;

        // constructor
        public Friendship()
        {
            _friends = new Dictionary<string, HashSet<string>>();
        }

        // This is for you to implement
        //
        // This method takes 2 String parameters and
        // makes them "friend" of each other.  
        //
        // Note: The order of names does not matter
        // Note: Don't forget to write tests to have good test coverage for this method
        public void makeFriend(String name1, String name2)
        {
            if(name1 == name2)
            {
                // should throw, but we just ignore for now
                return;
            }

            // TODO - should consider write locking  _friends[name1] and _friends[name2] for concurrent access
            if(!_friends.ContainsKey(name1))
            {
                _friends[name1] = new HashSet<string>();
            }

            _friends[name1].Add(name2);

            // the reverse is also true;

            if (!_friends.ContainsKey(name2))
            {
                _friends[name2] = new HashSet<string>();
            }

            _friends[name2].Add(name1);

        }

        // This is for you to implement
        //
        // This method takes 2 String parameters and
        // makes them no longer friends of each other.  
        //
        // Note: The order of names does not matter
        // Note: Don't forget to write tests to have good test coverage for this method
        public void unmakeFriend(String name1, String name2)
        {
            if(_friends.ContainsKey(name1))
            {
                _friends[name1].Remove(name2);
            }

            if (_friends.ContainsKey(name2))
            {
                _friends[name2].Remove(name1);
            }
        }

        // helper for verification - direct friends only
        public bool AreFriends(string name1, string name2)
        {
            return _friends.ContainsKey(name1) && _friends[name1].Contains(name2);
        }


        private bool AreIndirectFriends(string name1, string name2)
        {
            // TODO - if needed
            return false;
        }

        // This is for you to implement
        //
        // This method takes a single argument (name) and 
        // returns all the immediate "friends" of that name
        //
        // For example, A & B are friends, B & C are friends and C & D are friends.
        // getDirectFriends(B) would return A and C
        // getDirectFriends(D) would return C
        //
        // Note: It should not return duplicate names
        // Note: Don't forget to write tests to have good test coverage for this method
        public List<String> getDirectFriends(String name)
        {
            if(_friends.ContainsKey(name))
            {
                string[] friendsArray = new string[_friends[name].Count];
                _friends[name].CopyTo(friendsArray);
                return new List<string>(friendsArray);
            }
            else
            {
                return new List<string>();
            }

        }

        // This is for you to implement
        //
        // This method takes a single argument (name) and 
        // returns all the indirect "friends" of that name
        //
        // For example, A & B are friends, B & C are friends and C & D are friends.
        // getInirectFriends(A) would return C and D
        //
        // Note: It should not return duplicate names
        // Note: Don't forget to write tests to have good test coverage for this method
        public List<String> getIndirectFriends(String name)
        {
            if(!_friends.ContainsKey(name))
            {
                return new List<String>();
            }


            HashSet<string> visited = new HashSet<string>();
            Queue<string> futureFriends = new Queue<string>();
            visited.Add(name);

            // add friends to queue
            // initial set of friends
            foreach (var friend in _friends[name])
            {
                futureFriends.Enqueue(friend);
            }

            // take element from queue
            // if added continue
            // else add its friend to quese - and mark visited
            while (futureFriends.Count > 0)
            {
                string crtFriend = futureFriends.Dequeue();
                // if(!visited.Contains(crtFriend))  -- this should never happen since we never add visited
                {
                    visited.Add(crtFriend);
                    foreach (var nextFriend in _friends[crtFriend])
                    {
                        if(!visited.Contains(nextFriend))
                            futureFriends.Enqueue(nextFriend);
                    }
                }
            }


            // visited has all friends
            // direct friend need to be removed from list
            visited.Remove(name);
            foreach (var friend in _friends[name])
            {
                visited.Remove(friend);
            }
            
            // building the list from hashset
            string[] friendsArray = new string[visited.Count];
            visited.CopyTo(friendsArray);
            return new List<string>(friendsArray);
        }
    }
}
