using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public Chunk data = null;
    public Vector3 position = Vector3.zero;
    public Dictionary<Vector3, int> changedBlocks = new Dictionary<Vector3, int>();
}

public class WorldGlobal : MonoBehaviour
{
    private static WorldGlobal _instance;
    public static WorldGlobal Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<WorldGlobal>();
            return _instance;
        }
    }

    public bool isOnline = false;
    private Dictionary<Vector3, ChunkData> chunks = new Dictionary<Vector3, ChunkData>();

    public void Awake()
    {
        if (_instance != null)
        {
            if (_instance != this)
                Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    public ChunkData GetChunkAt(Vector3 position) { return chunks.ContainsKey(position) ? chunks[position] : null; }

    public bool InitializeChunk(Chunk chunk, Vector3 position)
    {
        if (!chunks.ContainsKey(position))
        {
            chunks.Add(position, new ChunkData() { data = chunk, position = position });
            return true;
        }
        return false;
    }

    public void UpdateChunk(Vector3 position, int blockID, Vector3 blockPos)
    {
        if (!chunks[position].changedBlocks.ContainsKey(blockPos))
            chunks[position].changedBlocks.Add(blockPos, blockID);
        else
            chunks[position].changedBlocks[blockPos]= blockID;

        //if (isOnline)
        //    RPC_ChunkUpdated(position);
    }

    //All RPC Methods

    public void RPC_ChunkUpdated(Vector3 position)
    {
        chunks[position].data.SetChangedBlocks(chunks[position].changedBlocks);
    }
}
