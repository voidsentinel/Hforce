using System;
using System.Collections.Generic;
using System.IO;

namespace Hforce
{
    public class ReplacementRuleLoader
    {
        /// <summary>
        /// Load all template files in the given directory

        /// </summary>
        /// <param name="sourceDirectory"> the directpry where to search for templates</param>
        /// <return> a list of all the templ    ate </return>
        public static ReplacementRuleList loadFromDirectory(String sourceDirectory, string name)
        {
            ReplacementRuleList templates = new ReplacementRuleList(name);
            loadFromDirectory(sourceDirectory, templates);
            return templates;
        }

        /// <summary>
        /// Load all template files in the given directory and put them in the given list

        /// </summary>
        /// <param name="sourceDirectory"> the directpry where to search for templates</param>
        /// <param name="templates"> The liust of template to add the templates to</param>
        public static void loadFromDirectory(string sourceDirectory, ReplacementRuleList templates)
        {
            try
            {
                Logger.Info($"Reading Replacement Rules in directory {sourceDirectory} into {templates._name}");
                Logger.Push();
                var txtFiles =
                    Directory.EnumerateFiles(sourceDirectory, "*.rm2");
                foreach (string currentFile in txtFiles)
                {
                    loadFromFile(currentFile, templates);
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
        /// Load a room from a file
        /// </summary>
        /// <param name="sourceFile">path of the file to use</param>
        public static void loadFromFile(string sourceFile, ReplacementRuleList templates)
        {
            Logger.Info($"Reading Replacement Rule in file {sourceFile}");
            Logger.Push();
            try
            {
                int format = 0;
                StreamReader reader = new StreamReader(sourceFile);
                Int32.TryParse(reader.ReadLine(), out format);
                switch (format)
                {
                    case 1:
                        Read_FileFormat_1(reader, templates);
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
        public static void Read_FileFormat_1(StreamReader reader, ReplacementRuleList templates)
        {
            try
            {
                int priority = 1;
                int xsize = 0;
                int ysize = 0;
                int chance = 100;
                Int32.TryParse(reader.ReadLine(), out priority);
                Int32.TryParse(reader.ReadLine(), out xsize);
                Int32.TryParse(reader.ReadLine(), out ysize);
                string operations = reader.ReadLine();

                char[,] initialData = loadData(reader, xsize, ysize);
                Int32.TryParse(reader.ReadLine(), out chance);
                char[,] replacementData = loadData(reader, xsize, ysize);

                // if requested, create mirrored copies
                List<char[,]> initialMirrors =
                    CharUtils.CreateMirrors(initialData, operations);
                List<char[,]> replacementMirrors =
                    CharUtils.CreateMirrors(replacementData, operations);
                for (int i = 0; i < initialMirrors.Count; i++)
                {
                    MapTemplate initial = new MapTemplate(initialMirrors[i]);
                    MapTemplate replacement = new MapTemplate(replacementMirrors[i]);
                    ReplacementRule replace = new ReplacementRule();
                    replace.InitialContent = initial;
                    replace.ReplacementContent = replacement;
                    replace.Priority = priority;
                    replace.Chance = chance;
                    if (replace.Check())
                        templates.Add(replace);
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
