using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SkinDisplayScript : MonoBehaviour
{
    public Sprite[] P1Skins;
    public Sprite[] P2Skins;
    public Sprite[] P3Skins;
    public Sprite[] P4Skins;

    GameObject[] skinDisplay;
    GameObject[] players;

    void Start()
    {
        players = GameManager.Instance.players;

        skinDisplay = GameObject.FindGameObjectsWithTag("SkinDisplay");
        //orderedSkinDisplay = skinDisplay.OrderBy(go => go.name);
        Array.Sort(skinDisplay, (a, b) => a.name.CompareTo(b.name));

        //Start scene skin desplay
        for (int i = 0; i< GameManager.Instance.players.Length; i++)
        {
            switch (i)
            {
                case 0:
                    skinDisplay[0].GetComponent<SpriteRenderer>().sprite = P1Skins[0];
                    break;
                case 1:
                    skinDisplay[1].GetComponent<SpriteRenderer>().sprite = P2Skins[0];
                    break;
                case 2:
                    skinDisplay[2].GetComponent<SpriteRenderer>().sprite = P3Skins[0];
                    break;
                case 3:
                    skinDisplay[3].GetComponent<SpriteRenderer>().sprite = P4Skins[0];
                    break;
            }
        }
        
    }
}
