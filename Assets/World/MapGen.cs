using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class MapGen : MonoBehaviour {

    [Header("Sprite Files")]
    public Sprite[] dirtSprites;
    public Sprite[] waterSprites;
    public Sprite[] woodSprites;
    public Sprite[] stoneSprites;
    public Sprite[] foundationSprites;

    Sprite[] allTerrainSprites;

    [Header("Map Generation")]
    public static MapGen instance;
    public bool reRoll;
    public int bake = 300;
    [Header("Force Step")]
    public bool tick;
    public int tickStep = 10;
    public Tile[,] tiles;
    [Header("Map Size")]

    public Vector2Int size;
    public Vector2Int seed;

    [Header("Terrain Shape")]
    public float noiseScale;
    public float noiseScale2;

    public float renderScale = 0.16f;

    [Header("Simulation Variables")]

    public float nutrientTransfer = 1;
    public float waterNutrientTransfer = 1.3f;
    public float treeDensity = 30;

    [Header("Playback Speed")]
    public float tickRate;
    float tickDelta;

    [Header("Simulation Runtime")]
    public int tickTime;


    [Header("Instances")]
    public List<Agent> agents = new List<Agent>();
    public List<House> houses;

    public RawImage worldRenderer;
    public Texture2D worldTexture;
    Color[,] terrainColours;
    int ppu = 100;
    int tileScale;

    // Start is called before the first frame update
    void Start() {
        Application.targetFrameRate = 60;
        Generate();
    }

    void SetupGraphics() {

        allTerrainSprites = new Sprite[dirtSprites.Length + waterSprites.Length + woodSprites.Length + stoneSprites.Length + foundationSprites.Length];
        int d = dirtSprites.Length;
        int w = waterSprites.Length;
        int wo = woodSprites.Length;
        int s = stoneSprites.Length;
        int f = foundationSprites.Length;
        dirtSprites.CopyTo(allTerrainSprites, 0);
        waterSprites.CopyTo(allTerrainSprites, d);
        woodSprites.CopyTo(allTerrainSprites, d + w);
        stoneSprites.CopyTo(allTerrainSprites, d + w + wo);
        foundationSprites.CopyTo(allTerrainSprites, d + w + wo + s);

        Tile.dirtSpriteOffset = 0;
        Tile.waterSpriteOffset = d;
        Tile.woodSpriteOffset = d + w;
        Tile.stoneSpriteOffset = d + w + wo;
        Tile.foundationSpriteOffset = d + w + wo + s;


        tileScale = Mathf.RoundToInt(renderScale * ppu);
        terrainColours = new Color[size.x, size.y];
        worldTexture = new Texture2D(size.x * tileScale, size.y * tileScale);
        worldTexture.filterMode = FilterMode.Point;
        worldRenderer.texture = worldTexture;
        worldRenderer.rectTransform.sizeDelta = new Vector2(size.x * renderScale, size.y * renderScale);
    }

    void Generate() {
        //update world texture
        SetupGraphics();

        tickTime = 0;
        tiles = new Tile[size.x, size.y];
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                tiles[i, j] = Generate(new Vector2Int(i, j));
            }
        }

        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                AssignNeighbours(i, j);
            }
        }

        Tick(bake);

        foreach (House h in houses) {
            if (h.prebuild) {
                h.PreBuild();
            }
        }
    }


    public void AssignNeighbours(int i, int j) {
        //set tile neighbours
        List<Tile> neighbours = new List<Tile>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }
                Tile neighbour = GetTile(i + x, j + y);
                if (neighbour != null) {
                    neighbours.Add(neighbour);
                }
            }
        }
        tiles[i, j].SetNeighbours(neighbours);
    }

    void Tick(int amount) {

        for (int x = 0; x < amount; x++) {
            for (int i = 0; i < size.x; i++) {
                for (int j = 0; j < size.y; j++) {
                    tiles[i, j].Tick();
                }
            }

            for (int i = 0; i < size.x; i++) {
                for (int j = 0; j < size.y; j++) {
                    tiles[i, j].Tock();
                }
            }
        }

        TickAgents();
        tickTime += amount;


        //update world texture
        if (worldTexture == null) {
            SetupGraphics();
        }




    }
    public void UpdateGraphics() {
        Color c;
        Texture2D temp = new Texture2D(tileScale, tileScale);
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                if (tiles[i, j] != null) {
                    c = tiles[i, j].GetColor();
                    if (c != terrainColours[i, j]) {

                        try {
                            worldTexture.SetPixels(
                                i * tileScale,
                                j * tileScale,
                                temp.width,
                                temp.height,
                                allTerrainSprites[tiles[i, j].GetSpriteIndex()].texture.GetPixels());
                            terrainColours[i, j] = c;
                        } catch (System.Exception e) {
                            Debug.Log(e + " | " + tiles[i, j] + " " + tiles[i, j].GetSpriteIndex() + " " + allTerrainSprites.Length);
                        }

                    }
                }
            }
        }
        worldTexture.Apply();

    }

    void TickAgents() {
        if (Application.isPlaying) {
            for (int i = 0; i < agents.Count; i++) {
                if (agents[i].ready) {
                    agents[i].Tick();
                }
            }
        }
    }

    private void OnValidate() {
        instance = this;

        if (tick) {
            tick = false;
            if (tiles != null) {

                Tick(tickStep);

            }
        } else {

            if (reRoll) {
                reRoll = false;
                seed = new Vector2Int(Random.Range(0, 10000), Random.Range(0, 10000));
            }
            Generate();
        }
    }

    // Update is called once per frame

    public void Update() {
        tickDelta += Time.deltaTime;
        while (tickDelta > 1f / tickRate) {
            tickDelta -= 1f / tickRate;
            Tick(1);
        }
        UpdateGraphics();
    }

    Tile Generate(Vector2Int pos) {
        float ns = noiseScale * 0.01f;
        float ns2 = noiseScale2 * 0.01f;

        //use noise to roll a number between 0 and 1
        float noise = Mathf.PerlinNoise((pos.x + seed.x) * ns, (pos.y + seed.y) * ns);

        //add another layer of noise
        noise += Mathf.PerlinNoise(pos.x * ns2, pos.y * ns2) * 0.1f;

        //radial gradiant to make the edges water

        Vector2 middle = new Vector2(size.x / 2, size.y / 2);
        float dist = Vector2.Distance(middle, pos);
        float maxDist = Vector2.Distance(Vector2.zero, middle);
        noise -= dist / maxDist * 0.2f;

        if (noise < 0.33f) {
            return new Water(pos);
        } else if (noise < 0.5f) {
            return new Dirt(pos);
        } else if (noise < 0.66f) {
            //randomly place trees every 1/30 tiles
            if (Random.value < 1 / treeDensity) {
                return new Wood(pos);
            }
            return new Dirt(pos);
        } else {
            return new Stone(pos);
        }
    }

    public Tile GetTile(Vector2Int pos) {
        if (pos.x < 0 || pos.x >= size.x || pos.y < 0 || pos.y >= size.y) {
            return null;
        }
        return tiles[pos.x, pos.y];
    }

    public Tile GetTile(int x, int y) {
        if (x < 0 || x >= size.x || y < 0 || y >= size.y) {
            return null;
        }
        return tiles[x, y];
    }

    public Tile ReplaceTile(Vector2Int pos, Tile tile) {
        Debug.Log("Replacing tile at " + pos);
        tiles[pos.x, pos.y] = tile;
        tile.pos = pos;

        //fix neighbours
        AssignNeighbours(pos.x, pos.y);
        foreach (Tile t in tile.neighbours) {
            AssignNeighbours(t.pos.x, t.pos.y);
        }

        return tile;
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying) {
            //DrawWorld();
            //DrawHouses();
        }
    }

    void DrawHouses() {
        if (houses == null) {
            return;
        }

        foreach (House h in houses) {
            h.Draw();
        }
    }

    void DrawWorld() {
        if (tiles == null) {
            return;
        }

        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                Gizmos.color = tiles[i, j].GetColor();
                Gizmos.DrawCube(new Vector3(i, j, 0) * renderScale, Vector3.one * renderScale);
            }
        }
    }


    public Vector3 MapToWorld(Vector2Int pos) {
        return new Vector3(pos.x, pos.y, 0) * renderScale;
    }
    public Vector2Int WorldToMap(Vector3 world) {
        return new Vector2Int((int)(world.x / renderScale), (int)(world.y / renderScale));
    }


    public Tile FindTileOfTypeInRadius(Vector2Int pos, int radius, System.Type type) {
        List<Tile> tiles = new List<Tile>();
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                Vector2Int p = pos + new Vector2Int(x, y);
                Tile t = GetTile(p);
                if (t != null && t.GetType() == type) {
                    tiles.Add(t);
                }
            }
        }

        if (tiles.Count > 0) {
            return tiles[Random.Range(0, tiles.Count)];
        }
        return null;

    }

    public List<Tile> FindTilesOfTypeInRadius(Vector2Int pos, int radius, System.Type type) {
        List<Tile> tiles = new List<Tile>();
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                Vector2Int p = pos + new Vector2Int(x, y);
                Tile t = GetTile(p);
                if (t != null && t.GetType() == type) {
                    tiles.Add(t);
                }
            }
        }

        return tiles;

    }

    public Bounds WorldBounds() {
        return new Bounds(Vector3.zero, new Vector3(size.x, size.y, 0) * renderScale);
    }

}
