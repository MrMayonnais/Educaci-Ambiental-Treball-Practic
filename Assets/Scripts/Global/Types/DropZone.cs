using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Global.Types
{
    public class DropZone : MonoBehaviour, IDropHandler
    {
        public List<DraggableItem> correctMatches;

        private DraggableItem _currentItem;
    
        private Tween imageTween;
    
        private void Start()
        {
        }
    
        private void OnEnable()
        {
            GameEvents.OnRestartLevel += ClearItem;
            GameEvents.AppearDropZoneImage += ShowImage;
            GameEvents.ForceDisappearDropZoneImage += HideImage;
            GameEvents.ForceItemReturn += CheckReturn;
        }

        public void OnDisable()
        {
            GameEvents.OnRestartLevel -= ClearItem;
            GameEvents.AppearDropZoneImage -= ShowImage;
            GameEvents.ForceDisappearDropZoneImage -= HideImage;
            GameEvents.ForceItemReturn -= CheckReturn;
        }

        public void OnDrop(PointerEventData eventData)
        {
        }
    
        public bool HasItem()
        {
            return _currentItem != null;
        }
    
        public void SetITem(DraggableItem item)
        {
            _currentItem = item;
        }
    
        private void ClearItem()
        {
            _currentItem = null;
        }

        public List<DraggableItem> GetCorrectItems()
        {
            return correctMatches;
        }
    
        private void ShowImage(DropZone dropZone)
        {
            if (dropZone != this) return;
        
            var itemImage = _currentItem.GetComponent<Image>();
            if (itemImage)
            {
                var c = itemImage.color;
                c.a = 0f;
                itemImage.color = c;
            }
        
        
            var image = Dlcs.Extensions.GetChildByName(gameObject, "image")?.GetComponent<Image>();
            if(image)imageTween = Tween.Color(image, endValue: new Color(1,1,1,1), duration: 0.5f);
        }

        private void HideImage(DropZone dropZone)
        {
            if (dropZone != this) return;
            imageTween.Stop();

            var image = Dlcs.Extensions.GetChildByName(gameObject, "image").GetComponent<Image>();
            var c = image.color;
            c.a = 0f;
            image.color = c;
        }

        private void CheckReturn(DraggableItem item)
        {
            if (item == _currentItem) 
                ClearItem();
        }

        public void SetItemText(string text, string specialText)
        {
            var normalTextComponent = Dlcs.Extensions.GetChildByName(gameObject, "Text")?.GetComponent<TextMeshProUGUI>();
            if (normalTextComponent != null)
            {
                normalTextComponent.text = text;
            
                var specialTextComponent = Dlcs.Extensions.GetChildByName(normalTextComponent.gameObject, "SpecialText")?.GetComponent<TextMeshProUGUI>();
                if (specialTextComponent != null)
                {
                    specialTextComponent.text = specialText;
                }
            }
        }
        
    }
}
