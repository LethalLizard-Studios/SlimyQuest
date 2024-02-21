using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textures : MonoBehaviour
{
    private static Textures _instance;
    public static Textures Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<Textures>();
            return _instance;
        }
    }

    public Texture2D[] All;
    public Texture2D[] Top;
    public Texture2D[] Icon;
}
