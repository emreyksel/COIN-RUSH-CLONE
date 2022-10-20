using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] items = new GameObject[3];

    public Transform obs;
    public Transform coins;

    private void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            int itemIndex = Random.Range(0, 3);
            switch (itemIndex)
            {
                case 0:
                    for (int j = 0; j < 2; j++)
                    {
                        Instantiate(items[itemIndex], RandomPosition(i, itemIndex) + Vector3.forward*j*2 , items[itemIndex].transform.rotation,coins);
                    }                   
                    break;
                case 1:
                    Instantiate(items[itemIndex], RandomPosition(i,itemIndex), items[itemIndex].transform.rotation,obs);
                    break;
                case 2:
                    Instantiate(items[itemIndex], RandomPosition(i,itemIndex), items[itemIndex].transform.rotation,obs);
                    break;
            }
        }
    }

    private Vector3 RandomPosition(int i,int index)
    {
        switch (index)
        {
            case 0: 
                Vector3 randPos0 = new Vector3(Random.Range(-4, 4), 0.225f, 6 * (i + 1)); 
                return randPos0;
            case 1: 
                Vector3 randPos1 = new Vector3(Random.Range(-2, 2), 0, 6 * (i + 1));
                return randPos1;
            case 2: 
                Vector3 randPos2 = new Vector3(-5.5f, 1.4f, 6 * (i + 1));
                return randPos2;
            default:
                return Vector3.zero;
        }
    }
}
