using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject AIdataObject;
    public GameObject bestplayer;
    public Button showdata;
    public Button showbestdata;

    public void setactiveBestPlayer()
    {
        bestplayer.SetActive(true);
    }
    public void sortAddedData()
    {
        AIdata aidata = AIdataObject.GetComponent<AIdata>();
        aidata.sortAddData();
        showdata.interactable = true;
        showbestdata.interactable = true;
    }
    public void PrintandSaveBestData()
    {
        AIdata aidata = AIdataObject.GetComponent<AIdata>();
        aidata.printGreatData();
    }
    public void _nextgeneration()
    {
        AIdata aidata = AIdataObject.GetComponent<AIdata>();
        aidata.nextgeneration();
    }
}
