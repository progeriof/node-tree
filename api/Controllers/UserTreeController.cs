using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NodeTree.Controllers;

[ApiController]
[Tags("user.tree")]
public class UserTreeController : ControllerBase
{
    private ApplicationDbContext dbContext;

    public UserTreeController(ApplicationDbContext _dbContext)
    {
        this.dbContext = _dbContext;
    }

    public static IEnumerable<Node> GetNodeAndDescendants(Node node)
    {
        return new[] { node }.Concat(node.children.SelectMany(GetNodeAndDescendants));
    }

    internal async Task AddRecursiveNodes(Node node)
    {
        var childNodes = await dbContext.Nodes.Where(x => x.parentNodeId == node.id).ToListAsync();
        if (childNodes.Count > 0)
        {
            node.children = childNodes;
            foreach (var child in node.children)
            {
                await AddRecursiveNodes(child);
            }
        }
    }

    /// <summary>
    /// Returns your entire tree. If your tree doesn't exist it will be created automatically.
    /// </summary>
    [HttpPost("api.user.tree.get")]
    public async Task<Node> Get([FromQuery, BindRequired] string treeName)
    {
        if (String.IsNullOrWhiteSpace(treeName))
        {
            throw new Exception("Tree Name cannot be null or whitespace!");
        }

        var node = await dbContext.Nodes.FirstOrDefaultAsync(x => x.name == treeName);

        if (node != null)
        {
            await AddRecursiveNodes(node);
            return node;
        }

        var newNode = new Node
        {
            name = treeName
        };

        using (var context = dbContext)
        {
            context.Nodes.Add(newNode);
            context.SaveChanges();

            return newNode;
        }
    }
}
