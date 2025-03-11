using System;
using System.Collections.Generic;
using System.Linq;
using Assessments.Shared.DTOs.NatureTypes;
using Assessments.Shared.Interfaces;
using Assessments.Shared.Options;
using Default;
using LazyCache;
using Microsoft.Extensions.Options;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Shared.Repositories;

public class NatureTypesRepository : INatureTypesRepository
{
    private readonly Container _context;
    private readonly IAppCache _appCache;

    public NatureTypesRepository(IOptions<ApplicationOptions> options, IAppCache appCache)
    {
        _appCache = appCache;
        _appCache.DefaultCachePolicy.DefaultCacheDurationSeconds = TimeSpan.FromHours(1).Seconds;

        _context = new Container(options.Value.NatureTypes.ODataUrl);
        _context.BuildingRequest += (_, e) => e.Headers.Add("X-API-KEY", options.Value.NatureTypes.ODataApiKey);
    }

    public IQueryable<Assessment> GetAssessments() => _context.Assessments.Expand(x => x.Committee);

    public Assessment GetAssessment(int id)
    {
        return _context.Assessments.Expand(x => x.Committee).FirstOrDefault(c => c.Id == id);
    }

    public List<Committee> GetCommittees()
    {
        return _appCache.GetOrAdd($"{nameof(NatureTypesRepository)}-{nameof(GetCommittees)}", () => _context.Committees.OrderBy(x => x.Name).ToList());
    }

    public List<CommitteeUserDto> GetCommitteeUsers()
    {
        return _appCache.GetOrAdd($"{nameof(NatureTypesRepository)}-{nameof(GetCommitteeUsers)}", () => _context.CommitteeUsers.OrderBy(x => x.Committee.Name).Select(x => new CommitteeUserDto
        {
            CommitteeId = (int)x.CommitteeId,
            CommitteeName = x.Committee.Name,
            Level = x.Level,
            UserLastName = x.User.LastName,
            UserFirstName = x.User.FirstName,
            UserCitationName = x.User.CitationName
        }).ToList());
    }
}