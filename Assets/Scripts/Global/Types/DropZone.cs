using System.Collections.Generic;
using Dlcs;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Global.Types
{
    public class DropZone : MonoBehaviour, IDropHandler
    {
        
        public bool multiDropAllowed = false;
        
        public List<DraggableItem> correctMatches;

        private List<DraggableItem> _currentItems = new List<DraggableItem>();
    
        private Tween imageTween;
    
        private void Start()
        {
        }
    
        private void OnEnable()
        {
            GameEvents.OnRestartLevel += ClearAllItems;
            GameEvents.AppearDropZoneImage += ShowImage;
            GameEvents.ForceDisappearDropZoneImages += HideImage;
            GameEvents.ForceItemReturn += CheckReturn;
        }

        public void OnDisable()
        {
            GameEvents.OnRestartLevel -= ClearAllItems;
            GameEvents.AppearDropZoneImage -= ShowImage;
            GameEvents.ForceDisappearDropZoneImages -= HideImage;
            GameEvents.ForceItemReturn -= CheckReturn;
        }

        public void OnDrop(PointerEventData eventData)
        {
        }
    
        public bool HasItem()
        {
            return _currentItems.Count > 0;
        }
    
        public void SetITem(DraggableItem item)
        {
            _currentItems.Add(item);
        }
    
        private void ClearItem(DraggableItem item)
        {
            _currentItems.Remove(item);
        }
        
        private void ClearAllItems()
        {
            _currentItems.Clear();
        }

        public List<DraggableItem> GetCorrectItems()
        {
            return correctMatches;
        }
    
        private void ShowImage(DropZone dropZone, DraggableItem item)
        {
            if (dropZone != this) return;
            
            var itemImage = item.GetComponent<Image>();
            if (itemImage)
            {
                var c = itemImage.color;
                c.a = 0f;
                itemImage.color = c;
            }

            TextMeshProUGUI itemText = null;

            if (itemImage)
            {
                itemText = Extensions.GetChildByName(item.gameObject, "Text")?.GetComponent<TextMeshProUGUI>();
                if (itemText)
                {
                    var c = itemText.color;
                    c.a = 0f;
                    itemText.color = c;
                }
            }

            if (itemText)
            {
                var itemSpecialText = Extensions.GetChildByName(itemText.gameObject, "Text")?
                    .transform.Find("SpecialText")?.GetComponent<TextMeshProUGUI>();
                if (itemSpecialText)
                {
                    var c = itemSpecialText.color;
                    c.a = 0f;
                    itemSpecialText.color = c;
                }
            }
        
        
            var image = Dlcs.Extensions.GetChildByName(gameObject, "image_"+item.name)?.GetComponent<Image>();
            if(image)imageTween = Tween.Color(image, endValue: new Color(1,1,1,1), duration: 0.5f);
        }

        private void HideImage(DropZone dropZone)
        {
            if (dropZone != this) return;
            imageTween.Stop();

            var images = dropZone.GetComponentsInChildren<Image>();
            
            foreach (var image in images)
            {

                if (image.gameObject == this.gameObject) continue;
                
                var c = image.color;
                    c.a = 0f;
                    image.color = c;
                    Debug.Log("hid image: " + image.gameObject.name);
            }
        }

        private void CheckReturn(DraggableItem item)
        {
            if (_currentItems.Contains(item)) 
                ClearItem(item);
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
