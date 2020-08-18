using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bgm : MonoBehaviour
{
    AudioSource music;
    public static bgm instance = null;
    private int count;
    void Awake()
    {
        music = GetComponent<AudioSource>();
        music.Play();
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}

