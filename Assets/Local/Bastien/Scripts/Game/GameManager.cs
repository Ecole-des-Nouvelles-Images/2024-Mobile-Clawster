using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Main Game Variables")]
    public int Level;               //Self-explanatory
    
    [Header("Game Start / End Variables")]
    public int CountdownTime;       //Time before game starts
    public string StartText;        //Go! or Bin It!
    public int EndTime;             //Time before game ends
    public string EndText;          //Finish! or Stop!
    public int Score;               //Self-explanatory
    public int HighScore;           //Highest score of the session (or the device)
    
    public bool GameStarted;        //Has the game started ?

    public List<GameObject> Stack;  //Items transported by Clawster
    
    public static GameManager gm;   //SINGLETON BABY
}
