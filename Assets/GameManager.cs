using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject farmerPrefab;
    public GameObject builderPrefab;
    public CharacterGen characterGen;
    public CharacterList characterDisplay;

    [Header("Drag")]
    public BoxCollider2D dragStartBox;
    public GameObject dragger;
    public CharacterPreview dragPreview;
    bool dragging = false;

    public RectTransform ui;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        /*
        if (Input.GetMouseButtonDown(0)) {
            PlaceFarmer();
        }

        if (Input.GetMouseButtonDown(1)) {
            PlaceBuilder();
        }
        */

        UpdateZoom();
        UpdateCameraTranslate();
        UpdateDragAndDrop();
    }

    void UpdateCameraTranslate() {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        Camera.main.transform.position += move * 0.1f;


        //calculate width and height of the camera viewport in worldspace
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        //clamp camera within the map bounds
        Camera.main.transform.position = new Vector3(
            Mathf.Clamp(Camera.main.transform.position.x, horzExtent, MapGen.instance.WorldBounds().size.x - horzExtent),
            Mathf.Clamp(Camera.main.transform.position.y, vertExtent, MapGen.instance.WorldBounds().size.y - vertExtent),
            Camera.main.transform.position.z
        );
    }

    float zoomVelocity = 0;
    int targetZoom = 1;

    public float scrollTranslateVelocity = 0;
    Vector2 scrollTranslate = Vector2.zero;

    void UpdateZoom() {
        float[] zoomLevels = { 0.25f, 0.5f, 1, 2 };

        //if the plus or minus key is pressed, zoom in or out with smoothdamp
        float baseZoom = 360 / 2 / 100f;
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetAxis("Mouse ScrollWheel") > 0) {
            targetZoom -= 1;
            //   scrollTranslate = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }
        //minus or scroll up    
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetAxis("Mouse ScrollWheel") < 0) {
            targetZoom += 1;
            //set camera position to be where the mouse is
            //  scrollTranslate = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        //smoothdamp the camera position to the mouse position
        //Camera.main.transform.position = Vector2.SmoothDamp(Camera.main.transform.position, scrollTranslate, ref scrollTranslate, 0.1f);
        //Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);


        targetZoom = Mathf.Clamp(targetZoom, 0, zoomLevels.Length - 1);
        Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, baseZoom * zoomLevels[targetZoom], ref zoomVelocity, 0.1f);
    }

    void UpdateDragAndDrop() {

        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Mouse Down");
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(Input.mousePosition + " " + dragStartBox.bounds.center);
            if (dragStartBox.bounds.Contains(Input.mousePosition)) {
                dragger.SetActive(true);
                dragPreview.hatRenderer.sprite = characterGen.preview.hatRenderer.sprite;
                dragPreview.faceRenderer.sprite = characterGen.preview.faceRenderer.sprite;
                dragPreview.bodyRenderer.sprite = characterGen.preview.bodyRenderer.sprite;
                dragging = true;
            }
        }

        if (dragging) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragger.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }

        if (!Input.GetMouseButton(0) && dragging) {
            //check whether the mouse is inside the UI recttransform. if it is, return
            if (!ui.rect.Contains(ui.InverseTransformPoint(Input.mousePosition))) {
                //spawn human at mouse position
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //if builder
                if (dragPreview.job == CharacterPreview.Job.Builder) {
                    PlaceBuilder();
                } else if (dragPreview.job == CharacterPreview.Job.Farmer) {
                    PlaceFarmer();
                }
            }
            dragger.SetActive(false);
            dragging = false;

        }
    }
    public void PlaceBuilder() {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Human h = characterGen.Generate("Builder", MapGen.instance.WorldToMap(mousePos));

        h.transform.position = new Vector2(mousePos.x, mousePos.y);
        h.birthTick = MapGen.instance.tickTime;
        MapGen.instance.agents.Add(h);
        characterDisplay.AddCharacter(h);

    }

    public void PlaceFarmer() {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Human h = characterGen.Generate("Farmer", MapGen.instance.WorldToMap(mousePos));

        h.transform.position = new Vector2(mousePos.x, mousePos.y);
        h.birthTick = MapGen.instance.tickTime;
        MapGen.instance.agents.Add(h);
        characterDisplay.AddCharacter(h);

    }

}
