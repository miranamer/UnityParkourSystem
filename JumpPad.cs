using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JumpPad : MonoBehaviour, IInteractable
{
    PlayerController controller;
    public bool canBeDestroyed = true;

    public void Interact()
    {
        controller.EquipItem(gameObject);
        Debug.Log("Jump Pad");
    }

    private void Update()
    {
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }
}
