using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CharacterEntry : MonoBehaviour {
    public Human human;
    public TMP_Text nameText;
    public TMP_Text jobText;
    public TMP_Text ageText;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        ageText.text = human.age.ToString();
        //set the colour of the text based on whether the human is alive or dead
        /*
        if (human.alive) {
            ageText.color = Color.green;
        } else {
            ageText.color = Color.red;

        }
        */
    }

    public void Setup(Human h) {
        human = h;
        nameText.text = human.name;
        jobText.text = human.GetType().Name;
        ageText.text = human.age.ToString();
    }
}
