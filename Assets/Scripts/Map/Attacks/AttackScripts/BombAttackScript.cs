using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombAttackScript : AttackScript {

    public Sprite atkSprite;

    //called in AttackDisplayManager, takes in template and in radius, displays tiles affected by explosion
    //returns list of anything displayed so it can be removed
    //returned list contains instantiated tiles from displayPos
    //can assume this is on top, since it is displayed after path
    public override List<Transform> MapEffect(Transform template, Unit unit) {
        int radius = unit.data.weapon.radius;
        HashSet<Vector2> displayPos = new HashSet<Vector2>();
        Vector2 endPos = unit.grid.manager.cursor.path.Last().gridPos; //where unit will be after moving
        GetRadiusPos(displayPos, unit.grid, endPos, radius);
        //removes unit gridpos
        displayPos.Remove(endPos);
        List<Transform> returnList = new List<Transform>();
        foreach (Vector2 pos in displayPos) {
            Transform newEffect = Instantiate(template, pos, Quaternion.identity);
            newEffect.gameObject.SetActive(true);
            newEffect.GetComponent<SpriteRenderer>().sprite = atkSprite;
            returnList.Add(newEffect);
        }
        return returnList;
    }


    //function for mapeffect, gets all positions in radius
    private void GetRadiusPos(HashSet<Vector2> set, MapGrid grid, Vector2 pos, int r) {
        if (grid.IsValidPos(pos)) {
            set.Add(pos);
        }
        else return;

        r--;
        if (r > 0) {
            GetRadiusPos(set, grid, pos + Vector2.up, r);
            GetRadiusPos(set, grid, pos + Vector2.down, r);
            GetRadiusPos(set, grid, pos + Vector2.left, r);
            GetRadiusPos(set, grid, pos + Vector2.right, r);
        }
    }

    //called in AttackManager, for additional UI display when selecting target (like status effects)    
    public override void UIEffect() {}


    //called in attack manager every time attacking
    public override void AttackEffect(Unit unit, Unit target) {
        unit.TakeDamage(unit.data.maxHp);
        HashSet<Vector2> damagePos = new HashSet<Vector2>();
        GetRadiusUnitPos(damagePos, unit.grid, unit.gridPos, unit.data.weapon.radius);
        damagePos.Remove(unit.gridPos);
        MapGrid grid = unit.grid;
        MapManager manager = grid.manager;
        int dmg = unit.data.weapon.mt;
        foreach (Vector2 pos in damagePos) {
            Unit tileTarget = grid.GetObject(pos).GetComponent<Unit>();
            if (tileTarget != null && tileTarget.team != unit.team) {
                tileTarget.TakeDamage(dmg);
                if (tileTarget != target && !tileTarget.isAlive) {
                    Debug.Log("removing unit");
                    manager.RemoveUnit(manager.unitLists[tileTarget.group], tileTarget);
                }
            }
        }
    }

    //like GetRadiusPos, but only returns spaces with units, for attackEffect
    private void GetRadiusUnitPos(HashSet<Vector2> set, MapGrid grid, Vector2 pos, int r) {
        if (grid.IsValidPos(pos) && (grid.GetObject(pos) != null)) {
            set.Add(pos);
        }
        else return;

        r--;
        if (r > 0) {
            GetRadiusUnitPos(set, grid, pos + Vector2.up, r);
            GetRadiusUnitPos(set, grid, pos + Vector2.down, r);
            GetRadiusUnitPos(set, grid, pos + Vector2.left, r);
            GetRadiusUnitPos(set, grid, pos + Vector2.right, r);
        }
    }

    //returns HP of 0 in display
    public override int ResultHitEffect(int curHp) { 
        return 0; 
    }
}