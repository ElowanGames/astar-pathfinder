
using System;
using System.Collections.Generic;       // Allows List<Location>()
using System.Linq;                      // Allows List functions .Min(...), .First(...)
using System.Text;


namespace AStarPathfinder
{
    //  **************************************************************
    //  This class keeps track of the tiles that have been visited and 
    //  the tiles that the pathfinder is considering visiting.
    //  **************************************************************

    class Location
    {
        public int X;
        public int Y;
        public int DS;                  // Distance from start point
        public int DD;                  // Distance to destination (estimated by horizontal + vertical tiles)
        public int DSDD;                // DS + DD, for "path scoring" score of this specific Location
        public Location Parent;         // Stores previous tile, effectively keeping track of path
    }


    //  *******************************
    //  This class runs the pathfinder.
    //  *******************************

    class Program
    {
        static void Main(string[] args)
        {
            string[] map = new String[]
            {
                    "+---------+",
                    "|         |",
                    "|A XX     |",
                    "|XXX      |",
                    "|   X     |",
                    "| B       |",
                    "|         |",
                    "+---------+",
            };

            foreach (var line in map)                       // Write each line of map in a separate line
                Console.WriteLine(line);

            Location current = null;
            var start = new Location { X = 1, Y = 2 };
            var target = new Location { X = 2, Y = 5 };
            var nextTiles = new List<Location>();
            var pastTiles = new List<Location>();
            int ds = 0;                                                 // Distance from starting location to current tile (+1 per movement)

            nextTiles.Add(start);                                       // Begin by adding start tile to the search list

            while (nextTiles.Count > 0)                                 // Run while there are still valid queued tiles to search
            {
                var lowestDSDD = nextTiles.Min(l => l.DSDD);                            // Get tile with lowest DSDD score
                current = nextTiles.First(l => l.DSDD == lowestDSDD);

                pastTiles.Add(current);                                         // Add current tile to "pastTiles" list
                nextTiles.Remove(current);                                      // Remove current from "nextTiles" list to prevent revisits

                Console.SetCursorPosition(current.X, current.Y);                        // Mark current tile with '.' graphically
                Console.Write('.');

                System.Threading.Thread.Sleep(300);                                     // Pause for a moment so user can watch

                if (pastTiles.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)      // Break if path is found
                    break;

                var adjSquares = GetWalkableAdjSquares(current.X, current.Y, map);
                ds++;


                foreach(var adjSquare in adjSquares)                            // Check every adjacent tile
                {
                    if (pastTiles.FirstOrDefault(l => l.X == adjSquare.X && l.Y == adjSquare.Y) != null)        // Ignore if already checked tile
                        continue;

                    if (nextTiles.FirstOrDefault(l => l.X == adjSquare.X && l.Y == adjSquare.Y) == null)        // If NOT in nextTiles
                    {
                        adjSquare.DS = ds;
                        adjSquare.DD = ComputeDD(adjSquare.X, adjSquare.Y, target.X, target.Y);
                        adjSquare.DSDD = adjSquare.DS + adjSquare.DD;
                        adjSquare.Parent = current;

                        nextTiles.Insert(0, adjSquare);                          // Add to nextTiles list for checking
                    }

                    else                // If using current DS score makes adjSquare's DD score lower, 
                    {                   // update the Parent b/c it's a better path
                        if (ds + adjSquare.DD < adjSquare.DSDD)
                        {
                            adjSquare.DS = ds;
                            adjSquare.DSDD = adjSquare.DS + adjSquare.DD;
                            adjSquare.Parent = current;
                        }
                    }

                }



            }


            while (current != null)
            {
                Console.SetCursorPosition(current.X, current.Y);
                Console.Write('*');
                Console.SetCursorPosition(current.X, current.Y);
                current = current.Parent;
                System.Threading.Thread.Sleep(300);
            }

            
        }




        static List<Location> GetWalkableAdjSquares(int x, int y, string[] map)
        {
            var possibleLocations = new List<Location>()
            {
                new Location { X=x, Y=y-1 },
                new Location { X=x, Y=y+1 },
                new Location { X=x-1, Y=y },
                new Location { X=x+1, Y=y },
            };

            return possibleLocations.Where(l => map[l.Y][l.X] == ' ' || map[l.Y][l.X] == 'B').ToList();
        }

        static int ComputeDD(int x, int y, int targetX, int targetY)                    // Finds distance to target with no obstacles
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

    }
}
