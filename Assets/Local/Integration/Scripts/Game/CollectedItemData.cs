namespace Local.Integration.Scripts.Game
{
    public class CollectedItemData
    {
        public string Name { get; set; }
        public int Weight { get; set; }
        public int Score { get; set; }
        public int Quantity { get; set; }

        public CollectedItemData(string name, int weight, int score, int quantity = 1)
        {
            Name = name;
            Weight = weight;
            Score = score;
            Quantity = quantity;
        }
    }
}