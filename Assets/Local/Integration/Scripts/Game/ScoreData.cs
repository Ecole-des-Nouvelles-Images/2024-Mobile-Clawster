using UnityEngine;

namespace Local.Integration.Scripts.Game
{
    [CreateAssetMenu(menuName = "Score Data")]
    public class ScoreData : ScriptableObject
    {
        public int CurrentScore;  
        public int BestScore;     
    }
}