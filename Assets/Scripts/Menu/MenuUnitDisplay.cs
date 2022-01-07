using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//used in TeamDisplayManager to display any group
//displays an individual group rather than player groups
//used to display selectable leaders
//moves around, not in a fixed position
public class MenuUnitDisplay : GroupEditor {


    public RectTransform teamDisplayParent;

    public Image frameDisplayTemplate;
    public Image unitDisplayTemplate;


    public UnitList presetGroup;
    internal UnitList listToEdit;

    
    private List<Image> displayedUnits, displayedFrames;

    public bool storeUnit; //used if units should be buttons or not (if you need to access data, should be true)

    public bool showLeader;
    public RectTransform leadUnitParent; //should be the same size as parent of teamDisplayParent
    public Image leadUnitTemplate; //can be null
    public Vector2 leadOffset = new Vector2(0,80);

    //doesn't get called at start? needs to be called when used in TEamDisplayManager
    public void Awake() {
        teamDisplayParent.gameObject.SetActive(false);
        unitDisplayTemplate.gameObject.SetActive(false);
        frameDisplayTemplate.gameObject.SetActive(false);
        if (leadUnitTemplate != null)
            leadUnitTemplate.gameObject.SetActive(false);
    }

    //temp, swaps leaders and removes display
    public void LeadUnitSwap(LeadUnitData newUnit) {
        listToEdit.SetLeadUnit(newUnit);
        ClearTeamDisplay();
    }

    //used only when leadUnitSwap needed
    public void SetUnitList(int i) {
        listToEdit = playerGroups[i];
    }

    //called by leadUnitButton
    public void DisplayGroup(Vector2 coords) {
        DisplayGroup(presetGroup, coords);
    }

    //called in teamDisplayManager
    public void DisplayGroup(UnitList group, Vector2 coords) {
        //clears display and makes new List (this display may be a clone)
        ClearTeamDisplay();
        displayedFrames = new List<Image>();
        displayedUnits = new List<Image>();

        
        //sets position
        List<UnitData> units = group.units;
        Vector2 offset = new Vector2(0, -3); //where to display actual team in realation to lead unit
        Vector2 newCoords = coords + offset;
        teamDisplayParent.GetComponent<RectTransform>().anchoredPosition = newCoords;

        if (units != null) {
            foreach(UnitData unit in group.units) {
                DisplayUnitSprite(unit, unitDisplayTemplate);
            }
        }

        //lead unit
        if (showLeader && leadUnitTemplate != null && group.hasLeadUnit) {
            LeadUnitButton unitButton = leadUnitTemplate.GetComponent<LeadUnitButton>();
            unitButton.playerGroup = group.playerGroup; //index always resets to 0
            DisplayLeadUnit(group.leadUnit, newCoords + leadOffset);
        }

        teamDisplayParent.gameObject.SetActive(true);
    }


    //displays single unit by instantiating templates and adding them to the grid
    private void DisplayUnitSprite(UnitData unitData, Image template) {
        if (unitData == null) {
            Debug.Log("invalid unit in DisplayUnitSprite");
            return;
        }
        Sprite unitSprite = unitData.initSprite;
        if (unitSprite != null) {
            Image unitFrame = Instantiate(frameDisplayTemplate, teamDisplayParent);
            unitFrame.sprite = unitData.frame;
            unitFrame.gameObject.SetActive(true);
            if (displayedFrames != null)
                displayedFrames.Add(unitFrame);

            Image unitImage = Instantiate(template, unitFrame.GetComponent<RectTransform>());
            unitImage.sprite = unitSprite;
            unitImage.gameObject.SetActive(true);
            if (displayedUnits != null)
                displayedUnits.Add(unitImage);
            if (storeUnit) {
                UnitButton unitB = unitImage.GetComponent<UnitButton>();
                unitB.unit = unitData;
            }
        } 
    }

    //displays lead unit, not really necessary since it does the same thing as above
    //but is slightly different since above adds it to a unity grid system rather than just placing it, so the position of the frame needs to be set by this function
    private void DisplayLeadUnit(UnitData unitData, Vector2 frameCoords) {
        if (unitData == null) {
            Debug.Log("invalid unit in DisplayUnitSprite");
            return;
        }

        Sprite unitSprite = unitData.initSprite;
        if (unitSprite != null) {
            Image unitFrame = Instantiate(frameDisplayTemplate, leadUnitParent);
            unitFrame.GetComponent<RectTransform>().anchoredPosition = frameCoords;
            unitFrame.sprite = unitData.frame;
            unitFrame.gameObject.SetActive(true);
            if (displayedFrames != null)
                displayedFrames.Add(unitFrame);

            Image unitImage = Instantiate(leadUnitTemplate, unitFrame.GetComponent<RectTransform>());
            unitImage.sprite = unitSprite;
            unitImage.gameObject.SetActive(true);
            if (displayedUnits != null)
                displayedUnits.Add(unitImage);
            if (storeUnit) {
                UnitButton unitB = unitImage.GetComponent<UnitButton>();
                unitB.unit = unitData;
            }
        } 
        
    }

    //clears units within dispaly, doesn't destroy display
    public void ClearTeamDisplay() {

        if (displayedUnits != null) {
            foreach(Image image in displayedFrames) {
                Object.Destroy(image.gameObject);
            }
            displayedFrames.Clear();

            foreach(Image image in displayedUnits) {
                Object.Destroy(image.gameObject);
            }
            displayedUnits.Clear();
        }

        teamDisplayParent.gameObject.SetActive(false);
    }

    //destroys object and clears team display
    public void Remove() {
        Destroy(gameObject);
    }
}
