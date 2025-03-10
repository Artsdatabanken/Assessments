using Assessments.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Assessments.Web.Controllers.Api;

public class AlienSpeciesAssessment2023Controller(DataRepository repository) : ODataController
{
    [EnableQuery(MaxTop = 100, PageSize = 100)]
    public async Task<ActionResult> Get() => Ok(await repository.GetAlienSpeciesAssessments());
    
    [EnableQuery]
    public async Task<ActionResult> Get(int key)
    {
        var query = await repository.GetAlienSpeciesAssessments();
        var assessment = query.FirstOrDefault(x => x.Id.Equals(key));

        return assessment != null ? Ok(assessment) : NotFound();
    }
}