using System;
using Global;
using UnityEngine;
using UnityEngine.EventSystems;

/*public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int itemIndex;
    public DragQuestionManager manager;
    
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _originalPosition;
    private Transform _originalParent;
    private bool _isPlaced;

    private void OnEnable()
    {
        LevelManager.OnRestartLevel += ReturnToOriginalPosition;
    }
    
    private void OnDisable()
    {
        LevelManager.OnRestartLevel -= ReturnToOriginalPosition;
    }
    
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (!_canvasGroup) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isPlaced) return;
        
        _originalPosition = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;
        
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
        
        transform.SetParent(_canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isPlaced) return;
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isPlaced) return;

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        // Buscar DropZone en el objeto sobre el que se soltó (pointerEnter)
        DropZone dropZone = null;
        if (eventData.pointerEnter != null)
        {
            dropZone = eventData.pointerEnter.GetComponent<DropZone>();
            Debug.Log(dropZone);
        }

        if (dropZone != null && !dropZone.HasItem())
        {
            Debug.Log("Dropped on drop zone " + dropZone.zoneIndex);
            transform.SetParent(dropZone.transform);
            _rectTransform.anchoredPosition = Vector2.zero;
            _isPlaced = true;
            dropZone.SetITem(this);
            manager.OnItemDropped(itemIndex, dropZone.zoneIndex);
        }
        else
        {
            Debug.Log("Returning to original position");
            transform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = _originalPosition;
        }
    }

    public void ResetItem()
    {
        _isPlaced = false;
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }
    
    public void ReturnToOriginalPosition()
    {
        Debug.Log("Returning to original position");
        transform.SetParent(_originalParent);
        _rectTransform.anchoredPosition = _originalPosition;
        _isPlaced = false;
    }
}*/
