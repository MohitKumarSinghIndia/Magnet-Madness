using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Magnet : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region ====== Inspector Variables ======

    [Header("Owner / Slot")]
    public PlayerTurn owner;
    public int slotIndex = -1;

    [Header("Pickup Settings")]
    public float minDistanceToPickup = 1f;

    [Header("Colors")]
    public Color player1Color = Color.blue;
    public Color player2Color = Color.red;

    #endregion

    #region ====== State Variables ======

    private bool isDragging = false;
    private bool isInCircle = false;
    private bool hasBeenDropped = false;

    private Camera mainCamera;
    private Image rendererImage;
    private CircleCollider2D colliderRef;
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private Tween activeTween;
    private Vector3 smoothDragTarget;
    public Vector3 dragOffset = new(0, .25f, 0);
    public float dragSmoothSpeed = 15f;

    public float highlightScaleUp = 1.15f;
    public float highlightDuration = 0.15f;

    #endregion

    #region ====== Unity Lifecycle ======

    void Awake()
    {
        mainCamera = Camera.main;
        rendererImage = GetComponent<Image>();
        colliderRef = GetComponent<CircleCollider2D>();
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        if (colliderRef != null)
            colliderRef.isTrigger = true;
    }

    void Start()
    {
        rendererImage.color = (owner == PlayerTurn.Player1) ? player1Color : player2Color;
    }

    void Update()
    {
        if (isDragging)
        {
            rectTransform.position = Vector3.Lerp(
                rectTransform.position,
                smoothDragTarget + dragOffset,
                Time.deltaTime * dragSmoothSpeed
            );
        }
    }

    #endregion

    #region ====== Public Setup ======

    public void SetOwner(PlayerTurn p)
    {
        owner = p;
        rendererImage.color = (owner == PlayerTurn.Player1) ? player1Color : player2Color;
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    #endregion

    #region ====== Drag Handling ======

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsGameOver()) return;
        if (hasBeenDropped) return;
        if (owner != GameManager.Instance.currentTurn) return;

        isDragging = true;
        KillTween();

        HighlightOnTouch();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 localPoint;

        Camera cam = (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                     ? null
                     : parentCanvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                eventData.position,
                cam,
                out localPoint))
        {
            smoothDragTarget = parentCanvas.transform.TransformPoint(localPoint);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        transform.DOScale(1f, highlightDuration);

        if (GameManager.Instance.circleAreaCollider.OverlapPoint(transform.position))
            PlaceInCircle();
        else
            SmoothReturnToSlot();
    }

    #endregion

    #region ====== Game Logic ======

    void PlaceInCircle()
    {
        hasBeenDropped = true;
        isInCircle = true;

        GameManager.Instance.RegisterMagnetInCircle(this);
        GameManager.Instance.RemoveMagnetFromPlayer(owner);
        GameManager.Instance.AddPlaced(owner);

        // MULTIPLE COLLISION CHECK
        List<Magnet> hits = GetAllHits();

        if (hits.Count > 0)
        {
            HandleMultipleHits(hits);
            return;
        }

        // Normal flow
        GameManager.Instance.CheckWinCondition();
        GameManager.Instance.SwitchTurn();
        UIManager.Instance.UpdateUI();
    }

    // Returns ALL magnets touching this magnet
    List<Magnet> GetAllHits()
    {
        List<Magnet> collided = new List<Magnet>();

        foreach (Magnet other in GameManager.Instance.GetMagnetsInCircle())
        {
            if (other == this) continue;

            if (colliderRef != null && colliderRef.IsTouching(other.GetComponent<Collider2D>()))
            {
                collided.Add(other);
            }
        }

        return collided;
    }

    void HandleMultipleHits(List<Magnet> hitMagnets)
    {
        // Shake this magnet
        MagnetShake();

        // Vibrate on collision
        HapticManager.VibrateMedium();

        // Shake all hit magnets
        foreach (var m in hitMagnets)
            m.MagnetShake();

        // Process after a delay
        DOVirtual.DelayedCall(0.5f, () =>
        {
            ProcessMultipleCollisions(hitMagnets);
        });
    }

    void ProcessMultipleCollisions(List<Magnet> magnetList)
    {
        foreach (Magnet hitMagnet in magnetList)
        {
            GameManager.Instance.UnregisterMagnetFromCircle(hitMagnet);
            GameManager.Instance.AddMagnetToPlayer(owner);
            GameManager.Instance.RemovePlaced(owner);
            GameManager.Instance.magnetSpawner.RespawnMagnet(owner);

            Destroy(hitMagnet.gameObject);
        }

        // Remove THIS magnet also
        GameManager.Instance.UnregisterMagnetFromCircle(this);
        GameManager.Instance.AddMagnetToPlayer(owner);
        GameManager.Instance.RemovePlaced(owner);

        SmoothReturnToSlot();

        hasBeenDropped = false;
        isInCircle = false;

        UIManager.Instance.UpdateUI();
    }

    #endregion

    #region ====== Visual Effects ======

    void HighlightOnTouch()
    {
        transform.DOScale(highlightScaleUp, highlightDuration).SetEase(Ease.OutBack);
    }

    void SmoothReturnToSlot()
    {
        Transform slot = GameManager.Instance.magnetSpawner.playerSlots[owner][slotIndex];

        KillTween();
        activeTween = rectTransform.DOMove(slot.position, 0.5f).SetEase(Ease.OutCubic);
    }

    void MagnetShake()
    {
        KillTween();
        activeTween = transform.DOShakePosition(0.5f, 10f, 20, 90, false, true);
    }

    void KillTween()
    {
        if (activeTween != null)
        {
            activeTween.Kill();
            activeTween = null;
        }
    }

    #endregion
}
