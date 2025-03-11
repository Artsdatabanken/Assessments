using System.Collections.Generic;
using System.Linq;
using Assessments.Shared.DTOs.NatureTypes;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Shared.Interfaces;

public interface INatureTypesRepository
{
    IQueryable<Assessment> GetAssessments();

    Assessment GetAssessment(int id);

    List<Committee> GetCommittees();

    List<CommitteeUserDto> GetCommitteeUsers();
}