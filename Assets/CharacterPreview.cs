using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterPreview : MonoBehaviour {
    public Image hatRenderer;
    public Image faceRenderer;
    public Image bodyRenderer;
    public TMPro.TMP_InputField nameInput;
    public TMPro.TMP_Text jobText;
    public enum Job {
        Farmer,
        Builder
    }

    public Job job;

    // Start is called before the first frame update
    void Start() {
        job = Job.Builder;
        hatRenderer.sprite = CharacterGen.instance.GetHat(job);
        bodyRenderer.sprite = CharacterGen.instance.GetRandomBody();
        faceRenderer.sprite = CharacterGen.instance.GetRandomFace();
    }

    // Update is called once per frame
    void Update() {
        jobText.text = job.ToString();

        //recttransform used to check bounds
        RectTransform rt = GetComponent<RectTransform>();

        //if the mouse was clicked inside of the preview, randomize the sprites
        if (Input.GetMouseButtonUp(0)) {
            Vector2 mousePos = Input.mousePosition;
            if (rt.rect.Contains(rt.InverseTransformPoint(mousePos))) {
                job = (Job)Random.Range(0, 2);
                hatRenderer.sprite = CharacterGen.instance.GetHat(job);
                bodyRenderer.sprite = CharacterGen.instance.GetRandomBody();
                faceRenderer.sprite = CharacterGen.instance.GetRandomFace();
            }
        }
    }
}
