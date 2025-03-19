using Assessments.Shared.DTOs.Drupal;
using Assessments.Shared.DTOs.Drupal.Enums;

namespace Assessments.Shared.Interfaces;

public interface IDrupalRepository
{
    Task<ContentRootResponseDto> ContentById(int id, CancellationToken cancellationToken = default);

    Task<ContentRootResponseDto> ContentByType(ContentModelType modelType, CancellationToken cancellationToken = default);
}