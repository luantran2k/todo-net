using MyApiProject.Models.Todo;
using Microsoft.Data.Sqlite;

namespace MyApiProject.Services
{
    public class TodoService
    {
        private readonly string _connectionString;

        public TodoService(IConfiguration config)
        {
            _connectionString = config["MySettings:dbConnectString"] ?? throw new ArgumentNullException(nameof(config), "Configuration is null");
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
            }

            InitializeDatabase();
        }

        public IEnumerable<Todo> GetAll()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM todos";
            var reader = command.ExecuteReader();
            var todos = new List<Todo>();
            while (reader.Read())
            {
                var todo = new Todo
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt64(reader.GetOrdinal("id")),
                    Title = reader.IsDBNull(reader.GetOrdinal("title")) ? string.Empty : reader.GetString(reader.GetOrdinal("title")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString(reader.GetOrdinal("description"))
                };
                todos.Add(todo);
            }
            return todos;
        }

        public Todo? GetById(long id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM todos WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            var reader = command.ExecuteReader();
            var todo = new Todo { Title = "" };
            if (reader.Read())
            {
                todo.Id = reader.GetInt64(0);
                todo.Title = reader.GetString(1);
                todo.IsCompleted = reader.GetBoolean(2);
                todo.Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            }
            return todo;
        }

        public Todo Create(Todo newTodo)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO todos (title, iscompleted, description) VALUES ($title, $iscompleted, $description); SELECT last_insert_rowid()";
            command.Parameters.AddWithValue("$title", newTodo.Title);
            command.Parameters.AddWithValue("$iscompleted", newTodo.IsCompleted);
            command.Parameters.AddWithValue("$description", newTodo.Description);
            newTodo.Id = (long)command.ExecuteScalar();
            return newTodo;
        }

        public void Update(Todo updatedTodo)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE todos SET title = $title, iscompleted = $iscompleted, description = $description WHERE id = $id";
            command.Parameters.AddWithValue("$title", updatedTodo.Title);
            command.Parameters.AddWithValue("$iscompleted", updatedTodo.IsCompleted);
            command.Parameters.AddWithValue("$description", updatedTodo.Description);
            command.Parameters.AddWithValue("$id", updatedTodo.Id);
            command.ExecuteNonQuery();
        }

        public void Delete(long id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM todos WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS todos (id INTEGER PRIMARY KEY, title TEXT, iscompleted INTEGER, description TEXT)";
            command.ExecuteNonQuery();
        }
    }
}

