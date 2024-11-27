using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float pullStrength = 0.1f;

    public GameObject soldierPrefab;
    public List<GameObject> soldiers = new List<GameObject>();
    public float moveSpeed = 5f;
    public float forwardSpeed = 10f;
    public float laneWidth = 3f;

    private float targetXPosition = 0f;

    void Start()
    {
        SpawnSoldiers(1); // Spawn initial soldiers
       
    }

    void Update()
    {
        // Continuous forward movement
        // transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }

        //else if (Input.GetKeyDown(KeyCode.F))
        //{
        //    ModifySoldiers("multiply", 2);
        //}
        else if (Input.GetKeyDown(KeyCode.F))
        {
            ModifySoldiers(OperationType.Add, 1);
        }


        PullSoldiersCloser();

        Vector3 targetPosition = new Vector3(targetXPosition, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void MoveLeft()
    {
        targetXPosition -= laneWidth;
    }

    void MoveRight()
    {
        targetXPosition += laneWidth;
    }

    public void ModifySoldiers(OperationType operation, int value)
    {
        int currentCount = soldiers.Count;

        int newSoldierCount = currentCount;

        switch (operation)
        {
            case OperationType.Add:
                newSoldierCount = currentCount + value;
                break;
            case OperationType.Multiply:
                newSoldierCount = currentCount * value;
                break;
            case OperationType.Subtract:
                newSoldierCount = Mathf.Max(currentCount - value, 0);
                break;
            case OperationType.SqrtAdd:
                newSoldierCount = Mathf.CeilToInt(Mathf.Sqrt(currentCount)) + value;
                break;
        }

        int difference = newSoldierCount - currentCount;
        if (difference > 0)
            SpawnSoldiers(difference);
        else
            RemoveSoldiers(-difference);

 


    }

    void SpawnSoldiers(int count)
    {

        int gridRows = Mathf.CeilToInt(Mathf.Sqrt(count));
        int gridCols = Mathf.CeilToInt((float)count / gridRows);
        float spacing = 1.8f;

        for (int i = 0; i < count; i++)
        {
            int row = i / gridCols;
            int col = i % gridCols;

            Vector3 spawnOffset = new Vector3((col - gridCols / 2f) * spacing, 0, (row - gridRows / 2f) * spacing);
            Vector3 spawnPosition = transform.position + spawnOffset;


            GameObject soldier = Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
            soldiers.Add(soldier);
            soldier.transform.parent = transform;
        }
    }

    void PullSoldiersCloser()
    {
        foreach (GameObject soldier in soldiers)
        {
            Rigidbody rb = soldier.GetComponent<Rigidbody>();

            Vector3 direction = (transform.position - soldier.transform.position).normalized;
            rb.AddForce(direction * pullStrength, ForceMode.Impulse);
        }
    }

    //void PullSoldiersCloser()
    //{
    //    foreach (GameObject soldier in soldiers)
    //    {
    //        Rigidbody rb = soldier.GetComponent<Rigidbody>();

    //        Vector3 direction = (transform.position - soldier.transform.position).normalized;
    //        rb.AddForce(direction * pullStrength, ForceMode.Impulse);
    //    }
    //}

    public void RemoveSoldiers(int count)
    {
        for (int i = 0; i < count && soldiers.Count > 0; i++)
        {
            GameObject soldierToRemove = soldiers[soldiers.Count - 1];
            soldiers.Remove(soldierToRemove);
            Destroy(soldierToRemove);
        }
    }
}
