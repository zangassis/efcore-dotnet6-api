namespace BlogManager.Models;

public record Author
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string EmailAddress { get; set; }
}
