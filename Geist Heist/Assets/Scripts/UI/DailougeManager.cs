using JetBrains.Annotations;
using UnityEngine;

public class DailougeManager : Singleton<DailougeManager>
{
    [SerializeField] private GameObject TextboxPrefab;
    [SerializeField] private GameObject PAPrefab;
        
    public GameObject ResizingTextbox;
    public TMPro.TMP_Text Textbox;

    public GameObject PAholder;
    public TMPro.TMP_Text PATextbox;

    private void Start()
    {
        ResizingTextbox = Instantiate(TextboxPrefab);
        Textbox = ResizingTextbox.GetComponentInChildren<TMPro.TMP_Text>();

        PAholder = Instantiate(PAPrefab);
        PATextbox = PAholder.GetComponentInChildren<TMPro.TMP_Text>();
    }
}
