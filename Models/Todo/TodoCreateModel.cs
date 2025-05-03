namespace MyApiProject.Models.Todo
{

    public class TodoCreateModel
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}

