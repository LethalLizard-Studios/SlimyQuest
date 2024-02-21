using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    private static PrefabManager _instance;

    public static PrefabManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PrefabManager>();
            return _instance;
        }
    }

    void Awake()
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

    [SerializeField] private GameObject[] prefabs;

    public GameObject SpawnPrefab(int index, Vector3 pos, Transform parent, bool randomRotation)
    {
        Transform prefab = Instantiate(prefabs[index], transform).transform;
        prefab.position = pos;
        prefab.parent = parent;

        if (randomRotation)
            prefab.rotation = Quaternion.Euler(new Vector3(prefab.eulerAngles.x, (90 * (pos.x % 4)) + 45, prefab.eulerAngles.z));

        return prefab.gameObject;
    }

    public void SpawnPrefab(int index, Vector3 pos, List<int> blocks, List<Vector3> blockInfo)
    {
        Transform prefab = Instantiate(prefabs[index], transform).transform;
        prefab.position = pos;
        prefab.parent = transform;

        Machine machine = prefab.GetComponent<Machine>();

        for (int i = 0; i < blocks.Count; i++)
            machine.AddBlock(blocks[i], blockInfo[i]);

        machine.Initialize();
    }
}
