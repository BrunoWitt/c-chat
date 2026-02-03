using Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public static class ConversationService
    {
        public static async Task<List<(int id, DateTime createdAt)>> ListConversationsAsync(int userId)
        {
            var Repository = new ConversationRepository();
            return await Repository.GetListConversationDB(userId);
        }


        public static async Task<int> GetOrCreatePVAsync(int userId, int otherUserId)
        {
            var repo = new ConversationRepository();
            return await repo.GetOrCreatePVDB(userId, otherUserId);
        }


        public static async Task<bool> IsParticipantAsync(int conversationId, int userId)
        {
            var repo = new ConversationRepository();
            return await repo.IsParticipantDB(conversationId, userId);
        }
    }
}
