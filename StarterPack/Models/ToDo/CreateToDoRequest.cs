namespace StarterPack.Models.ToDo
{
    public class CreateToDoRequest
    {
        public string Text { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
