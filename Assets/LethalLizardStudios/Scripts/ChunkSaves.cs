using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChunkSaves : MonoBehaviour
{
    private static ChunkSaves _instance;

    public static ChunkSaves Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ChunkSaves>();
            return _instance;
        }
    }

    public void Awake()
    {
        if (_instance != null)
        {
            if (_instance != this)
                Destroy(this);
        }
        else
            _instance = this;
    }

    private Dictionary<Vector3, string> blockData = new Dictionary<Vector3, string>();

    private string saveFolder = "WorldSaves";
    private string type = ".blockdata";
    private string path;

    public void Start()
    {
        path = Application.dataPath + $"/{saveFolder}/";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (File.Exists(path + "World.seed"))
        {
            String seed = File.ReadAllText(path + "World.seed");
            string[] vector = seed.Split(',');
            Vector3 seedValue = new Vector3(int.Parse(vector[0]), int.Parse(vector[1]), int.Parse(vector[2]));
            ComputeManager.Instance.seedOffset = seedValue;
        }
        else
        {
            var sr = File.CreateText(path + "World.seed");
            Vector3 seed = ComputeManager.Instance.seedOffset;
            sr.Write(seed.x+","+ seed.y+","+seed.z);
            sr.Close();
        }
    }

    public void Save(ChunkData chunkData)
    {
        if (chunkData == null)
            return;

        if (chunkData.changedBlocks.Count == 0)
            return;

        Debug.Log("<color=magenta>SAVING: " + "Chunk_" + chunkData.position.x + "</color>");

        string blockStr = "";

        foreach (var blocks in chunkData.changedBlocks)
        {
            Vector3 pos = blocks.Key;
            blockStr += "/" + Mathf.RoundToInt(pos.x) + "," + Mathf.RoundToInt(pos.y) + "," + Mathf.RoundToInt(pos.z) + ":" + blocks.Value;
        }

        if (blockData.ContainsKey(chunkData.position))
            blockData[chunkData.position] = blockStr;
        else
            blockData.Add(chunkData.position, blockStr);

        if (File.Exists(path + "Chunk_"+chunkData.position.x + type))
            File.WriteAllText(path + "Chunk_" + chunkData.position.x + type, blockStr);
        else
        {
            var sr = File.CreateText(path + "Chunk_" + chunkData.position.x + type);
            sr.Write(blockStr);
            sr.Close();
        }
    }

    public Dictionary<Vector3, int> Load(ChunkData chunkData)
    {
        string blockStr = "";

        if (blockData.ContainsKey(chunkData.position))
        {
            blockStr = blockData[chunkData.position];
        }
        else
        {
            if (File.Exists(path + "Chunk_" + chunkData.position.x + type))
                blockStr = File.ReadAllText(path + "Chunk_" + chunkData.position.x + type);
            else
                return null;
        }

        if (blockStr.Equals(""))
            return null;

        Debug.Log("<color=magenta>LOADING: " + "Chunk_" + chunkData.position.x + "</color>");

        Dictionary<Vector3, int> loadedBlocks = new Dictionary<Vector3, int>();

        string[] allBlocks = blockStr.Split('/');

        for (int i = 1; i < allBlocks.Length; i++)
        {
            int id;
            Vector3 pos;

            string[] half = allBlocks[i].Split(':');
            id = int.Parse(half[1]);

            string[] vector = half[0].Split(',');
            pos = new Vector3(int.Parse(vector[0]), int.Parse(vector[1]), int.Parse(vector[2]));

            loadedBlocks.Add(pos, id);
        }

        return loadedBlocks;
    }
}
