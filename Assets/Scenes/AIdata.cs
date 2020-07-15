using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AIdata : MonoBehaviour
{
    struct gen
    {
        public string gen1;
        public string gen2;
        public int score;
        public gen(string gen1, string gen2, int score)
        {
            this.gen1 = gen1;
            this.gen2 = gen2;
            this.score = score;
        }
    }
    class sort : IComparer
    {
        int IComparer.Compare(object _x, object _y)
        {
            gen x = (gen)_x;
            gen y = (gen)_y;
            return y.score.CompareTo(x.score);
        }

    }
    List<gen> top1gene;

    //public static AIdata instance = null;
    public int generation = 0;
    public int howmanysamples = 50;
    gen[] gene = new gen[50];
    void Awake()
    {
        /*if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);*/
        top1gene = new List<gen>();
    }
    private void Update()
    {
        Debug.Log(Savegene.SavedGene1[0]);
        Debug.Log(Savegene.SavedGene2[0]);
    }
    public void AddData(int playernum, string ai, string jumpai, int score)
    {
        gene[playernum] = new gen(ai, jumpai, score);
    }
    public void sortAddData()
    {
        IComparer gosort = new sort();
        Array.Sort(gene, gosort);
        Debug.Log("Finish Sort");
    }
    public void printGreatData()
    {
        for(int i = 0; i < 50; i++)
        {
            Debug.Log(gene[i].score + "and" + gene[i].gen1 + "and" + gene[i].gen2);
        }
        top1gene.Add(gene[generation]);
        for(int i = 0; i < 20; i++)
        {
            Savegene.SavedGene1[i] = gene[i].gen1;
            Savegene.SavedGene2[i] = gene[i].gen2;
        }
    }
    public string gettopgen1()
    {
        return top1gene[generation].gen1;
    }
    public string gettopgen2()
    {
        return top1gene[generation].gen2;
    }
    public void nextgeneration()
    {
        SceneManager.LoadScene(0);
        generation++;
    }
}
