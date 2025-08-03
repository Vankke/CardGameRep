using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanelController : MonoBehaviour
{
    public Image QuestIcon;
    public TextMeshProUGUI Description;
    public GameObject CompletionMarker;
    public QuestController.Quest quest;

    bool SelfDestruct;
    float DestructTime = 3;
    public void StartSelfDestruct()
    {
        SelfDestruct = true;
    }

    private void Start()
    {
        QuestIcon.sprite = quest.sprite;
        Description.text = quest.Description;
        CompletionMarker.SetActive(false);
    }

    public void Update()
    {
        if (SelfDestruct)
        {
            if(DestructTime > 0)
            {
                DestructTime -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
