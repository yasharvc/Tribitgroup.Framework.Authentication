namespace Tribitgroup.Framework.Shared.Types
{
    public class Session
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; } = true;

        public void SetUserId(string userId)
        {
            UserId = userId;
            IsAnonymous = false;
        }
    }
}