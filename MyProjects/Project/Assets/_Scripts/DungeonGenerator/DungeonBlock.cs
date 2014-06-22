using UnityEngine;
using System.Collections;

public class  DungeonBlock
{
	public Transform BasePrefab;
	public ExitVariant[] NorthExitVariants;
	public ExitVariant[] EastExitVariants;
	public ExitVariant[] WestExitVariants;
	public ExitVariant[] SouthExitVariants;
	
	public string BlockFamily = string.Empty;
	public bool IsEmpty = true;
	public bool HasNorthExit = false;
	public bool HasEastExit = false;
	public bool HasWestExit = false;
	public bool HasSouthExit = false;
	public bool IsUniqueBlock = false;
	public bool IsRoom = false;
	public Transform Prefab = null;
	public Material BasicMaterial = null;
	
	const string PRT_CEILING = "Ceiling";
	const string PRT_WALL = "Wall";
	const string PRT_FLOOR = "Floor";
	const string LOC_NORTH = "N";
	const string LOC_EAST = "E";
	const string LOC_WEST = "W";
	const string LOC_SOUTH = "S";
	const string LOC_FULL = "F";
	
	const float POS_CEILINGDF_MODIFIER = 0.0f;
	
	// N=Z+ S=Z-
	// E=X+ W=X-
	const float POS_CEILING05_MODIFIER = 7.25f;
	
	// N=Z+ S=Z-
	// E=X+ W=X-
	const float POS_WALL10_MODIFIER = 9.75f;
	
	// Crux Positions
	// N=XB+- ZA+
	// S=XB+- ZA-
	// E=XA+  ZB+-
	// W=XA-  ZB+-
	
	// Standard Positions
	// N=XB+- ZC+
	// S=XB+- ZC-
	// E=XC+  ZB+-
	// W=XC-  ZB+-
	const float POS_WALL05_MODIFIER_A = 4.75f;
	const float POS_WALL05_MODIFIER_B = 7.25f;
	const float POS_WALL05_MODIFIER_C = 9.75f;
	/*	
    public bool HasNorthExit(Transform db)
    {
		if (db.Find(EXIT_05_N) || 
			db.Find(EXIT_10_N) ||
			db.Find(EXIT_15_N) ||
			db.Find(EXIT_20_N))
		{
			return true;
		}
		return false;
    }

    public bool HasEastExit(Transform db)
    {		
		if (db.Find(EXIT_05_E) || 
			db.Find(EXIT_10_E) ||
			db.Find(EXIT_15_E) ||
			db.Find(EXIT_20_E))
		{
			return true;
		}
		return false;
    }

    public bool HasWestExit(Transform db)
    {		
		if (db.Find(EXIT_05_W) || 
			db.Find(EXIT_10_W) ||
			db.Find(EXIT_15_W) ||
			db.Find(EXIT_20_W))
		{
			return true;
		}
		return false;
    }

    public bool HasSouthExit(Transform db)
    {
		if (db.Find(EXIT_05_S) || 
			db.Find(EXIT_10_S) ||
			db.Find(EXIT_15_S) ||
			db.Find(EXIT_20_S))
		{
			return true;
		}
		return false;
    }
	
	public bool HasCeiling(Transform db)
	{
		if (db.Find(CEILING_15_H) ||
			db.Find(CEILING_15_V) ||
			db.Find(CEILING_20_F))
		{
			return true;
		}
		return false;
	}
	
	public bool HasFloor(Transform db)
	{
		if (db.Find(FLOOR_20_F))
		{
			return true;
		}
		return false;
	}
	*/
	
	
}


