using System;
using System.Collections.Generic;
using System.IO;

namespace Hforce
{
    public class CharGroupsLoader
    {

        public static void loadFromDirectory(String sourceDirectory)
        {
            Logger.Info($"Reading chars group in directory {sourceDirectory}");
            Logger.Push();
            try
            {
                var txtFiles =
                    Directory.EnumerateFiles(sourceDirectory, "*.cgr");
                foreach (string currentFile in txtFiles)
                {
                    loadFromFile(currentFile);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Console.WriteLine(e.Message);
            }
            Logger.Pop();
        }

        public static void loadFromFile(string sourceFile)
        {
            Logger.Info($"Reading char groups in file {sourceFile}");
            Logger.Push();
            try
            {
                StreamReader reader = new StreamReader(sourceFile);

                while (true)
                {
                    String groupname = reader.ReadLine();
                    String charList = reader.ReadLine();

                    int groupId = CharGroups.addGroup(groupname);
                    Logger.Info($"Creating groups {groupname}");
                    foreach (char v in charList)
                    {
                        CharGroups.addCharacterGroup(v, groupId);
                    }
                }

            }
            catch (Exception)
            {
                // do nothing
            }
            Logger.Pop();
        }
    }

}
