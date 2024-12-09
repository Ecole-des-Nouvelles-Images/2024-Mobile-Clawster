using UnityEngine;

namespace Local.Integration.Scripts.SCORE
{
    [CreateAssetMenu(menuName = "Score Data")]
    public class ScoreData : ScriptableObject
    {
        public int CurrentScore;  
        public int BestScore;     
    }
}