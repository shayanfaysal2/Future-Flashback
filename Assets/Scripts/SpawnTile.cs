using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTile : MonoBehaviour
{
    private Vector3 direction;
    private Vector3 mainDirection;
    private Vector3 otherDirection;

    public Transform player;
    private Vector3 endPos;
    public GameObject[] prefabs;
    public GameObject[] rtTurnPrefabs;
    public GameObject[] ltTurnPrefabs;
    public GameObject startPrefab;
    public float playerDistance;

    // Start is called before the first frame update
    void Start()
    {
        mainDirection = Vector3.forward;
        otherDirection = Vector3.right;   

        direction = mainDirection;

        SpawnPrefab(startPrefab.transform.Find("EndPos").position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.position, endPos) < playerDistance)
        {
            SpawnPrefab(endPos);
        } 
    }

    void SpawnPrefab(Vector3 pos)
    {
        float x = Random.value;

        if (x < 0.5f)
        {
            direction = mainDirection;
        }
        else
        {
            if (direction == Vector3.forward)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);

                GameObject newPrefab = Instantiate(rtTurnPrefabs[Random.Range(0, rtTurnPrefabs.Length)], pos, rotation);
                endPos = newPrefab.transform.Find("EndPos").position;

                pos = endPos;
            }
            else
            {
                Quaternion rotation = Quaternion.LookRotation(direction);

                GameObject newPrefab = Instantiate(ltTurnPrefabs[Random.Range(0, ltTurnPrefabs.Length)], pos, rotation);
                endPos = newPrefab.transform.Find("EndPos").position;

                pos = endPos;
            }

            Vector3 temp = direction;
            direction = otherDirection;
            mainDirection = direction;
            otherDirection = temp;
        }

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);

            int a = Random.Range(0, 3);
            float b;
            if (a == 0)
                b = -2.5f;
            else if (a == 1)
                b = 2.5f;
            else
                b = 0;

            GameObject newPrefab = Instantiate(prefabs[Random.Range(0, prefabs.Length)], pos, rotation);
            Transform new3Coins = newPrefab.transform.Find("3Coins");
            if (new3Coins != null)
            {
                new3Coins.position += new Vector3(b, 0, 0);
            }
            endPos = newPrefab.transform.Find("EndPos").position;
        }
    }
}