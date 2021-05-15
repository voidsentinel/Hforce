using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Map definition
/// </summary>
namespace Hforce
{
    public class Map
    {
        /// <value>X size of the map
        public int XSize { get; }

        /// <value>Y size of the map
        public int YSize { get; }

        /// content of the map
        public char[,] Content { get; set; }

        public Map(int xsize, int ysize)
        {
            Logger.Info($"Empty map created with size {xsize}x{ysize}");
            XSize = xsize;
            YSize = ysize;
            Content = new char[ysize, xsize];

            // set the map to jocker content
            InitializeMap('?');
        }

        /// <summary>
        /// Initialize the content of the map with the given character

        /// </summary>
        public void InitializeMap(char value)
        {
            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    Content[y, x] = value;
                }
            }
        }

        public bool isInside(int x, int y)
        {
            return (x >= 0 && x < XSize && y >= 0 && y < YSize);
        }

        /// <summary>
        /// place a given template on the map at a given position.
        /// Only filled part of the template characters (!= '?') are used
        /// </summary>
        /// <param name="Template">the replacing template</param>
        /// <param name="xpos">X position</param>
        /// <param name="ypos">Y position</param>
        public void Place(MapTemplate template, int xpos, int ypos)
        {
            CharUtils.Place(Content, template.Content, xpos, ypos);
        }

        /// <summary>
        /// check the template against a given position on the map and return true if the template matche
        /// </summary>
        /// <param name="template">The template to check</param>
        /// <param name="xpos">X position</param>
        /// <param name="ypos">Y position</param>
        /// <returns>true if the template Matches, false otherwise</returns>
        public bool Matches(MapTemplate template, int xpos, int ypos)
        {
            return CharUtils.Matches(Content, template.Content, xpos, ypos);
        }

        /// <summary>
        /// check the given template against the content of the object return the list of position where the template matche
        /// </summary>
        /// <param name="template">The template to chek </param>
        /// <param name="operation">operations to perform on the template (RXY) </param>
        /// <returns>List of position where the template Matches</returns>
        public List<Position> Matches(MapTemplate template, String operations)
        {
            return CharUtils.Matches(Content, template.Content, operations);
        }


        /// <summary>
        /// check the template against a given positoin on the object content and rpelace it if it matche
        /// </summary>
        /// <param name="toFind">The template to chek </param>
        /// <param name="toReplace">The template to replace it</param>
        /// <param name="xpos">X position</param>
        /// <param name="ypos">Y position</param>
        /// <returns>List of position where the template Matches</returns>
        public bool Replace(MapTemplate toFind, MapTemplate toReplace, int xpos, int ypos)
        {
            if (Matches(toFind, xpos, ypos))
            {
                Place(toReplace, xpos, ypos);
            }
            return true;
        }

        /// <summary>
        /// Search a template in the content of this objetc, and replace it with another one, 
        /// </summary>
        /// <param name="toFind">the template to find</param>
        /// <param name="toReplace">the template to replace with</param>
        /// <returns>true if at least one modification occured</returns>
        public bool Replace(MapTemplate toFind, MapTemplate toReplace)
        {
            return CharUtils
                .Replace(Content, toFind.Content, toReplace.Content, 100, "");
        }

        /// <summary>
        /// Search a template in the content of this objetc, and replace it with another one, 
        /// taking operations into account (ie the mirrored template would be replaced by the mirrored remplacament template)
        /// </summary>
        /// <param name="toFind">the template to find</param>
        /// <param name="toReplace">the template to replace with</param>
        /// <param name="operations">if the templates should be modiief taking into account some operations (Rotation, Mittor...)</param>
        /// <returns>true if at least one modification occured</returns>
        public bool
        Replace(MapTemplate toFind, MapTemplate toReplace, String operations)
        {
            return CharUtils
                .Replace(Content,
                toFind.Content,
                toReplace.Content,
                100,
                operations);
        }

        /// <summary>
        /// Search a template in the content of this objetc, and replace it with another one, 
        /// with a % of chance of the rpelacement occurring and taking operations into account
        /// </summary>
        /// <param name="toFind">the template to find</param>
        /// <param name="toReplace">the template to replace with</param>
        /// <param name="chance">0-100, %chance for the replacement to occurs</param>
        /// <param name="operations">if the templates should be modiief taking into account some operations (Rotation, Mittor...)</param>
        /// <returns>true if at least one modification occured</returns>
        public bool
        Replace(
            MapTemplate toFind,
            MapTemplate toReplace,
            int chance,
            String operations
        )
        {
            return CharUtils
                .Replace(Content,
                toFind.Content,
                toReplace.Content,
                chance,
                operations);
        }

        /// <summary>
        /// Replace all occurence of a character on the map by another one
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public void ReplaceAll(char source, char dest)
        {
            CharUtils.ReplaceAll(Content, source, dest);
        }
    }
}
