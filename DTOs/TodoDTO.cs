using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Todo.DTOs;

public record TodoCreateDTO
{
    [Required]
    [MinLength(3)]
    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    public int UserId { get; set; }
}

public record TodoUpdateDTO
{
    [JsonPropertyName("title")]
    [MinLength(3)]
    [MaxLength(255)]
    public string Title { get; set; } = null;

    [JsonPropertyName("is_completed")]
    public bool? IsCompleted { get; set; } = null;
}