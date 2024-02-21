using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(ChunkFog))]
public class WorldManager : MonoBehaviour
{
    public Transform player;
    public Material worldMaterial;
    public Texture2D[] connectionTextures;
    [Space(8)]
    public WorldSettings worldSettings;

    public Dictionary<Vector3, Chunks> chunks = new Dictionary<Vector3, Chunks>();

    public Hotbar hotbar;

    private int currentChunkX = 0;

    private const int RenderDist = 1;

    private ChunkFog fog;

    private bool isInitialized = false;

    void Start()
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

        fog = GetComponent<ChunkFog>();

        StartCoroutine(LoadWorld());
    }

    private void FixedUpdate()
    {
        if (isInitialized)
        {
            float chunkID = Mathf.Abs(player.position.x + worldSettings.containerSize + transform.position.x - 0.5f);
            float remainder = (chunkID % worldSettings.containerSize);

            int newChunkX = Mathf.RoundToInt(Mathf.Abs(chunkID - remainder) - worldSettings.containerSize);

            if (newChunkX != currentChunkX)
            {
                if (newChunkX < worldSettings.containerSize || newChunkX > (worldSettings.chunksRendered * worldSettings.containerSize))
                    return;

                currentChunkX = newChunkX;

                LoadChunksAroundPlayer(newChunkX);
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (!isInitialized)
            return;

        foreach (Chunks activeChunk in chunks.Values)
        {
            ChunkData chunkData = WorldGlobal.Instance.GetChunkAt(activeChunk.ID.position);
            ChunkSaves.Instance.Save(chunkData);
        }
    }

    private IEnumerator LoadWorld()
    {
        //Wait for blocks & textures to be loaded.
        do
        {
            yield return null;
        }
        while (!Registry.IsDone());

        Registry.FetchTextures();

        do
        {
            yield return null;
        }
        while (Registry.AtIndex(0).m_properties.m_texture == null);

        SetConnectionTextureArray();

        hotbar.Initialize();

        //Load materials and other world settings.
        WorldSettings = worldSettings;
        ComputeManager.Instance.Initialize(1);

        if (Registry.blockList.Count > 0)
        {
            worldMaterial.SetTexture("_TextureArray", GenerateTextureArray());
            worldMaterial.SetTexture("_TextureArrayTop", GenerateTextureArrayTop());
        }

        isInitialized = true;
    }

    private async void LoadChunksAroundPlayer(int chunkOffsetX)
    {
        var tasks = new Task[(RenderDist * 2) + 1];
        for (int i = -RenderDist; i <= RenderDist; i++)
        {
            tasks[i + RenderDist] = LoadChunk(i, chunkOffsetX);
        }

        await Task.WhenAll(tasks);

        fog.SetBorder(ref chunks);
        AgeAllChunks();
    }

    private async Task LoadChunk(int index, int chunkOffsetX)
    {
        Vector3 pos = new Vector3(chunkOffsetX + (index * 32), 0, 0);

        if (!chunks.ContainsKey(pos))
        {
            GameObject cont = new GameObject("Chunk");

            cont.transform.parent = transform;
            cont.transform.localPosition = pos;
            cont.name = "Chunk_" + pos.x;

            chunks.Add(pos, new Chunks { ID = cont.AddComponent<Chunk>(), timeAlive = 0 });

            Chunk chunk = chunks[pos].ID;
            chunk.Initialize(worldMaterial, pos);

            ComputeManager.Instance.GenerateVoxelData(ref chunk, pos);
        }
        else //Reset time alive if still active
            chunks[pos].timeAlive = 0;
    }

    private void AgeAllChunks()
    {
        List<Vector3> deleteChunks = new List<Vector3>();

        foreach (var chunk in chunks)
        {
            chunk.Value.timeAlive++;

            if (chunk.Value.timeAlive > 2)
                deleteChunks.Add(chunk.Key);
        }

        foreach (Vector3 chunk in deleteChunks)
        {
            ChunkData chunkData = WorldGlobal.Instance.GetChunkAt(chunk);
            ChunkSaves.Instance.Save(chunkData);

            Destroy(chunks[chunk].ID.gameObject);
            chunks.Remove(chunk);
        }
    }

    public static WorldSettings WorldSettings;
    private static WorldManager _instance;

    public static WorldManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<WorldManager>();
            return _instance;
        }
    }

    public void SetConnectionTextureArray()
    {
        Texture2D tex = connectionTextures[0];
        Texture2DArray texArrayAlbedo = new Texture2DArray(tex.width, tex.height, connectionTextures.Length, tex.format, false);
        texArrayAlbedo.anisoLevel = tex.anisoLevel;
        texArrayAlbedo.filterMode = FilterMode.Point;
        texArrayAlbedo.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < connectionTextures.Length; i++)
        {
            Graphics.CopyTexture(connectionTextures[i], 0, 0, texArrayAlbedo, i, 0);
        }
        
        worldMaterial.SetTexture("_Connections", texArrayAlbedo);
    }

    public Texture2DArray GenerateTextureArray()
    {
        if (Registry.blockList.Count > 0)
        {
            Texture2D tex = Registry.AtIndex(0).m_properties.m_texture;
            Texture2DArray texArrayAlbedo = new Texture2DArray(tex.width, tex.height, Registry.blockList.Count, tex.format, false);
            texArrayAlbedo.anisoLevel = tex.anisoLevel;
            texArrayAlbedo.filterMode = FilterMode.Point;
            texArrayAlbedo.wrapMode = TextureWrapMode.Clamp;

            for (int i = 0; i < Registry.blockList.Count; i++)
            {
                Graphics.CopyTexture(Registry.AtIndex(i).m_properties.m_texture, 0, 0, texArrayAlbedo, i, 0);
            }

            return texArrayAlbedo;
        }
        Debug.Log("No Textures found while trying to generate Tex2DArray");

        return null;
    }

    public Texture2DArray GenerateTextureArrayTop()
    {

        if (Registry.blockList.Count > 0)
        {
            Texture2D tex = Registry.AtIndex(0).m_properties.m_topTexture;
            Texture2DArray texArrayAlbedo = new Texture2DArray(tex.width, tex.height, Registry.blockList.Count, tex.format, false);
            texArrayAlbedo.anisoLevel = tex.anisoLevel;
            texArrayAlbedo.filterMode = tex.filterMode;
            texArrayAlbedo.wrapMode = tex.wrapMode;

            for (int i = 0; i < Registry.blockList.Count; i++)
            {
                Graphics.CopyTexture(Registry.AtIndex(i).m_properties.m_topTexture, 0, 0, texArrayAlbedo, i, 0);
            }

            return texArrayAlbedo;
        }
        Debug.Log("No Textures found while trying to generate Tex2DArray");

        return null;
    }
}

[System.Serializable]
public class Chunks
{
    public Chunk ID;
    public int timeAlive = 0;
}

[System.Serializable]
public class WorldSettings
{
    public int chunksRendered = 8;
    public int containerSize = 16;
    public int maxHeight = 512;
}