using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ComputeManager : MonoBehaviour
{
    public ComputeShader shader;

    private List<VoxelBuffer> allNoiseComputeBuffers = new List<VoxelBuffer>();
    private Queue<VoxelBuffer> availableNoiseComputeBuffers = new Queue<VoxelBuffer>();

    private int xThreads;
    private int yThreads;

    public Vector3 seedOffset = Vector3.zero;


    public void Awake()
    {
        seedOffset = new Vector3(Random.Range(0, 1000), 0, Random.Range(0, 1000));
    }

    public void Initialize(int count = 256)
    {
        xThreads = WorldManager.WorldSettings.containerSize / 8 + 1;
        yThreads = WorldManager.WorldSettings.maxHeight / 8;

        shader.SetInt("containerSizeX", WorldManager.WorldSettings.containerSize);
        shader.SetInt("containerSizeY", WorldManager.WorldSettings.maxHeight);

        shader.SetBool("hasCaves", true);

        shader.SetInt("maxHeight", WorldManager.WorldSettings.maxHeight);

        shader.SetFloat("noiseScale", 0.004f);
        shader.SetFloat("caveScale", 0.046f);
        shader.SetFloat("caveThreshold", 0.3f);

        SetupBlocks();

        for (int i = 0; i < count; i++)
        {
            CreateNewNoiseBuffer();
        }
    }

    public void SetupBlocks()
    {
        shader.SetInt("surfaceID", Blocks.Grass.ID);
        shader.SetInt("subsurfaceID", Blocks.Dirt.ID);
        shader.SetInt("surfaceAltID", Blocks.Sand.ID);

        shader.SetInt("sandstoneVoxelID", Blocks.Sandstone.ID);
        shader.SetInt("stoneVoxelID", Blocks.Stone.ID);
        shader.SetInt("deepStoneVoxelID", Blocks.Slate.ID);
        shader.SetInt("unbreakableVoxelID", Blocks.Unbreakium.ID);

        shader.SetInt("coalVoxelID", Blocks.CoalOre.ID);
        shader.SetInt("copperVoxelID", Blocks.CopperOre.ID);
        shader.SetInt("ironVoxelID", Blocks.IronOre.ID);
        shader.SetInt("marbleVoxelID", Blocks.Marble.ID);
        shader.SetInt("bedrockVoxelID", Blocks.Hellrock.ID);
        shader.SetInt("stoneDeadGrassVoxelID", Blocks.DeadGrassStone.ID);
        shader.SetInt("magmaBedrockVoxelID", Blocks.MagmaHellrock.ID);
        shader.SetInt("amethystVoxelID", Blocks.CinnabarOre.ID);
        shader.SetInt("opalVoxelID", Blocks.OpalOre.ID);
        shader.SetInt("aluminumVoxelID", Blocks.AluminumOre.ID);
        shader.SetInt("slimeVoxelID", Blocks.SlimyOre.ID);
    }

    #region Voxel Buffers

    #region Pooling
    public VoxelBuffer GetNoiseBuffer()
    {
        if (availableNoiseComputeBuffers.Count > 0)
            return availableNoiseComputeBuffers.Dequeue();
        else
        {
            return CreateNewNoiseBuffer(false);
        }
    }

    public VoxelBuffer CreateNewNoiseBuffer(bool enqueue = true)
    {
        VoxelBuffer buffer = new VoxelBuffer();
        buffer.InitializeBuffer();
        allNoiseComputeBuffers.Add(buffer);

        if (enqueue)
            availableNoiseComputeBuffers.Enqueue(buffer);

        return buffer;
    }

    public void ClearAndRequeueBuffer(VoxelBuffer buffer)
    {
        ClearVoxelData(buffer);
        availableNoiseComputeBuffers.Enqueue(buffer);
    }
    #endregion

    #region Compute Helpers

    public void GenerateVoxelData(ref Chunk cont, Vector3 pos)
    {
        WorldGlobal.Instance.RPC_ChunkUpdated(cont.position);

        shader.SetBuffer(0, "voxelArray", cont.data.noiseBuffer);
        shader.SetBuffer(0, "count", cont.data.countBuffer);

        shader.SetVector("chunkPosition", cont.position);
        shader.SetVector("seedOffset", seedOffset);

        shader.Dispatch(0, xThreads, yThreads, xThreads);

        AsyncGPUReadback.Request(cont.data.noiseBuffer, (callback) =>
        {
            callback.GetData<Voxel>(0).CopyTo(WorldManager.Instance.chunks[pos].ID.data.voxelArray.array);
            WorldManager.Instance.chunks[pos].ID.RenderMesh();

        });
    }

    private void ClearVoxelData(VoxelBuffer buffer)
    {
        buffer.countBuffer.SetData(new int[] { 0 });
        shader.SetBuffer(1, "voxelArray", buffer.noiseBuffer);
        shader.Dispatch(1, xThreads, yThreads, xThreads);
    }
    #endregion
    #endregion

    private void OnApplicationQuit()
    {
        DisposeAllBuffers();
    }

    public void DisposeAllBuffers()
    {
        foreach (VoxelBuffer buffer in allNoiseComputeBuffers)
            buffer.Dispose();
    }


    private static ComputeManager _instance;

    public static ComputeManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ComputeManager>();
            return _instance;
        }
    }
}