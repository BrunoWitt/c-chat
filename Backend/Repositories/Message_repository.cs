using Npgsql;
using Database;
using Models;

namespace Repository
{
    internal class MessageRepository
    {
        public async Task<List<Message>> listConversationMessagesDB(int conversationId, int userId, int limit = 50)
        {
            if (conversationId <= 0) throw new ArgumentException("conversationId inválido");
            if (userId <= 0) throw new ArgumentException("userId inválido");

            var safeLimit = (limit > 0 && limit <= 200) ? limit : 50;
            var messages = new List<Message>();

            await using var connection = Databaseconnection.GetConnection();
            await connection.OpenAsync();

            // valida participante
            const string checkMemberSql = @"
                SELECT 1
                FROM conversation_participants
                WHERE conversation_id = @conversationId AND user_id = @userId
                LIMIT 1;
            ";

            await using (var checkCmd = new NpgsqlCommand(checkMemberSql, connection))
            {
                checkCmd.Parameters.AddWithValue("@conversationId", conversationId);
                checkCmd.Parameters.AddWithValue("@userId", userId);

                var memberResult = await checkCmd.ExecuteScalarAsync();
                if (memberResult == null || memberResult == DBNull.Value)
                    throw new UnauthorizedAccessException("Sem acesso a esta conversa");
            }

            const string listMessagesSql = @"
                SELECT
                    m.id,
                    m.conversation_id,
                    m.sender_id,
                    u.name AS sender_name,
                    m.content,
                    m.created_at,
                    m.edited_at,
                    m.deleted_at
                FROM messages m
                JOIN users u ON u.id = m.sender_id
                WHERE m.conversation_id = @conversationId
                AND m.deleted_at IS NULL
                ORDER BY m.created_at ASC
                LIMIT @limit;
            ";

            await using (var cmd = new NpgsqlCommand(listMessagesSql, connection))
            {
                cmd.Parameters.AddWithValue("@conversationId", conversationId);
                cmd.Parameters.AddWithValue("@limit", safeLimit);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var message = new Message
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("id")),
                        ConversationId = reader.GetInt64(reader.GetOrdinal("conversation_id")),
                        SenderId = reader.GetInt64(reader.GetOrdinal("sender_id")),
                        SenderName = reader.GetString(reader.GetOrdinal("sender_name")),
                        Content = reader.GetString(reader.GetOrdinal("content")),
                        CreatedAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("created_at")),
                        EditedAt = reader.IsDBNull(reader.GetOrdinal("edited_at"))
                            ? null
                            : reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("edited_at")),
                        DeletedAt = reader.IsDBNull(reader.GetOrdinal("deleted_at"))
                            ? null
                            : reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("deleted_at")),
                    };

                    messages.Add(message);
                }
            }

            return messages;
        }


        public async Task<(int id, int conversationId, int senderId, string content, DateTime createdAt)>
            CreateConversationMessageDB(int conversationId, int userId, string content)
        {
            if (conversationId <= 0) throw new ArgumentException("conversationId inválido");
            if (userId <= 0) throw new ArgumentException("userId inválido");

            var text = (content ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Mensagem vazia");

            await using var connection = Databaseconnection.GetConnection();
            await connection.OpenAsync();

            const string memberSql = @"
                SELECT 1
                FROM conversation_participants
                WHERE conversation_id = @conversationId AND user_id = @userId
                LIMIT 1;
            ";

            await using (var memberCmd = new NpgsqlCommand(memberSql, connection))
            {
                memberCmd.Parameters.AddWithValue("@conversationId", conversationId);
                memberCmd.Parameters.AddWithValue("@userId", userId);

                var member = await memberCmd.ExecuteScalarAsync();
                if (member == null || member == DBNull.Value)
                    throw new InvalidOperationException("Você não é membro dessa conversa");
            }

            const string insertSql = @"
                INSERT INTO messages (conversation_id, sender_id, content)
                VALUES (@conversationId, @userId, @content)
                RETURNING id, conversation_id, sender_id, content, created_at;
            ";

            await using (var cmd = new NpgsqlCommand(insertSql, connection))
            {
                cmd.Parameters.AddWithValue("@conversationId", conversationId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@content", text);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    throw new Exception("Falha ao inserir mensagem");

                var id = reader.GetInt32(reader.GetOrdinal("id"));
                var cid = reader.GetInt32(reader.GetOrdinal("conversation_id"));
                var sid = reader.GetInt32(reader.GetOrdinal("sender_id"));
                var msg = reader.GetString(reader.GetOrdinal("content"));
                var createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));

                return (id, cid, sid, msg, createdAt);
            }
        }

    }
}