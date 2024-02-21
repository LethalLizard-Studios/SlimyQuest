/* Rights Reserved to Leland TL Carter of LethalLizard Studios ©2024
-- Last Change: 11/19/2023
*/

using System.IO;
using UnityEngine;

public static class TextureReader
{
    private static string currentPack = "TexturePacks/Core";
    private static string path = Application.dataPath + $"/{currentPack}/";

    public static Texture2D FetchTexture(string referenceName, int blockIndex, int type)
    {
        if (!Directory.Exists(path))
        {
            Debug.Log("Texture Pack does not exist!");
            Application.Quit();
        }

        var rawData = File.ReadAllBytes(path + referenceName);
        Texture2D tex = new Texture2D(16, 16); // Create an empty Texture; size doesn't matter (she said)
        tex.LoadImage(rawData);

        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;

        tex.Apply(true, false);

        if (tex == null)
            Debug.LogError(path + referenceName + ", texture reference does not exist!");

        switch (type)
        {
            case 0:
                Registry.blockList[blockIndex].m_properties.m_texture = tex;
                break;
            case 1:
                Registry.blockList[blockIndex].m_properties.m_topTexture = tex;
                break;
            case 2:
                Registry.blockList[blockIndex].m_properties.m_icon = tex;
                break;
        }

        return tex;
    }
}
