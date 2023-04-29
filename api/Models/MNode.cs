using System.ComponentModel.DataAnnotations;

public class Node
{
    public int id { get; set; }
    [Required]
    public string name { get; set; }
    public List<Node> children { get; set; }
    public int? parentNodeId { get; set; }
}