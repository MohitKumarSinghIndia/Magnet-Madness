using UnityEngine;
using System.Collections.Generic;

public class Magnet : MonoBehaviour
{
    public PlayerTurn owner;
    public int slotIndex = -1;

    public float minDistanceToPickup = 1f;

    private bool isDragging = false;
    private bool isInCircle = false;
    private bool hasBeenDropped = false;

    public Color player1Color = Color.blue;
    public Color player2Color = Color.red;

    private Vector3 dragOffset;

    private Camera mainCamera;
    private SpriteRenderer rendererRef;
    private CircleCollider2D colliderRef;

    void Awake()
    {
        mainCamera = Camera.main;
        rendererRef = GetComponent<SpriteRenderer>();
        colliderRef = GetComponent<CircleCollider2D>();
        colliderRef.isTrigger = true;
    }

    void Start()
    {
        rendererRef.color = (owner == PlayerTurn.Player1) ? player1Color : player2Color;
    }

    public void SetOwner(PlayerTurn p)
    {
        owner = p;
        rendererRef.color = (owner == PlayerTurn.Player1) ? player1Color : player2Color;
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver() || hasBeenDropped) return;
        if (owner != GameManager.Instance.currentTurn) return;

        HandleMouse();
        HandleTouch();
    }

    void HandleMouse()
    {
        if (Input.touchCount > 0) return;

        Vector3 pos = ScreenToWorld(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) &&
            Vector3.Distance(transform.position, pos) <= minDistanceToPickup)
            BeginDrag(pos);

        if (Input.GetMouseButton(0) && isDragging)
            transform.position = pos + dragOffset;

        if (Input.GetMouseButtonUp(0) && isDragging)
            EndDrag();
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);
        Vector3 pos = ScreenToWorld(t.position);

        if (t.phase == TouchPhase.Began &&
            Vector3.Distance(transform.position, pos) <= minDistanceToPickup)
            BeginDrag(pos);

        if (t.phase == TouchPhase.Moved && isDragging)
            transform.position = pos + dragOffset;

        if ((t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) && isDragging)
            EndDrag();
    }

    void BeginDrag(Vector3 pointer)
    {
        dragOffset = transform.position - pointer;
        isDragging = true;
        rendererRef.sortingOrder = 10;
    }

    void EndDrag()
    {
        isDragging = false;
        rendererRef.sortingOrder = 1;

        if (GameManager.Instance.circleAreaCollider.OverlapPoint(transform.position))
            PlaceInCircle();
        else
            ReturnToSlot();
    }

    void PlaceInCircle()
    {
        hasBeenDropped = true;
        isInCircle = true;

        GameManager.Instance.RegisterMagnetInCircle(this);
        GameManager.Instance.RemoveMagnetFromPlayer(owner);
        GameManager.Instance.AddPlaced(owner);

        CheckHits();

        GameManager.Instance.CheckWinCondition();
        GameManager.Instance.SwitchTurn();
    }

    void CheckHits()
    {
        List<Magnet> copy = new List<Magnet>(GameManager.Instance.GetMagnetsInCircle());

        foreach (Magnet other in copy)
        {
            if (other == null || other == this) continue;

            if (colliderRef.IsTouching(other.GetComponent<Collider2D>()))
            {
                // Remove OTHER magnet
                GameManager.Instance.UnregisterMagnetFromCircle(other);
                GameManager.Instance.AddMagnetToPlayer(owner);
                GameManager.Instance.RemovePlaced(owner);
                GameManager.Instance.magnetSpawner.RespawnMagnet(owner);

                Destroy(other.gameObject);

                // Remove THIS magnet
                GameManager.Instance.UnregisterMagnetFromCircle(this);
                GameManager.Instance.AddMagnetToPlayer(owner);
                GameManager.Instance.RemovePlaced(owner);
                ReturnToSlot();

                hasBeenDropped = false;
                isInCircle = false;

                UIManager.Instance.UpdateUI();
                return;
            }
        }
    }

    void ReturnToSlot()
    {
        Transform slot = GameManager.Instance.magnetSpawner.playerSlots[owner][slotIndex];
        transform.position = slot.position;
    }

    Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 pos = screenPos;
        pos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(pos);
    }
}
