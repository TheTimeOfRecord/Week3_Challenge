using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private PlayerController playerController;
    private PlayerCondition playerCondition;

    private ItemData selectedItem;
    private int selectedItemIndex = -1;

    int curEquipIndex;


    // Start is called before the first frame update
    void Start()
    {
        playerController = CharacterManager.Instance.Player.controller;
        playerCondition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        playerController.onInventoryAction += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>(); ;
            slots[i].index = i;
            slots[i].inventory = this;
        }
        ClearSelectedItemWindow();
        UpdateUI(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }

    private bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    private void AddItem()
    {
        ItemData itemData = CharacterManager.Instance.Player.itemData;

        //아이템이 중복 가능한지 canStack
        if (itemData.canStack)
        {
            ItemSlot slot = GetStackSlot(itemData);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        //비어있는 슬롯 가져오기
        ItemSlot emptySlot = GetEmptySlot();

        //있다면
        if (emptySlot != null)
        {
            emptySlot.item = itemData;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        //없다면
        ThrowItem(itemData);

        CharacterManager.Instance.Player.itemData = null;
    }



    private void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    private ItemSlot GetStackSlot(ItemData itemData)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == itemData && slots[i].quantity < itemData.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    private ItemSlot GetEmptySlot()
    {
        for  (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }
    private void ThrowItem(ItemData itemData)
    {
        Instantiate(itemData.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.forward));
    }

    public void SetSelectedItem(int index)
    {
        if (slots[index].item == null) return;
        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0;i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += $"{selectedItem.consumables[i].type}\n";
            selectedStatValue.text += $"{selectedItem.consumables[i].value}\n";
        }
        SetSelectedItemButton(index);

    }

    private void SetSelectedItemButton(int index)
    {
        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
        unEquipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton()
    {
        if(selectedItem.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Hunger:
                        playerCondition.Eat(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Health:
                        playerCondition.Heal(selectedItem.consumables[i].value);
                        break;
                }
            }
            RemoveSelectedItme();
        }
    }
    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        if (selectedItemIndex == curEquipIndex) UnEquip(curEquipIndex);
        RemoveSelectedItme();
    }


    void RemoveSelectedItme()
    {
        slots[selectedItemIndex].quantity--;
        if (slots[selectedItemIndex].quantity <= 0)
        {
            slots[selectedItemIndex].item = null;
            selectedItem = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }
        UpdateUI();
    }
    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equipment.EquipNew(selectedItem);
        UpdateUI();

        SetSelectedItemButton(selectedItemIndex);
    }

    private void UnEquip(int index)
    {
        slots[index].equipped = false;
        CharacterManager.Instance.Player.equipment.UnEquip();
        UpdateUI();

        if(selectedItemIndex == index)
        {
            SetSelectedItem(selectedItemIndex);
        }
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }
}
