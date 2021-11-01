using System;
using System.Collections;
using System.Collections.Generic;
namespace Hforce
{

    public class CharGroups
    {

        static private Dictionary<Char, int> groupsChar = new Dictionary<Char, int>();
        static private Dictionary<int, String> groupNames = new Dictionary<int, String>();

        static private int currentIndex = 1;

        /// <summary>
        /// Create a group with a given name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static public int addGroup(String name)
        {
            int currentValue = currentIndex;
            groupNames.Add(currentIndex, name);
            currentIndex = currentIndex << 1;
            return currentValue;
        }

        static public String getGroupName(int group)
        {
            String name;
            // if it's a single group
            if (groupNames.TryGetValue(group, out name))
            {
                return name;
            }
            // otherwise, check each group in the name list for those who are in the given group
            Dictionary<int, String>.KeyCollection keys = groupNames.Keys;
            foreach (int key in keys)
            {
                if (CharGroups.areSameGroup(key, group))
                {
                    name = name + " " + groupNames[key];
                }
            }
            return name;
        }

        /// <summary>
        /// add a group to a character's group list
        /// </summary>
        /// <param name="character">The character</param>
        /// <param name="group">the group at which it belongs</param>
        static public void addCharacterGroup(Char character, int groupIndex)
        {
            int oldIndex = 0;
            if (!groupsChar.TryGetValue(character, out oldIndex))
            {
                groupsChar.Add(character, oldIndex);
            }
            groupsChar[character] = oldIndex | groupIndex;
        }


        /// <summary>
        /// remove a group from a character's  list. This is done only if the group is in the character's list
        /// </summary>
        /// <param name="character">The character</param>
        /// <param name="group">the group to be removed</param>
        static public void removeCharGroup(int groupIndex, Char character)
        {
            int currentIndex = 0;
            if (!groupsChar.TryGetValue(character, out currentIndex))
            {
                groupsChar.Add(character, currentIndex);
            }
            // if the old
            if ((currentIndex & groupIndex) == groupIndex)
            {
                groupsChar[character] = currentIndex & (~groupIndex);
            }
        }

        /// <summary>
        ///    return the set of groups that the characters belongs to.This is done as a single group whose index is the sum of all the index of belonging group
        /// </summary>
        /// <param name="character">the character</param>
        /// <returns>a group index that represent all the group the character belongs to</returns>
        static public int getCharacterGroups(Char character)
        {
            return groupsChar[character];
        }

        /// <summary>
        /// indicate if a characters belongs to a group
        /// </summary>
        /// <param name="character">the character to check</param>
        /// <param name="group"></param>
        /// <returns></returns>
        static public Boolean areSameGroup(Char character, int group)
        {
            int g1 = CharGroups.getCharacterGroups(character);
            return ((g1 & group) > 0);
        }

        /// <summary>
        /// indicate if a group belongs to another group. This is used for composite group (sum of index of other group)
        /// </summary>
        /// <param name="character">the character to check</param>
        /// <param name="group"></param>
        /// <returns></returns>
        static public Boolean areSameGroup(int group1, int group2)
        {
            return ((group1 & group2) > 0);
        }


        /// <summary>
        /// Check to see if 2 characters have a common group 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>true if a groups belongs to both characters</returns>
        static public bool areSameGroup(Char c1, Char c2)
        {
            int g1 = CharGroups.getCharacterGroups(c1);
            int g2 = CharGroups.getCharacterGroups(c1);
            return ((g1 & g2) > 0);
        }

    }
}
