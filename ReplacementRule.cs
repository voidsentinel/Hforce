using System.Collections.Generic;

/// <summary>
/// Room Definition.
/// <para>A room is composed of a n*m array of char, where the chars define what is present at this position in the room

/// </summary>
namespace Hforce
{
    public class ReplacementRule
    {
        private static int created = 0;

        /// <value>get the id of the room. The Id is auto-defined at creation.
        public int Id { get; }

        public int Chance { get; set; }

        public int Priority { get; set; }

        //
        public MapTemplate InitialContent { get; set; }

        public MapTemplate ReplacementContent { get; set; }

        /// <summary>
        /// Constructor for a basic room

        /// </summary>
        /// <remarks>
        /// You need to fille the room later
        /// </remarks>

        public ReplacementRule()
        {
            Id = created;
            created = created + 1;
            Logger.Info($"Empty Replacement Rule:{Id} created");
        }

        public bool Check()
        {

            if (InitialContent == null || ReplacementContent == null)
            {
                return false;
            }
            return (InitialContent.XSize == ReplacementContent.XSize && InitialContent.YSize == ReplacementContent.YSize);
        }

    }
}
