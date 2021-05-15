using System.Collections.Generic;

/// <summary>
/// Room Definition.
/// <para>A room is composed of a n*m array of char, where the chars define what is present at this position in the room

/// </summary>
namespace Hforce
{
    public class MapTemplate
    {
        protected static int created = 0;

        /// <value>get the id of the room. The Id is auto-defined at creation.
        public int Id { get; }

        /// <value>get the id of the room. The Id is auto-defined at creation.
        public int SourceId { get; set; }

        /// <value>get the X SortValue of the room
        public int XSize { get; }

        /// <value>get the Y SortValue of the room
        public int YSize { get; }

        public int SortValue { get; }

        public int Usage { get; set; }

        // the content of the room
        public char[,] Content { get; set; }

        /// <summary>
        /// Constructor for a basic room

        /// </summary>
        /// <remarks>
        /// You need to fille the room later
        /// </remarks>

        public MapTemplate(int X, int Y)
        {
            Id = created;
            SourceId = created;
            XSize = X;
            YSize = Y;
            SortValue = (XSize + YSize) / 10;
            Usage = 0;
            Content = new char[YSize, XSize];

            created = created + 1;
            Logger
                .Info($"Template: {Id}/{SourceId} created with SortValue {XSize}x{YSize}  (Coeff {SortValue})");
        }

        public MapTemplate(char[,] content)
        {
            Id = created;
            SourceId = created;
            XSize = content.GetLength(1);
            YSize = content.GetLength(0);
            SortValue = (XSize + YSize) / 10;
            Usage = 0;
            Content = content;

            created = created + 1;
            Logger
                .Info($"Template: {Id}/{SourceId} created with SortValue {XSize}x{YSize}  (Coeff {SortValue})");
        }

        /// <summary>
        /// Create a filled template
        /// </summary>
        /// <param name="XSize"></param>
        /// <param name="YSize"></param>
        /// <param name="source">The content of the template, as a String. should contains XSize*YSize char</param>
        public MapTemplate(int X, int Y, string source)
        {
            Id = created;
            SourceId = created;
            XSize = X;
            YSize = Y;
            SortValue = (XSize + YSize) / 10;
            Usage = 0;
            Content = new char[YSize, XSize];
            created = created + 1;
            Logger.Info($"Template: {Id}:{SourceId} created with SortValue {XSize}x{YSize}  (Coeff {SortValue})",
            Logger.LogAction.PUSH);
            if (XSize * YSize != source.Length)
            {
                Logger
                    .Warning($"Invalid source data for Template {Id} : is {source.Length} chars instead of { XSize * YSize}",
                        Logger.LogAction.POP);
                return;
            }

            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    Content[y, x] = source[x + (y * XSize)];
                }
            }
            Logger.Pop();
        }

        /// <summary>
        /// Copy-Constructor for a room
        /// </summary>
        public MapTemplate(MapTemplate room)
        {
            Id = created;
            SourceId = room.SourceId;
            created = created + 1;
            XSize = room.XSize;
            YSize = room.YSize;
            SortValue = (XSize + YSize) / 10;
            Usage = 0;
            Content = new char[YSize, XSize];
            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    this.setMapElement(x, y, room.getMapElement(x, y));
                }
            }
            Logger
                .Info($"Template: {Id}/{SourceId} created from {room.Id} with SortValue {XSize}x{YSize}, coeff {SortValue}");
        }

        /// <summary>
        /// Initialize the content of the template with the given character 
        /// </summary>
        /// <param name="value"> the value to fill the template</param>
        public void InitializeContent(char value)
        {
            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    Content[y, x] = value;
                }
            }
        }


        /// <summary>
        ///Check that a position is inside the template 
        /// </summary>
        /// <param name="x">the x position</param>
        /// <param name="y">the y position</param>
        /// <returns>true if the position is inside the template</returns>
        public bool isInside(int x, int y)
        {
            return (x >= 0 && x < XSize && y >= 0 && y < YSize);
        }

        /// <summary>
        /// Allows to set the element at the given position in the room
        /// </summary>
        /// <remarks>
        /// positions starts at 1
        /// </remarks>

        public void setMapElement(int xpos, int ypos, char value)
        {
            Content[ypos, xpos] = value;
        }

        /// <summary>
        /// Allows to get the element at the given position in the room
        /// </summary>
        /// <remarks>
        /// positions starts at 1
        /// </remarks>
        /// <returns>
        /// Tthe element type at the given position
        /// </returns>
        public char getMapElement(int xpos, int ypos)
        {
            return Content[ypos, xpos];
        }

        /// <summary>
        /// check the template against a given one and return the list of position where both template matche
        /// </summary>
        /// <param name="destination">The template to chekc the template against. It should be bigger than the template</param>
        /// <returns>List of position where the template Matches</returns>
        public List<Position> Matches(MapTemplate content)
        {
            return CharUtils
                .Matches(map: content.Content, template: Content, "");
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
            return CharUtils.Matches(pattern: Content, map: template.Content, xpos: xpos, ypos: ypos);
        }

        /// <summary>
        /// check the given template against the content of the object return the list of position where the template matche
        /// </summary>
        /// <param name="template">The template to chek </param>
        /// <param name="operation">operations to perform on the template (RXY) </param>
        /// <returns>List of position where the template Matches</returns>
        public List<Position> Matches(MapTemplate template, string operations)
        {
            return CharUtils.Matches(Content, template.Content, operations);
        }

        /// <summary>
        /// place a given template on the content at a given position.
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
        /// take the content of this template when it matche a second one, and replacce it by a third one 
        /// </summary>
        /// <param name="toFind">The template to find </param>
        /// <param name="toReplace">The template to replace it</param>
        /// <param name="xpos">X position</param>
        /// <param name="ypos">Y position</param>
        /// <returns>true if the template match</returns>
        public bool Replace(MapTemplate toFind, MapTemplate toReplace, int xpos, int ypos)
        {
            if (Matches(toFind, xpos, ypos))
            {
                Place(toReplace, xpos, ypos);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Search a template in the content of this objetc, and replace it with another one, 
        /// </summary>
        /// <param name="toFind">the template to find</param>
        /// <param name="toReplace">the template to replace with</param>
        /// <returns>true if at least one modification occured</returns>
        public bool ReplaceAll(MapTemplate toFind, MapTemplate toReplace)
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
        ReplaceAll(MapTemplate toFind, MapTemplate toReplace, string operations)
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
        ReplaceAll(MapTemplate toFind,
                MapTemplate toReplace,
                int chance,
                string operations)
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
        /// <param name="source">the character to replace</param>
        /// <param name="dest">the new character to replace with</param>
        public void ReplaceAll(char source, char dest)
        {
            CharUtils.ReplaceAll(Content, source, dest);
        }




    }
}
