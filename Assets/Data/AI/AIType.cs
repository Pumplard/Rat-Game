using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class AIType : ScriptableObject {
    
    public abstract IEnumerator MakeBestMove(Unit unit);
}
