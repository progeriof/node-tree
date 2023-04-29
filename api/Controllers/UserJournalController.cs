using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NodeTree.Controllers;

[ApiController]
[Tags("user.journal")]
public class UserJournalController : ControllerBase
{
    private ApplicationDbContext dbContext;

    public UserJournalController(ApplicationDbContext _dbContext)
    {
        this.dbContext = _dbContext;
    }

    public class Filter
    {
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public string? search { get; set; }
    }

    /// <summary>
    /// Provides the pagination API. Skip means the number of items should be skipped by server. Take means the maximum number items should be returned by server. All fields of the filter are optional.
    /// </summary>
    [HttpPost("api.user.journal.getRange")]
    public MRange<Journal> GetRange(
        [FromQuery, BindRequired] int skip,
        [FromQuery, BindRequired] int take,
        [FromBody, BindRequired] Filter filter
        )
    {
        var journals = dbContext.Journals.AsQueryable();

        if (filter.from != null) {
            journals = journals.Where(x => x.createdAt >= filter.from);
        }

        if (filter.to != null) {
            journals = journals.Where(x => x.createdAt <= filter.to);
        }

        if (!String.IsNullOrWhiteSpace(filter.search)) {
            journals = journals.Where(x => x.text.Contains(filter.search));
        }

        int count = dbContext.Journals.Count();

        var result = new MRange<Journal>{
            skip = skip,
            count = count,
            items = journals.ToList()
        };

        return result;
    }

    /// <summary>
    /// Returns the information about an particular event by ID.
    /// </summary>
    [HttpPost("api.user.journal.getSingle")]
    public Journal? GetSingle([FromQuery, BindRequired] int id)
    {
        var journal = dbContext.Journals
            .FirstOrDefault(x => x.id == id);

        return journal;
    }
}
