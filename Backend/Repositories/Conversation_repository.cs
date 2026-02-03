using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database;

namespace Repository
{
    internal class ConversationRepository
    {
        public async Task<List<(int id, DateTime createdAt)>> GetListConversationDB(int userId)
        {
            var list = new List<(int id, DateTime createdAt)>();

            await using var connection = Databaseconnection.GetConnection();
            await connection.OpenAsync();

            var query = @"
                SELECT c.id, c.created_at
                FROM conversations c
                JOIN conversation_participants cp ON cp.conversation_id = c.id
                WHERE cp.user_id = @id
                ORDER BY c.created_at DESC;
            ";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", userId);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32(reader.GetOrdinal("id"));
                var createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));

                list.Add((id, createdAt));
            }

            return list;
        }


        public async Task<int> GetOrCreatePVDB(int userId, int otherUserId)
        {
            if (userId <= 0 || otherUserId <= 0)
                throw new ArgumentException("userId e otherUserId são obrigatórios");

            if (userId == otherUserId)
                throw new ArgumentException("PV precisa ser com outro usuário");

            await using var connection = Databaseconnection.GetConnection();
            await connection.OpenAsync();

            const string findSql = @"
                SELECT c.id
                FROM conversations c
                JOIN conversation_participants cp1 
                    ON cp1.conversation_id = c.id AND cp1.user_id = @userId
                JOIN conversation_participants cp2 
                    ON cp2.conversation_id = c.id AND cp2.user_id = @otherUserId
                LIMIT 1;
            ";

            await using (var findCmd = new NpgsqlCommand(findSql, connection))
            {
                findCmd.Parameters.AddWithValue("@userId", userId);
                findCmd.Parameters.AddWithValue("@otherUserId", otherUserId);

                var existing = await findCmd.ExecuteScalarAsync();
                if (existing != null && existing != DBNull.Value)
                    return Convert.ToInt32(existing);
            }

            await using var tx = await connection.BeginTransactionAsync();

            try
            {
                const string insertConversationSql = @"INSERT INTO conversations DEFAULT VALUES RETURNING id;";
                int conversationId;

                await using (var insertConversationCmd = new NpgsqlCommand(insertConversationSql, connection, tx))
                {
                    var result = await insertConversationCmd.ExecuteScalarAsync();
                    conversationId = Convert.ToInt32(result);
                }

                const string insertParticipantSql = @"
                    INSERT INTO conversation_participants (conversation_id, user_id)
                    VALUES (@conversationId, @userId);
                ";

                await using (var p1 = new NpgsqlCommand(insertParticipantSql, connection, tx))
                {
                    p1.Parameters.AddWithValue("@conversationId", conversationId);
                    p1.Parameters.AddWithValue("@userId", userId);
                    await p1.ExecuteNonQueryAsync();
                }

                await using (var p2 = new NpgsqlCommand(insertParticipantSql, connection, tx))
                {
                    p2.Parameters.AddWithValue("@conversationId", conversationId);
                    p2.Parameters.AddWithValue("@userId", otherUserId);
                    await p2.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
                return conversationId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> IsParticipantDB(int conversationId, int userId)
        {
            if (conversationId <= 0 || userId <= 0)
                throw new ArgumentException("conversationId e userId são obrigatórios");

            await using var connection = Databaseconnection.GetConnection();
            await connection.OpenAsync();

            const string sql = @"
                SELECT 1
                FROM conversation_participants
                WHERE conversation_id = @conversationId AND user_id = @userId
                LIMIT 1;
            ";

            await using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@conversationId", conversationId);
            cmd.Parameters.AddWithValue("@userId", userId);

            var result = await cmd.ExecuteScalarAsync();
            return result != null && result != DBNull.Value;
        }
    }
}