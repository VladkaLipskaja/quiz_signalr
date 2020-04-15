using System.Collections.Generic;

namespace WebApplication1
{
    public class QuizConfiguration
    {
        public int NumberOfPlayers { get; set; }
        
        public List<Question> Questions { get; set; }
    }
}