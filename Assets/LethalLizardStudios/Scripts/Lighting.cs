using System.Collections.Generic;
using UnityEngine;

public static class Lighting
{
    public static bool IsLightingGroup(VoxelBuffer data, Vector3 startPos, ref List<Vector3> lightingGroupList, ref List<Vector3> shadowMap)
    {
        for (int i = 0; i < groupChecks.Length; i++)
        {
            Vector3 checkBlockPos = groupChecks[i] + startPos;
            if (UpdateShadows(data, checkBlockPos, ref shadowMap) != 1.0f)
                return false;
        }
        lightingGroupList.Add(groupChecks[0] + startPos);
        lightingGroupList.Add(groupChecks[1] + startPos);
        lightingGroupList.Add(groupChecks[2] + startPos);
        return true;
    }

    public static float UpdateShadows(VoxelBuffer data, Vector3 pos, ref List<Vector3> shadowMap)
    {
        if (pos.z == 2)
            return 0.5f;

        if (pos.z == 3)
            return 0.65f;

        if (shadowMap.Contains(pos))
            return 1f;

        float shadowStrength = 1f;

        for (int i = 0; i < shadowChecks.Length; i++)
        {
            Vector3 checkBlockPos = shadowChecks[i] + pos;
            if (checkBlockPos.x > 0 && checkBlockPos.x <= 32 && checkBlockPos.y > 0)
            {
                Voxel checkBlock = data[checkBlockPos];
                if (!checkBlock.isSolid || checkBlock.ID == 0)
                {
                    if (i < 4)
                        return 0;
                    shadowStrength -= 0.05f;
                }
            }
        }

        return Mathf.Clamp(shadowStrength, 0f, 1f);
    }

    static readonly Vector3[] groupChecks = new Vector3[3]
    {
            new Vector3(1,1,0),//diag
            new Vector3(1,0,0),//right
            new Vector3(0,1,0),//top
    };

    static readonly Vector3[] shadowChecks = new Vector3[24]
    {
            new Vector3(-1,0,0),//left
            new Vector3(1,0,0),//right
            new Vector3(0,-1,0),//bottom
            new Vector3(0,1,0),//top
            new Vector3(-1,1,0),//diag
            new Vector3(1,1,0),
            new Vector3(-1,-1,0),
            new Vector3(1,-1,0),
            new Vector3(-2,0,0),//left
            new Vector3(2,0,0),//right
            new Vector3(0,-2,0),//bottom
            new Vector3(0,2,0),//top
            new Vector3(-3,0,0),//left
            new Vector3(3,0,0),//right
            new Vector3(0,-3,0),//bottom
            new Vector3(0,3,0),//top
            new Vector3(-2,1,0),//left
            new Vector3(2,1,0),//right
            new Vector3(1,-2,0),//bottom
            new Vector3(1,2,0),//top
            new Vector3(-2,-1,0),//left
            new Vector3(2,-1,0),//right
            new Vector3(-1,-2,0),//bottom
            new Vector3(-1,2,0),//top
    };
}
