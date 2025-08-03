using UnityEngine;

public class DestroyInThreeSeconds : MonoBehaviour
{
    public float TimeToDestroy;
    

    // Update is called once per frame
    void Update()
    {
        TimeToDestroy -= Time.deltaTime;
        if(TimeToDestroy < 0)
        {
            Destroy(gameObject);
        }    
    }
}
