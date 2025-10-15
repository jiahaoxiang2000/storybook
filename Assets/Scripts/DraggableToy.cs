using UnityEngine;

public class DraggableToy : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private Vector3 originalPosition;
    
    void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;
    }
    
    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }
    
    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }
    
    void OnMouseUp()
    {
        isDragging = false;
    }
    
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
    
    public void HideToy()
    {
        gameObject.SetActive(false);
    }
}
