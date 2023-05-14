using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public float speed = 10f;
    public float explosiveForce = 20f;
    public float teleportTimer = 0;
    public float teleportTime = 1.5f;
    public bool onSpeedPad = false;

    public int teleportCounter = 0;
    
    public Rigidbody playerRb;
    public AchievementManager achievementManager;

    public List<string> achievementsToUnlock;

    public float lookDistance = 3f;
    public Dictionary<GameObject, int> inventory = new Dictionary<GameObject, int>();
    public GameObject equippedItem;

    public Vector3 spawnPos;

    public TextMeshProUGUI equipText;
    public TextMeshProUGUI equippedText;

    public float equipTime = 2f;
    public float equipTimer = 0f;


    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        achievementManager = GameObject.Find("AchievementManager").GetComponent<AchievementManager>();
    }

    void Update()
    {
        HandleMovement();
        CheckAchievementsToUnlock();
        InteractWithObject();
        SpawnObject();
        DestroyObject();
        DeactivateEquipText();
    }

    public void DeactivateEquipText()
    {
        if (Time.time >= equipTimer + equipTime)
        {
            equipText.gameObject.SetActive(false);
        }
    }

    public void InteractWithObject()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //Debug.DrawRay(ray.origin, ray.direction * lookDistance);

        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, lookDistance))
        {

            //spawnPos = ray.GetPoint(lookDistance);
            GameObject hitObject = hitData.transform.gameObject;

            if (Input.GetMouseButtonDown(0) && hitObject.GetComponent<IInteractable>() != null)
            {
                hitObject.GetComponent<IInteractable>().Interact();
            }
        }
    }

    public void SpawnObject() // teleporter does not spawn where it should. No Clue Why!
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (equippedItem == null)
            {
                return;
            }
            else
            {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                spawnPos = ray.GetPoint(lookDistance);

                if (spawnPos.y <= 5.37f)
                {
                    spawnPos.y = 5.37f;
                }

                //GameObject equippedCopy = Instantiate(equippedItem);

                Instantiate(equippedItem, spawnPos, equippedItem.transform.rotation);
            }
        }
    }

    public void DestroyObject()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hitData;

            if (Physics.Raycast(ray, out hitData, lookDistance))
            {
                GameObject hitObject = hitData.transform.gameObject;

                if (hitObject.GetComponent<IDestroyable>() != null)
                {
                    hitObject.GetComponent<IDestroyable>().DestroyObject();
                }
            }
        }
    }

    public bool RemFromInventory(GameObject item)
    {
        if (!inventory.ContainsKey(item) || inventory[item] <= 0)
        {
            return false;
        }
        else
        {
            inventory[item]--;
            return true;
        }
    }

    public void AddToInventory(GameObject item)
    {
        if (!inventory.ContainsKey(gameObject))
        {
            inventory[item] = 1;
        }
        else
        {
            inventory[item]++;
        }
    }

    public void EquipItem(GameObject item)
    {

        equippedItem = item;

        equippedText.text = "Equipped: " + equippedItem.gameObject.name;

        equipText.text = equippedItem.gameObject.name + " - Equipped";
        equipText.gameObject.SetActive(true);

        equipTimer = Time.time;
    }

    void CheckAchievementsToUnlock()
    {
        if (achievementsToUnlock.Count > 0)
        {
            if (!achievementManager.achievementTMP.gameObject.activeInHierarchy)
            {
                UnlockAchievement(achievementsToUnlock[0]);
                achievementsToUnlock.RemoveAt(0);
            }
        }
    }

    void UnlockAchievement(string achievementName)
    {

        if (achievementManager.achievementTMP.gameObject.activeInHierarchy) // if another achievement is curr popped up
        {
            achievementsToUnlock.Add(achievementName);
        }
        
        else if (!achievementManager.achievements[achievementName].achievementUnlocked)
        {
            StartCoroutine(achievementManager.ShowAchievement(achievementManager.achievements[achievementName], achievementManager.achievementTMP));
        }
    }

    void HandleMovement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        if(Input.GetKey(KeyCode.LeftShift))
        {
            if (!onSpeedPad)
            {
                speed = 20f;
            }
        }
        else if(!onSpeedPad)
        {
            speed = 10f;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            playerRb.AddForce(Vector3.up * explosiveForce * 5f);
        }

        transform.Translate(new Vector3(horizontalMovement, 0, verticalMovement) * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "BouncePad")
        {
            UnlockAchievement("JumpPad");
            playerRb.AddForce(Vector3.up * explosiveForce, ForceMode.Impulse);
        }

        if (other.gameObject.name == "Teleport1")
        {
            if (Time.time >= teleportTimer + teleportTime)
            {
                TeleportPlayer(other, "Teleport2");
            }
        }

        if (other.gameObject.name == "Teleport2")
        {
            if (Time.time >= teleportTimer + teleportTime)
            {
                TeleportPlayer(other, "Teleport1");
            }
        }

        if(other.gameObject.name == "SpeedPad")
        {
            UnlockAchievement("SpeedPad");

            speed = 30f;
            onSpeedPad = true;
        }

        if(other.gameObject.name != "SpeedPad" && onSpeedPad)
        {
            onSpeedPad = false;
        }
    }

    void TeleportPlayer(Collider other, string teleportPoint)
    {

        UnlockAchievement("Teleport");

        Transform parent = other.gameObject.transform.parent;
        Transform tp = parent.Find(teleportPoint);

        transform.position = new Vector3(tp.position.x, transform.position.y, tp.position.z);
        teleportTimer = Time.time;

        teleportCounter++;
    }
}
