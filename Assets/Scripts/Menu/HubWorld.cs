using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class HubWorld : MonoBehaviour
{
    public GameObject hub;
    public GameObject map;
    public List<UnitList> playerGroups;
    public List<GroupEditor> groupEditors;
    public List<UnitInfoDisplay> displays;
    public AudioManager bgm;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (GroupEditor e in groupEditors) {
            e.SetGroups(playerGroups);
        }
        foreach (UnitInfoDisplay d in displays) {
            d.Init();
        }
    }

    void Start() {
        bgm.Play("hub_bgm");
    }

    public void ToMap() {
        hub.SetActive(false);
        map.SetActive(true);
        bgm.Stop();
        bgm.Play("map_bgm");
    }

    public void ToHub() {
        map.SetActive(false);
        hub.SetActive(true);
        bgm.Stop();
        bgm.Play("hub_bgm");
    }
}
