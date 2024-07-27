using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour {

    public static MapGen instance;
    public bool reRoll;
    public int bake = 300;
    public bool tick;
    public int tickStep = 10;
    public Tile[,] tiles;

    public Vector2Int size;
    public Vector2Int seed;

    public float noiseScale;
    public float noiseScale2;

    public float renderScale = 0.16f;


    public float nutrientTransfer = 1;
    public float waterNutrientTransfer = 1.3f;
    public float treeDensity = 30;
    public List<Agent> agents = new List<Agent>();
    public List<House> houses;
    public float tickRate;
    float tickDelta;


    public float tickTime;
    // Start is called before the first frame update
    void Start() {
        Application.targetFrameRate = 60;
        Generate();
    }

    void Generate() {
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
        DrawWorld();

        DrawHouses();
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

}
