namespace PathAndPaws.Models;

public class Lead
{
    public int Id { get; set; }

    public string OwnersName { get; set; } = string.Empty;

    public string DogsName {get; set;  } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public string Website {get;set;} = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}