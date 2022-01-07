using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMenuManager : MonoBehaviour
{
    public MapManager manager;
    
    public GameObject returnButton;
    // Start is called before the first frame update
    void Awake() {
        returnButton.SetActive(false);
    }

    public void Display(){
        gameObject.SetActive(true);
    }

    public void Return() {
        gameObject.SetActive(true);
        returnButton.SetActive(false);
    }

    public void ViewMapDisplay() {
        gameObject.SetActive(false);
        returnButton.SetActive(true);
    }

    public void StopDisplay() {
        gameObject.SetActive(false);
    }

}
