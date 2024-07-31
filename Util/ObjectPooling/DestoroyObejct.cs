using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoroyObejct : MonoBehaviour
{
    public float EndTime = 2.0f;
    
    private void OnEnable() 
    {
        GB.Timer.Create(EndTime, () => { GB.ObjectPooling.Destroy(this.gameObject); });

        
    }


}
