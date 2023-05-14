using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable, IDestroyable
{
    public PlayerController controller;
    public GameObject chestPrefab;
    
    public bool canBeDestroyed = true;

    public void Interact()
    {
        controller.EquipItem(chestPrefab);
    }

    public void Start()
    {
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
