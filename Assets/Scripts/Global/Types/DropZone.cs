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
        private Tween textTween;
    
        private void Start()
        {
        }
    
        private void OnEnable()
        {
            GameEvents.OnRestartLevel += ClearAllItems;
            GameEvents.AppearDropZoneImage += ShowImage;
            GameEvents.ForceDisappearDropZoneImages += HideImage;
            GameEvents.ForceItemReturn += CheckReturn;
            DraggableItem.OnItemDropped += AddItem;
        }

        public void OnDisable()
        {
            GameEvents.OnRestartLevel -= ClearAllItems;
            GameEvents.AppearDropZoneImage -= ShowImage;
            GameEvents.ForceDisappearDropZoneImages -= HideImage;
            GameEvents.ForceItemReturn -= CheckReturn;
            DraggableItem.OnItemDropped -= AddItem;
        }

        public void OnDrop(PointerEventData eventData)
        {
        }
    
        public bool HasItem()
        {
            return _currentItems.Count > 0;
        }
        
        public bool HasItem(DraggableItem item)
        {
            return _currentItems.Contains(item);
        }

        private void AddItem(DraggableItem item, DropZone dropZone)
        {
            if(dropZone == this)
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
            
            var imageText = Dlcs.Extensions.GetChildByName(image?.gameObject, "Text")?.GetComponent<TextMeshProUGUI>();
            
            
            if(image)imageTween = Tween.Color(image, endValue: new Color(1,1,1,1), duration: 0.5f);
            if(imageText)textTween = Tween.Color(imageText, endValue: new Color(1,1,1,1), duration: 0.5f);
        }

        private void HideImage(DropZone dropZone)
        {
            if (dropZone != this) return;
            imageTween.Stop();
            textTween.Stop();

            var images = dropZone.GetComponentsInChildren<Image>();
            
            foreach (var image in images)
            {

                if (image.gameObject == this.gameObject) continue;
                
                var c = image.color;
                    c.a = 0f;
                    image.color = c;
                    Debug.Log("hid image: " + image.gameObject.name);
                    
                var text = image.gameObject.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
                if (text)
                {
                    var c2 = text.color;
                    c2.a = 0f;
                    text.color = c2;
                }
            }
        }

        private void CheckReturn(DraggableItem item)
        {
            if (_currentItems.Contains(item))
            {
                item.VisibleText();
                ClearItem(item);
            }
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
