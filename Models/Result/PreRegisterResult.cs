namespace Portifolio.Models.Result
{
    public class PreRegisterResult
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;

        public static PreRegisterResult Success() => new() { IsSuccess = true };
        public static PreRegisterResult Failure(string message) => new() { IsSuccess = false, ErrorMessage = message };
    }
}
