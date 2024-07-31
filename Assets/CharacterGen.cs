using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGen : MonoBehaviour {

    public GameObject farmerPrefab;
    public GameObject builderPrefab;

    public List<Sprite> hats;
    public List<Sprite> faces;
    public List<Sprite> bodies;

    public CharacterPreview preview;
    //public Sprite
    public static CharacterGen instance;


    // Start is called before the first frame update
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {



    }

    public Sprite GetHat(CharacterPreview.Job job) {
        if (job == CharacterPreview.Job.Farmer) {
            return hats[2];
        } else {
            return hats[0];
        }
    }
    public Sprite GetRandomFace() {
        return faces[Random.Range(0, faces.Count)];
    }

    public Sprite GetRandomBody() {
        return bodies[Random.Range(0, bodies.Count)];
    }

    public Human Generate(string job, Vector2Int pos) {
        GameObject g = null;
        if (job == "Farmer") {
            g = Instantiate(farmerPrefab);

        } else if (job == "Builder") {
            g = Instantiate(builderPrefab);
        }



        Human h = g.GetComponent<Human>();
        h.pos = pos;

        h.hatRenderer.sprite = preview.hatRenderer.sprite;
        h.faceRenderer.sprite = preview.faceRenderer.sprite;
        h.bodyRenderer.sprite = preview.bodyRenderer.sprite;

        h.BeBorn(preview.nameInput.text, preview.hatRenderer.sprite, preview.faceRenderer.sprite, preview.bodyRenderer.sprite);
        return h;


    }

}
