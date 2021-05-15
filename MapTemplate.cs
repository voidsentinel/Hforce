using System.Collections.Generic;

/// <summary>
/// Room Definition.
/// <para>A room is composed of a n*m array of char, where the chars define what is present at this position in the room

/// </summary>
namespace Hforce
{
    public class MapTemplate
    {
        private static int created = 0;

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
