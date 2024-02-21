using System.Collections.Generic;
using UnityEngine;

public abstract class Machine : MonoBehaviour
{
    [SerializeField] protected List<int> blockID;
    [SerializeField] protected List<Vector3> blockInfo;

    [SerializeField] protected GameObject[] blocks;

    public virtual void Initialize()
    {
        for (int i = 0; i < blockID.Count; i++)
        {
            Transform block = Instantiate(blocks[blockID[i]], transform).transform;
            block.localPosition = blockInfo[i];
        }
    }

    public virtual void AddBlock(int ID, Vector3 info)
    {
        blockID.Add(ID);
        blockInfo.Add(info);
    }
}
