using System;
using System.Collections.Generic;
using System.IO;

namespace Hforce
{
    public class MapTemplateLoader
    {
        /// <summary>
        /// Load all template files in the given directory

        /// </summary>
        /// <param name="sourceDirectory"> the directpry where to search for templates</param>
        /// <return> a list of all the templ    ate </return>
        public static MapTemplateList loadFromDirectory(String sourceDirectory, string name)
        {
            MapTemplateList templates = new MapTemplateList(name);
            loadFromDirectory(sourceDirectory, templates);
            return templates;
        }

        /// <summary>
        /// Load all template files in the given directory and put them in the given list

        /// </summary>
        /// <param name="sourceDirectory"> the directpry where to search for templates</param>
        /// <param name="templates"> The liust of template to add the templates to</param>
        public static void loadFromDirectory(
            string sourceDirectory,
            MapTemplateList templates
        )
        {
            try
            {
                Logger.Info($"Reading templates in directory {sourceDirectory} into {templates._name}");
                Logger.Push();
                var txtFiles =
                    Directory.EnumerateFiles(sourceDirectory, "*.rm1");
                foreach (string currentFile in txtFiles)
                {
                    loadFromFile(currentFile, templates);
                }
                templates
                    .collection
                    .Sort(delegate (MapTemplate x, MapTemplate y)
                    {
                        var xx = x.YSize * x.XSize;
                        var yy = y.YSize * y.XSize;
                        if (xx == yy) return 0;
                        if (xx < yy)
                            return -1;
                        else
                            return 1;
                    });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Console.WriteLine(e.Message);
            }
            Logger.Pop();
        }

        /// <summary>
        /// Load a room from a file
        /// </summary>
        /// <param name="sourceFile">path of the file to use</param>
        public static void loadFromFile(string sourceFile, MapTemplateList templates)
        {
            Logger.Info($"Reading template in file {sourceFile}");
            Logger.Push();
            try
            {
                int format = 0;
                StreamReader reader = new StreamReader(sourceFile);
                Int32.TryParse(reader.ReadLine(), out format);
                switch (format)
                {
                    case 0:
                        Read_FileFormat_0(reader, templates);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Console.WriteLine(e.Message);
            }
            Logger.Pop();
        }

        /// <summary>
        /// Read a room in the "0" FileFormat
        /// First line is XSize
        ///Second Line is YSize
        ///Third line are operation to generate Mirror rooms (Rotate : X, H Mirror : X, V Mirror : Y)
        ///Followed by YSize lines of XSize chars
        /// </summary>
        /// <param name="reader"></param>
        public static void Read_FileFormat_0(StreamReader reader, MapTemplateList templates)
        {
            try
            {
                int xsize = 0;
                int ysize = 0;
                Int32.TryParse(reader.ReadLine(), out xsize);
                Int32.TryParse(reader.ReadLine(), out ysize);
                string operations = reader.ReadLine();

                char[,] roomData = loadData(reader, xsize, ysize);

                // if requested, create mirrored copies
                List<char[,]> mirrors =
                    CharUtils.CreateMirrors(roomData, operations);
                int count = 0;
                int sourceId = 0;
                foreach (char[,] copy in mirrors)
                {
                    MapTemplate tmp = new MapTemplate(copy);
                    if (count == 0)
                        sourceId = tmp.Id;
                    tmp.SourceId = sourceId;
                    templates.Add(tmp);
                    CharUtils.saveAsImage($"./assets/images/{templates._name}_{tmp.Id}.png", copy);
                    count++;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Console.WriteLine(e.Message);
            }

        }


        /// <summary>
        /// create a room of the given size and load it's content from the reader
        /// </summary>
        /// <param name="reader">The reader to the file containing the data</param>
        /// <param name="xsize">The x size of the room</param>
        /// <param name="ysize">The y size of the room</param>
        /// <returns>a roomTemplate</returns>
        private static char[,] loadData(StreamReader reader, int xsize, int ysize)
        {
            char[,] response = new char[ysize, xsize];
            for (int y = 0; y < ysize; y++)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < xsize; x++)
                {
                    response[y, x] = line[x];
                }
            }
            return response;
        }

    }
}
