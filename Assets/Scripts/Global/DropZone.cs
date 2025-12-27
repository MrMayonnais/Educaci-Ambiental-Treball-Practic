using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public int zoneIndex;
    public DragQuestionManager manager;

    private DraggableItem _currentItem;
    
    public void OnDrop(PointerEventData eventData)
    {//noop
    }
    
    public bool HasItem()
    {
        return _currentItem != null;
    }
    
    public void SetITem(DraggableItem item)
    {
        _currentItem = item;
    }
    
    public void ClearItem()
    {
        _currentItem = null;
    }
        
}
