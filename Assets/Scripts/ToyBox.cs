using UnityEngine;

public class ToyBox : MonoBehaviour
{
    [SerializeField] private Sprite normalBoxSprite;  // Drag empty box sprite here
    [SerializeField] private Sprite fullBoxSprite;    // Drag full box sprite here
    private SpriteRenderer spriteRenderer;
    private int toysCollected = 0;
    private int totalToys = 3;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (normalBoxSprite != null)
        {
            spriteRenderer.sprite = normalBoxSprite;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Toy"))
        {
            DraggableToy toy = other.GetComponent<DraggableToy>();
            if (toy != null)
            {
                toy.HideToy();
                toysCollected++;
                
                Debug.Log("Toys collected: " + toysCollected + "/" + totalToys);
                
                // When all toys collected, change box sprite
                if (toysCollected >= totalToys)
                {
                    if (fullBoxSprite != null)
                    {
                        spriteRenderer.sprite = fullBoxSprite;
                        Debug.Log("All toys collected! Box is full!");
                    }
                }
            }
        }
    }
}
