using UnityEngine;

public class DailougeManager : Singleton<DailougeManager>
{
    [SerializeField] private GameObject TextboxPrefab;
        
    public GameObject ResizingTextbox;
    public TMPro.TMP_Text Textbox;

    private void Start()
    {
        ResizingTextbox = Instantiate(TextboxPrefab);
        Textbox = ResizingTextbox.GetComponentInChildren<TMPro.TMP_Text>();
    }
}
