using System;
using System.Collections.Generic;

namespace Hforce
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.initialisation();
            // get the rooms & doors template
            MapTemplateList rooms = MapTemplateLoader.loadFromDirectory("./assets/rooms", "rooms");
            // get the rooms & doors template
            MapTemplateList doors = MapTemplateLoader.loadFromDirectory("./assets/doors", "doors");
            // get the cleaning template rules
            ReplacementRuleList modifications = ReplacementRuleLoader.loadFromDirectory("./assets/modifications", "modifications");

            // create the map
            Map map = new Map(100, 100);
            // create the generator & call it
            MapGenerator1 generator = new MapGenerator1(rooms, doors, modifications);
            generator.GenerateMap(map);



            CharUtils.saveAsImage("./assets/map.png", map.Content);
        }
    }
}


