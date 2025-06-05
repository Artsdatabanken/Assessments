using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Shared.Constants;
using Assessments.Web.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Assessments.Web.Controllers.Api;

[EnableCors(nameof(CorsConstants.AllowAny))]
[Produces("application/json")]
public class AlienSpeciesAssessment2023Controller(DataRepository repository) : ODataController
{
    [EnableQuery(PageSize = 100)]
    public async Task<IQueryable<AlienSpeciesAssessment2023>> Get() => await repository.GetAlienSpeciesAssessments();

    [EnableQuery]
    [Produces<AlienSpeciesAssessment2023>]
    public async Task<IActionResult> Get(int key)
    {
        var query = await repository.GetAlienSpeciesAssessments();
        return Ok(SingleResult.Create(query.Where(x => x.Id.Equals(key))));
    }
}