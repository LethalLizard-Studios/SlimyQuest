using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public Vector3 position;

    public VoxelBuffer data;
    private MeshData meshData = new MeshData();

    private Dictionary<Vector3, int> changedBlocks = new Dictionary<Vector3, int>();
    private Dictionary<Vector3, int> toBeRemoved = new Dictionary<Vector3, int>();

    private List<Vector3> lightingGroupMember = new List<Vector3>();
    private List<Vector3> lightingGroupMemberlod2 = new List<Vector3>();
    private List<Vector3> shadowMap = new List<Vector3>();

    public Dictionary<Vector3, GameObject> chunkPrefabs = new Dictionary<Vector3, GameObject>();

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public void Initialize(Material mat, Vector3 position)
    {
        ConfigureComponents();
        data = ComputeManager.Instance.GetNoiseBuffer();
        meshRenderer.sharedMaterial = mat;
        this.position = position;

        if (WorldGlobal.Instance.InitializeChunk(this, position))
        {
            ChunkData chunkData = WorldGlobal.Instance.GetChunkAt(position);
            Dictionary<Vector3, int> blockData = ChunkSaves.Instance.Load(chunkData);

            //Load block data
            if (blockData != null)
            {
                WorldGlobal.Instance.GetChunkAt(position).changedBlocks = blockData;
            }
        }
        else
            SetChangedBlocks(WorldGlobal.Instance.GetChunkAt(position).changedBlocks);
    }

    public void SetChangedBlocks(Dictionary<Vector3, int> changedBlocks) { this.changedBlocks = changedBlocks; }

    public void ClearData()
    {
        ComputeManager.Instance.ClearAndRequeueBuffer(data);
    }

    public void RenderMesh()
    {
        meshData.ClearData();
        GenerateMesh();
        UploadMesh();
    }

    public void ToggleBlock(Vector3 pos, int id)
    {
        id--;
        Dictionary<Vector3, int> placedBlocksTemp = new Dictionary<Vector3, int>();

        Debug.Log(Registry.AtIndex(id).m_name);
        Debug.Log(Registry.AtIndex(id).m_properties.m_isTogglable);

        //Crystalarium
        if (id == Blocks.Crystalarium.ID)
        {
            float crystalChance = Mathf.PerlinNoise(ComputeManager.Instance.seedOffset.x + position.x + 0.1f, ComputeManager.Instance.seedOffset.y + position.x + 0.1f);

            if (crystalChance > 0.45f)
            {
                Geode.SetParameters(Blocks.RawAmethyst.ID, Blocks.Quartz.ID, Blocks.Celadonite.ID, 15);
                Geode.Create(this, pos);
            }
            else
            {
                Geode.SetParameters(Blocks.Citrine.ID, Blocks.Quartz.ID, Blocks.Celadonite.ID, 10);
                Geode.Create(this, pos);
            }
        }

        //Circuit
        if (id == Blocks.BasicCircuit.ID || id == Blocks.AdvancedCircuit.ID)
        {
            Dictionary<Vector3, int> containedBlocks = new Dictionary<Vector3, int>();

            for (int i = 0; i < surroundingChecks.Length; i++)
            {
                Vector3 checkBlock = surroundingChecks[i] + pos;

                if (changedBlocks.ContainsKey(checkBlock))
                {
                    int checkedID = changedBlocks[checkBlock];

                    //Check if applicable block
                    if (checkedID == Blocks.Drill.ID - 1)
                    {
                        data[checkBlock] = new Voxel() { ID = 0 };
                        toBeRemoved.Add(checkBlock, changedBlocks[checkBlock]);
                        containedBlocks.Add(surroundingChecks[i], 0);
                    }
                    else if (checkedID == Blocks.SpeedUpgrade.ID - 1)
                    {
                        data[checkBlock] = new Voxel() { ID = 0 };
                        toBeRemoved.Add(checkBlock, changedBlocks[checkBlock]);
                        containedBlocks.Add(surroundingChecks[i], 1);
                    }
                }
            }

            if (containedBlocks.Count > 0)
            {
                //Create Miner
                data[pos] = new Voxel() { ID = 0 };

                List<Vector3> blockInfo = new List<Vector3>();
                List<int> blockIDs = new List<int>();

                foreach (var key in containedBlocks.Keys)
                    blockInfo.Add(key);
                foreach (var value in containedBlocks.Values)
                    blockIDs.Add(value);

                PrefabManager.Instance.SpawnPrefab(0, new Vector3(pos.x + 0.5f + transform.position.x, pos.y + 0.5f, pos.z + 0.5f),
                    blockIDs, blockInfo);

                toBeRemoved.Add(pos, changedBlocks[pos]);
            }
        }

        foreach (var blocks in placedBlocksTemp)
        {
            changedBlocks.Add(blocks.Key, blocks.Value);
        }
    }

    public void GenerateMesh()
    {
        shadowMap.Clear();
        lightingGroupMember.Clear();
        lightingGroupMemberlod2.Clear();

        float structureChance = Mathf.PerlinNoise(ComputeManager.Instance.seedOffset.x + position.x + 0.1f, ComputeManager.Instance.seedOffset.y + position.x + 0.1f);

        if (structureChance > 0.56f)
        {
            Room treasureRoom = new Room(Blocks.BlackrockBrick, 9, 4);
            treasureRoom.m_floorBlock = Blocks.BlackrockFloor;
            treasureRoom.m_backWallBlock = Blocks.BlackrockBrick;
            treasureRoom.HasCornerBlock(Blocks.BlackrockChisel);
            treasureRoom.m_xOffset = -3;
            treasureRoom.HasRubble(2, Blocks.GoldBlock);

            Room livingRoom = new Room(Blocks.BlackrockBrick, 5, 4);
            livingRoom.m_floorBlock = Blocks.BlackrockFloor;
            livingRoom.m_backWallBlock = Blocks.Planks;
            livingRoom.HasCornerBlock(Blocks.BlackrockChisel);

            Room destroyedRoom = new Room(Blocks.BlackrockBrick, 7, 5);
            destroyedRoom.m_floorBlock = Blocks.BlackrockMossyFloor;
            destroyedRoom.m_backWallBlock = Blocks.BlackrockMossyBrick;
            destroyedRoom.HasCornerBlock(Blocks.BlackrockChisel);
            destroyedRoom.HasDestroyedTop(5, null);
            destroyedRoom.m_xOffset = -4;

            Building.Reset();
            Building.CreateRoom(treasureRoom);
            Building.CreateRoom(livingRoom);
            Building.CreateRoom(destroyedRoom);
            Building.Build(ref data, this);
        }
        else if (structureChance > 0.52f)
        {
            if (position.x % 64 == 0)
            {
                Geode.SetParameters(Blocks.RawAmethyst.ID, Blocks.Quartz.ID, Blocks.Celadonite.ID, 15);
                Geode.Create(ref data, this);
            }
            else
            {
                Geode.SetParameters(Blocks.Citrine.ID, Blocks.Quartz.ID, Blocks.Celadonite.ID, 10);
                Geode.Create(ref data, this);
            }
        }

        Vector3 blockPos;
        Voxel block;

        int counter = 0;
        Vector3[] faceVertices = new Vector3[4];
        Vector2[] faceUVs = new Vector2[4];

        Color voxelColorAlpha;

        if (changedBlocks.Count > 0)
        {
            foreach (var keys in changedBlocks.Keys)
            {
                int id = changedBlocks[keys];

                //Debug.Log(data[keys].ID);
                data[keys] = new Voxel() { ID = id };
            }
        }

        if (toBeRemoved.Count > 0)
        {
            foreach (var keys in toBeRemoved.Keys)
            {
                changedBlocks[keys] = 0;
                data[keys] = new Voxel() { ID = 0 };
            }
            toBeRemoved.Clear();
        }

        Dictionary<Vector3, GameObject> tempChunkPrefabs = new Dictionary<Vector3, GameObject>();

        for (int x = 1; x < WorldManager.WorldSettings.containerSize + 1; x++)
            for (int y = 0; y < WorldManager.WorldSettings.maxHeight; y++)
                for (int z = 1; z < 4; z++)
                {
                    blockPos = new Vector3(x, y, z);
                    block = this[blockPos];

                    //Only check on solid blocks
                    if (!block.isSolid || block.ID == 0)
                        continue;

                    voxelColorAlpha = Color.white;
                    voxelColorAlpha.a = (block.ID - 1f) / 100f;

                    float shadows = Lighting.UpdateShadows(data, blockPos, ref shadowMap);
                    voxelColorAlpha.r = shadows;
                    voxelColorAlpha.g = (shadows < 1f && z == 1) ? 0f : 1f;

                    int hasConnection = 0;
                    int connectionIndex = 0;
                    int connectionID = 0;

                    int ID = data[blockPos].ID;

                    if (ID == Blocks.CopperOre.ID || ID == Blocks.CoalOre.ID)
                    {
                        connectionID = data[surroundingChecks[2] + blockPos].ID - 1;

                        if (connectionID == -1)
                            connectionID = data[surroundingChecks[3] + blockPos].ID - 1;
                        if (connectionID == -1)
                            connectionID = Blocks.Stone.ID - 1;

                        hasConnection = 1;

                        if (ID == Blocks.CopperOre.ID)
                            connectionIndex = 4;
                        else if (ID == Blocks.CoalOre.ID)
                            connectionIndex = 5;
                    }

                    if (Registry.AtIndex(ID-1).m_properties.m_hasConnections)
                    {
                        bool[] hasConnectionAt = new bool[8];

                        for (int i = 0; i < surroundingChecks.Length; i++)
                        {
                            Vector3 checkPos = surroundingChecks[i] + blockPos;

                            if (checkPos.y < 0 || checkPos.x < 0 || checkPos.x >= 32)
                            {
                                hasConnectionAt[i] = false;
                                continue;
                            }

                            int blockID = data[checkPos].ID;
                            hasConnectionAt[i] = (blockID != ID && blockID != 0 && Registry.AtIndex(blockID - 1).m_properties.m_hasConnections);
                        }

                        /*if (hasConnectionAt[0] && hasConnectionAt[1] && hasConnectionAt[2] && hasConnectionAt[3])
                        {
                            connectionID = data[surroundingChecks[2] + blockPos].ID - 1;
                            hasConnection = 1;
                            connectionIndex = 3;
                        }*/
                        if (hasConnectionAt[3])
                        {
                            connectionID = data[surroundingChecks[3] + blockPos].ID - 1;
                            hasConnection = 1;
                            connectionIndex = 0;
                        }
                        else if (hasConnectionAt[0] && x % 2 == 0)
                        {
                            connectionID = data[surroundingChecks[0] + blockPos].ID - 1;
                            hasConnection = 1;
                            connectionIndex = 1;
                        }
                        else if (hasConnectionAt[1] && x % 2 == 0)
                        {
                            connectionID = data[surroundingChecks[1] + blockPos].ID - 1;
                            hasConnection = 1;
                            connectionIndex = 2;
                        }
                    }
                    
                    if (chunkPrefabs.Count <= 0)
                    {
                        GameObject prefab = null;
                        Vector3 baseBlock = Vector3.zero;

                        //Desert
                        /*if (z == 2 && data[blockPos].ID == Blocks.Sand.ID)
                        {
                            if (data[blockPos + new Vector3(0, 1, 0)].ID == 0
                                && data[blockPos + new Vector3(0, 2, 0)].ID == 0)
                            {
                                Vector3 placement = blockPos + new Vector3(0.5f + transform.position.x, 1, 0.5f);

                                baseBlock = blockPos;

                                if (blockPos.x % 10 == 2)
                                    prefab = PrefabManager.Instance.SpawnPrefab(1, placement, transform, true);
                                else if (blockPos.x % 10 == 7)
                                    prefab = PrefabManager.Instance.SpawnPrefab(2, placement, transform, true);
                            }
                        }
                        else*/
                        if (z == 1 && data[blockPos].ID == Blocks.Slate.ID) //Cave
                        {
                            if (data[blockPos + new Vector3(0, -1, 0)].ID == 0
                                && data[blockPos + new Vector3(0, -2, 0)].ID == 0)
                            {
                                Vector3 placement = blockPos + new Vector3(0.5f + transform.position.x, -0.5f, 0.5f);

                                baseBlock = blockPos;

                                if (blockPos.x % 12 == 3)
                                    prefab = PrefabManager.Instance.SpawnPrefab(3, placement, transform, false);
                            }
                        }
                        else if (z == 1 && (data[blockPos].ID == Blocks.Grass.ID || data[blockPos].ID == Blocks.BlackrockMossyFloor.ID)) //Grass
                        {
                            if (data[blockPos + new Vector3(0, 1, 0)].ID == 0)
                            {
                                Vector3 placement = blockPos + new Vector3(0.5f + transform.position.x, 1, 0.5f);

                                baseBlock = blockPos;

                                float noise = Mathf.PerlinNoise(x + 0.1f, y + 0.1f) * 100f;
                                if (noise > 52f)
                                    prefab = PrefabManager.Instance.SpawnPrefab(4, placement, transform, true);
                                else if (blockPos.x % 5 != 3)
                                    prefab = PrefabManager.Instance.SpawnPrefab(5, placement, transform, true);

                            }
                        }

                        if (prefab != null && baseBlock != Vector3.zero)
                            tempChunkPrefabs.Add(baseBlock, prefab);
                    }

                    //Iterate over each face direction
                    for (int i = 0; i < 6; i++)
                    {
                        int faceSize = 1;

                        //If edge of chunk show
                        if ((blockPos.x == 1 && i == 2) || (blockPos.x == 32 && i == 3))
                        {

                        }
                        else
                        {
                            /*if (voxelColorAlpha.r == 1.0f)
                            {
                                if (blockPos.x % 2 == 1 && blockPos.y % 2 == 1 && blockPos.x < 32)
                                {
                                    if (lightingGroupMemberlod2.Contains(blockPos))
                                        continue;

                                    if (Lighting.IsLightingGroup(data, blockPos, ref lightingGroupMember, ref shadowMap))
                                    {
                                        faceSize = 2;

                                        if (blockPos.x < 30 && blockPos.x % 4 == 1 && blockPos.y % 4 == 1
                                            && Lighting.IsLightingGroup(data, blockPos + new Vector3(2,0,0), ref lightingGroupMember, ref shadowMap)
                                            && Lighting.IsLightingGroup(data, blockPos + new Vector3(0, 2, 0), ref lightingGroupMember, ref shadowMap)
                                            && Lighting.IsLightingGroup(data, blockPos + new Vector3(2, 2, 0), ref lightingGroupMember, ref shadowMap))
                                        {
                                            lightingGroupMemberlod2.Add(blockPos + new Vector3(2, 0, 0));
                                            lightingGroupMemberlod2.Add(blockPos + new Vector3(0, 2, 0));
                                            lightingGroupMemberlod2.Add(blockPos + new Vector3(2, 2, 0));
                                            faceSize = 4;
                                        }
                                    }
                                }
                                else if (lightingGroupMember.Contains(blockPos))
                                {
                                    continue;
                                }
                            }*/

                            if (z == 1)
                            {
                                if (i != 0)
                                    if (checkVoxelIsSolid(blockPos + voxelFaceChecks[i]))
                                        continue;
                            }
                            else
                            {
                                if (checkVoxelIsSolid(blockPos + voxelFaceChecks[i]))
                                    continue;
                            }
                        }

                        //Collect the appropriate vertices from the default vertices and add the block position
                        for (int j = 0; j < 4; j++)
                        {
                            faceVertices[j] = (voxelVertices[voxelVertexIndex[i, j]] * faceSize) + blockPos;
                            faceUVs[j] = voxelUVs[j];
                        }

                        for (int j = 0; j < 6; j++)
                        {
                            meshData.vertices.Add(faceVertices[voxelTris[i, j]]);
                            meshData.UVs.Add(faceUVs[voxelTris[i, j]]);

                            if (i != 5)
                            {
                                voxelColorAlpha.b = 0;
                                meshData.colors.Add(voxelColorAlpha);
                            }
                            else
                            {
                                voxelColorAlpha.b = (Registry.AtIndex(data[blockPos].ID - 1).m_properties.m_hasTop) ? 1.0f : 0.0f;
                                meshData.colors.Add(voxelColorAlpha);
                            }

                            meshData.UVs2.Add(new Vector2(hasConnection, connectionID));
                            meshData.UVs3.Add(new Vector2(connectionIndex, 0));
                            meshData.triangles.Add(counter++);
                        }
                    }

                }

        if (tempChunkPrefabs.Count > 0)
            chunkPrefabs = tempChunkPrefabs;
    }

    public void UploadMesh()
    {
        meshData.UploadMesh();

        if (meshRenderer == null)
            ConfigureComponents();

        meshFilter.mesh = meshData.mesh;
        if (meshData.vertices.Count > 3)
            meshCollider.sharedMesh = meshData.mesh;
    }

    private void ConfigureComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public bool checkVoxelIsSolid(Vector3 point)
    {
        if (point.y < 0 || (point.x > WorldManager.WorldSettings.containerSize + 2) || (point.z > WorldManager.WorldSettings.containerSize + 2))
            return true;
        else
            return this[point].ID > 0 || this[point].isSolid;
            //return this[point].isSolid;
    }

    public Voxel this[Vector3 index]
    {
        get
        {
            return data[index];
        }

        set
        {
            data[index] = value;
        }
    }

    #region Mesh Data

    public struct MeshData
    {
        public Mesh mesh;
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> UVs;
        public List<Vector2> UVs2;
        public List<Vector2> UVs3;
        public List<Color> colors;
        public bool Initialized;

        public void ClearData()
        {
            if (!Initialized)
            {
                vertices = new List<Vector3>();
                triangles = new List<int>();
                UVs = new List<Vector2>();
                UVs2 = new List<Vector2>();
                UVs3 = new List<Vector2>();
                colors = new List<Color>();

                Initialized = true;
                mesh = new Mesh();
            }
            else
            {
                vertices.Clear();
                triangles.Clear();
                UVs.Clear();
                UVs2.Clear();
                UVs3.Clear();
                colors.Clear();

                mesh.Clear();
            }
        }
        public void UploadMesh(bool sharedVertices = false)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0, false);
            mesh.SetColors(colors);

            mesh.SetUVs(0, UVs);
            mesh.SetUVs(2, UVs2);
            mesh.SetUVs(3, UVs3);

            mesh.Optimize();

            mesh.RecalculateNormals();

            mesh.RecalculateBounds();

            mesh.UploadMeshData(false);
        }
    }
    #endregion

    #region Static Variables
    static readonly Vector3[] voxelVertices = new Vector3[8]
    {
            new Vector3(0,0,0),//0
            new Vector3(1,0,0),//1
            new Vector3(0,1,0),//2
            new Vector3(1,1,0),//3

            new Vector3(0,0,1),//4
            new Vector3(1,0,1),//5
            new Vector3(0,1,1),//6
            new Vector3(1,1,1),//7
    };

    static readonly Vector3[] surroundingChecks = new Vector3[8]
    {
            new Vector3(-1,0,0),//left
            new Vector3(1,0,0),//right
            new Vector3(0,-1,0),//bottom
            new Vector3(0,1,0),//top
            new Vector3(-1,1,0),//diag
            new Vector3(1,1,0),
            new Vector3(-1,-1,0),
            new Vector3(1,-1,0),
    };

    static readonly Vector3[] voxelFaceChecks = new Vector3[6]
    {
            new Vector3(0,0,-1),//front
            new Vector3(0,0,1),//back
            new Vector3(-1,0,0),//left
            new Vector3(1,0,0),//right
            new Vector3(0,-1,0),//bottom
            new Vector3(0,1,0)//top
    };

    static readonly int[,] voxelVertexIndex = new int[6, 4]
    {
            {0,1,2,3},
            {4,5,6,7},
            {4,0,6,2},
            {5,1,7,3},
            {0,1,4,5},
            {2,3,6,7},
    };

    static readonly Vector2[] voxelUVs = new Vector2[4]
    {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(1,1)
    };

    static readonly int[,] voxelTris = new int[6, 6]
    {
            {0,2,3,0,3,1},
            {0,1,2,1,3,2},
            {0,2,3,0,3,1},
            {0,1,2,1,3,2},
            {0,1,2,1,3,2},
            {0,2,3,0,3,1},
    };
    #endregion
}

