using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit: GridMovable
{   

    //public int baseAtk = 0;
    //right now mr is initialized to 50 every map
    public AIType ai { get {return data.ai;}}//
    public Sprite portrait { get {return data.portrait;}}//
    public Sprite initSprite { get {return data.initSprite;}}//  

    //given during mapgen
    public Sprite groupIcon;



    //map relevant
    public bool isAlive = true;
    internal bool activePhase = false; //only set by turnmanager
    internal bool changingPhase = false; //used specifically for phase-shift morale, called in unitList OnTurnStart and OnTurnEnd
    

    //called in mapgen
    public void Initialize(MapGrid newGrid, Vector2 newPos) {
        Initialize(newGrid);
        gridPos = newPos;
        grid.Insert(gridPos, gameObject);
    }

    private void Initialize(MapGrid newGrid) {
        Initialize();
        grid = newGrid;
        response = new SelectUnit(this);
    }

    private void Initialize() {
        
        isAlive = true;
        hasActed = true;
        activePhase = false;


        if (animator != null) {
            animator.runtimeAnimatorController = data.aController;
            //can be null
        }

        GetComponent<SpriteRenderer>().sprite = initSprite;
    }

    
    public int GetHitPercent(Unit target) {
        int chance = data.hit.GetValue() - target.data.avo.GetValue();
        if (chance > 100) return 100;
        if (chance < 0) return 0; 
        return chance;
    }

    public bool CheckFollowUp(Unit target) {
        int diff = data.spd.GetValue() - target.data.spd.GetValue();
        if (diff >= 5) {
            return true;
        }
        return false;
    }

    public void SetTeam(int newTeam) {
        team = newTeam;
    }

    public void SetGroup(int newGroup, LeadUnitData lead) {
        if (lead != null) {
            groupIcon = lead.groupIcon;
        }
        group = newGroup;
    }

    public void SetPlayerGroup(int newGroup) {
        data.playerGroup = newGroup;
    }
    

    public void TakeDamage(int dmg) {
        if (dmg > 0)
            data.hp -= dmg;

        if (data.hp <= 0)
            isAlive = false;
    }
}

