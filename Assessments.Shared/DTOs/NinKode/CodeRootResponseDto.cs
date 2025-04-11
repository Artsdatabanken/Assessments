namespace Assessments.Shared.DTOs.NinKode;

public record CodeRootResponseDto
{
    public List<VariablerResponseDto> Variabler { get; init; } = [];
}

public record VariablerResponseDto
{
    public List<VariabelnavnResponseDto> Variabelnavn { get; init; } = [];

    public string Navn { get; init; }

    public KodeResponseDto Kode { get; init; }
}

public record VariabelnavnResponseDto
{
    public string Navn { get; init; }

    public KodeResponseDto Kode { get; init; }

    public List<VariabelnavnTrinnResponseDto> Variabeltrinn { get; init; } = [];
}

public record KodeResponseDto
{
    public string Id { get; init; }

    public string Langkode { get; init; }
}

public record VariabelnavnTrinnResponseDto
{
    public List<TrinnResponseDto> Trinn { get; init; } = [];
}

public record TrinnResponseDto
{
    public string Verdi { get; init; }

    public string Kode { get; init; }

    public string Beskrivelse { get; init; }
}