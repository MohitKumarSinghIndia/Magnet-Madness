using UnityEngine;
using System.Collections.Generic;

public class Magnet : MonoBehaviour
{
    [Header("Magnet Properties")]
    public PlayerTurn owner;
    public Color player1Color = Color.blue;
    public Color player2Color = Color.red;

    [Header("Drag Settings")]
    public float minDistanceToPickup = 1f;

    private bool isDragging = false;
    private bool isInCircle = false;
    private bool hasBeenDropped = false;

    private Vector3 dragOffset;
    private Vector3 originalPosition;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D magnetCollider;

    void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        magnetCollider = GetComponent<CircleCollider2D>();

        if (magnetCollider != null)
            magnetCollider.isTrigger = true;
    }

    void Start()
    {
        UpdateMagnetColor();
        originalPosition = transform.position; // Save original spawn point
    }

    void UpdateMagnetColor()
    {
        spriteRenderer.color = (owner == PlayerTurn.Player1) ? player1Color : player2Color;
    }

    // ---------------------- UPDATE (PC + Mobile) ----------------------
    void Update()
    {
        if (GameManager.Instance.IsGameOver() || hasBeenDropped) return;
        if (owner != GameManager.Instance.GetCurrentTurn()) return;
        if (GameManager.Instance.GetPlayerMagnetCount(owner) <= 0) return;

        HandleMouseInput();   // PC
        HandleTouchInput();   // Mobile
    }

    // ===================== PC MOUSE INPUT ======================
    void HandleMouseInput()
    {
        if (Input.touchCount > 0) return; // Avoid double detection

        Vector3 mousePos = GetPointerWorldPosition(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Vector3.Distance(transform.position, mousePos) <= minDistanceToPickup)
                StartDragging(mousePos);
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            transform.position = mousePos + dragOffset;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            StopDragging();
        }
    }

    // ===================== MOBILE TOUCH INPUT ======================
    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Vector3 touchPos = GetPointerWorldPosition(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (Vector3.Distance(transform.position, touchPos) <= minDistanceToPickup)
                    StartDragging(touchPos);
                break;

            case TouchPhase.Moved:
                if (isDragging)
                    transform.position = touchPos + dragOffset;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (isDragging)
                    StopDragging();
                break;
        }
    }

    // ======================= DRAG LOGIC ==========================
    void StartDragging(Vector3 pointerPos)
    {
        isDragging = true;
        dragOffset = transform.position - pointerPos;
        spriteRenderer.sortingOrder = 10; // Bring to front
    }

    void StopDragging()
    {
        isDragging = false;
        spriteRenderer.sortingOrder = 1;
        DropMagnet();
    }

    // ======================= DROP LOGIC ==========================
    void DropMagnet()
    {
        if (GameManager.Instance.circleAreaCollider == null) return;

        if (GameManager.Instance.circleAreaCollider.OverlapPoint(transform.position))
        {
            PlaceInCircle();
        }
        else
        {
            ReturnToHolder();
        }
    }

    void PlaceInCircle()
    {
        hasBeenDropped = true;
        isInCircle = true;

        GameManager.Instance.RegisterMagnetInCircle(this);
        GameManager.Instance.RemoveMagnetFromPlayer(owner);

        CheckCollisionsWithOtherMagnets();
        GameManager.Instance.SwitchTurn();
    }

    // =================== COLLISION COLLECTION ====================
    void CheckCollisionsWithOtherMagnets()
    {
        List<Magnet> magnetsInCircle = GameManager.Instance.GetMagnetsInCircle();
        List<Magnet> collectedMagnets = new List<Magnet>();

        foreach (Magnet otherMagnet in magnetsInCircle)
        {
            if (otherMagnet == this) continue;
            if (otherMagnet.owner == this.owner) continue;

            if (magnetCollider.IsTouching(otherMagnet.GetComponent<Collider2D>()))
            {
                collectedMagnets.Add(otherMagnet);
            }
        }

        foreach (Magnet collected in collectedMagnets)
        {
            GameManager.Instance.UnregisterMagnetFromCircle(collected);
            GameManager.Instance.AddMagnetToPlayer(owner);

            GameManager.Instance.magnetSpawner.RespawnMagnet(owner);

            Destroy(collected.gameObject);
        }
    }

    // ---------------- RETURN TO ORIGINAL POSITION ----------------
    void ReturnToHolder()
    {
        transform.position = originalPosition; // return to starting slot
    }

    // ---------------- UTILITY ----------------
    Vector3 GetPointerWorldPosition(Vector2 screenPos)
    {
        Vector3 pos = screenPos;
        pos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(pos);
    }

    void OnDestroy()
    {
        if (isInCircle)
            GameManager.Instance.UnregisterMagnetFromCircle(this);
    }

    public void SetOwner(PlayerTurn newOwner)
    {
        owner = newOwner;
        UpdateMagnetColor();
    }
}
