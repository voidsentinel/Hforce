using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Hforce
{
    /// <summary>
    /// List of Template.
    /// You can search a Room in the list by id or by index.
    /// </summary>
    public class ReplacementRuleList : IEnumerable
    {
        public List<ReplacementRule> collection { get; set; }

        public string _name { get; }

        /// create a Template list with the given name
        public ReplacementRuleList(string name)
        {
            collection = new List<ReplacementRule>();
            _name = name;
        }

        public bool Add(ReplacementRule template)
        {
            if (template != null)
            {
                collection.Add(template);
                return true;
            }
            return false;
        }

        public IEnumerator GetEnumerator()
        {
            return collection.GetEnumerator();
        }
    }
}
