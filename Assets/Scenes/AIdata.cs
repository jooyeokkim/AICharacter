using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIdata : MonoBehaviour
{
    public static AIdata instance = null;
    public int count;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        count = 0;
        up();
    }
    void up()
    {
        count++;
        Invoke("up", 2);
    }
}
