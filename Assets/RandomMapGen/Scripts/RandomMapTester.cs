/// <summary>
/// Random map tester.
/// </summary>
using UnityEngine;
using System.Collections;

public class RandomMapTester : MonoBehaviour
{
     
    [Header("Map Dimensions")]
    public int mapWidth = 20;
    public int mapHeight = 20;

    [Space]
    [Header("Vizualize Map")]
    public GameObject mapContainer;
    public GameObject tilePrefab;
    public Vector2 tileSize = new Vector2(16, 16);

    [Space]
    [Header("Map Sprites")]
    public Texture2D islandTexture;
    public Texture2D fowTexture;

    [Space]
    [Header("Player")]
    public GameObject playerPrefab;
    public GameObject player;
    // Expand players vision in fog of war, how many tiles to reveal around player every move (needs to be odd number)
    public int distance = 3;

    [Space]
    [Header("Decorate Map")]
    [Range(0, .9f)]
    public float erodePercent = .5f;
    public int erodeIterations = 2;
    [Range(0, .9f)]
    public float treePercent = .3f;
    [Range(0, .9f)]
    public float hillPercent = .2f;
    [Range(0, .9f)]
    public float mountainsPercent = .1f;
    [Range(0, .9f)]
    public float townPercent = .05f;
    [Range(0, .9f)]
    public float monsterPercent = .1f;
    [Range(0, .9f)]
    public float lakePercent = .05f;

    public Map map;

    // Used whenever need to calculate a position on the map
    private int tmpX;
    private int tmpY;
    // Fog of war
    private Sprite[] islandTileSprites;
    private Sprite[] fowTileSprites;

    // Use this for initialization
    void Start()
    {
        // Fog of war
        islandTileSprites = Resources.LoadAll<Sprite>(islandTexture.name);
        fowTileSprites = Resources.LoadAll<Sprite>(fowTexture.name);

        map = new Map();
        MakeMap();
        // Delay creating player until map added and all calculations done
        StartCoroutine(AddPlayer());
    }

    // Create a new map button
    public void Reset() 
    {
        map = new Map();
        MakeMap();
        StartCoroutine(AddPlayer());
    }

    // Delay after map created
    IEnumerator AddPlayer()
    {
        // Delay creating player until end of frame, to create map and perform all calcualations before adding player
        yield return new WaitForEndOfFrame();
        CreatePlayer();
    }

    public void MakeMap()
    {
        map.NewMap(mapWidth, mapHeight);
        map.CreateIsland(
            erodePercent,
            erodeIterations,
            treePercent,
            hillPercent,
            mountainsPercent,
            townPercent,
            monsterPercent,
            lakePercent
        );
        CreateGrid();
        CenterMap(map.castleTile.id);
    }

    void CreateGrid()
    {
        ClearMapContainer();

        var total = map.tiles.Length;
        var maxColumns = map.columns;
        var column = 0;
        var row = 0;

        for (var i = 0; i < total; i++)
        {
            column = i % maxColumns;

            var newX = column * tileSize.x;
            var newY = -row * tileSize.y;

            var go = Instantiate(tilePrefab);
            go.name = "Tile " + i;
            go.transform.SetParent(mapContainer.transform);
            go.transform.position = new Vector3(newX, newY, 0);

            DecorateTile(i);

            if (column == (maxColumns - 1))
            {
                row++;
            }
        }
    }

    // Fog of war
    private void DecorateTile(int tileID)
    {
        var tile = map.tiles[tileID];
        var spriteID = tile.autotileID;
        var go = mapContainer.transform.GetChild(tileID).gameObject;
       
        if (spriteID >= 0)
        {
            var sr = go.GetComponent<SpriteRenderer>();
            // If the tile has not been visited, add fog of war
            // TODO fix fog of war
            if (tile.visited) {
                sr.sprite = islandTileSprites[spriteID];
            } else {
                // Go through current tile and neighbours to calculate fog of war autotile id
                tile.CalculateFoWAutotileID();
                // Make sure fow tiles are always in range as there's more island tiles than fow tiles
                sr.sprite = fowTileSprites[Mathf.Min(tile.fowAutoTileID, fowTileSprites.Length - 1)];
            }
        }
    }

    public void CreatePlayer()
    {
        player = Instantiate(playerPrefab);
        player.name = "Player";
        // Set player game object to be inside map container, where all other game objects are
        player.transform.SetParent(mapContainer.transform);

        // Set player starting position next to castle at start
        var controller = player.GetComponent<MapMovementController>();
        controller.map = map;
        controller.tileSize = tileSize;

        // Trigger event when player lands on tile
        controller.tileActionCallback += TileActionCallback;

        // Camera follows player
        var moveScript = Camera.main.GetComponent<MoveCamera>();
        moveScript.target = player;

        controller.MoveTo(map.castleTile.id);
    }

    // Trigger event when player lands on this tile
    void TileActionCallback(int type)
    {
        var tileID = player.GetComponent<MapMovementController>().currentTile;
        VisitTile(tileID);
    }

    void ClearMapContainer()
    {
        var children = mapContainer.transform.GetComponentsInChildren<Transform>();
        for (var i = children.Length - 1; i > 0; i--)
        {
            Destroy(children[i].gameObject);
        }

    }

    void CenterMap(int index)
    {
        var camPos = Camera.main.transform.position;
        var width = map.columns;

        PositionUtility.CalculatePosition(index, width, out tmpX, out tmpY);

        camPos.x = tmpX * tileSize.x;
        camPos.y = -tmpY * tileSize.y;
        Camera.main.transform.position = camPos;
    }

    // Fog of war
    void VisitTile(int index)
    {
        // Expanded vision around player
        int column, newX, newY, row = 0;
        PositionUtility.CalculatePosition(index, map.columns, out tmpX, out tmpY);
        // Find halfway point in fov
        var half = Mathf.FloorToInt(distance / 2f);
        // Shift over area to look at tiles around player
        tmpX -= half;
        tmpY -= half;

        var total = distance * distance;
        // Max columns of tiles to visit for reveal tiles
        var maxColumns = distance - 1;

        for (int i = 0; i < total; i++) {
            // Current column we're on
            column = i % distance;

            newX = column + tmpX;
            newY = row + tmpY;

            // Recalculate index starting loop on
            PositionUtility.CalculateIndex(newX, newY, map.columns, out index);

            // Make sure index in range
            if (index > -1 && index < map.tiles.Length) {
                var tile = map.tiles [index];
                tile.visited = true;
                DecorateTile (index);

                // Update neighbouring tiles
                foreach (var neighbour in tile.neighbours) {

                    if (neighbour != null) {
                        if (!neighbour.visited) {
                            neighbour.CalculateFoWAutotileID ();
                            DecorateTile (neighbour.id);
                        }
                    }
                }
            }
                
            if (column == maxColumns) {
                row++;
            }
        }
    }
}
