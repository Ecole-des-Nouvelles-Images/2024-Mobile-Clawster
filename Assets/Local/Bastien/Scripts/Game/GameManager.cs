using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Main Game Variables")]
    public int Level;               //Self-explanatory
    
    [Header("Game Start / End Variables")]
    public static int CountdownTime;       //Time before game starts
    public static string StartText;        //Go! or Bin It!
    public static int EndTime;             //Time before game ends
    public static string EndText;          //Finish! or Stop!
    public static int Score;               //Self-explanatory
    public static int HighScore;           //Hghest score of the session (or the device)
    
    public static bool GameStarted;         //Has the game started ?

    public static List<GameObject> Stack;   //Items transported by Clawster
    
    public static GameManager gm;   //SINGLETON BABY
}
