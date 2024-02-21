using UnityEngine;

public static class DwarfFortress
{
    static int wallID;
    static int floorID;
    static int pillarID;
    static int treasureID;

    static int floorSize;
    static int floorCount;

    public static void SetParameters(int wall, int floor, int pillar, int treasure, int floorHeight, int floorAmount)
    {
        wallID = wall;
        floorID = floor;
        pillarID = pillar;
        treasureID = treasure;
        floorSize = floorHeight;
        floorCount = floorAmount;
    }


    public static void Create(ref VoxelBuffer data, Chunk chunk)
    {
        int width = 6;

        ChunkData chunkData = WorldGlobal.Instance.GetChunkAt(chunk.position);

        Vector3 startPos = new Vector3(11 + ((ComputeManager.Instance.seedOffset.x + chunk.transform.position.x) % 12),
            20 + ((ComputeManager.Instance.seedOffset.y + chunk.transform.position.x) % 47), 1);

        startPos = new Vector3(Mathf.Clamp(startPos.x, Mathf.RoundToInt(width / 1.5f), 32 - Mathf.RoundToInt(width / 1.5f)),
            Mathf.Clamp(startPos.y, 1, 50), 1);

        for (int x = -width; x <= width; x++) {
            for (int floor = 1; floor <= floorCount; floor++) {
                for (int y = floorSize * (floor - 1); y < floorSize * floor; y++)
                {
                    Vector3 offset = new Vector3(x, y, 0);
                    Vector3 offsetBG = new Vector3(x, y, 1);

                    Vector2 offsetNoise = new Vector2(startPos.x + x + chunk.transform.position.x + 0.1f, startPos.y + y + chunk.transform.position.y + 0.1f);

                    if (x < width && x > -width && y < floorSize * floorCount)
                    {
                        if (y == 0) //Bottom Floor
                        {
                            if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                                data[startPos + offset] = new Voxel { ID = floorID };
                            if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                                data[startPos + offsetBG] = new Voxel { ID = floorID };
                        }
                        else if (y % floorSize == 0)
                        {
                            if (x < width - Mathf.RoundToInt(width / 2) - 1 && x > -width + Mathf.RoundToInt(width / 2) + 1)
                            {
                                if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                                    data[startPos + offset] = new Voxel { ID = 0 };
                                if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                                    data[startPos + offsetBG] = new Voxel { ID = floorID };
                            }
                            else
                            {
                                if (floor % 2 == 0)
                                {
                                    if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                                        data[startPos + offset] = new Voxel { ID = wallID };
                                    if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                                        data[startPos + offsetBG] = new Voxel { ID = wallID };
                                }
                                else
                                {
                                    if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                                        data[startPos + offset] = new Voxel { ID = floorID };
                                    if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                                        data[startPos + offsetBG] = new Voxel { ID = floorID };
                                }
                            }
                        }
                        else
                        {
                            if (floor % 2 == 0) //Hallway Floors
                            {
                                if (x < width - Mathf.RoundToInt(width / 2) && x > -width + Mathf.RoundToInt(width / 2))
                                {
                                    if (x < width - Mathf.RoundToInt(width / 2) - 1 && x > -width + Mathf.RoundToInt(width / 2) + 1)
                                    {
                                        if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                                            data[startPos + offset] = new Voxel { ID = 0 };
                                    }
                                    else
                                    {
                                        if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                                            data[startPos + offset] = new Voxel { ID = wallID };
                                    }

                                    if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                                        data[startPos + offsetBG] = new Voxel { ID = Blocks.BlackrockChisel.ID };
                                }
                            }
                            else
                            {
                                if (!chunkData.changedBlocks.ContainsKey(startPos + offset)) //Treasure at Bottom
                                {
                                    if (y == 1 || y == 2)
                                    {
                                        float noise = Mathf.PerlinNoise(offsetNoise.x * 10f, offsetNoise.y) * 100f;
                                        if (noise > 50 || y == 1)
                                            data[startPos + offset] = new Voxel { ID = treasureID };
                                        else
                                            data[startPos + offset] = new Voxel { ID = 0 };
                                    }
                                    else
                                        data[startPos + offset] = new Voxel { ID = 0 };
                                }

                                if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG)) // Main Back Walls
                                {
                                    float noise = Mathf.PerlinNoise(offsetNoise.x * 10f, offsetNoise.y) * 100f;
                                    if (floor == floorCount && noise > 50 && y > (floorSize * floor) - 4) //Broken Top
                                    {
    
                                    }
                                    else if (floor < floorCount && x >= Mathf.RoundToInt(width/2) - 1 && x <= Mathf.RoundToInt(width / 2) + 1
                                        && (y - floorSize * (floor - 1)) >= 2 && (y - floorSize * (floor - 1)) <= floorSize - 2)
                                    {
                                        if (x == Mathf.RoundToInt(width / 2) && (y - floorSize * (floor - 1)) >= 3 && (y - floorSize * (floor - 1)) <= floorSize - 3)
                                            data[startPos + offsetBG] = new Voxel { ID = Blocks.IronBars.ID };
                                        else
                                            data[startPos + offsetBG] = new Voxel { ID = Blocks.Blackrock.ID };
                                    }
                                    else
                                    {
                                        if (x % 3 == 0)
                                            data[startPos + offsetBG] = new Voxel { ID = pillarID };
                                        else
                                            data[startPos + offsetBG] = new Voxel { ID = wallID };
                                    }
                                }
                            }
                        }
                    }
                    else if (floor % 2 != 0 || (y - floorSize * (floor - 1)) == 0) //Side Walls of Rooms
                    {
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offset))
                            data[startPos + offset] = new Voxel { ID = wallID };
                        if (!chunkData.changedBlocks.ContainsKey(startPos + offsetBG))
                            data[startPos + offsetBG] = new Voxel { ID = wallID };
                    }
                }
            }
        }
    }
}
