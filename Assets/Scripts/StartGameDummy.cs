using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameDummy : MonoBehaviour
{
    public List<Sprite> SlideShow;
    public Image SlideShowScreen;
    public int slideTracker;
    private void Start()
    {
        SpaceAction();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            SpaceAction();
        }       
    }

    public void SpaceAction()
    {
        if(slideTracker < SlideShow.Count)
        {
            SlideShowScreen.sprite = SlideShow[slideTracker];
            slideTracker++;
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
}
