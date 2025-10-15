using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Object sceneAsset;  // Drag scene here in Inspector
    
    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(LoadScene);
        }
    }
    
    public void LoadScene()
    {
        if (sceneAsset != null)
        {
            SceneManager.LoadScene(sceneAsset.name);
        }
        else
        {
            Debug.LogWarning("No scene assigned!");
        }
    }
}
