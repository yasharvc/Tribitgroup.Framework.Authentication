namespace Tribitgroup.Framework.Shared.Types
{
    public class ExceptionWithCode : Exception
    {
        public string Code { get; protected set; }
        public ExceptionWithCode(string code, string message = "") : base(message) => Code = code;
    }
}