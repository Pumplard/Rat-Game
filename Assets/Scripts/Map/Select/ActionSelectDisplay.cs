using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//displays actionselect
//called in actionselect
public class ActionSelectDisplay : MonoBehaviour
{
    public RectTransform parent;
    public RectTransform template;
    public TMP_Text textTemp;

    public Color lowlight, normal;
    private List<GameObject> displayedTemplates;

    private Image highlightedImage;

    private Vector2 startPos;
    private float offset;

    //called in mapmanager
    public void Init() { 
        gameObject.SetActive(false);
        template.gameObject.SetActive(false);
        displayedTemplates = new List<GameObject>();
        startPos = template.anchoredPosition;
        offset = template.rect.height*template.localScale.y;

    }

    public void DisplayActions(CircularList<Action> actions){
        Vector2 pos = new Vector2(startPos.x, startPos.y);
        gameObject.SetActive(true);
        for (int i = 0; i < actions.Count; i++) { 
            DisplayAction(actions[i].ToString(), pos);
            pos.y -= offset;
        }
    }

    private void DisplayAction(string action, Vector2 pos) {
        RectTransform newDisplay = Instantiate(template, parent);
        newDisplay.anchoredPosition = pos;
        Transform childText = newDisplay.GetChild(0);
        childText.GetComponent<TMP_Text>().text = action;
        newDisplay.gameObject.SetActive(true);
        displayedTemplates.Add(newDisplay.gameObject);
    }

    public void HighlightAction(int i) {
        ClearHighlight();
        highlightedImage = displayedTemplates[i].GetComponent<Image>();
        highlightedImage.color = lowlight;
    }

    private void ClearHighlight() {
        if (highlightedImage != null) {
            highlightedImage.color = normal;
        }
    }

    public void Clear() {
        foreach (GameObject display in displayedTemplates)  {
            Destroy(display);
        }
        displayedTemplates.Clear();
        highlightedImage = null;
        gameObject.SetActive(false);
    }

}

