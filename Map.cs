using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Map definition
/// </summary>
namespace Hforce
{
    public class Map : MapTemplate
    {
        public PatternPosition UpExit { get; set; }     // position of the 'Up' exit 
        public PatternPosition DownExit { get; set; }   // position of the 'Down' exit
        public Map(int X, int Y) : base(X, Y)
        {
            Logger.Info($"Empty map created with size {XSize}x{YSize}");
            // set the map to jocker content
            InitializeContent('?');
        }

    }
}
