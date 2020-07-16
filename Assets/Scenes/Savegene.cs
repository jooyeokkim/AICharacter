using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Savegene
{
    public static string[] SavedGene1 = new string[50];
    public static string[] SavedGene2 = new string[50];
    public static string[] Concat = new string[50];
    public static int generation = 0;
    public static void generationcount()
    {
        generation++;
    }
}
