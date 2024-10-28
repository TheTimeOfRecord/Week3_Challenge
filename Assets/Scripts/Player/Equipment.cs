using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;

    private PlayerController playerController;
    private PlayerCondition playerCondition;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerCondition = GetComponent<PlayerCondition>();
    }

    public void EquipNew(ItemData itemData)
    {
        UnEquip();
        curEquip = Instantiate(itemData.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if(curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed && curEquip != null && playerController.canLook)
        {
            curEquip.OnAttackInput();
        }
    }
}
