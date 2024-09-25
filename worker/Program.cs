using Npgsql;
using StackExchange.Redis;
using System;
using System.Threading;

namespace VotingWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            var redisConnection = "redis:6379";
            var redis = ConnectionMultiplexer.Connect(redisConnection);
            var dbRedis = redis.GetDatabase();
            Console.WriteLine("Conectado a Redis");

            var connString = "Host=db;Username=postgres;Password=yourpassword;Database=mydb";
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            Console.WriteLine("Conectado a PostgreSQL");

            CreateTableIfNotExists(conn);

            while (true)
            {
                UpdateVotesSummary(conn, dbRedis);

                Thread.Sleep(1000);
            }
        }

        static void CreateTableIfNotExists(NpgsqlConnection conn)
        {
            string createVotesTableQuery = @"
                CREATE TABLE IF NOT EXISTS votes (
                    id SERIAL PRIMARY KEY,
                    cats_votes INTEGER DEFAULT 0,
                    dogs_votes INTEGER DEFAULT 0
                );
            ";
            using var cmd = new NpgsqlCommand(createVotesTableQuery, conn);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Tabla 'votes' creada/verificada en PostgreSQL.");

            string initVotesQuery = @"
                INSERT INTO votes (id, cats_votes, dogs_votes)
                VALUES (1, 0, 0)
                ON CONFLICT (id) DO NOTHING;
            ";
            using var initCmd = new NpgsqlCommand(initVotesQuery, conn);
            initCmd.ExecuteNonQuery();
        }

        static void UpdateVotesSummary(NpgsqlConnection conn, IDatabase dbRedis)
        {
            var voteCats = dbRedis.StringGet("a");  
            var voteDogs = dbRedis.StringGet("b");  

            int catsVotes = (int)(voteCats.IsNullOrEmpty ? 0 : voteCats);
            int dogsVotes = (int)(voteDogs.IsNullOrEmpty ? 0 : voteDogs);

            string updateQuery = @"
                UPDATE votes
                SET cats_votes = @catsVotes,
                    dogs_votes = @dogsVotes
                WHERE id = 1;
            ";
            
            using var cmd = new NpgsqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("catsVotes", catsVotes);
            cmd.Parameters.AddWithValue("dogsVotes", dogsVotes);
            cmd.ExecuteNonQuery();

            Console.WriteLine("Tabla 'votes' actualizada: Gatos = {0}, Perros = {1}", catsVotes, dogsVotes);
        }
    }
}
