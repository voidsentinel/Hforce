using System;
using System.Collections.Generic;
using System.Drawing;

public class CharUtils
{
    /// <summary>
    /// Generate a pattern that is the mirror of the given one, along the X axis (ie Y axis is reversed)
    /// </summary>
    /// <param name="pattern">the pattern to mirror</param>
    /// <returns>the new pattern</returns>
    public static char[,] ReverseY(char[,] pattern)
    {
        char[,] patternY = new char[pattern.GetLength(0), pattern.GetLength(1)];
        for (int y = 0; y < pattern.GetLength(0); y++)
        {
            for (int x = 0; x < pattern.GetLength(1); x++)
            {
                patternY[pattern.GetLength(0) - y - 1, x] = pattern[y, x];
            };
        }
        return patternY;
    }

    /// <summary>
    /// Generate a pattern that is the mirror of the given one, along the Y axis (ie X axis is reversed)
    /// </summary>
    /// <param name="pattern">the pattern to mirror</param>
    /// <returns>the new pattern</returns>

    public static char[,] ReverseX(char[,] pattern)
    {
        char[,] patternX = new char[pattern.GetLength(0), pattern.GetLength(1)];
        for (int y = 0; y < pattern.GetLength(0); y++)
        {
            for (int x = 0; x < pattern.GetLength(1); x++)
            {
                patternX[y, pattern.GetLength(1) - x - 1] = pattern[y, x];
            }
        }
        return patternX;
    }

    /// <summary>
    /// Generate a pattern that is a mirror of the given one on both axis (X and Y axis are reversed)
    /// </summary>
    /// <param name="pattern">the pattern to mirror</param>
    /// <returns>the new pattern</returns>
    public static char[,] Reverse(char[,] pattern)
    {
        char[,] patternX = ReverseX(pattern);
        char[,] patternY = ReverseY(patternX);
        return patternY;
    }

    /// <summary>
    /// Generate a pattern that is a rotation  of the given one (X and Y axis are swapped)
    /// </summary>
    /// <param name="pattern">the pattern to mirror</param>
    /// <returns>the new pattern</returns>
    public static char[,] Rotate(char[,] pattern)
    {
        char[,] patternR = new char[pattern.GetLength(1), pattern.GetLength(0)];
        for (int y = 0; y < pattern.GetLength(0); y++)
        {
            for (int x = 0; x < pattern.GetLength(1); x++)
            {
                patternR[x, y] = pattern[y, x];
            }
        }
        return patternR;
    }

    /// <summary>
    /// Generate a set of pattern form the given one. 
    /// </summary>
    /// <param name="pattern">the pattern to mirror</param>
    /// <param name="operations">Operations to perform. each operation will give a separate result in the list 
    /// If X is present in operations, then ReverseX will be performed.
    /// If Y is present in operations, then ReverseY will be performed.
    /// If both X and Y are present, then Reverse will be performed
    /// If R is present, then a Rotation will be performed, and X/Y will also be performed on the rotated pattern if requested
    /// </param>
    /// <returns> a list of patterns</returns>
    public static List<char[,]> CreateMirrors(char[,] pattern, String operations)
    {
        List<char[,]> response = new List<char[,]>();

        response.Add(pattern);
        if (operations.Contains('X'))
            response.Add(ReverseX(pattern));
        if (operations.Contains('Y'))
            response.Add(ReverseY(pattern));
        if (operations.Contains('X') && operations.Contains('Y'))
            response.Add(Reverse(pattern));

        // rotate image if necessary
        if (operations.Contains('R'))
        {
            char[,] pattern2 = Rotate(pattern);
            response.Add(pattern2);
            // generate mirror image of rotated if needed (operation are inverted)
            if (operations.Contains('Y'))
                response.Add(ReverseX(pattern2));
            if (operations.Contains('X'))
                response.Add(ReverseY(pattern2));
            if (operations.Contains('X') && operations.Contains('Y'))
                response.Add(Reverse(pattern2));
        }
        return response;

    }

    public static char[,] Place(char[,] destination, char[,] source, int xpos, int ypos)
    {

        for (int y2 = 0; y2 < source.GetLength(0); y2++)
        {
            if ((ypos + y2) >= 0 && (ypos + y2) < destination.GetLength(0))
            {
                for (int x2 = 0; x2 < source.GetLength(1); x2++)
                {
                    if ((xpos + x2) >= 0 && (xpos + x2) < destination.GetLength(1))
                    {
                        if (source[y2, x2] != '?')
                            destination[ypos + y2, xpos + x2] = source[y2, x2];
                    }
                }
            }
        }
        return destination;
    }



    /// <summary>
    /// check a given template against a given position in a map and return true if the template matche.
    /// </summary>
    /// <param name="map">The map to the template against</param>
    /// <param name="pattern">The template to check</param>
    /// <param name="xpos">X position</param>
    /// <param name="ypos">Y position</param>
    /// <returns>true if the template Matches, false otherwise</returns>
    public static bool FullMatch(char[,] map, char[,] pattern, int xpos, int ypos)
    {
        if ((xpos + pattern.GetLength(1) - 1 >= map.GetLength(1)) || (ypos + pattern.GetLength(0) - 1 >= map.GetLength(0)))
            return false;

        if ((xpos < 0) || (ypos < 0))
            return false;

        bool value = true;
        // check the template
        for (int y2 = 0; y2 < pattern.GetLength(0); y2++)
        {
            for (int x2 = 0; x2 < pattern.GetLength(1); x2++)
            {
                if (!(pattern[y2, x2] == '?' || map[ypos + y2, xpos + x2] == '?')){
                    if (pattern[y2, x2] != map[ypos + y2, xpos + x2]){
                        return false;
                    }
                }
            }
        }
        return value;
    }

    /// <summary>
    /// check a given template against a given position in a map and return true if the template matche.
    /// Joker (?) in the map are considered as any other characters, but are always considered to match 
    /// the underlying map in the pattern. This so that a 
    /// </summary>
    /// <param name="map">The map to the template against</param>
    /// <param name="pattern">The template to check</param>
    /// <param name="xpos">X position</param>
    /// <param name="ypos">Y position</param>
    /// <returns>true if the template Matches, false otherwise</returns>
    public static bool Match(char[,] map, char[,] pattern, int xpos, int ypos)
    {
        if ((xpos + pattern.GetLength(1) - 1 >= map.GetLength(1)) || (ypos + pattern.GetLength(0) - 1 >= map.GetLength(0)))
            return false;

        if ((xpos < 0) || (ypos < 0))
            return false;

        bool value = true;
        // check the template
        for (int y2 = 0; y2 < pattern.GetLength(0); y2++)
        {
            for (int x2 = 0; x2 < pattern.GetLength(1); x2++)
            {
                if (!(pattern[y2, x2] == '?')){
                    if (pattern[y2, x2] != map[ypos + y2, xpos + x2]){
                        return false;
                    }
                }
            }
        }
        return value;
    }

    /// <summary>
    /// check the template against a given map and return the list of position where the template matche
    /// </summary>
    /// <param name="map">The map to chek the template against</param>
    /// <param name="template">The template to chek </param>
    /// <param name="operation">operations to perform on the template (RXY) </param>
    /// <returns>List of position where the template Matches</returns>
    public static List<Position> Matches(char[,] map, char[,] template, String operations)
    {
        List<char[,]> patterns = CharUtils.CreateMirrors(template, operations);

        List<Position> response = new List<Position>();
        bool value = false;

        foreach (char[,] pattern in patterns)
        {
            if (pattern.GetLength(0) <= map.GetLength(0) && pattern.GetLength(1) <= map.GetLength(1))
            {
                // for each postion on the map
                for (int y1 = 0; y1 < map.GetLength(0) - pattern.GetLength(0) + 1; y1++)
                {
                    for (int x1 = 0; x1 < map.GetLength(1) - pattern.GetLength(1) + 1; x1++)
                    {
                        value = Match(map, pattern, x1, y1);
                        if (value)
                        {
                            response.Add(new Position(x1, y1));
                        }
                    }
                }
            }
        }
        return response;
    }

    /// <summary>
    /// replace occurence of the template in the map by another template, based on a roll. 
    /// The operations allows to perform the same action on mirrors / rotation of the template. 
    /// In this case, the same operations will be performed on the result template 
    /// </summary>
    /// <param name="map">the map to check the templace for</param>
    /// <param name="result">the replacing template</param>
    /// <param name="chance">the chance that the template will be replaced by the result, for each match. Must be between 0 and 100</param>
    /// <returns>true if at least one replacement was made </returns>
    public static bool Replace(char[,] map, char[,] toFind, char[,] toReplace, int chance, String operations)
    {
        Random rnd = new Random(111);
        bool value = false;
        bool done = false;

        List<char[,]> patterns = CharUtils.CreateMirrors(toFind, operations);
        List<char[,]> results = CharUtils.CreateMirrors(toReplace, operations);

        for (int i = 0; i < patterns.Count; i++)
        {
            // basic checks; 
            if (patterns[i].GetLength(0) < map.GetLength(0) && patterns[i].GetLength(1) < map.GetLength(1))
            {
                // for each postion on the map
                for (int y1 = 0; y1 < map.GetLength(0) - patterns[i].GetLength(0) + 1; y1++)
                {
                    for (int x1 = 0; x1 < map.GetLength(1) - patterns[i].GetLength(1) + 1; x1++)
                    {
                        // if it matche
                        value = Match(map, patterns[i], x1, y1);
                        if (value)
                        {
                            if (rnd.Next(100) < chance)
                            {
                                Place(map, results[i], x1, y1);
                                done = true;
                            }
                        }
                    }
                }
            }
        }
        return done;
    }

    public static bool ReplaceAll(char[,] map, char source, char dest)
    {   bool modified = false;
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == source)
                    map[y, x] = dest;
                    modified = true;
            }
        }
        return modified;
    }


    public static void saveAsImage(String filename, char[,] pattern)
    {
        int size = 5;
        int XSize = pattern.GetLength(1);
        int YSize = pattern.GetLength(0);
        Bitmap bitmap = new Bitmap(XSize * size, YSize * size);
        Color mapColor1 = Color.Brown;
        Color mapColor2 = Color.Brown;
        // for each element of the map
        for (var x = 0; x < XSize; x++)
            for (var y = 0; y < YSize; y++)
            {
                // diplay a 5*5 square
                switch (pattern[y, x])
                {
                    case ' ':
                        mapColor1 = Color.Black;
                        mapColor2 = Color.Black;
                        break;
                    case '#':
                        mapColor1 = Color.DarkOrange;
                        mapColor2 = Color.DarkOrange;
                        break;
                    case '=':
                        mapColor1 = Color.SaddleBrown;
                        mapColor2 = Color.SaddleBrown;
                        break;
                    case 'X':
                        mapColor1 = Color.SkyBlue;
                        mapColor2 = Color.SkyBlue;
                        break;
                    case 'x':
                        mapColor1 = Color.SlateBlue;
                        mapColor2 = Color.SlateBlue;
                        break;
                    case '-':
                        mapColor1 = Color.Black;
                        mapColor2 = Color.Gray;
                        break;
                    case '~':
                        mapColor1 = Color.DarkOrange;
                        mapColor2 = Color.SaddleBrown;
                        break;
                    default:
                        mapColor1 = Color.Red;
                        mapColor2 = Color.Red;
                        break;
                }
                for (var px = 0; px < size; px++)
                {
                    for (var py = 0; py < size; py++)
                    {
                        // if (px == 0 || py == 0)
                        // {
                        //     bitmap.SetPixel(x * size + px, y * size + py, Color.DimGray);
                        // }
                        // else 
                        if ((px + py) % 2 == 0)
                        {
                            bitmap.SetPixel(x * size + px, y * size + py, mapColor1);
                        }
                        else
                        {
                            bitmap.SetPixel(x * size + px, y * size + py, mapColor2);
                        }
                    }
                }
            }

        bitmap.Save(filename);
    }

}