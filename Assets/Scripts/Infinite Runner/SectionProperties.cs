using UnityEngine;
using System;
using System.Collections;

public class SectionProperties : MonoBehaviour
{
    [SerializeField]
    private int difficulty;

    public int Difficulty
    {
        get
        {
            return difficulty;
        }
    }
}
