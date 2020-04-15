using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WebApplication1
{
    public class QuizHub : Hub
    {
        private readonly IQuizService _service;

        public QuizHub(IQuizService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task Connect(string nick)
        {
            var configs = _service.Connect(nick);
            
            await Groups.AddToGroupAsync(Context.ConnectionId, configs.roomGuid);

            if (configs.isFull)
            {
                await TrySendAnotherQuestion(configs.roomGuid);
            }
        }

        public async Task TrySendAnotherQuestion(string guid)
        {
            var nextStep = _service.GetNextStep(guid);

            if (nextStep.shouldFinalize)
            {
                await Finalize(guid);
            }
            else if (nextStep.shouldSendNextQuestion)
            {
                await SendQuestion(guid, nextStep.questionNumber);
            }
        }

        public async Task GetAnswer(string nick, string answer)
        {
            var guid = _service.ProcessAnswer(nick, answer);
            await TrySendAnotherQuestion(guid);
        }

        public async Task SendQuestion(string guid, int questionNumber)
        {
            var question = _service.GetQuestion(guid, questionNumber);
            await Clients.Group(guid).SendAsync("GetQuestion", question);
        }

        public async Task Finalize(string guid)
        {
            var winnerName = _service.GetWinnerName(guid);
            await Clients.Group(guid).SendAsync("GetWinner", winnerName);
        }
    }
}