namespace RobotFootball.RobotFootballGame
{
    public class Player
    {
        private int _score;

        public Player()
        {
            _score = 0;
        }

        public string ConnectionId { get; set; }
        public int Score => _score;

        public void IncreaseScore()
        {
            _score++;
        }
    }
}
