using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoxelColor
{
    public Color color;
    public float metallic;
    public float smoothness;
}

[System.Serializable]
public struct VoxelColor32
{
    public float color;
    public float metallic;
    public float smoothness;
}