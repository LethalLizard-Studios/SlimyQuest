using System.Collections.Generic;
using UnityEngine;

public static class Geode
{
    static int centreID;
    static int innerID;
    static int outerID;

    static int maxSize;

    public static void SetParameters(int centre, int inner, int outer, int size)
    {
        centreID = centre;
        innerID = inner;
        outerID = outer;
        maxSize = size;
    }

    private static float GetDistance(Vector3 startPos, Vector3 blockPos)
    {
        var R = 6371; // Radius of the earth in km
        var x = ToRadians(blockPos.x - startPos.x);
        var y = ToRadians(blockPos.y - startPos.y);
        var a =
            Mathf.Sin(x / 2) * Mathf.Sin(x / 2) +
            Mathf.Cos(ToRadians(x)) * Mathf.Cos(ToRadians(x)) *
            Mathf.Sin(y / 2) * Mathf.Sin(y / 2);

        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        var d = R * c; // Distance in km
        return d;
    }

    private static float ToRadians(float deg)
    {
        return deg * (Mathf.PI / 180);
    }

    public static void Create(ref VoxelBuffer data, Chunk chunk)
    {
        ChunkData chunkData = WorldGlobal.Instance.GetChunkAt(chunk.position);

        Vector3 startPos = new Vector3(11 + ((ComputeManager.Instance.seedOffset.x + chunk.transform.position.x) % 12),
            20 + ((ComputeManager.Instance.seedOffset.y + chunk.transform.position.x) % 47), 1);

        startPos = new Vector3(Mathf.Clamp(startPos.x, Mathf.RoundToInt(maxSize/1.5f), 32 - Mathf.RoundToInt(maxSize / 1.5f)),
            Mathf.Clamp(startPos.y, Mathf.RoundToInt(maxSize / 1.5f), 50), 1);

        for (int x = -maxSize; x <= maxSize; x++) {
            for (int y = -maxSize; y <= maxSize; y++) {

                Vector3 offset = new Vector3(x, y, 0);
                Vector3 offsetBG = new Vector3(x, y, 1);
                float dist = GetDistance(startPos, startPos + offset);

                if (dist < 15 * maxSize)
                {
                    if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                        data[startPos + offset] = new Voxel { ID = 0 };
                    if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                        data[startPos + offsetBG] = new Voxel { ID = centreID };
                }
                else if (dist < 31 * maxSize)
                {
                    if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                        data[startPos + offset] = new Voxel { ID = centreID };
                    if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                        data[startPos + offsetBG] = new Voxel { ID = centreID };
                }
                else if (dist < 49 * maxSize)
                {
                    Vector2 offsetNoise = new Vector2(startPos.x + x + chunk.transform.position.x + 0.1f, startPos.y + y + chunk.transform.position.y + 0.1f);
                    float noise = Mathf.PerlinNoise(offsetNoise.x, offsetNoise.y) * 100f;

                    if (noise > 50f && dist < 43 * maxSize)
                    {
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                            data[startPos + offset] = new Voxel { ID = centreID };
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                            data[startPos + offsetBG] = new Voxel { ID = centreID };
                    }
                    else
                    {
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                            data[startPos + offset] = new Voxel { ID = innerID };
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                            data[startPos + offsetBG] = new Voxel { ID = innerID };
                    }
                }
                else if (outerID != -1 && dist < 60 * maxSize)
                {
                    if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                        data[startPos + offset] = new Voxel { ID = outerID };
                    if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                        data[startPos + offsetBG] = new Voxel { ID = outerID };
                }
            }
        }
    }

    public static void Create(Chunk chunk, Vector3 startPos)
    {
        ChunkData chunkData = WorldGlobal.Instance.GetChunkAt(chunk.position);

        for (int x = -maxSize; x <= maxSize; x++)
        {
            for (int y = -maxSize; y <= maxSize; y++)
            {
                Vector3 offset = new Vector3(x, y, 0);
                Vector3 offsetBG = new Vector3(x, y, 1);
                float dist = GetDistance(startPos, startPos + offset);

                if (startPos.x + offset.x > 0 && startPos.x + offset.x < 33 && startPos.y + offset.y > 1)
                {
                    if (dist < 15 * maxSize)
                    {
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                            chunkData.changedBlocks.Add(startPos + offset, 0);

                        if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                            chunkData.changedBlocks.Add(startPos + offsetBG, centreID);
                    }
                    else if (dist < 31 * maxSize)
                    {
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                            chunkData.changedBlocks.Add(startPos + offset, centreID);
                        else
                            chunkData.changedBlocks[startPos + offset] = centreID;
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                            chunkData.changedBlocks.Add(startPos + offsetBG, centreID);
                        else
                            chunkData.changedBlocks[startPos + offsetBG] = centreID;
                    }
                    else if (dist < 49 * maxSize)
                    {
                        Vector2 offsetNoise = new Vector2(startPos.x + x + chunk.transform.position.x + 0.1f, startPos.y + y + chunk.transform.position.y + 0.1f);
                        float noise = Mathf.PerlinNoise(offsetNoise.x, offsetNoise.y) * 100f;

                        if (noise > 50f && dist < 43 * maxSize)
                        {
                            if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                                chunkData.changedBlocks.Add(startPos + offset, centreID);
                            else
                                chunkData.changedBlocks[startPos + offset] = centreID;
                            if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                                chunkData.changedBlocks.Add(startPos + offsetBG, centreID);
                            else
                                chunkData.changedBlocks[startPos + offsetBG] = centreID;
                        }
                        else
                        {
                            if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                                chunkData.changedBlocks.Add(startPos + offset, innerID);
                            else
                                chunkData.changedBlocks[startPos + offset] = innerID;
                            if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                                chunkData.changedBlocks.Add(startPos + offsetBG, innerID);
                            else
                                chunkData.changedBlocks[startPos + offsetBG] = innerID;
                        }
                    }
                    else if (outerID != -1 && dist < 60 * maxSize)
                    {
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                            chunkData.changedBlocks.Add(startPos + offset, outerID);
                        else
                            chunkData.changedBlocks[startPos + offset] = outerID;
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                            chunkData.changedBlocks.Add(startPos + offsetBG, outerID);
                        else
                            chunkData.changedBlocks[startPos + offsetBG] = outerID;
                    }
                }
            }
        }
    }
}
