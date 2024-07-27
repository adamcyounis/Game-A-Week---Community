using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject farmerPrefab;
    public GameObject builderPrefab;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            PlaceFarmer();
        }

        if (Input.GetMouseButtonDown(1)) {
            PlaceBuilder();
        }
    }
    public void PlaceBuilder() {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject g = Instantiate(builderPrefab, mousePos, Quaternion.identity);
        Builder a = g.GetComponent<Builder>();

        MapGen.instance.agents.Add(a);
        a.pos = MapGen.instance.WorldToMap(mousePos);

    }

    public void PlaceFarmer() {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject g = Instantiate(farmerPrefab, mousePos, Quaternion.identity);
        Farmer a = g.GetComponent<Farmer>();

        MapGen.instance.agents.Add(a);
        a.pos = MapGen.instance.WorldToMap(mousePos);
    }

}
