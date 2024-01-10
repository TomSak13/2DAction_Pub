using UnityEngine;

public class EnemyDeathAnimation : MonoBehaviour
{
    public void OnCompleteAnimation()
    {
        Destroy(gameObject);
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
