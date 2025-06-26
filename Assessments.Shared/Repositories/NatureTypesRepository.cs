using Assessments.Shared.DTOs.NatureTypes;
using Assessments.Shared.Interfaces;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using RodlisteNaturtyper.Data;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Shared.Repositories;

public class NatureTypesRepository(IAppCache cache, RodlisteNaturtyperDbContext dbContext) : INatureTypesRepository
{
    public IQueryable<Assessment> GetAssessments() => dbContext.Assessments.Where(x => x.Category != Category.NA);

    public async Task<Assessment> GetAssessment(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Assessments
            .Include(x => x.Committee)
            .Include(x => x.AreaInformation)
            .Include(x => x.CriteriaInformation)
            .Include(x => x.Regions)
            .Include(x => x.References)
            .Include(x => x.NinCodeTopic)
            .Where(x => x.Category != Category.NA)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<List<CodeItemDto>> GetAssessmentCodeItemModels(int id, CancellationToken cancellationToken)
    {
        var assessment = await dbContext.Assessments.Include(x => x.CodeItems).ThenInclude(x => x.CodeItemParamLevel).FirstAsync(x => x.Id == id, cancellationToken: cancellationToken);

        var codeItemModels = new List<CodeItemDto>();

        codeItemModels.AddRange(assessment.CodeItems.OrderBy(x => x.CodeItemId).GroupBy(x => new { x.CodeItemId, x.AssessmentId }).Select(group =>
            new CodeItemDto
            {
                CodeItemId = group.First().CodeItemId,
                CodeItemDescription = group.First().CodeItemDescription,
                TimeOfIncident = group.First(x => x.CodeItemParamLevel.CodeItemParamTypeId == 1).CodeItemParamLevel
                    .Description,
                InfluenceFactor = group.First(x => x.CodeItemParamLevel.CodeItemParamTypeId == 2).CodeItemParamLevel
                    .Description,
                Magnitude = group.First(x => x.CodeItemParamLevel.CodeItemParamTypeId == 3).CodeItemParamLevel
                    .Description,
            }));

        if (codeItemModels.Count == 0)
            return codeItemModels;

        // TODO: bruke cache
        var codeItems = await dbContext.CodeItems.ToListAsync(cancellationToken: cancellationToken);

        foreach (var model in codeItemModels.OrderBy(x => x.CodeItemId))
        {
            var codeItem = codeItems.First(x => x.Id == model.CodeItemId);
            
            if (codeItem.ParentId == 0)
            {
                model.ParentCodeItems.Add(codeItem);
            }

            while (codeItem != null && codeItem.ParentId != 0)
            {
                var parent = codeItems.FirstOrDefault(a => a.Id == codeItem.ParentId);
                
                if (parent != null)
                {
                    model.ParentCodeItems.Add(parent);
                }

                model.ParentCodeItems.Add(codeItem);
                codeItem = parent;
            }

            model.ParentCodeItems = [.. model.ParentCodeItems.OrderBy(x => x.ParentId)];
        }

        return codeItemModels;
    }

    public async Task<List<CommitteeUserDto>> GetCommitteeUsers(CancellationToken cancellationToken)
    {
        return await cache.GetOrAddAsync($"{nameof(NatureTypesRepository)}-{nameof(GetCommitteeUsers)}", () => dbContext.CommitteeUsers.OrderBy(x => x.Committee.Name).Select(x => new CommitteeUserDto
        {
            CommitteeId = x.CommitteeId,
            CommitteeName = x.Committee.Name,
            Level = x.Level,
            UserLastName = x.User.LastName,
            UserFirstName = x.User.FirstName,
            UserCitationName = x.User.CitationName
        }).ToListAsync(cancellationToken: cancellationToken));
    }

    public async Task<List<Region>> GetRegions(CancellationToken cancellationToken)
    {
        return await cache.GetOrAddAsync($"{nameof(NatureTypesRepository)}-{nameof(GetRegions)}", () => dbContext.Regions.OrderBy(x => x.SortOrder).ToListAsync(cancellationToken: cancellationToken));
    }

    public async Task<List<NinCodeTopic>> GetNinCodeTopics(CancellationToken cancellationToken)
    {
        return await cache.GetOrAddAsync($"{nameof(NatureTypesRepository)}-{nameof(GetNinCodeTopics)}", () => dbContext.NinCodeTopics.OrderBy(x => x.Name).ToListAsync(cancellationToken: cancellationToken));
    }

    public async Task<List<KeyValuePair<string, int>>> GetNinCodeTopicSuggestions(CancellationToken cancellationToken)
    {
        return await cache.GetOrAddAsync($"{nameof(NatureTypesRepository)}-{nameof(GetNinCodeTopicSuggestions)}", async () =>
        {
            List<KeyValuePair<string, int>> items = [];

            var topics = await GetNinCodeTopics(cancellationToken);

            items.AddRange(topics.Select(ninCodeTopic => new KeyValuePair<string, int>(new string($"{ninCodeTopic.ShortCode} {ninCodeTopic.Name} (Tema: {ninCodeTopic.Description})"), ninCodeTopic.Id)));

            return items;
        });
    }

    public async Task<List<CodeItem>> GetCodeItems(CancellationToken cancellationToken)
    {
        return await cache.GetOrAddAsync($"{nameof(NatureTypesRepository)}-{nameof(GetCodeItems)}", () => dbContext.CodeItems.ToListAsync(cancellationToken: cancellationToken));
    }
}