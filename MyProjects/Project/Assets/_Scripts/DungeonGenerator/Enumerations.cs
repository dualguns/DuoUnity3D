using UnityEngine;
using System.Collections;

public class Enumerations
{
    public enum Direction
    {
        North = 0,
        East = 1,
        West = 2,
		South = 3
    }

    public enum DungeonBlockType
    {
        None,
        Empty,
        PathNorthEastWestSouth,
        PathEastWest,
        PathNorthSouth,
        PathNorthWest,
        PathNorthEast,
        PathEastSouth,
        PathWestSouth,
        PathNorthEastWest,
        PathEastWestSouth,
        PathNorthEastSouth,
        PathNorthWestSouth,
        EndWest,
        EndNorth,
        EndEast,
        EndSouth
    }
}