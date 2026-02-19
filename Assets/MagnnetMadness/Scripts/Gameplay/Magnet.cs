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

    [Header("Magnets Colors Settings")]
    public Image magnetImage;
    public GameObject waveEffect;

    [Header("Drag Collision Shake Settings")]
    public float maxShakeStrength = 10f;
    public float minShakeStrength = 2f;
    public float shakeDistance = 120f;
    public float dragShakeDuration = 0.15f;

    [Header("Visual Root")]
    public Transform visualRoot;

    #endregion

    #region ====== State Variables ======

    private bool isDragging = false;
    private bool isInCircle = false;
    private bool hasBeenDropped = false;

    private Camera mainCamera;
    private CircleCollider2D colliderRef;
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private CanvasGroup canvasGroup;

    [Header("Drag Offest Settings")]
    public Vector3 dragOffset = new(0, 100f, 0);
    public float dragSmoothSpeed = 15f;
    private Vector3 smoothDragTarget;
    private Tween activeTween;

    public float highlightScaleUp = 1.15f;
    public float highlightDuration = 0.15f;

    private Dictionary<Magnet, Tween> dragShakeTweens = new();

    #endregion

    #region ====== Unity Lifecycle ======

    private void OnEnable()
    {
        mainCamera = Camera.main;
        colliderRef = GetComponent<CircleCollider2D>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();

        if (waveEffect != null)
            waveEffect.SetActive(false);

        if (colliderRef != null)
            colliderRef.isTrigger = true;
    }

    void Update()
    {
        if (!isDragging) return;

        rectTransform.position = Vector3.Lerp(
            rectTransform.position,
            smoothDragTarget + dragOffset,
            Time.deltaTime * dragSmoothSpeed
        );

        HandleDragCollisionShake();
    }

    #endregion

    #region ====== Public Setup ======

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

        GameManager.Instance.activeDraggingMagnet = this;

        if (waveEffect != null)
            waveEffect.SetActive(true);

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

        GameManager.Instance.activeDraggingMagnet = null;

        waveEffect.SetActive(false);
        StopAllDragShakes();
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
        waveEffect.SetActive(false);
        StopAllDragShakes();

        hasBeenDropped = true;
        isInCircle = true;

        GameManager.Instance.RegisterMagnetInCircle(this);
        GameManager.Instance.RemoveMagnetFromPlayer(owner);
        GameManager.Instance.AddPlaced(owner);

        List<Magnet> hits = GetAllHits();

        if (hits.Count > 0)
        {
            HandleMultipleHits(hits);
            return;
        }

        SetMagnetCanvasGroupAlpha(isInCircle);
        GameManager.Instance.CheckWinCondition();
        GameManager.Instance.SwitchTurn();
        UIManager.Instance.UpdateGameplayUI();
    }

    List<Magnet> GetAllHits()
    {
        List<Magnet> collided = new();

        foreach (Magnet other in GameManager.Instance.GetMagnetsInCircle())
        {
            if (colliderRef != null && colliderRef.IsTouching(other.GetComponent<Collider2D>()))
                collided.Add(other);
        }

        return collided;
    }

    void HandleMultipleHits(List<Magnet> hitMagnets)
    {
        StopAllDragShakes();

        AudioManager.Instance?.PlayMagnetCollapse();

        MagnetShake();

        foreach (var m in hitMagnets)
            m.MagnetShake();

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
            GameManager.Instance.RemovePlaced(hitMagnet.owner);
            GameManager.Instance.magnetSpawner.RespawnMagnet(owner);

            Destroy(hitMagnet.gameObject);
        }

        GameManager.Instance.UnregisterMagnetFromCircle(this);
        GameManager.Instance.AddMagnetToPlayer(owner);
        GameManager.Instance.RemovePlaced(owner);

        SmoothReturnToSlot();

        waveEffect.SetActive(false);
        hasBeenDropped = false;
        isInCircle = false;

        SetMagnetCanvasGroupAlpha(isInCircle);
        UIManager.Instance.UpdateGameplayUI();
        GameManager.Instance.CheckWinCondition();
    }
    private void SetMagnetCanvasGroupAlpha(bool isInsideCircle)
    {
        canvasGroup.ignoreParentGroups = isInsideCircle;
    }

    #endregion

    #region ====== Drag Collision Shake ======

    void HandleDragCollisionShake()
    {
        if (!GameManager.Instance.circleAreaCollider.OverlapPoint(transform.position))
        {
            StopAllDragShakes();
            return;
        }

        bool touchingAny = false;
        float closestStrength = 0f;

        foreach (Magnet other in GameManager.Instance.GetMagnetsInCircle())
        {
            if (other == this) continue;

            Collider2D otherCol = other.GetComponent<Collider2D>();
            if (colliderRef == null || otherCol == null) continue;

            if (colliderRef.IsTouching(otherCol))
            {
                touchingAny = true;

                float dist = Vector3.Distance(transform.position, other.transform.position);
                float t = Mathf.InverseLerp(shakeDistance, 0f, dist);
                float strength = Mathf.Lerp(minShakeStrength, maxShakeStrength, t);

                StartOrUpdateDragShake(other, strength);
                closestStrength = Mathf.Max(closestStrength, strength);
            }
            else
            {
                StopDragShake(other);
            }
        }

        if (touchingAny)
            StartOrUpdateSelfDragShake(closestStrength);
        else
            StopSelfDragShake();
    }

    void StartOrUpdateDragShake(Magnet target, float strength)
    {
        if (dragShakeTweens.ContainsKey(target)) return;

        Tween t = target.visualRoot.DOShakePosition(
            dragShakeDuration,
            strength,
            15,
            90,
            false,
            true
        ).SetLoops(-1).SetEase(Ease.Linear);

        dragShakeTweens[target] = t;
    }

    void StartOrUpdateSelfDragShake(float strength)
    {
        if (dragShakeTweens.ContainsKey(this)) return;

        Tween t = visualRoot.DOShakePosition(
            dragShakeDuration,
            strength,
            15,
            90,
            false,
            true
        ).SetLoops(-1).SetEase(Ease.Linear);

        dragShakeTweens[this] = t;
    }

    void StopDragShake(Magnet target)
    {
        if (!dragShakeTweens.ContainsKey(target)) return;

        dragShakeTweens[target].Kill();
        dragShakeTweens.Remove(target);
        target.visualRoot.localPosition = Vector3.zero;

        Debug.Log("Stop Shaking--------------");
    }

    void StopSelfDragShake()
    {
        if (!dragShakeTweens.ContainsKey(this)) return;

        dragShakeTweens[this].Kill();
        dragShakeTweens.Remove(this);
        this.visualRoot.localPosition = Vector3.zero;

        Debug.Log("Stop SELF Shaking--------------");
    }

    void StopAllDragShakes()
    {
        foreach (var t in dragShakeTweens)
        {
            t.Value.Kill();
            t.Key.visualRoot.localPosition = Vector3.zero;
        }

        dragShakeTweens.Clear();
    }

    #endregion

    #region ====== Visual Effects ======

    void HighlightOnTouch()
    {
        transform.DOScale(highlightScaleUp, highlightDuration)
            .SetEase(Ease.OutBack);
    }

    void SmoothReturnToSlot()
    {
        Transform slot = GameManager.Instance.magnetSpawner.playerSlots[owner][slotIndex];

        KillTween();
        activeTween = rectTransform.DOMove(slot.position, 0.5f)
            .SetEase(Ease.OutCubic);
    }

    public void ForceReturnToSlot()
    {
        isDragging = false;
        hasBeenDropped = false;
        isInCircle = false;

        waveEffect?.SetActive(false);
        StopAllDragShakes();
        KillTween();

        transform.DOScale(1f, highlightDuration);
        SmoothReturnToSlot();
    }

    void MagnetShake()
    {
        KillTween();
        activeTween = visualRoot.DOShakePosition(
            0.5f, 10f, 20, 90, false, true
        );

        CameraShakeManager.Instance.ShakeCamera(0.2f, 20f, true);
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
