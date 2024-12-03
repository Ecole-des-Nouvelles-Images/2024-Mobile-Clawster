using UnityEngine;
using UnityEngine.Serialization;

namespace Local.Noah.Scripts.GAME
{
    [CreateAssetMenu(menuName = "Score Data")]
    public class Item : ScriptableObject
    {
        public GameObject Prefab;
        public int Weight;
        public int Score;
    }
}
