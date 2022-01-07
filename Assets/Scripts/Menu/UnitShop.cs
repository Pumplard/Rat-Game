using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitShop : GroupEditor {
    public List<UnitData> availableUnits;
    public Transform shopUnitParent;
    public UnitPortrait shopUnitTemplate;
    public TeamDisplayManager teamDisplay;
    public MoneyManager moneyManager;

    public UnitList curUnitList;

    void Awake() {
        shopUnitTemplate.gameObject.SetActive(false);

        //no save
        foreach(UnitList unitList in playerGroups) {
            unitList.ResetInitialUnits();
        }
        //
    }

    void Start() {
        curUnitList = playerGroups[0];
        createUnitButtons();
    }

    void OnEnable() {
        teamDisplay.DisplayTeam(playerGroups);
    }
    
    private void createUnitButtons() {

        int shopUnitCount = availableUnits.Count;
        int startX = 0;//(-95*(shopUnitCount-1)+50*(shopUnitCount-1));
        Vector2 position = new Vector2(startX, 50);
        foreach(UnitData unit in availableUnits) {
            if (unit != null) {
                createUnitButton(unit, position);
                position.x += 50;
                if (position.x >= 150) {
                    position.x = 0;
                    position.y -= 60;
                }
            }
        }
    }


    private void createUnitButton(UnitData unitData, Vector2 position) {
        //Unit unit = unitObject.GetComponent<Unit>();
        if (unitData == null) {
            Debug.Log("Invalid unit in unitShop");
            return;
        }

        UnitPortrait shopUnit = Instantiate(shopUnitTemplate, shopUnitParent);
        RectTransform unitTransform = shopUnit.GetComponent<RectTransform>();

        unitTransform.anchoredPosition = position;

        Sprite unitPortrait = unitData.portrait;
        if (unitPortrait != null) {
            shopUnit.SetPortrait(unitPortrait);
        }
        shopUnit.SetUnit(unitData);

        shopUnit.gameObject.SetActive(true);
    }
    
    //adds new unit instance to team and displays
    public void BuyUnit(UnitButton button) {
        //dollar check
        int unitPrice = button.unit.cost;
        if (unitPrice > moneyManager.curAmount) {
            Debug.Log("poor");
            return;
        }

        //available space check
        if (curUnitList.unitTotal >= curUnitList.maxUnits) {
            UnitList nextOpenGroup = NextOpenGroup();
            if (nextOpenGroup != null) 
                curUnitList = nextOpenGroup;
            else {
                Debug.Log("Max units for all groups reached");
                return;
            }
        }
       
        UnitData unit = Instantiate(button.unit);
        if (unit != null) {
            
            curUnitList.AddUnit(unit);
            if (unit.weapon != null)
                unit.EquipWeapon(unit.weapon);
        }

        moneyManager.RemoveAmount(unitPrice);
        teamDisplay.DisplayTeam(playerGroups);
    }

    public UnitList NextOpenGroup() {
        foreach(UnitList group in playerGroups) {
            if (group.unitTotal < group.maxUnits)
                return group;
        }
        return null;
    }
}
