using UnityEngine;

namespace Local.Integration.Scripts.Game
{
    [CreateAssetMenu(menuName = "Item")]
    public class Item : ScriptableObject
    {
        public int Weight;
        public int Score;
    }
}
