using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultAI", menuName = "AIType/DefaultWait")]

public class DefaultWaitAI : AIType {

    public AttackManager attackManager;
    public PathManager pathfinding;
    public FullGridPathManager outOfRangePathfinding;

    public override IEnumerator MakeBestMove(Unit unit) {
        unit.SetValidMoves();
        //foreach (GameObject inRange in unit.inRangeEnemies) {
       //     Debug.Log(inRange.transform.position);
       // }
        Unit bestTarget = null;
        if (unit.inRangeEnemies.Any()) {
            bestTarget = BestTarget(unit, unit.inRangeEnemies);
            if (bestTarget != null) {
                MoveUnit(unit, bestTarget.gridPos);
                while (unit.isMoving) {
                    yield return null; 
                }
                yield return unit.StartCoroutine(attackManager.DoAttacks(unit, bestTarget));
            }
        }
        unit.ClearValidMoves();
    }

    //finds path, moves unit
    public void MoveUnit(Unit unit, Vector2 endPos) {
        pathfinding.ClearPath();
        pathfinding.FindPath(unit.gridPos, endPos);
        unit.StartCoroutine(unit.Move(pathfinding.foundPath));
    }


    public Unit BestTarget(Unit unit, IEnumerable<GameObject> targetList) {
        Unit bestTarget = null;
        int bestResult = -1; //won't work if attack results in enemy healing
        foreach (GameObject gridObject in targetList) {
            Unit target = gridObject.GetComponent<Unit>();
            if (target != null && target.isAlive) { //eeeeeeeeeeeeeeeee
                int atkResult = AttackResult(unit, target);
                if (atkResult > bestResult) {
                    bestResult = atkResult;
                    bestTarget = target;
                }
            }
        }
        return bestTarget;
    }

    //chooses best on attack based on damage done, where killing blows have highest priorty.
    public int AttackResult(Unit attacker, Unit defender) {
        int dmgDone = attackManager.CalcDamage(attacker, defender, false).Item2;
        if (attacker.CheckFollowUp(defender)) {
            dmgDone += dmgDone;
        }
        int defenderResultHP = defender.data.hp - dmgDone;
        if (defenderResultHP <= 0)
            return dmgDone + defender.data.maxHp;
        else
            return dmgDone;
    }
}
