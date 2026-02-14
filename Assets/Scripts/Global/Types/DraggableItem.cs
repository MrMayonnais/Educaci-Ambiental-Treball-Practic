using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Global.Types
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Vector2 _originalPosition;
        private Transform _originalParent;
        private bool _isPlaced;
        
        private Tween _returnTween;
    
        public static Action<DraggableItem, DropZone> OnItemDropped;

        private void OnEnable()
        {
            GameEvents.ForceItemReturn += CheckReturn;
        }
    
        private void OnDisable()
        {
            GameEvents.ForceItemReturn -= CheckReturn;
        }
    
        private void Awake()
        {
            _originalParent = transform.parent;
            _originalPosition = transform.position;
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            if (!_canvasGroup) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isPlaced) return;
        
            _canvasGroup.blocksRaycasts = false;
        
            transform.SetParent(_canvas.transform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isPlaced) return;
    
            // Usar la posición del puntero directamente en lugar de delta
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                eventData.position,
                _canvas.worldCamera,
                out Vector2 localPoint
            );
    
            _rectTransform.anchoredPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isPlaced) return;
        
            _canvasGroup.blocksRaycasts = true;

            DropZone dropZone = null;
        
            if (eventData.pointerEnter != null)
            {
                dropZone = eventData.pointerEnter.GetComponent<DropZone>();
            }

            if (dropZone != null && !dropZone.HasItem(this))
            {
                transform.SetParent(dropZone.transform);
                _rectTransform.anchoredPosition = Vector2.zero;
                _isPlaced = true;
                _canvasGroup.blocksRaycasts = false;
                OnItemDropped?.Invoke(this, dropZone);
            }
            else
            {
                Debug.Log("Item not dropped on a valid drop zone, returning to original position: " + gameObject.name);
                ReturnToOriginalPosition();
            }
        }
    
        private void CheckReturn(DraggableItem item)
        {
            if (item == this && _isPlaced)
            {
                Debug.Log("Forcing return of item: " + gameObject.name);
                ReturnToOriginalPosition();
            }
        }
    
        private void ReturnToOriginalPosition()
        {
            gameObject.SetActive(true);
        
            transform.SetParent(_originalParent);
            _isPlaced = false;
            _canvasGroup.blocksRaycasts = false;
            var c = GetComponent<Image>().color;
            c.a = 1f;
            GetComponent<Image>().color = c;

            _returnTween.Stop();
            _returnTween = Tween.Position(_rectTransform, _originalPosition, 0.5f, Easing.Overshoot(0.5f)).OnComplete(() =>
            {
                _canvasGroup.blocksRaycasts = true;
            });
        }
    
        public void SetItemText(string text, string specialText)
        {
            var textComponent = Dlcs.Extensions.GetChildByName(gameObject, "Text")?.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = text;
            
                var specialTextComponent = Dlcs.Extensions.GetChildByName(textComponent.gameObject, "SpecialText")?.GetComponent<TextMeshProUGUI>();
                if (specialTextComponent != null)
                {
                    specialTextComponent.text = specialText;
                }
            }
        }

        public void VisibleText()
        {
            var textComponent = Dlcs.Extensions.GetChildByName(gameObject, "Text")?.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                var c = textComponent.color;
                c.a = 1f;
                textComponent.color = c;

                var specialTextComponent = Dlcs.Extensions.GetChildByName(textComponent.gameObject, "SpecialText")
                    ?.GetComponent<TextMeshProUGUI>();
                if (specialTextComponent != null)
                {
                    var sc = specialTextComponent.color;
                    sc.a = 1f;
                    specialTextComponent.color = sc;
                }
            }
        }
    }
}
