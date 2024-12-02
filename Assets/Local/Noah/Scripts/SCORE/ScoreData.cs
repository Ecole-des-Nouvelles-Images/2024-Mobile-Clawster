using UnityEngine;
using UnityEngine.Serialization;

namespace Local.Noah.Scripts.GAME
{
    [CreateAssetMenu(menuName = "Score Data")]
    public class ScoreData : ScriptableObject
    {
        public int CurrentScore;  
        public int BestScore;     
    }
}