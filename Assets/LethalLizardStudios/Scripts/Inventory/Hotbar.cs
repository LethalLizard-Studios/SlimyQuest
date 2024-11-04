using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextPopup))]
public class Hotbar : MonoBehaviour
{
    [SerializeField] private Transform selector;
    [SerializeField] private Transform pageSelect;
    [SerializeField] private RawImage[] slotIcons;
    [SerializeField] private RawImage[] slotMasks;
    [SerializeField] private GameObject[] slotOverlays;
    [SerializeField] private TextMeshProUGUI[] slotAmounts;

    [SerializeField] private int[] itemInSlots = new int[36];
    [SerializeField] private int[] itemAmounts = new int[36];

    private int currentSlot = 0;
    public int currentItemID = 0;

    private int currentPage = 0;
    private int currentPageOffset = 0;

    private TextPopup popup;

    private const int HOTBAR_WIDTH = 9;
    private const int MAX_PAGES = 4;

    //Get Current Slot Index with Page Offset
    public int CurrentSlotAdjusted() { return currentSlot + currentPageOffset; }
    public int GetItemInSlot(int slotID) { return itemInSlots[slotID]; }

    private void Start()
    {
        popup = GetComponent<TextPopup>();
    }

    public void Initialize()
    {
        popup.Display();
        UpdateAmounts();
        UpdateIcons();

        currentItemID = itemInSlots[CurrentSlotAdjusted()];
    }

    private void Update()
    {
        //Adjust Slot with Plus/Minus
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            SelectedSlot(currentSlot + 1);
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            SelectedSlot(currentSlot - 1);

        //Select Slot 1-9 Keys
        if (Input.GetKeyDown(KeyCode.Alpha1)) { SelectedSlot(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { SelectedSlot(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { SelectedSlot(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { SelectedSlot(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { SelectedSlot(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { SelectedSlot(5); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { SelectedSlot(6); }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { SelectedSlot(7); }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { SelectedSlot(8); }

        //Change Hotbar Page
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentPage = (currentPage < MAX_PAGES-1) ? currentPage + 1 : 0;
            currentPageOffset = HOTBAR_WIDTH * currentPage;
            currentItemID = itemInSlots[CurrentSlotAdjusted()];

            popup.Display();
            UpdateAmounts();
            UpdateIcons();
        }

        //Scroll to Change Slot
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
            SelectedSlot(scroll > 0 ? currentSlot - 1 : currentSlot + 1);
    }

    private void FixedUpdate()
    {
        selector.localPosition = Vector3.Lerp(selector.localPosition, new Vector3((currentSlot * 40) - 160, 0, 0), Time.deltaTime * 16f);
        pageSelect.localPosition = Vector3.Lerp(pageSelect.localPosition, new Vector3(0, currentPage * -13.3f, 0), Time.deltaTime * 16f);
    }

    public void SetSlot(int slotID, int itemID, int amount)
    {
        //Set current selected slot
        if (slotID == -1 || slotID == CurrentSlotAdjusted())
        {
            itemInSlots[CurrentSlotAdjusted()] = itemID;
            currentItemID = itemID;

            if (itemID > 0)
                itemAmounts[CurrentSlotAdjusted()] = amount;
            else
                itemAmounts[CurrentSlotAdjusted()] = 0;
        }
        else
        {
            itemInSlots[slotID] = itemID;

            if (itemID > 0)
                itemAmounts[slotID] = amount;
            else
                itemAmounts[slotID] = 0;
        }

        UpdateIcons();
        UpdateAmounts();
        popup.Display();
    }

    public void RemoveItem()
    {
        Item item = Inventory.Instance.RemoveItem(itemInSlots[CurrentSlotAdjusted()]);

        if (item == null || item.Count() <= 0)
        {
            itemInSlots[CurrentSlotAdjusted()] = -1;
            itemAmounts[CurrentSlotAdjusted()] = 0;
            currentItemID = -1;
            UpdateIcons();
            UpdateAmounts();
            return;
        }

        itemAmounts[CurrentSlotAdjusted()] = item.Count();
        UpdateAmounts();
    }

    public int ItemAmount()
    {
        return itemAmounts[CurrentSlotAdjusted()];
    }

    public bool ItemCannotBePlaced()
    {
        return !Registry.AtIndex(itemInSlots[CurrentSlotAdjusted()]).m_properties.m_canBePlaced;
    }

    public int FindEmptySlot()
    {
        for(int i = 0; i < itemInSlots.Length; i++)
            if (itemInSlots[i + currentPageOffset] <= 0)
                return i + currentPageOffset;
        return -2;
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (itemInSlots[i + currentPageOffset] <= 0)
            {
                slotIcons[i].enabled = false;
                slotOverlays[i].SetActive(false);
                slotMasks[i].enabled = false;
            }
            else
            {
                Block block = Registry.AtIndex(itemInSlots[i + currentPageOffset] - 1);

                if (block == null)
                {
                    slotIcons[i].enabled = false;
                    slotOverlays[i].SetActive(false);
                    slotMasks[i].enabled = false;
                }
                else
                {
                    bool hasVoxelIcon = block.m_properties.m_hasVoxelIcon;
                    slotOverlays[i].SetActive(hasVoxelIcon);
                    slotMasks[i].enabled = hasVoxelIcon;

                    if (!block.m_properties.m_hasOtherDrop)
                        slotIcons[i].texture = block.m_properties.m_icon;
                    else
                        slotIcons[i].texture = block.m_properties.m_otherdrop.m_properties.m_icon;
                    slotIcons[i].enabled = true;
                }
            }
        }
    }

    private void UpdateAmounts()
    {
        for (int i = 0; i < slotAmounts.Length; i++)
        {
            if (itemAmounts[i + currentPageOffset] <= 0)
                slotAmounts[i].enabled = false;
            else
            {
                slotAmounts[i].text = itemAmounts[i + currentPageOffset].ToString();
                slotAmounts[i].enabled = true;
            }
        }
    }

    private void SelectedSlot(int slot)
    {
        if (slot >= 9)
        {
            currentSlot = 0;
            selector.localPosition = new Vector3((currentSlot * 40) - 160, 0, 0);
        }
        else if (slot < 0)
        {
            currentSlot = 8;
            selector.localPosition = new Vector3((currentSlot * 40) - 160, 0, 0);
        }
        else
            currentSlot = slot;

        currentItemID = itemInSlots[CurrentSlotAdjusted()];

        popup.Display();
    }
}
