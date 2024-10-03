using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoroyObejct : MonoBehaviour
{
    public float EndTime = 2.0f;

    float _time;
    private void OnEnable() 
    {
        _time = 0;
        
    }
    void Update()
    {
        _time += Time.deltaTime;
        if(_time > EndTime)
        {
            GB.ObjectPooling.Destroy(this.gameObject);
        }
    }
}
