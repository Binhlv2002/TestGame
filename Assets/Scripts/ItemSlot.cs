using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public TextMeshProUGUI scoreText;
    private static int score = 0;
    public GameObject coinPrefab;
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggedItem = dropped.GetComponent<DraggableItem>();
            draggedItem.parentAfterDrag = transform;

            dropped.transform.SetParent(transform);
            dropped.transform.localPosition = Vector3.zero;

            CheckAndDeleteAdjacentItems();
        }
    }

    private void CheckAndDeleteAdjacentItems()
    {
        Vector2Int currentPos = GetSlotPosition();

        if (CheckAndDeleteIfMatched(currentPos, Vector2Int.right)) return;
        if (CheckAndDeleteIfMatched(currentPos, Vector2Int.left)) return;
        if (CheckAndDeleteIfMatched(currentPos, Vector2Int.up)) return;
        if (CheckAndDeleteIfMatched(currentPos, Vector2Int.down)) return;
    }

    private bool CheckAndDeleteIfMatched(Vector2Int currentPos, Vector2Int direction)
    {
        ItemSlot adjacentSlot = GetAdjacentSlot(currentPos, direction);
        if (adjacentSlot != null && adjacentSlot.transform.childCount > 0 && transform.childCount > 0)
        {
            Transform currentItem = transform.GetChild(0);
            Transform adjacentItem = adjacentSlot.transform.GetChild(0);

            if (currentItem.name == adjacentItem.name)
            {
                Destroy(currentItem.gameObject);
                Destroy(adjacentItem.gameObject);
                
                ShowCoinAnimation(currentItem, adjacentItem);
    
                IncrementScore();

                return true;
            }
        }
        return false;
    }
    private void ShowCoinAnimation(Transform currentItem, Transform adjacentItem)
    {
        if (coinPrefab != null)
        {
           
            GameObject coin = Instantiate(coinPrefab);

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                coin.transform.SetParent(canvas.transform, false);
                coin.transform.SetAsLastSibling();

                Vector3 currentWorldPos = currentItem.position;
                Vector3 adjacentWorldPos = adjacentItem.position;
                Vector3 middleWorldPos = (currentWorldPos + adjacentWorldPos) / 2f;

                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, middleWorldPos);
                Vector2 canvasPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, canvas.worldCamera, out canvasPosition);

                
                coin.GetComponent<RectTransform>().anchoredPosition = canvasPosition;

                
                Animator coinAnimator = coin.GetComponent<Animator>();

                
                if (coinAnimator != null)
                {
                    coinAnimator.Play("Coin"); 
                }
                

                StartCoroutine(DestroyCoinAfterDelay(coin, 1));
            }
            else
            {
                Destroy(coin);
            }
        }

    }

    private IEnumerator DestroyCoinAfterDelay(GameObject coin, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(coin);
    }
    private Vector2Int GetSlotPosition()
    {
        string[] nameParts = gameObject.name.Split('_');
        int x = int.Parse(nameParts[1]);
        int y = int.Parse(nameParts[2]);
        return new Vector2Int(x, y);
    }

    private ItemSlot GetAdjacentSlot(Vector2Int currentPos, Vector2Int direction)
    {
        Vector2Int adjacentPos = currentPos + direction;
        string adjacentSlotName = "ItemSlot_" + adjacentPos.x + "_" + adjacentPos.y;
        GameObject adjacentSlotObject = GameObject.Find(adjacentSlotName);
        return adjacentSlotObject != null ? adjacentSlotObject.GetComponent<ItemSlot>() : null;
    }

    private void IncrementScore()
    {
        score++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }
      
    }
}
