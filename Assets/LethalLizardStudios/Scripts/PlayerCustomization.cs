using UnityEngine;

public class PlayerCustomization : MonoBehaviour
{
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color color;

    [SerializeField] private Material[] materials;

    void Start()
    {
        color.a = 220.0f / 255.0f;

        foreach (Material material in materials)
        {
            material.SetColor("_BaseColor", color);
        }
    }
}
