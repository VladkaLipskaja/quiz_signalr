namespace WebApplication1
{
    public interface IQuizService
    {
        string GetWinnerName(string guid);
        
        string GetQuestion(string guid, int questionNumber);
        
        string ProcessAnswer(string nick, string answer);
        
        (bool shouldFinalize, bool shouldSendNextQuestion, int questionNumber) GetNextStep(string guid);

        (string roomGuid, bool isFull) Connect(string nick);
    }
}