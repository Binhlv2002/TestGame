using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public RectTransform spawnArea;

    private List<Vector3> spawnPositions = new List<Vector3>();
    private List<GameObject> spawnedItems = new List<GameObject>();

    void Start()
    {
        SpawnItems();
    }

    void Update()
    {

        if (IsPanelEmpty())
        {
            SpawnItems();
        }
    }

    void SpawnItems()
    {

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                Vector3 spawnPos = GetRandomPositionInSpawnArea();
                GameObject item = Instantiate(itemPrefabs[i], spawnPos, Quaternion.identity, spawnArea);
                item.name = itemPrefabs[i].name;

                RectTransform itemRect = item.GetComponent<RectTransform>();
                if (itemRect != null)
                {
                    itemRect.anchoredPosition = spawnPos;
                }

                spawnPositions.Add(spawnPos);
                spawnedItems.Add(item);


                DraggableItem draggable = item.GetComponent<DraggableItem>();
                if (draggable != null)
                {
                    draggable.initialPosition = spawnPos;
                }
            }
        }
    }

    Vector3 GetRandomPositionInSpawnArea()
    {
        float x = Random.Range(spawnArea.rect.xMin, spawnArea.rect.xMax);
        float y = Random.Range(spawnArea.rect.yMin, spawnArea.rect.yMax);
        return new Vector3(x, y, 0);
    }

    bool IsPanelEmpty()
    {

        foreach (Transform child in spawnArea)
        {
            if (child.gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }
}
