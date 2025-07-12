namespace StarterPack.Models.ToDo
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        public string FormattedDueDate
        {
            get
            {
                if (DueDate.Date == DateTime.Today)
                    return "due today";
                return DueDate.ToString("dd/MM/yyyy");
            }
        }
    }
}
