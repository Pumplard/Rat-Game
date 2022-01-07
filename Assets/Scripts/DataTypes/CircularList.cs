using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CircularList<T> : List<T> {

    private int index = 0;

    public void SetIndex(int i) {
        index = i;
    }

    public int CurrentIndex {
        //why did I make this a function
        get { return index;}
    }
    public T CurrentItem() {
        return this[index];
    }
    public T NextItem() {
        if (index < this.Count - 1) {
            index += 1;
            return this[index];
        }
        index = 0;
        return this[index];
    }

    public T PreviousItem() {
        if (index > 0) {
            index -= 1;
            return this[index];
        }
        index = this.Count -1;
        return this[index];
    }
}
