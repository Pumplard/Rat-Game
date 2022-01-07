using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;

//class handled mostly by leadunitbutton
public class LeaderDropDown : GroupEditor {

    public UnitList curList;
    public List<LeadUnitData> options;
    public TMPro.TMP_Dropdown dropDown;

    internal RectTransform rTransform;

    void Awake() {
        rTransform = GetComponent<RectTransform>();
    }
    public void OpenDropDown(Vector2 coords) {
        gameObject.SetActive(true);
        Vector2 offset = new Vector2(0, -1);
        rTransform.SetPositionAndRotation(coords + offset, Quaternion.identity);
        //EventSystem.current.SetSelectedGameObject(gameObject);
    }

    //sets unitlist based on index
    public void SetUnitList(int i) {
        curList = playerGroups[i];
    }
    public void AddOption(LeadUnitData newUnit) {
        //DropDownMenuItem menuitem = new DropDownMenuItem();
        //dropDown.MenuItems.Add();
    }

    public void OnSelect() {
        curList.SetLeadUnit(options[dropDown.value]);
    }

    //called whenever mouse clicked outside, set as selected object in event system by leadunitbutten
    //public void OnDeselect(BaseEventData eventData){
     //  CloseSelf();
    //}
}

