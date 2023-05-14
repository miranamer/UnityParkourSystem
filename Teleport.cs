using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour, IInteractable
{
    public PlayerController controller;
    public GameObject teleporter;

    public void Interact()
    {
        controller.EquipItem(teleporter);
    }

    public void Start()
    {
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
