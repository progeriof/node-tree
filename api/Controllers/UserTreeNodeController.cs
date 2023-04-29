using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NodeTree.Controllers;

[ApiController]
[Tags("user.tree.node")]
public class UserTreeNodeController : ControllerBase
{
    private ApplicationDbContext dbContext;

    public UserTreeNodeController(ApplicationDbContext _dbContext)
    {
        this.dbContext = _dbContext;
    }
    /// <summary>
    /// Create a new node in your tree. You must to specify a parent node ID that belongs to your tree. A new node name must be unique across all siblings.
    /// </summary>
    [HttpPost("api.user.tree.node.create")]
    public async Task<Node> Create(
        [FromQuery, BindRequired] string treeName,
        [FromQuery, BindRequired] int parentNodeId,
        [FromQuery, BindRequired] string nodeName
        )
    {
        var targetNode = dbContext.Nodes.FirstOrDefault(x => x.id == parentNodeId);

        if (targetNode == null)
        {
            throw new Exception($"Parent node not found for node id: {parentNodeId}");
        }

        bool existingSiblingName = dbContext.Nodes.Any(x => x.parentNodeId == parentNodeId && x.name == nodeName);
        if (existingSiblingName)
        {
            throw new Exception($"A sibling node already exists with name: {nodeName}");
        }

        var newNode = new Node
        {
            name = nodeName,
            parentNodeId = parentNodeId
        };

        await dbContext.Nodes.AddAsync(newNode);
        await dbContext.SaveChangesAsync();

        return newNode;
    }

    /// <summary>
    /// Delete an existing node in your tree. You must specify a node ID that belongs your tree.
    /// </summary>
    [HttpPost("api.user.tree.node.delete")]
    public async Task<IActionResult> Delete(
        [FromQuery, BindRequired] string treeName, [FromQuery, BindRequired] int nodeId
    )
    {
        var node = await dbContext.Nodes.FirstOrDefaultAsync(x => x.id == nodeId);

        if (node == null)
        {
            throw new Exception("Node tree not found for given id!");
        }

        if (dbContext.Nodes.Any(x => x.parentNodeId == node.id))
        {
            throw new DataMisalignedException("You must delete all child nodes first!");
        }

        dbContext.Remove(node);
        await dbContext.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Rename an existing node in your tree. You must specify a node ID that belongs your tree. A new name of the node must be unique across all siblings.
    /// </summary>
    [HttpPost("api.user.tree.node.rename")]
    public async Task<IActionResult> Rename(
        [FromQuery, BindRequired] string treeName,
        [FromQuery, BindRequired] int nodeId,
        [FromQuery, BindRequired] string newNodeName
    )
    {
        var node = await dbContext.Nodes.FirstOrDefaultAsync(x => x.id == nodeId);

        if (node == null)
        {
            throw new Exception("Node tree not found for given id!");
        }

        if (dbContext.Nodes.Any(x => x.parentNodeId == node.parentNodeId && x.name == newNodeName && x.id != node.id))
        {
            throw new Exception($"A sibling node already exists with given name: {newNodeName}");
        }

        node.name = newNodeName;
        await dbContext.SaveChangesAsync();

        return Ok();
    }
}
