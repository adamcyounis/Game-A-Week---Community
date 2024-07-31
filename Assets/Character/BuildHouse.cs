using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
public class BuildHouse : State {
    public GameObject housePrefab;
    public GatherResource<Wood> gatherWood;
    public GatherResource<Stone> gatherStone;
    public Navigate navigate;
    public House house;
    public bool first;
    public override void Enter() {
        //Debug.Log("Building house..");
        //pick destination for house
        Vector2Int destination = FindNewFoundation();
        if (destination.x >= 0) {
            // Debug.Log("Building house at " + destination);
            GameObject houseObj = GameObject.Instantiate(housePrefab, new Vector3(destination.x, destination.y, 0), Quaternion.identity);
            house = houseObj.GetComponent<House>();
            MapGen.instance.houses.Add(house);
            house.pos = destination;
            house.SetupFoundation();
            GetNextRequirement();

        } else {
            complete = true;
            return;
        }
    }

    public override void Tick() {
        if (house == null) {
            return;
        }

        if (state.complete) {
            if (!house.built) {

                //if the agent is carrying the required resources
                if (CarryingNextResource()) {
                    DropOffResources();
                } else {
                    GetNextRequirement();
                }
            } else {
                complete = true;
            }
        }
    }

    public override void Exit() {

    }
    bool CarryingNextResource() {
        return agent.carryingTile != null && agent.carryingTile.GetType() == house.GetNextRequirement();
    }
    void DropOffResources() {
        //if the agent is at the house
        if (agent.pos == house.nextRequirementPos) {
            //drop the resource
            MapGen.instance.ReplaceTile(agent.pos, agent.carryingTile);
            agent.carryingTile.collectable = false;
            agent.carryingTile = null;
            //Debug.Log("dropping where I'm standing");

        } else {
            //navigate to the house
            navigate.target = house.nextRequirementPos;
            machine.Set(navigate, true);
            // Debug.Log("going to house");
        }

    }

    void GetNextRequirement() {

        Type requirement = house.GetNextRequirement();
        //gather the next requirement
        if (requirement == typeof(Wood)) {
            //Debug.Log("Gathering wood");
            machine.Set(gatherWood, true);
        } else if (requirement == typeof(Stone)) {
            // Debug.Log("Gathering stone");
            machine.Set(gatherStone, true);
        }
    }

    public Vector2Int FindNewFoundation() {

        agent.pos = MapGen.instance.WorldToMap(agent.transform.position);

        //check the map for the list of all houses
        List<House> houses = MapGen.instance.houses.ToList();

        if (houses.Count == 0 || first) {
            return agent.pos;
        }

        //TODO: shuffle these
        foreach (House h in houses) {
            //sweep around it in a circle with the radius of 8
            //there will be 16 possible points in the circle
            for (int i = 0; i < 16; i++) {
                //get the point on the circle

                float x = h.pos.x + Mathf.Cos(i * Mathf.PI / 8) * 8;
                float y = h.pos.y + Mathf.Sin(i * Mathf.PI / 8) * 8;
                Vector2Int point = new Vector2Int((int)x, (int)y);
                bool cleared = CheckPotentialFoundation(point);
                if (cleared) {
                    return point;
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool CheckPotentialFoundation(Vector2Int point) {
        //at every point in the loop, check if the 7x7 area is clear
        bool cleared = true;
        for (int x = -3; x < 4; x++) {
            for (int y = -3; y < 4; y++) {
                //if it is, return that point

                Tile tile = MapGen.instance.GetTile(point + new Vector2Int(x, y));
                if (!(tile is Dirt) || ((Dirt)tile).foundation) {
                    cleared = false;
                }
            }
        }
        return cleared;
    }

}