using System;
using System.Collections.Generic;

namespace Hforce
{
    class MapGenerator1
    {

        private MapTemplateList _rooms;                // room template
        private MapTemplateList _doors;                // door template

        private ReplacementRuleList _modifications;    // replacement rules

        // rooms grouped by doors type (id)
        private Dictionary<int, MapTemplateList> doorGroups = new Dictionary<int, MapTemplateList>();

        private Random rnd = new Random(666);

        private int operationCount = 0;

        public bool Debug { get; set; }

        /// <summary>
        /// This structure is used to store the position and Id of a pattern (template) on the map 
        /// </summary>
        public class PatternPosition
        {
            public int x = 0;
            public int y = 0;
            public int id = 0;
        }


        public MapGenerator1(MapTemplateList rooms, MapTemplateList doors, ReplacementRuleList modifications)
        {
            Logger.Info($"Creating a map generator ", Logger.LogAction.PUSH);
            Logger.Info($"with {rooms.Count()} rooms");
            Logger.Info($"with {doors.Count()} type of doors");
            Logger.Info($"with {modifications.collection.Count} possible replacement");

            _rooms = rooms;
            _doors = doors;
            _modifications = modifications;
            Debug = false;
            // put rooms into collections grouped by door type 
            InitializeRooms();
            Logger.Pop();
        }

        // /// <summary>
        // /// Place all rooms into buckets depending on the doors they conatins.
        // /// A room can be in several bucket/list, if threre a re differnt type of doors
        // /// each cell of the  rooms table will contains a list of rooms with the same 
        // /// knd of doors, the index of the cell beeing the index of the door in the doors table
        // /// </summary>;
        private void InitializeRooms()
        {
            for (int i = 0; i < _doors.Count(); i++)
            {
                MapTemplate doorPattern = _doors.getTemplate(i);
                doorGroups[doorPattern.Id] = new MapTemplateList($"door{doorPattern.Id}");
                for (int j = 0; j < _rooms.Count(); j++)
                {
                    MapTemplate roomPattern = _rooms.getTemplate(j);
                    if (doorPattern.Matches(roomPattern).Count > 0)
                    {
                        doorGroups[doorPattern.Id].Add(roomPattern);
                    }

                }
                Logger.Info($"Door {doorPattern.Id} contains {doorGroups[doorPattern.Id].Count()} rooms");
            }
        }


        public void GenerateMap(Map map)
        {
            Logger.Info($"Generation for map size {map.XSize}x{map.YSize}");
            List<PatternPosition> availablesExits = new List<PatternPosition>();
            List<PatternPosition> usedRooms = new List<PatternPosition>();

            // get a random entry
            PatternPosition entry = generateEntry(map, _doors);
            GenerateMap(map, entry);
        }

        public void GenerateMap(Map map, PatternPosition entry)
        {
            Logger.Info($"Generation for map size {map.XSize}x{map.YSize}", Logger.LogAction.PUSH);
            List<PatternPosition> availablesExits = new List<PatternPosition>();
            List<PatternPosition> usedRooms = new List<PatternPosition>();
            List<PatternPosition> rejectedExits = new List<PatternPosition>();

            // use given entry as the initial entry/door
            map.Place(_doors.Find(entry.id), entry.x, entry.y);
            availablesExits.Add(entry);
            // also add it to the toremove (it will not be used by another room...)
            // since it's the first, it will be the "Up" (see ManageEntries)
            rejectedExits.Add(entry);

            if (Debug) CharUtils.saveAsImage($"./assets/map{operationCount++}.png", map.Content);
            // While there are not checked exits
            while (availablesExits.Count > 0)
            {
                // Select one door on the map (random)
                //PatternPosition exit = availablesExits[rnd.Next(0, availablesExits.Count)];
                // Select one door on the map (breadth first)
                PatternPosition exit = availablesExits[0];
                // Select one door on the map (deapth first)
                //PatternPosition exit = availablesExits[availablesExits.Count - 1];

                // try to place a room to this exit
                this.PlaceRoomForDoor(map, exit, availablesExits, usedRooms, rejectedExits);

                if (Debug) CharUtils.saveAsImage($"./assets/map{operationCount++}.png", map.Content);
                Logger.Pop();
            }

            // change "unknown" to "wall"
            map.ReplaceAll('?', '#');

            // generate an entry and exit
            ManageEntries(map, rejectedExits);

            // remove unwanted artifacts (not used doors...)        
            Clean(map);

            // place some new element / remove some based on pattern
            Decorate(map);


            Logger.Pop();
        }



        private PatternPosition generateEntry(Map map, MapTemplateList doors)
        {
            MapTemplate door = doors.getTemplate(rnd.Next(doors.Count()));
            PatternPosition entry = new PatternPosition();
            entry.y = rnd.Next(1, map.XSize);
            entry.x = rnd.Next(1, map.YSize);
            entry.id = door.Id;

            return entry;
        }

        /// <summary>
        /// Try to place a room, given an exit position ( ie door ). The room will be taken in the list of room having this kind of exit.
        /// if a room is placed, new exits will be adde to the list, and old one (common to another room) removed.
        /// </summary>
        /// <param name="exit">the exit position & pattern</param>
        /// <param name="availablesExits">the list of available exit</param>
        /// <returns>true if a room was placed</returns>
        private bool PlaceRoomForDoor(Map map, PatternPosition exit, List<PatternPosition> availablesExits, List<PatternPosition> usedRooms, List<PatternPosition> rejectedExits)
        {
            ;
            MapTemplateList possiblesRooms = doorGroups[exit.id];
            possiblesRooms.collection.Shuffle();
            possiblesRooms.collection.Sort(
                    delegate (MapTemplate x,
                              MapTemplate y)
                    {
                        var xx = x.SortValue - x.Usage;
                        var yy = y.SortValue - y.Usage;
                        if (xx == yy) return 0;
                        if (xx < yy) return 1;
                        else return -1;
                    }
            );

            bool placed = false;
            // for all room that contains our exit
            //possiblesRooms.Reset();
            foreach (MapTemplate room in possiblesRooms)
            {
                // get all position in this room of the exi we're working on
                List<Position> possibleExits = _doors.Find(exit.id).Matches(room);
                possibleExits.Shuffle();
                foreach (Position pos in possibleExits)
                {
                    // check if the room can be put, mapping the choosen room exit with the exit we were given
                    if (CheckRoom(map, room, exit.x - pos.X, exit.y - pos.Y, usedRooms))
                    {
                        // place the room
                        PlaceRoom(map, room, exit.x - pos.X, exit.y - pos.Y, availablesExits, usedRooms);
                        // change all usage of room with the same SourceId
                        UpdateUsage(room);
                        // flag the map as modified
                        placed = true;
                        break; // leave the search for a valid exit
                    }
                }
                // if a room has been placed onto the exit, leave the search for a room 
                if (placed)
                {
                    break;
                }
            }

            // if no room found, remove the exit from the list of available (if placed, this has already be done)
            if (!placed)
            {
                availablesExits.RemoveAll(template => (exit.x == template.x && exit.y == template.y && exit.id == template.id));
                rejectedExits.Add(exit);
            }
            return placed;
        }

        private void Clean(Map map)
        {
            Logger.Info($"Cleaning the map from unwanted artifact", Logger.LogAction.PUSH);
            bool modified = true;
            // get all rentries that are always used...
            List<ReplacementRule> always = _modifications.collection.FindAll(template => (template.Chance == 100));
            Logger.Info($"{always.Count} rules found");
            always.Sort(
                    delegate (ReplacementRule x,
                              ReplacementRule y)
                    {
                        if (x.Priority == y.Priority) return 0;
                        if (x.Priority > y.Priority) return 1;
                        else return -1;
                    }
            );
            while (modified)
            {
                modified = false;
                foreach (ReplacementRule rule in always)
                {
                    modified = modified || map.ReplaceAll(rule.InitialContent, rule.ReplacementContent);
                }
                if (Debug) CharUtils.saveAsImage($"./assets/map{operationCount++}.png", map.Content);
            }
            Logger.Pop();
        }

        private void Decorate(Map map)
        {
            Logger.Info($"Modifiying the map ", Logger.LogAction.PUSH);
            // get all rentries that are always used...
            List<ReplacementRule> sometimes = _modifications.collection.FindAll(template => (template.Chance != 100));
            Logger.Info($"{sometimes.Count} rules found");
            sometimes.Sort(
                    delegate (ReplacementRule x,
                              ReplacementRule y)
                    {
                        if (x.Priority == y.Priority) return 0;
                        if (x.Priority > y.Priority) return 1;
                        else return -1;
                    }
            );
            foreach (ReplacementRule rule in sometimes)
            {
                Logger.Info($" replacement of {rule.InitialContent.Id} by {rule.ReplacementContent.Id} with {rule.Chance}% chance ");
                map.ReplaceAll(rule.InitialContent, rule.ReplacementContent, rule.Chance, "");
                if (Debug) CharUtils.saveAsImage($"./assets/map{operationCount++}.png", map.Content);
            }
            Logger.Pop();
        }

        /// <summary>
        /// Check that a room can be placed, comparing the map and the room pattern
        /// </summary>
        /// <param name="map">the map where to add the room to</param>
        /// <param name="room">the room to add</param>
        /// <param name="xpos">xposition of the left side of the room in the map</param>
        /// <param name="ypos">yposition of the top of the room in the map</param>
        /// <param name="usedRooms">List of all already used room, with their position</param>
        /// <returns>true if the room can be added</returns>
        private bool CheckRoom(Map map, MapTemplate room, int xpos, int ypos, List<PatternPosition> usedRooms)
        {
            char[,] roomContent = room.Content;
            if ((xpos + room.XSize >= (map.XSize - 1)) || (ypos + room.YSize >= (map.YSize - 1)))
            {
                return false;
            }
            if ((xpos <= 0) || (ypos <= 0))
            {
                return false;
            }

            // check that we didn't put the new room exactly at the same place as a previous room with the same Id (ie same look)
            int count = usedRooms.FindAll(used => (used.x == xpos && used.y == ypos && used.id == room.SourceId)).Count;
            if (count > 0)
            {
                return false;
            }


            for (int y1 = 0; y1 < room.YSize; y1++)
            {
                for (int x1 = 0; x1 < room.XSize; x1++)
                {
                    if (room.Content[y1, x1] != '?')
                    {
                        // check if map cont either is "not filled" or filled with same data as room
                        if (room.Content[y1, x1] != map.Content[ypos + y1, xpos + x1] &&
                            map.Content[ypos + y1, xpos + x1] != '?')
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Place a room on the map.
        /// Add the exits of the room to the list of available exits for the map generations
        /// </summary>
        /// <param name="room"></param>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        /// <param name="availablesExits"></param>
        /// <param name="used"></param>
        /// <returns>true</returns>
        private bool PlaceRoom(Map map, MapTemplate room, int xpos, int ypos, List<PatternPosition> availablesExits, List<PatternPosition> usedRooms)
        {
            Logger.Info($"placement of {room.Id} at {xpos}x{ypos}", Logger.LogAction.PUSH);
            // put the content of the room on the map.
            for (int y1 = 0; y1 < room.YSize; y1++)
            {
                for (int x1 = 0; x1 < room.XSize; x1++)
                {
                    if (room.Content[y1, x1] != '?')
                    {
                        map.Content[ypos + y1, xpos + x1] = room.Content[y1, x1];
                    }
                }
            }

            // store the room so that we do not try the same room for one of its own exit 
            // (as it may work, as the 2 rooms would be superposed)
            PatternPosition newRoom = new PatternPosition();
            newRoom.x = xpos;
            newRoom.y = ypos;
            newRoom.id = room.SourceId;
            usedRooms.Add(newRoom);

            // for  all doors from this room, either add it to the 'available' list, or remove it if allready present
            // (this is a common exit with a previous room, and is now used)
            foreach (MapTemplate doorPattern in _doors)
            {
                //find all occurnece of this kind of doors in the room
                List<Position> doorsList = doorPattern.Matches(room);
                //doorsList.Shuffle();
                foreach (Position position in doorsList)
                {
                    PatternPosition roomDoor = new PatternPosition();
                    roomDoor.x = position.X + xpos;
                    roomDoor.y = position.Y + ypos;
                    roomDoor.id = doorPattern.Id;
                    // if already present in the availables list : common exit with a previous room (remove it), 
                    if (availablesExits.FindAll(exit => (roomDoor.x == exit.x && roomDoor.y == exit.y && roomDoor.id == exit.id)).Count > 0)
                    {
                        availablesExits.RemoveAll(exit => (roomDoor.x == exit.x && roomDoor.y == exit.y && roomDoor.id == exit.id));
                    }
                    // otherwise add it
                    else
                    {
                        availablesExits.Add(roomDoor);
                    }
                }
            }
            Logger.Pop();
            return true;
        }

        private void UpdateUsage(MapTemplate room)
        {
            List<MapTemplate> samerooms = _rooms.collection.FindAll(template => (template.SourceId == room.SourceId));
            foreach (MapTemplate template in samerooms)
            {
                template.Usage = template.Usage + 1;
            }
        }

        private void ManageEntries(Map map, List<PatternPosition> rejectedExits)
        {
            // choose the exit
            PatternPosition exitPos = rejectedExits[0];
            // get the template
            MapTemplate door = _doors.Find(exitPos.id);
            // duplicate it and change it
            MapTemplate exit = new MapTemplate(door);
            CharUtils.ReplaceAll(exit.Content, '=', 'U');
            // update the map
            map.Place(exit, exitPos.x, exitPos.y);

            exitPos = rejectedExits[rejectedExits.Count - 1];
            // get the template
            door = _doors.Find(exitPos.id);
            // duplicate it and change it
            exit = new MapTemplate(door);
            CharUtils.ReplaceAll(exit.Content, '=', 'D');
            // update the map
            map.Place(exit, exitPos.x, exitPos.y);

        }

    }
}