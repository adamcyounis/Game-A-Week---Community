using System;
using UnityEngine;

public class House : MonoBehaviour {
    public Vector2Int pos;
    bool occupied = false;
    public bool prebuild;
    public bool built;
    public Type[,] requirements = new Type[7, 6];
    public Vector2Int nextRequirementPos;

    public static Vector2Int tileOffset = new Vector2Int(3, 3);
    public void Awake() {
        //roof
        requirements[3, 5] = typeof(Wood);
        requirements[2, 4] = typeof(Wood);
        requirements[4, 4] = typeof(Wood);
        requirements[1, 3] = typeof(Wood);
        requirements[5, 3] = typeof(Wood);
        requirements[0, 2] = typeof(Wood);
        requirements[6, 2] = typeof(Wood);
        //walls
        requirements[1, 2] = typeof(Stone);
        requirements[5, 2] = typeof(Stone);
        requirements[1, 1] = typeof(Stone);
        requirements[5, 1] = typeof(Stone);

        //floor
        requirements[1, 0] = typeof(Stone);
        requirements[2, 0] = typeof(Stone);
        requirements[4, 0] = typeof(Stone);
        requirements[5, 0] = typeof(Stone);

    }


    public void PreBuild() {
        if (prebuild) {
            //set position based on worldpos
            pos = MapGen.instance.WorldToMap(transform.position);
            SetupFoundation();
            //replace all tiles in the requirements with new tiles
            for (int i = 0; i < 7; i++) {
                for (int j = 0; j < 6; j++) {
                    if (requirements[i, j] != null) {
                        Vector2Int p = pos - tileOffset + new Vector2Int(i, j);
                        Tile tile = (Tile)Activator.CreateInstance(requirements[i, j], new object[] { p });
                        MapGen.instance.ReplaceTile(p, tile);
                        MapGen.instance.GetTile(p).collectable = false;
                    }
                }
            }
        }
        transform.position = MapGen.instance.MapToWorld(pos);
    }

    public void SetupFoundation() {
        //make every tile in the 7x7 area foundation
        for (int x = -2; x < 3; x++) {
            for (int y = -3; y < 3; y++) {
                Vector2Int p = pos + new Vector2Int(x, y);
                Dirt tile = new Dirt(p);
                tile.foundation = true;
                MapGen.instance.ReplaceTile(p, tile);
            }
        }
        ((Dirt)MapGen.instance.GetTile(pos - tileOffset + new Vector2Int(1, 5))).foundation = false;
        ((Dirt)MapGen.instance.GetTile(pos - tileOffset + new Vector2Int(1, 4))).foundation = false;
        ((Dirt)MapGen.instance.GetTile(pos - tileOffset + new Vector2Int(2, 5))).foundation = false;
        ((Dirt)MapGen.instance.GetTile(pos - tileOffset + new Vector2Int(4, 5))).foundation = false;
        ((Dirt)MapGen.instance.GetTile(pos - tileOffset + new Vector2Int(5, 4))).foundation = false;
        ((Dirt)MapGen.instance.GetTile(pos - tileOffset + new Vector2Int(5, 5))).foundation = false;


    }

    public Type GetNextRequirement() {
        if (!built) {
            for (int i = 0; i < 7; i++) {
                for (int j = 0; j < 6; j++) {
                    if (requirements[i, j] != null) {
                        Tile next = MapGen.instance.GetTile(pos - tileOffset + new Vector2Int(i, j));
                        if (next != null && next.GetType() != requirements[i, j]) {
                            nextRequirementPos = pos - tileOffset + new Vector2Int(i, j);
                            return requirements[i, j];
                        }
                    }
                }
            }
        }
        built = true;
        return null;
    }

    public void Draw() {
        /*
        for (int i = 0; i < 16; i++) {
            //get the point on the circle

            float x = pos.x + Mathf.Cos(i * Mathf.PI / 8) * 8;
            float y = pos.y + Mathf.Sin(i * Mathf.PI / 8) * 8;
            Vector2Int point = new Vector2Int((int)x, (int)y);
            Gizmos.DrawSphere(MapGen.instance.MapToWorld(point), 0.02f);
        }
        */
    }
}
