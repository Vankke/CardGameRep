using UnityEngine;

public class DestroyInThreeSeconds : MonoBehaviour
{
    public float TimeToDestroy;
    public bool RescaleBeforeDestroy;
    float scaleT = 1f;
    // Update is called once per frame
    void Update()
    {
        TimeToDestroy -= Time.deltaTime;
        if(TimeToDestroy < 0)
        {
            Destroy(gameObject);
        }
        if (RescaleBeforeDestroy)
        {
            scaleT -= Time.deltaTime * 2;
            var qwe = Mathf.Clamp(scaleT - Time.deltaTime * 2, 0, 1);
            Vector3 newScale = new Vector3(scaleT, scaleT, scaleT);
            gameObject.transform.localScale = newScale;
        }
    }
}
