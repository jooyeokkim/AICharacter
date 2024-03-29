﻿using System;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AIdata : MonoBehaviour
{
    //캐릭터의 유전자(이동 지시열, 점프 지시열, 점수)
    struct gen
    {
        public string gen1;
        public string gen2;
        public float score;
        public gen(string gen1, string gen2, float score)
        {
            this.gen1 = gen1;
            this.gen2 = gen2;
            this.score = score;
        }
    }

    // 점수 순 정렬 IComparer
    class sort : IComparer
    {
        int IComparer.Compare(object _x, object _y)
        {
            gen x = (gen)_x;
            gen y = (gen)_y;
            return y.score.CompareTo(x.score);
        }

    }

    public int generation = 0;
    private int addcount = 0; //auto
    public int howmanysamples = 50;
    public GameObject bestplayer;

    // 각각의 과정을 버튼을 눌러 실행시킬 경우 필요한 버튼 오브젝트
    // 기본은 자동 모드 / 수동 모드 필요 시 함수 끝에 있는 //auto 주석 줄을 제거할 것)
    public Button showdata;
    public Button showbestdata;
    public Button learning;
    public Button shownextgeneration;
    public Button nextGeneration;

    public Text generationtext;
    gen[] gene;
    gen[] top1gene;

    void Awake()
    {
        gene = new gen[50];
        top1gene = new gen[300];
        generation = Savegene.generation;
        generationtext.text = generation + "세대";
    }

    public void AddData(int playernum, string ai, string jumpai, float score)
    {
        gene[playernum] = new gen(ai, jumpai, score);
        addcount++;
        if (addcount >= 50) sortAddData(); //auto
    }

    public void sortAddData() // 점수 순으로 정렬
    {
        IComparer gosort = new sort();
        Array.Sort(gene, gosort);
        Debug.Log("Finish Sort!");
        showdata.interactable = true;
        printGreatData();//auto
    }

    public void printGreatData() //and save data!
    {
        for (int i = 0; i < 50; i++)
        {
            Debug.Log(gene[0].score + "and" + gene[0].gen1 + "and" + gene[0].gen2);
        }
        top1gene[generation] = gene[0];
        showbestdata.interactable = true;
        learning.interactable = true;
        Learning(); //auto
    }

    public string gettopgen1() // 최고득점 캐릭터의 이동 유전자 저장
    {
        return top1gene[generation].gen1;
    }

    public string gettopgen2() // 최고득점 캐릭터의 점프 유전자 저장
    {
        return top1gene[generation].gen2;
    }

    public void nextgeneration() // 다음 세대 시작
    {
        Savegene.generationcount();
        SceneManager.LoadScene(0);
    }

    public void setactiveBestPlayer() // 최고득점 캐릭터 활성화
    {
        bestplayer.SetActive(true);
    }

    string mate(int x, int y) //교배
    {
        string child = "";
        for (int i = 0; i < 100; i++) {
            int a = UnityEngine.Random.Range(0, 100);
            if (a < 50)
            {
                child += Savegene.Concat[x][i];
            }
            else child += Savegene.Concat[y][i];
        }
        return child;
    }

    public void Learning() //and save data!
    {
        string[] babies = new string[50];
        Debug.Log(gene[0].score + "and" + gene[0].gen1 + "and" + gene[0].gen2);
        top1gene[generation] = gene[0];
        for (int i = 0; i < 10; i++)
        {
            Savegene.SavedGene1[i] = gene[i].gen1;
            Savegene.SavedGene2[i] = gene[i].gen2;
            Savegene.Concat[i] = Savegene.SavedGene1[i] + Savegene.SavedGene2[i];
            Debug.Log(Savegene.Concat[i].Length);
        }
        for (int i = 10; i < 20; i++)
        {
            int a = UnityEngine.Random.Range(11, 50);
            Debug.Log(i + "lucky" + a);
            Savegene.Concat[i] = gene[a].gen1 + gene[a].gen2;
        }

        //shuffle them
        for (int i = 0; i < 20; i++)
        {
            int a = UnityEngine.Random.Range(0, 20);
            int b = UnityEngine.Random.Range(0, 20);
            string temp = Savegene.Concat[a];
            Savegene.Concat[a] = Savegene.Concat[b];
            Savegene.Concat[b] = temp;
        }

        //create babies
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                babies[i * 5 + j] = mate(i, 20 - 1 - i);
            }
        }

        //mutation
        for (int i = 0; i < 50; i++)
        {
            int a = UnityEngine.Random.Range(0, 100);
            if (a < 10) //mutation probability
            {
                string randomPlayGene;
                int index = UnityEngine.Random.Range(0, 100);
                if (index < 50)
                {
                    randomPlayGene = "" + UnityEngine.Random.Range(0, 3);
                }
                else randomPlayGene = "" + UnityEngine.Random.Range(0, 2);
                babies[i] = babies[i].Remove(index, 1);
                babies[i] = babies[i].Insert(index, randomPlayGene);
            }
        }
        for(int i = 0; i < 50; i++)
        {
            Debug.Log(babies[i] + " length="+babies[i].Length);
        }
        //savetoSavegene
        for(int i = 0; i < 50; i++)
        {
            Savegene.SavedGene1[i] = babies[i].Substring(0, 50);
            Savegene.SavedGene2[i] = babies[i].Substring(50, 50);
            Savegene.Concat[i] = babies[i];
        }
        Debug.Log("Finish learning");
        shownextgeneration.interactable = true;
        ShowNextGeneration();//auto
    }

    public void ShowNextGeneration()
    {
        Debug.Log("ShowNextGeneration");
        for (int i = 0; i < 50; i++)
        {
            Debug.Log(Savegene.SavedGene1[i] + " and " + Savegene.SavedGene2[i]);
        }
        nextGeneration.interactable = true;
        nextgeneration();//auto
    }
}
