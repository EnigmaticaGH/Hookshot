using UnityEngine;
using System;
using System.Collections;

public class PrefabProperties : MonoBehaviour
{
    [SerializeField]
    private int sectionDifficulty;

    public int Difficulty
    {
        get
        {
            return sectionDifficulty;
        }
    }
}
