using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class OldDungeonGenerator : MonoBehaviour
{
	private class DungeonBlock
	{
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
	}
	
	private class DungeonBlockType
	{
		public const string None = null;
        public const string Empty = "OTHXXXX";
        public const string PathNEWS = "PTHNEWS";
        public const string PathEW = "PTHXEWX";
        public const string PathNS = "PTHNXXS";
        public const string PathNW = "PTHNXWX";
        public const string PathNE = "PTHNEXX";
        public const string PathES = "PTHXEXS";
        public const string PathWS = "PTHXXWS";
        public const string PathNEW = "PTHNEWX";
        public const string PathEWS = "PTHXEWS";
        public const string PathNES = "PTHNEXS";
        public const string PathNWS = "PTHNXWS";
        public const string EndW = "ENDXEXX";
        public const string EndN = "ENDNXXX";
        public const string EndE = "ENDXEXX";
        public const string EndS = "ENDXXXS";
        public static string[] allBlocks = new string[17] { null,"OTHXXXX","PTHNEWS","PTHXEWX",
                                            "PTHNXXS","PTHNXWX","PTHNEXX","PTHXEXS",
                                            "PTHXXWS","PTHNEWX","PTHXEWS","PTHNEXS",
                                            "PTHNXWS","ENDXEXX","ENDNXXX","ENDXEXX",
                                            "ENDXXXS" };
	}
	
    private class CellLocation
    {
        public CellLocation(int setX, int setZ)
        {
            x = setX;
            z = setZ;
        }

        public int x;
        public int z;

        public override bool Equals(object obj)
        {
            if (obj is CellLocation)
            {
                CellLocation testObj = (CellLocation)obj;
                if (testObj.x == x && testObj.z == z)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
	
	// For all Big and Empty Blocks
	public Transform PFB_STANDARD;
	
	// For all Path, End, and Crux Blocks
	public Transform PFB_CRUX;
		
    public Transform PrefabBuildingBlockParent;

    public int X_DB_COUNT;
    public int Z_DB_COUNT;

    public float X_DB_WIDTH;
    public float Z_DB_HEIGHT;

    [HideInInspector]
    public int Y_DB_COUNT = 1;

    private string[,] dbtGrid;

    void Start()
    {
        dbtGrid = new string[X_DB_COUNT, Z_DB_COUNT];
		
        CreateDungeonFloor();
    }

    void CreateDungeonFloor()
    {
        PopulateDungeonBlockTypeGrid();

        ProcessDungeonBlockTypeGrid();
    }

    void PopulateDungeonBlockTypeGrid()
    {
        // Populate grid with empty dungeon blocks
        PrepareGrid();

        // Populate grid with valid dungeon blocks
        FillOutGrid();

        // 
        FixUpIslands();

        //
        CapOffDeadEnds();
    }

    void PrepareGrid()
    {
        for (int x = 0; x < X_DB_COUNT; ++x)
        {
            for (int z = 0; z < Z_DB_COUNT; ++z)
            {
                dbtGrid[x, z] = DungeonBlockType.Empty;
            }
        }
    }
	
    void FillOutGrid()
    {
        for (int x = 0; x < X_DB_COUNT; ++x)
        {
            for (int z = 0; z < Z_DB_COUNT; ++z)
            {
                if (dbtGrid[x, z] == DungeonBlockType.Empty)
                {
                    dbtGrid[x, z] = ChooseValidBuildingBlockType(new CellLocation(x, z));
                }
            }
        }
    }

    string ChooseValidBuildingBlockType(CellLocation location)
    {
        bool hasWest = false;
        bool hasNorth = false;
        bool hasEast = false;
        bool hasSouth = false;
        bool noWest = false;
        bool noNorth = false;
        bool noEast = false;
        bool noSouth = false;
		
		DungeonBlock dBlock = new DungeonBlock();
		
		// No Blocks that has SOUTH EXIT
        if (location.x > 0 && dbtGrid[location.x - 1, location.z] != DungeonBlockType.Empty)
        {
            if (CellLocationHasEast(new CellLocation(location.x - 1, location.z)))
            {
				dBlock.HasWestExit = true;
                hasWest = true;
            }
            else
            {
				dBlock.HasWestExit = false;
                noWest = true;
            }
        }

        if (location.x < X_DB_COUNT - 1 && dbtGrid[location.x + 1, location.z] != DungeonBlockType.Empty)
        {
            if (CellLocationHasWest(new CellLocation(location.x + 1, location.z)))
            {
				dBlock.HasEastExit = true;
                hasEast = true;
            }
            else
            {
				dBlock.HasEastExit = false;
                noEast = true;
            }
        }

        if (location.z > 0 && dbtGrid[location.x, location.z - 1] != DungeonBlockType.Empty)
        {
            if (CellLocationHasNorth(new CellLocation(location.x, location.z - 1)))
            {
				dBlock.HasSouthExit = true;
                hasSouth = true;
            }
            else
            {
				dBlock.HasSouthExit = false;
                noSouth = true;
            }
        }

        if (location.z < Z_DB_COUNT - 1 && dbtGrid[location.x, location.z + 1] != DungeonBlockType.Empty)
        {
            if (CellLocationHasSouth(new CellLocation(location.x, location.z + 1)))
            {
				dBlock.HasNorthExit = true;
                hasNorth = true;
            }
            else
            {
				dBlock.HasNorthExit = false;
                noNorth = true;
            }
        }
		
		// If X0,Z0 is always NE
        string qualifyingBlockType = GetQualifyingBlockType(location.x, location.z, hasWest, hasNorth, hasEast, hasSouth, noWest, noNorth, noEast, noSouth, false);
    	
		return qualifyingBlockType;
	}

    string GetQualifyingBlockType(int placementGridX, int placementGridZ, bool hasWest, bool hasNorth, bool hasEast, bool hasSouth, bool noWest, bool noNorth, bool noEast, bool noSouth, bool useDeadEnds)
    {
        if (useDeadEnds)
        {
            return DungeonBlockType.Empty;
        }

        ArrayList currentQualifyingTypes = GetListOfAllBuildingBlockTypes();

        currentQualifyingTypes.Remove(DungeonBlockType.Empty);
        currentQualifyingTypes.Remove(DungeonBlockType.None);

        currentQualifyingTypes.Remove(DungeonBlockType.EndW);
        currentQualifyingTypes.Remove(DungeonBlockType.EndN);
        currentQualifyingTypes.Remove(DungeonBlockType.EndE);
        currentQualifyingTypes.Remove(DungeonBlockType.EndS);

        if (hasWest == true)
        {
            currentQualifyingTypes.Remove(DungeonBlockType.PathNS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNE);
            currentQualifyingTypes.Remove(DungeonBlockType.PathES);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNES);
        }

        if (hasNorth == true)
        {
            currentQualifyingTypes.Remove(DungeonBlockType.PathEW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathES);
            currentQualifyingTypes.Remove(DungeonBlockType.PathEWS);
        }

        if (hasEast == true)
        {
            currentQualifyingTypes.Remove(DungeonBlockType.PathNS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNWS);
        }

        if (hasSouth == true)
        {
            currentQualifyingTypes.Remove(DungeonBlockType.PathEW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNE);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNEW);
        }

        if (placementGridX == 0 || noWest == true)
        {
            currentQualifyingTypes.Remove(DungeonBlockType.PathNEWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathEW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNEW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathEWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNWS);
        }

        if (placementGridX == X_DB_COUNT - 1 || noEast == true)
        {
            currentQualifyingTypes.Remove(DungeonBlockType.PathNEWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathEW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNE);
            currentQualifyingTypes.Remove(DungeonBlockType.PathES);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNEW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathEWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNES);
        }

        if (placementGridZ == 0 || noSouth == true)
        {
            currentQualifyingTypes.Remove(DungeonBlockType.PathNEWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathES);
            currentQualifyingTypes.Remove(DungeonBlockType.PathEWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNES);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNWS);
        }

        if (placementGridZ == Z_DB_COUNT - 1 || noNorth == true)
        {
            currentQualifyingTypes.Remove(DungeonBlockType.PathNEWS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNS);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNE);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNEW);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNES);
            currentQualifyingTypes.Remove(DungeonBlockType.PathNWS);
        }

        if (currentQualifyingTypes.Count == 0)
        {
            return DungeonBlockType.Empty;
        }
        else
        {
            return (string)currentQualifyingTypes[Random.Range(0, currentQualifyingTypes.Count)];
        }
    }

    bool CellLocationHasEast(CellLocation testLocation)
    {
        if (testLocation.x == X_DB_COUNT - 1)
        {
            return false;
        }

        string testType = dbtGrid[testLocation.x, testLocation.z];
        switch (testType)
        {
            case DungeonBlockType.PathNEWS:
            case DungeonBlockType.PathEW:
            case DungeonBlockType.PathNE:
            case DungeonBlockType.PathES:
            case DungeonBlockType.PathNEW:
            case DungeonBlockType.PathEWS:
            case DungeonBlockType.PathNES:
            case DungeonBlockType.EndE:
                return true;
            default:
                return false;
        }
    }

    bool CellLocationHasWest(CellLocation testLocation)
    {
        if (testLocation.x == 0)
        {
            return false;
        }

        string testType = dbtGrid[testLocation.x, testLocation.z];
        switch (testType)
        {
            case DungeonBlockType.PathNEWS:
            case DungeonBlockType.PathEW:
            case DungeonBlockType.PathNW:
            case DungeonBlockType.PathWS:
            case DungeonBlockType.PathNEW:
            case DungeonBlockType.PathEWS:
            case DungeonBlockType.PathNWS:
            case DungeonBlockType.EndW:
                return true;
            default:
                return false;
        }
    }

    bool CellLocationHasNorth(CellLocation testLocation)
    {
        if (testLocation.z == Z_DB_COUNT - 1)
        {
            return false;
        }

        string testType = dbtGrid[testLocation.x, testLocation.z];
        switch (testType)
        {
            case DungeonBlockType.PathNEWS:
            case DungeonBlockType.PathNS:
            case DungeonBlockType.PathNE:
            case DungeonBlockType.PathNW:
            case DungeonBlockType.PathNEW:
            case DungeonBlockType.PathNES:
            case DungeonBlockType.PathNWS:
            case DungeonBlockType.EndN:
                return true;
            default:
                return false;
        }
    }

    bool CellLocationHasSouth(CellLocation testLocation)
    {
        if (testLocation.z == 0)
        {
            return false;
        }

        string testType = dbtGrid[testLocation.x, testLocation.z];
        switch (testType)
        {
            case DungeonBlockType.PathNEWS:
            case DungeonBlockType.PathNS:
            case DungeonBlockType.PathWS:
            case DungeonBlockType.PathES:
            case DungeonBlockType.PathEWS:
            case DungeonBlockType.PathNES:
            case DungeonBlockType.PathNWS:
            case DungeonBlockType.EndS:
                return true;
            default:
                return false;
        }
    }

    void FixUpIslands()
    {
        ArrayList knownExistingConnections = new ArrayList();

        for (int x = 0; x < X_DB_COUNT; ++x)
        {
            for (int z = 0; z < Z_DB_COUNT; ++z)
            {
                if (dbtGrid[x, z] != DungeonBlockType.Empty && !CellConnectedToCell(x, z, X_DB_COUNT / 2, Z_DB_COUNT / 2, ref knownExistingConnections))
                {
                    dbtGrid[x, z] = DungeonBlockType.Empty;
                }
            }
        }
    }

    void CapOffDeadEnds()
    {
        for (int x = 0; x < X_DB_COUNT; ++x)
        {
            for (int z = 0; z < Z_DB_COUNT; ++z)
            {
                if (dbtGrid[x, z] == DungeonBlockType.Empty)
                {
                    dbtGrid[x, z] = GetDeadEndCapType(x, z);
                }
            }
        }
    }

    string GetDeadEndCapType(int locX, int locZ)
    {
        if (locX > 0 && CellLocationHasEast(new CellLocation(locX - 1, locZ)))
        {
            return DungeonBlockType.EndW;
        }
        else if (locX < X_DB_COUNT - 1 && CellLocationHasWest(new CellLocation(locX + 1, locZ)))
        {
            return DungeonBlockType.EndE;
        }
        else if (locZ > 0 && CellLocationHasNorth(new CellLocation(locX, locZ - 1)))
        {
            return DungeonBlockType.EndS;
        }
        else if (locZ < Z_DB_COUNT - 1 && CellLocationHasSouth(new CellLocation(locX, locZ + 1)))
        {
            return DungeonBlockType.EndN;
        }

        return DungeonBlockType.Empty;
    }

    void GeneratePrefabFromBuildingBlockType(string bbType, float xPos, float yPos)
    {			
        Regex rgxbbType = new Regex("^(?<TYPE>.{3})(?<NEXIT>N{1}|X{1})(?<EEXIT>E{1}|X{1})(?<WEXIT>W{1}|X{1})(?<SEXIT>S{1}|X{1})$");

        Match m = rgxbbType.Match(bbType);
		string type = m.Groups[1].Value; 
		string nExit = m.Groups[2].Value; 
		string eExit = m.Groups[3].Value; 
		string wExit = m.Groups[4].Value; 
		string sExit = m.Groups[5].Value; 
		string[] exits = new string[4] {nExit,eExit,wExit,sExit};
		
		if (type == "PTH" ||
			type == "END")
		{
			Transform newBlock = (Transform)Instantiate(PFB_CRUX, new Vector3(xPos, 0.0f, yPos), Quaternion.identity);
			newBlock.parent = PrefabBuildingBlockParent;   
			
			foreach (Transform area in newBlock)
			{				
				foreach (Transform part in area)
				{
					//Turn off all Ceilings
					if (part.name == "Ceiling")
					{
						part.renderer.enabled = false;
						part.collider.enabled = false;
					}
					

					if (nExit == "X" &&
						area.name == "WingN")
					{
						part.renderer.enabled = false;
						part.collider.enabled = false;
					}
					else if (nExit == "N")
					{
						if ((part.parent.name == "Center" &&
							part.name == "WallN") ||
							(part.parent.name == "WingN" &&
							part.name == "Exit"))
						{
							part.renderer.enabled = false;
							part.collider.enabled = false;
						} 
					}
					
					if (eExit == "X" &&
						area.name == "WingE")
					{
						part.renderer.enabled = false;
						part.collider.enabled = false;
					}
					else if (eExit == "E")
					{
						if ((part.parent.name == "Center" &&
							part.name == "WallE") ||
							(part.parent.name == "WingE" &&
							part.name == "Exit"))
						{
							part.renderer.enabled = false;
							part.collider.enabled = false;
						} 
					}
					
					if (wExit == "X" &&
						area.name == "WingW")
					{
						part.renderer.enabled = false;
						part.collider.enabled = false;
					}
					else if (wExit == "W")
					{
						if ((part.parent.name == "Center" &&
							part.name == "WallW") ||
							(part.parent.name == "WingW" &&
							part.name == "Exit"))
						{
							part.renderer.enabled = false;
							part.collider.enabled = false;
						} 
					}
					
					if (sExit == "X" &&
						area.name == "WingS")
					{
						part.renderer.enabled = false;
						part.collider.enabled = false;
					}
					else if (sExit == "S")
					{
						if ((part.parent.name == "Center" &&
							part.name == "WallS") ||
							(part.parent.name == "WingS" &&
							part.name == "Exit"))
						{
							part.renderer.enabled = false;
							part.collider.enabled = false;
						} 
					}
				}
			}	
		}	
    }

    DungeonBlockType GetRandomBuildingBlockType()
    {
        ArrayList allTypes = GetListOfAllBuildingBlockTypes();
        return (DungeonBlockType)allTypes[Random.Range(0, allTypes.Count)];
    }

    void ProcessDungeonBlockTypeGrid()
    {
        //prefabBuildingBlockWidth = DBPrefabWNES.localScale.x;
        //prefabBuildingBlockHeight = DBPrefabWNES.localScale.z;

        for (int x = 0; x < X_DB_COUNT; ++x)
        {
            for (int z = 0; z < Z_DB_COUNT; ++z)
            {             
                if (dbtGrid[x, z] != DungeonBlockType.None)
                {
                    float instantiateXPosition = transform.position.x + (x * X_DB_WIDTH);
                    float instantiateZPosition = transform.position.z + (z * Z_DB_HEIGHT);

                    GeneratePrefabFromBuildingBlockType(dbtGrid[x, z], instantiateXPosition, instantiateZPosition);  
                }
            }
        }
    }

    ArrayList GetListOfAllBuildingBlockTypes()
    {
        ArrayList returnList = new ArrayList();
        foreach (string bbType in DungeonBlockType.allBlocks)
        {
            returnList.Add(bbType);
        }
        return returnList;
    }

    bool CellConnectedToCell(int startLocX, int startLocZ, int goalLocX, int goalLocZ, ref ArrayList knownExistingConnections)
    {
        ArrayList alreadySearchedList = new ArrayList();
        ArrayList toSearchList = new ArrayList();

        bool foundPath = false;
        bool doneWithSearch = false;
        toSearchList.Add(new CellLocation(startLocX, startLocZ));

        while (!doneWithSearch)
        {
            if (toSearchList.Count == 0)
            {
                doneWithSearch = true;
                break;
            }

            CellLocation toSearch = (CellLocation)toSearchList[0];
            toSearchList.RemoveAt(0);
            if (alreadySearchedList.Contains(toSearch) == false)
            {
                alreadySearchedList.Add(toSearch);
            }

            if ((toSearch.x == goalLocX && toSearch.z == goalLocZ) || knownExistingConnections.Contains(toSearch))
            {
                doneWithSearch = true;
                foundPath = true;
                
                foreach (CellLocation pos in alreadySearchedList)
                {
                    knownExistingConnections.Add(pos);
                }

                foreach (CellLocation pos in toSearchList)
                {
                    knownExistingConnections.Add(pos);
                }                

                break;
            }
            else
            {
                if (CellLocationHasEast(new CellLocation(toSearch.x, toSearch.z)))
                {
                    CellLocation newLocation = new CellLocation(toSearch.x + 1, toSearch.z);
                    if (toSearchList.Contains(newLocation) == false && alreadySearchedList.Contains(newLocation) == false)
                    {
                        toSearchList.Add(newLocation);
                    }
                }

                if (CellLocationHasWest(new CellLocation(toSearch.x, toSearch.z)))
                {
                    CellLocation newLocation = new CellLocation(toSearch.x - 1, toSearch.z);
                    if (toSearchList.Contains(newLocation) == false && alreadySearchedList.Contains(newLocation) == false)
                    {
                        toSearchList.Add(newLocation);
                    }
                }

                if (CellLocationHasNorth(new CellLocation(toSearch.x, toSearch.z)))
                {
                    CellLocation newLocation = new CellLocation(toSearch.x, toSearch.z + 1);
                    if (toSearchList.Contains(newLocation) == false && alreadySearchedList.Contains(newLocation) == false)
                    {
                        toSearchList.Add(newLocation);
                    }
                }

                if (CellLocationHasSouth(new CellLocation(toSearch.x, toSearch.z)))
                {
                    CellLocation newLocation = new CellLocation(toSearch.x, toSearch.z - 1);
                    if (toSearchList.Contains(newLocation) == false && alreadySearchedList.Contains(newLocation) == false)
                    {
                        toSearchList.Add(newLocation);
                    }
                }
            }
        }

        return foundPath;
    }
	
	bool HasEast(CellLocation cellLocation)
    {
        if (cellLocation.x == X_DB_COUNT - 1)
        {
            return false;
        }

        string testType = dbtGrid[cellLocation.x, cellLocation.z];
        switch (testType)
        {
            case DungeonBlockType.PathNEWS:
            case DungeonBlockType.PathEW:
            case DungeonBlockType.PathNE:
            case DungeonBlockType.PathES:
            case DungeonBlockType.PathNEW:
            case DungeonBlockType.PathEWS:
            case DungeonBlockType.PathNES:
            case DungeonBlockType.EndE:
                return true;
            default:
                return false;
        }
    }
	
    


 










}
