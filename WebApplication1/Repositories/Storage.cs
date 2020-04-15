
using System.Collections.Generic;

namespace WebApplication1
{
    // imitation of db and storing data
    public static class Storage
    {
        // key = id of room
        // value = list of nicks
        public static Dictionary<string, List<string>> RoomsOfPlayers = new Dictionary<string, List<string>>();

        // key = id of room
        // value = last question
        public static Dictionary<string, int> QuestionNumbers = new Dictionary<string, int>();

        // key = id of room
        // value = <playerName, [isCorrectAnswer]>
        public static Dictionary<string, Dictionary<string, bool?[]>> PlayerAnswers =
            new Dictionary<string, Dictionary<string, bool?[]>>();
    }
}