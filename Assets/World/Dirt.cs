using UnityEngine;

public class Dirt : Tile {
    public float health;
    public float nextHealth;

    static int maxHealth = 10;
    /*states
        *dirt
        *seeded grass
        *grass
        *dead grass
    */
    int surroundingDirt = 0;
    float dirtHealth = 0;

    Tile neighbour;

    int immunity;

    public bool foundation = false;
    public Dirt(Vector2Int pos) {
        this.pos = pos;
        health = Random.Range(maxHealth * 0.25f, maxHealth * 0.75f);
        nextHealth = health;
    }

    public override void Tick() {

        if (foundation) {
            nextHealth = 0;
        } else {
            UpdateHealthByEnvirons();
        }

        if (immunity <= 0) {
            nextHealth -= Random.Range(0.6f, 2f);
        }

        nextHealth = Mathf.Clamp(nextHealth, 0, maxHealth);
        immunity = Mathf.Clamp(immunity - 1, 0, 100);
        //if there is water next to me, health goes up

        //if none of my neighbours are health, health goes down
    }

    public override void Tock() {
        health = Mathf.Clamp(nextHealth, 0, maxHealth);
    }

    void UpdateHealthByEnvirons() {
        dirtHealth = 0;
        surroundingDirt = 0;

        for (int i = 0; i < neighbours.Count; i++) {
            neighbour = neighbours[i];

            if (neighbour is Water) {
                nextHealth += MapGen.instance.waterNutrientTransfer;

            } else if (neighbour is Wood) {
                nextHealth += MapGen.instance.waterNutrientTransfer;

            } else {
                if (neighbour is Dirt d) {
                    surroundingDirt++;
                    dirtHealth += (d.health + d.immunity) / maxHealth;
                }

            }
        }

        if (surroundingDirt != 0) {
            dirtHealth /= surroundingDirt;
            nextHealth += dirtHealth * MapGen.instance.nutrientTransfer;
        }

    }


    public void Plant() {
        health = maxHealth;
        immunity = 100;

    }

    public bool IsUnseeded() {
        return immunity <= 0 && health <= maxHealth * 0.1f && !foundation;
    }



    public override Color GetColor() {
        float healthStepped = Mathf.Floor(health * 3) / 3;
        float healthColour = Map(healthStepped, 0, maxHealth, 0.15f, 0.45f);

        Color grassColour = new Color(0.2f, healthColour, 0);
        Color seedColour = new Color(0, 1, 0);
        Color foundationColour = new Color(0.2f, 0.2f, 0.2f);
        //brown
        Color myCol = immunity > 0 ? seedColour : grassColour;
        if (foundation) {
            myCol = foundationColour;
        }
        return myCol;
    }

    float Map(float value, float start1, float stop1, float start2, float stop2) {
        return (value - start1) / (stop1 - start1) * (stop2 - start2) + start2;
    }

    public override int GetSpriteIndex() {
        return Mathf.RoundToInt(Map(health, 0, maxHealth, 0, 7)) + dirtSpriteOffset;
    }

}
