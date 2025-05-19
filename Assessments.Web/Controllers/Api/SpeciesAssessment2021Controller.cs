using Assessments.Mapping.RedlistSpecies;
using Assessments.Shared.Constants;
using Assessments.Web.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Assessments.Web.Controllers.Api;

[EnableCors(nameof(CorsConstants.AllowAny))]
public class SpeciesAssessment2021Controller(DataRepository repository) : ODataController
{
    [EnableQuery(PageSize = 100)]
    public async Task<IQueryable<SpeciesAssessment2021>> Get() => await repository.GetSpeciesAssessments();

    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
        var query = await repository.GetSpeciesAssessments();
        return Ok(SingleResult.Create(query.Where(x => x.Id.Equals(key))));
    }
}