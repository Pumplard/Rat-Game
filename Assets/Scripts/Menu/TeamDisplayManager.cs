using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//used in shop and barracks, displays all player groups
//displays group text and lead unit, but uses MenuUnitDisplay for actual group
public class TeamDisplayManager : GroupEditor {

    public Transform unitDisplayParent;
    public Transform textParent;

    public MenuUnitDisplay unitDisplayTemplate;
    public TMP_Text groupText;
    private List<MenuUnitDisplay> curDisplays;
    private List<int> displayedGroups;

    int xDiff = 100; //spacing between each team

void Awake() {
        unitDisplayTemplate.gameObject.SetActive(false);
        unitDisplayTemplate.Awake();

        groupText.gameObject.SetActive(false);

        //UnitDisplayParent must be active        
        curDisplays = new List<MenuUnitDisplay>();
        displayedGroups = new List<int>();

    }

    void OnEnable() {
        DisplayTeam();
    }

    //called in shop, barracks, and leaderdropdwon
    public void DisplayTeam() {
        DisplayTeam(playerGroups);
    }

    //redraw function if shop has too many
    //displays initial player team
    //needs to be cleared first
    public void DisplayTeam(List<UnitList> groups) {
        ClearTeamDisplay();

        Vector2 start = unitDisplayTemplate.GetComponent<RectTransform>().anchoredPosition;


        Vector2 curDiff = Vector2.zero;
        int i = 0;//used for assigning playergroup to leadUnit templates
        foreach(UnitList group in groups) {
            DisplayGroup(group, start + curDiff);
            //instantiates leadertemplate, sets its playergroup
            if (!displayedGroups.Contains(group.playerGroup)) {
                DisplayGroupText(curDiff, group.playerGroup);
                displayedGroups.Add(group.playerGroup);
            }

            i++;
            curDiff.x += xDiff;
        }
    }

    private void DisplayGroup(UnitList group, Vector2 coords) {
        MenuUnitDisplay curDisplay = Instantiate(unitDisplayTemplate, unitDisplayParent);
        curDisplay.DisplayGroup(group, coords);
        curDisplays.Add(curDisplay);
        curDisplay.gameObject.SetActive(true);
    }

    private void DisplayGroupText(Vector2 offset, int groupNumber) {
        TMP_Text curGroupText = Instantiate(groupText, textParent);
        RectTransform groupTransform = curGroupText.GetComponent<RectTransform>();
        groupTransform.anchoredPosition = groupTransform.anchoredPosition + offset;
        curGroupText.text += (" " + (groupNumber+1));
        curGroupText.gameObject.SetActive(true);
    }

    //destroy images, clear displayed units, clear swap
    public void ClearTeamDisplay() {
        if (curDisplays == null)
            return;

        foreach(MenuUnitDisplay unitDisplay in curDisplays) {
            unitDisplay.Remove();
        }
        curDisplays.Clear();
    }

}
