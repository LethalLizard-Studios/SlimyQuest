using System.Collections.Generic;
using UnityEngine;

public static class Building
{
    private static ChunkData chunkData;
    private static List<Room> rooms = new List<Room>();

    private const int FLOOR_ROOF_THICKNESS = 1;
    private const int WALL_THICKNESS = 1;

    public static void Reset() { rooms.Clear(); }
    public static void CreateRoom(Room room) { rooms.Add(room); }

    public static void Build(ref VoxelBuffer data, Chunk chunk)
    {
        //Check if Rooms Exist
        if (rooms.Count < 1)
            return;

       chunkData = WorldGlobal.Instance.GetChunkAt(chunk.position);

        Vector3 startPos = new Vector3(11 + ((ComputeManager.Instance.seedOffset.x + chunk.transform.position.x) % 12),
            20 + ((ComputeManager.Instance.seedOffset.y + chunk.transform.position.x) % 47), 1);

        startPos = new Vector3(Mathf.Clamp(startPos.x, Mathf.RoundToInt(rooms[0].m_width / 1.5f),
            32 - Mathf.RoundToInt(rooms[0].m_width / 1.5f)), Mathf.Clamp(startPos.y, 1, 50), 1);

        Create(ref data, startPos);
    }

    private static void Create(ref VoxelBuffer data, Vector3 pos)
    {
        for (int floor = 1; floor <= rooms.Count; floor++)
        {
            Room room = rooms[floor - 1];

            int leftEdge = -(room.m_width + WALL_THICKNESS);
            int rightEdge = room.m_width + WALL_THICKNESS;

            int roofLine = RoomEndHeight(floor) - FLOOR_ROOF_THICKNESS;
            int floorLine = RoomStartHeight(floor) + FLOOR_ROOF_THICKNESS;

            for (int y = RoomStartHeight(floor); y < RoomEndHeight(floor); y++)
            {
                for (int x = leftEdge; x <= rightEdge; x++)
                {
                    Vector2 noiseOffset = new Vector2(pos.x + x + chunkData.position.x + 0.1f, pos.y + y + chunkData.position.x + 0.1f);

                    Vector3 adjustedPos = new Vector3(pos.x + x + room.m_xOffset, pos.y + y, pos.z);
                    Vector3 adjustedPosBG = new Vector3(pos.x + x + room.m_xOffset, pos.y + y, pos.z + 1);

                    //Side Walls
                    if (x == leftEdge || x == rightEdge)
                    {
                        if (room.m_corners) //Different Corners
                        {
                            bool isCorner = (y >= roofLine || y >= roofLine || y < floorLine || y < floorLine);

                            if (isCorner)
                            {
                                SetBlock(ref data, adjustedPos, room.m_cornerBlock);
                                SetBlock(ref data, adjustedPosBG, room.m_wallBlock);
                                continue;
                            }
                        }
                        SetBlock(ref data, adjustedPos, room.m_wallBlock);
                        SetBlock(ref data, adjustedPosBG, room.m_wallBlock);
                        continue;
                    }

                    //Destoyed Top
                    if (room.m_destoyedTopHeight != -1 && y >= roofLine - room.m_destoyedTopHeight)
                    {
                        if (Mathf.PerlinNoise(noiseOffset.x * 10f, noiseOffset.y) * 100f > 50f - (y - floorLine))
                        {
                            SetBlock(ref data, adjustedPos, null);
                            SetBlock(ref data, adjustedPosBG, room.m_destroyedBlock);
                            continue;
                        }
                    }

                    if (y >= roofLine) //Roof
                    {
                        SetBlock(ref data, adjustedPos, room.m_roofBlock);
                        SetBlock(ref data, adjustedPosBG, room.m_wallBlock);
                    }
                    else if (y < floorLine) //Floor
                    {
                        SetBlock(ref data, adjustedPos, room.m_floorBlock);
                        SetBlock(ref data, adjustedPosBG, room.m_wallBlock);
                    }
                    else //Background
                    {
                        SetBlock(ref data, adjustedPos, null);
                        SetBlock(ref data, adjustedPosBG, room.m_backWallBlock);
                    }

                    //Rubble
                    if (room.m_rubbleTopHeight != -1 && y <= room.m_rubbleTopHeight && y >= floorLine)
                    {
                        if (Mathf.PerlinNoise(noiseOffset.x * 10f, noiseOffset.y) * 100f > 35f + (y - floorLine)*5)
                        {
                            SetBlock(ref data, adjustedPos, room.m_rubbleBlock);
                        }
                    }
                }
            }
        }
    }

    private static bool SetBlock(ref VoxelBuffer data, Vector3 pos, Block block)
    {
        if (!chunkData.changedBlocks.ContainsKey(pos))
        {
            if (block != null)
                data[pos] = new Voxel { ID = block.ID };
            else
                data[pos] = new Voxel { ID = 0 };
            return true;
        }
        return false;
    }

    private static int RoomStartHeight(int floor)
    {
        int combinedHeight = 0;
        for (int i = 0; i < floor-1; i++)
        {
            combinedHeight += rooms[i].m_height + (FLOOR_ROOF_THICKNESS * 2);
        }
        return combinedHeight;
    }

    private static int RoomEndHeight(int floor)
    {
        int combinedHeight = 0;
        for (int i = 0; i < floor; i++)
        {
            combinedHeight += rooms[i].m_height + (FLOOR_ROOF_THICKNESS * 2);
        }
        return combinedHeight;
    }
}

[System.Serializable]
public class Room
{
    public int m_height;
    public int m_width;

    public int m_xOffset = 0;

    public Block m_roofBlock;
    public Block m_wallBlock;
    public Block m_backWallBlock;
    public Block m_floorBlock;

    public Room(Block block, int height, int width)
    {
        m_height = height;
        m_width = width;

        m_roofBlock = block;
        m_wallBlock = block;
        m_backWallBlock = block;
        m_floorBlock = block;
    }

    public bool m_corners = false;
    public Block m_cornerBlock;

    public void HasCornerBlock(Block block)
    {
        m_cornerBlock = block;
        m_corners = true;
    }

    public int m_destoyedTopHeight = -1;
    public Block m_destroyedBlock;

    public void HasDestroyedTop(int startHeight, Block block)
    {
        m_destroyedBlock = block;
        m_destoyedTopHeight = startHeight;
    }

    public int m_rubbleTopHeight = -1;
    public Block m_rubbleBlock;

    public void HasRubble(int startHeight, Block block)
    {
        m_rubbleBlock = block;
        m_rubbleTopHeight = startHeight;
    }
}
