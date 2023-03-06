using ChatApp.Models;
using Prism.Events;

namespace Business.Events
{
    public class ChatMemberAddRemoveEvent : PubSubEvent<ChatMemberAddRemovePayload>
    {

    }

    public class ChatMemberAddRemovePayload
    {
        public ChatMemberChangeActionEnum ChatMemberChangeAction { get; set; }
        public UserModel User { get; set; } = new();
    }

    public enum ChatMemberChangeActionEnum
    {
        Add,
        Remove,
    }
}
