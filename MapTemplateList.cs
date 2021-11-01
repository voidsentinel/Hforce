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
    public class MapTemplateList : IEnumerable
    {
        public List<MapTemplate> collection { get; }

        public string _name { get; }

        /// create a Template list with the given name
        public MapTemplateList(string name)
        {
            collection = new List<MapTemplate>();
            _name = name;
        }

        public bool Add(MapTemplate template)
        {
            if (template != null)
            {
                MapTemplate previous = Find(template.Id);
                if (previous != null)
                {
                    Logger.Push();
                    Logger
                        .Info($"Removing existing previous template {template.Id}from list {_name}");
                    Logger.Pop();
                    Remove(template.Id);
                }
                collection.Add(template);
                return true;
            }
            return false;
        }

        ///
        public bool Remove(int id)
        {
            return (collection.RemoveAll(x => (id == x.Id)) > 0);
        }

        public MapTemplate Find(int id)
        {
            return collection.Find(x => (id == x.Id));
        }

        public int Count()
        {
            return collection.Count;
        }

        public MapTemplate getTemplate(int index)
        {
            return collection[index];
        }


        public IEnumerator GetEnumerator()
        {
            return collection.GetEnumerator();
        }
    }
}
