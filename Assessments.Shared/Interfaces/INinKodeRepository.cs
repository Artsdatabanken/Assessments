using Assessments.Shared.DTOs.NinKode;

namespace Assessments.Shared.Interfaces;

public interface INinKodeRepository
{
    Task<List<VariablerResponseDto>> VariablerAlleKoder(CancellationToken cancellationToken = default);
}