/* Rights Reserved to Leland TL Carter of LethalLizard Studios ©2024
-- Last Change: 11/1/2023
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static BlockBehaviours;

public static class Registry
{
    //List that contains all blocks
    public static List<Block> blockList = new List<Block>();

    private static int blocksLoaded = 0;

    public static bool IsDone()
    {
        return (blocksLoaded >= Blocks.Count - 1);
    }



    public static Block AddBlock(Block block)
    {
        Block.Properties properties = block.m_properties;

        blockList.Add(block);

        //MAKE SURE THIS IS AFTER ADDING
        block.ID = blockList.Count;

        //Apply Textures
        TextureReader.FetchTexture(properties.m_textureName, blocksLoaded, 0);
        TextureReader.FetchTexture(properties.m_topTextureName, blocksLoaded, 1);
        TextureReader.FetchTexture(properties.m_iconName, blocksLoaded, 2);

        blocksLoaded++;

        return block;
    }

    public static void FetchTextures()
    {
        for (int i = 0; i < blockList.Count; i++)
        {
            Block.Properties properties = blockList[i].m_properties;

            if (properties.m_texture == null)
                TextureReader.FetchTexture(properties.m_textureName, i, 0);
            if (properties.m_topTexture == null)
                TextureReader.FetchTexture(properties.m_topTextureName, i, 1);
            if (properties.m_icon == null)
                TextureReader.FetchTexture(properties.m_iconName, i, 2);
        }
    }

    public static Block AtIndex(int index)
    {
        if (index < blockList.Count)
            return blockList[index];

        Debug.LogError("No Block at Index: " + index + ", Count: "+ blockList.Count);
        return null;
    }
}
