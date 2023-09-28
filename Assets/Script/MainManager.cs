using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }


    void Awake()
    {
        //轉換場景時保留物件
        DontDestroyOnLoad(gameObject);
    }
}
