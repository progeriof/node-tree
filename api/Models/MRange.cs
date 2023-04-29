using System.ComponentModel.DataAnnotations;

public class MRange<T>
{
    [Required]
    public int skip { get; set; }
    [Required]
    public int count { get; set; }
    public List<T>? items { get; set; }
}