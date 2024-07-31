using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterList : MonoBehaviour {
    public GameObject entryPrefab;
    public List<CharacterEntry> characters;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void AddCharacter(Human h) {
        GameObject g = Instantiate(entryPrefab, transform);
        CharacterEntry ce = g.GetComponent<CharacterEntry>();
        ce.Setup(h);
        characters.Add(ce);
    }
}
