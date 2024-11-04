using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject horizontalListPrefab;

    [SerializeField] private Transform verticalListContent;

    private List<Transform> _horizontalLists = new List<Transform>();

    private bool _isBuilt = false;

    private const int HORIZONTAL_MAX_LENGTH = 6;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            menu.SetActive(!menu.activeSelf);

            if (!_isBuilt)
                Build();
        }
    }

    public void Build()
    {
        for (int i = 0; i < Mathf.RoundToInt(Registry.blockList.Count / HORIZONTAL_MAX_LENGTH); i++)
        {
            _horizontalLists.Add(Instantiate(horizontalListPrefab, verticalListContent).transform);
        }

        // Add for remaining items
        if (Registry.blockList.Count % HORIZONTAL_MAX_LENGTH > 0)
            _horizontalLists.Add(Instantiate(horizontalListPrefab, verticalListContent).transform);

        for (int i = 0; i < Registry.blockList.Count; i++)
        {
            int groupingIndex = (int)Math.Floor(i / (float)HORIZONTAL_MAX_LENGTH);

            if (_horizontalLists.Count > groupingIndex)
            {
                SlotView slot = Instantiate(itemPrefab, _horizontalLists[groupingIndex]).GetComponent<SlotView>();

                Block block = Registry.blockList[i];

                slot.SetData(block.m_properties.m_hasVoxelIcon, block.m_properties.m_hasVoxelIcon,
                    block.m_properties.m_hasOtherDrop ? block.m_properties.m_otherdrop.m_properties.m_icon : block.m_properties.m_icon,
                    block.m_properties.m_hasOtherDrop ? block.m_properties.m_otherdrop.m_name.Substring(0, Mathf.Min(4, block.m_properties.m_otherdrop.m_name.Length)) : block.m_name.Substring(0, Mathf.Min(4, block.m_name.Length)));
            }
            else
                Debug.LogError("ItemDisplayer: No Horizontal Lists Left at Index: " + groupingIndex);
        }

        _isBuilt = true;
    }
}
