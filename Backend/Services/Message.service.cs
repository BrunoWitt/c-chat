using Models;
using Repository;
using Backend.Realtime;

namespace Backend.Services
{
    public static class MessageService
    {
        public static async Task<List<Message>> ListConversationMessages(int conversationId, int userId)
        {
            var repo = new MessageRepository();
            return await repo.listConversationMessagesDB(conversationId, userId);
        }


        public static async Task<(int id, int conversationId, int senderId, string content, DateTime createdAt)> CreateConversationMessage(int conversationId, int userId, string content, ChatNotifier notifier)
        {
            var repo = new MessageRepository();
            var message = await repo.CreateConversationMessageDB(conversationId, userId, content);
            
            await notifier.NotifyNewMessage(conversationId, message);

            return message;
        }
    }
}