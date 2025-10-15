using UnityEngine;

public class FigureSwapper : MonoBehaviour
{
    [Header("Drag Two GameObjects Here")]
    [SerializeField] private GameObject figure1;  // First figure
    [SerializeField] private GameObject figure2;  // Second figure
    
    public void SwapFigures()
    {
        if (figure1 != null && figure2 != null)
        {
            // Swap visibility
            bool figure1Active = figure1.activeSelf;
            figure1.SetActive(!figure1Active);
            figure2.SetActive(figure1Active);
            
            Debug.Log("Figures swapped! Figure1: " + figure1.activeSelf + ", Figure2: " + figure2.activeSelf);
        }
        else
        {
            Debug.LogWarning("Please assign both figures in Inspector!");
        }
    }
    
    // Optional: Show specific figure
    public void ShowFigure1()
    {
        if (figure1 != null) figure1.SetActive(true);
        if (figure2 != null) figure2.SetActive(false);
    }
    
    public void ShowFigure2()
    {
        if (figure1 != null) figure1.SetActive(false);
        if (figure2 != null) figure2.SetActive(true);
    }
}
