using Assessments.Shared.Constants;
using Assessments.Web.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using RodlisteNaturtyper.Data;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Web.Controllers.Api;

[ApiKeyRequired] // TODO: rln fjern etter lansering
[EnableCors(nameof(CorsConstants.AllowAny))]
[Produces("application/json")]
public class NatureTypeAssessment2025Controller(RodlisteNaturtyperDbContext dbContext) : ODataController
{
    [EnableQuery(PageSize = 100)]
    public IQueryable<Assessment> Get() => dbContext.Assessments.Where(x => x.Category != Category.NA).AsQueryable();

    [EnableQuery]
    public IActionResult Get(int key)
    {
        return Ok(SingleResult.Create(dbContext.Assessments.Where(x => x.Id == key && x.Category != Category.NA)));
    }
}