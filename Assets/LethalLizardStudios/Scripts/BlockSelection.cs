using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelection : MonoBehaviour
{
    private static BlockSelection _instance;

    public static BlockSelection Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<BlockSelection>();
            return _instance;
        }
    }

    [SerializeField] private Transform player;
    [SerializeField] private LayerMask layerMask;
    [Space(8)]
    [SerializeField] private Material material;
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    [SerializeField] private Color enabled;
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    [SerializeField] private Color disabled;

    [SerializeField] private ParticleSystem breakParticles;
    [SerializeField] private MultiSFX placeSFX;

    [SerializeField] private Hotbar hotbar;

    [SerializeField] private MeshRenderer breakPreview;
    private bool isBreaking = false;

    [SerializeField] private Transform[] placeHolderBlocks;
    private List<Vector3> placeHolderPositions = new List<Vector3>();
    private bool isPlacing = false;

    private bool isCreative = false;

    private bool canSee = false;

    private int layerSelected = 1;

    private Vector3 lastPos = Vector3.zero;

    private void Start()
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

        QualitySettings.vSyncCount = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            layerSelected = 2;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            layerSelected = 1;

        if (canSee)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StopAllCoroutines();
                if (!isCreative)
                    StartCoroutine(BreakBlock(transform.position, true)); //Break block
                else
                    PlaceBlock(0, transform.position, true);
                isBreaking = true;
                //breakParticles.GetComponent<ParticleSystemRenderer>().material.SetColor("_BaseColor", WorldManager.Instance.blocks[2].color);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                isPlacing = true;

                //if (hotbar.currentItemID > 0)
                //    breakParticles.GetComponent<ParticleSystemRenderer>().material.SetColor("_BaseColor", WorldManager.Instance.blocks[hotbar.currentItemID - 1].color);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                placeSFX.Pitch(Random.Range(0.99f, 1.01f));
                placeSFX.Play();

                PlaceMultipleBlocks(hotbar.currentItemID, placeHolderPositions, true);
                ResetPlaceholders();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopAllCoroutines();
            breakPreview.gameObject.SetActive(false);
            isBreaking = false;
        }

        if (Input.GetMouseButtonUp(1))
            ResetPlaceholders();

        if (isCreative && Input.GetMouseButtonDown(2))
            CopyBlock(transform.position);
    }

    private void ResetPlaceholders()
    {
        foreach (Transform block in placeHolderBlocks)
            block.gameObject.SetActive(false);

        placeHolderPositions.Clear();

        isPlacing = false;
    }

    public void CopyBlock(Vector3 copyPos)
    {
        float chunkID = Mathf.Abs(copyPos.x + WorldManager.Instance.transform.position.x - 0.5f);
        float remainder = (chunkID % 32);

        Vector3 chunkPos = new Vector3(Mathf.Abs(chunkID - remainder), 0, 0);
        Vector3 pos = new Vector3(copyPos.x - 0.5f - chunkPos.x, copyPos.y - 0.5f, layerSelected);

        //Error correction
        if (pos.x == 0)
        {
            pos.x = 32;
            chunkPos.x -= 32;
        }

        if (pos.x < 1 || pos.x > 32)
            return;

        Chunk chunk = WorldManager.Instance.chunks[chunkPos].ID;
        Debug.Log(chunk.data[pos].ID);

        Inventory.Instance.AddCreativeItem(chunk.data[pos].ID);
    }

    private void PlacePlaceholder(Vector3 placePos)
    {
        placeHolderBlocks[placeHolderPositions.Count - 1].position = placePos;
        placeHolderBlocks[placeHolderPositions.Count - 1].gameObject.SetActive(true);
    }

    public void PlaceMultipleBlocks(int blockID, List<Vector3> positions, bool showParticles)
    {
        List<ChunkData> chunksData = new List<ChunkData>();

        foreach (Vector3 placePos in positions)
        {
            float chunkID = Mathf.Abs(placePos.x + WorldManager.Instance.transform.position.x - 0.5f);
            float remainder = (chunkID % 32);

            Vector3 chunkPos = new Vector3(Mathf.Abs(chunkID - remainder), 0, 0);
            Vector3 pos = new Vector3(placePos.x - 0.5f - chunkPos.x, placePos.y - 0.5f, layerSelected);

            //Error correction
            if (pos.x == 0)
            {
                pos.x = 32;
                chunkPos.x -= 32;
            }

            if (pos.x < 1 || pos.x > 32)
                continue;

            if (!WorldManager.Instance.chunks.ContainsKey(chunkPos))
                continue;

            ChunkData chunkData = WorldGlobal.Instance.GetChunkAt(chunkPos);

            //Check if block already there before placement
            if (blockID != 0)
            {
                int currentBlockID = chunkData.data.data[pos].ID;

                if (currentBlockID != 0)
                {
                    //If is a togglable block
                    if (Registry.AtIndex(currentBlockID - 1).m_properties.m_isTogglable)
                    {
                        chunkData.data.ToggleBlock(pos, currentBlockID);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                if (hotbar.ItemAmount() <= 0)
                    continue;

                hotbar.RemoveItem();
            }
            else
            {
                if (chunkData.data.data[pos].ID == 0)
                    continue;

                //Destroy Prefabs Attached
                if (chunkData.data.chunkPrefabs.ContainsKey(pos))
                {
                    Destroy(chunkData.data.chunkPrefabs[pos]);
                    chunkData.data.chunkPrefabs.Remove(pos);
                }

                Inventory.Instance.AddItem(chunkData.data.data[pos].ID);
            }

            if (showParticles)
            {
                breakParticles.transform.position = transform.position;
                breakParticles.Play();
            }

            if (!chunkData.changedBlocks.ContainsKey(pos))
                chunkData.changedBlocks.Add(pos, blockID);
            else
                chunkData.changedBlocks[pos] = blockID;

            chunkData.data.data[pos] = new Voxel() { ID = blockID };

            if (!chunksData.Contains(chunkData))
                chunksData.Add(chunkData);
        }

        for (int i = 0; i < chunksData.Count; i++)
        {
            ChunkData current = chunksData[i];
            ComputeManager.Instance.GenerateVoxelData(ref current.data, current.position);
        }
    }

    public IEnumerator BreakBlock(Vector3 breakPos, bool showParticles)
    {
        float chunkID = Mathf.Abs(breakPos.x + WorldManager.Instance.transform.position.x - 0.5f);
        float remainder = (chunkID % 32);

        Vector3 chunkPos = new Vector3(Mathf.Abs(chunkID - remainder), 0, 0);
        Vector3 pos = new Vector3(breakPos.x - 0.5f - chunkPos.x, breakPos.y - 0.5f, layerSelected);

        //Error correction
        if (pos.x == 0)
        {
            pos.x = 32;
            chunkPos.x -= 32;
        }

        if (pos.x < 1 || pos.x > 32 || !WorldManager.Instance.chunks.ContainsKey(chunkPos))
        {

        }
        else
        {
            ChunkData chunkData = WorldGlobal.Instance.GetChunkAt(chunkPos);
            int currentBlockID = chunkData.data.data[pos].ID;

            if (currentBlockID > 0)
            {
                Block block = Registry.AtIndex(currentBlockID - 1);

                if (block != null)
                {
                    breakPreview.gameObject.SetActive(true);

                    breakPreview.materials[0].SetFloat("_Index", 0);
                    //breakPreview.Play("Anim_BreakBlock", -1, 0f);

                    int count = Mathf.RoundToInt(breakPreview.materials[0].GetFloat("_Sheet_Count"));

                    for (int i = 0; i < count; i++)
                    {
                        breakPreview.materials[0].SetFloat("_Index", i);
                        yield return new WaitForSeconds(block.m_properties.m_toughness / count);
                    }

                    breakPreview.gameObject.SetActive(false);

                    if (isBreaking)
                        PlaceBlock(0, breakPos, showParticles);
                }
            }
        }
    }

    public void PlaceBlock(int blockID, Vector3 placePos, bool showParticles)
    {
        float chunkID = Mathf.Abs(placePos.x + WorldManager.Instance.transform.position.x - 0.5f);
        float remainder = (chunkID % 32);

        Vector3 chunkPos = new Vector3(Mathf.Abs(chunkID - remainder), 0, 0);
        Vector3 pos = new Vector3(placePos.x - 0.5f - chunkPos.x, placePos.y - 0.5f, layerSelected);

        //Error correction
        if (pos.x == 0)
        {
            pos.x = 32;
            chunkPos.x -= 32;
        }

        if (pos.x < 1 || pos.x > 32)
            return;

        if (!WorldManager.Instance.chunks.ContainsKey(chunkPos))
            return;

        ChunkData chunkData = WorldGlobal.Instance.GetChunkAt(chunkPos);

        //Check if block already there before placement
        if (blockID != 0)
        {
            if (hotbar.ItemAmount() <= 0)
                return;

            hotbar.RemoveItem();
        }
        else
        {
            if (chunkData.data.data[pos].ID == 0)
                return;

            //Destroy Prefabs Attached
            if (chunkData.data.chunkPrefabs.ContainsKey(pos))
            {
                Destroy(chunkData.data.chunkPrefabs[pos]);
                chunkData.data.chunkPrefabs.Remove(pos);
            }

            Inventory.Instance.AddItem(chunkData.data.data[pos].ID);
        }

        if (showParticles)
        {
            breakParticles.transform.position = transform.position;
            breakParticles.Play();
        }

        if (!chunkData.changedBlocks.ContainsKey(pos))
            chunkData.changedBlocks.Add(pos, blockID);
        else
            chunkData.changedBlocks[pos] = blockID;

        chunkData.data.data[pos] = new Voxel() { ID = blockID };

        ComputeManager.Instance.GenerateVoxelData(ref chunkData.data, chunkPos);
    }

    Vector3 worldPosition;
    Vector2 cursorLastPos = new Vector2(0, 0);

    private void FixedUpdate()
    {
        if (isPlacing)
        {
            if (transform.position.x != cursorLastPos.x || transform.position.y != cursorLastPos.y)
            {
                cursorLastPos = new Vector2(transform.position.x, transform.position.y);

                if (placeHolderPositions.Count < placeHolderBlocks.Length && !placeHolderPositions.Contains(transform.position))
                {
                    placeHolderPositions.Add(transform.position);
                    PlacePlaceholder(transform.position);
                }
            }
        }

        material.SetColor("_EmissionColor", canSee ? enabled : disabled);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, 100, layerMask))
        {
            worldPosition = hitData.point;
        }

        worldPosition = new Vector3(worldPosition.x - 0.5f, worldPosition.y - 0.5f, worldPosition.z);

        float dist = Vector3.Distance(player.position, transform.position);
        canSee = dist < 10 && dist > 0.8f;

        transform.position = new Vector3(Mathf.Round(worldPosition.x) + 0.5f, Mathf.Round(worldPosition.y) + 0.5f, layerSelected + 0.5f);

        if (lastPos != transform.position)
        {
            StopAllCoroutines();
            breakPreview.gameObject.SetActive(false);
            isBreaking = false;

            lastPos = transform.position;
        }
    }
}
