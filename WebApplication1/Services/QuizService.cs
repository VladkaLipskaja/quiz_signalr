using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace WebApplication1
{
    public class QuizService : IQuizService
    {
        private readonly QuizConfiguration _configurations;

        public QuizService(IOptionsSnapshot<QuizConfiguration> configurations)
        {
            _configurations = configurations.Value;
        }

        public (string roomGuid, bool isFull) Connect(string nick)
        {
            var room = Storage.RoomsOfPlayers.FirstOrDefault(x =>
                x.Value.Count < _configurations.NumberOfPlayers);

            var roomGuid = string.Empty;

            if (room.Key == null)
            {
                roomGuid = AddPlayerToNewRoom(nick);
            }
            else
            {
                roomGuid = AddToExistingRoom(nick, room);
            }

            var isFull = Storage.RoomsOfPlayers[roomGuid].Count == _configurations.NumberOfPlayers;

            return (roomGuid, isFull);
        }

        private string AddPlayerToNewRoom(string nick)
        {
            var newGuid = Guid.NewGuid().ToString();
            Storage.RoomsOfPlayers.Add(newGuid, new List<string> {nick});
            Storage.PlayerAnswers.Add(newGuid, new Dictionary<string, bool?[]> { {nick, new bool?[_configurations.Questions.Count]} });
            Storage.QuestionNumbers.Add(newGuid, 1);
            return newGuid;
        }

        private string AddToExistingRoom(string nick, KeyValuePair<string, List<string>> room)
        {
            room.Value.Add(nick);
            Storage.PlayerAnswers[room.Key].Add(nick, new bool?[_configurations.Questions.Count]);
            return room.Key;
        }

        public (bool shouldFinalize, bool shouldSendNextQuestion, int questionNumber) GetNextStep(string guid)
        {
            var answers = Storage.PlayerAnswers[guid];
            var questionNumber = Storage.QuestionNumbers[guid];

            var isNotCompleted = questionNumber - 2 >= 0 && answers.Any(x => x.Value[questionNumber - 2] == null);

            if (!isNotCompleted && questionNumber - 1 == _configurations.Questions.Count)
            {
                return (true, false, questionNumber);
            }
            else if (!isNotCompleted || questionNumber == 1)
            {
                return (false, true, questionNumber);
            }
            
            return (false, false, questionNumber);
        }

        public string ProcessAnswer(string nick, string answer)
        {
            var guid = Storage.RoomsOfPlayers.First(x => x.Value.Contains(nick)).Key;

            var questionNumber = Storage.QuestionNumbers[guid] - 1;

            var isAnswerCorrect = _configurations.Questions[questionNumber - 1].AnswerText == answer;

            var answers = Storage.PlayerAnswers[guid];

            SetScore(answers, isAnswerCorrect, nick, questionNumber);

            return guid;
        }

        private void SetScore(Dictionary<string, bool?[]> answers, bool isAnswerCorrect, string nick, int questionNumber)
        {
            var playerAnswer = answers.FirstOrDefault(x =>
                x.Key == nick);
            
            if (playerAnswer.Key == null)
            {
                var value = new bool?[5];
                value[0] = isAnswerCorrect;
                answers.Add(nick, value);
            }
            else
            {
                playerAnswer.Value[questionNumber - 1] = isAnswerCorrect;
            }
        }

        public string GetQuestion(string guid,  int questionNumber)
        {
            Storage.QuestionNumbers[guid] = questionNumber + 1;

            var question = _configurations.Questions[questionNumber - 1];

            return question.QuestionText;
        }

        public string GetWinnerName(string guid)
        {
            var answers = Storage.PlayerAnswers[guid];

            var maxNumber = -1;
            var winnerName = string.Empty;

            foreach (var item in answers)
            {
                var number = item.Value.Count(x => x.Value);

                winnerName = maxNumber > number ? winnerName : item.Key;
                maxNumber = maxNumber > number ? maxNumber : number;
            }

            return winnerName;
        }
    }
}