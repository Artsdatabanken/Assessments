using Assessments.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Assessments.Web.Views.NatureTypes.Components.VariableNames;

public class VariableNamesViewComponent(INinKodeRepository ninKodeRepository) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string appliedVariables)
    {
        var viewModel = new VariableNamesModel
        {
            VariableNames = await GetVariableNames(appliedVariables)
        };

        return View("VariableNames", viewModel);
    }

    private async Task<List<VariableNameViewModel>> GetVariableNames(string appliedVariables)
    {
        var variableNames = new List<VariableNameViewModel>();

        if (string.IsNullOrEmpty(appliedVariables))
            return variableNames;

        var variables = await ninKodeRepository.VariablerAlleKoder();
        var appliedVariablesList = appliedVariables.Split(";");

        foreach (var appliedVariable in appliedVariablesList)
        {
            var variable = appliedVariable.Split("_").First();

            var variablerResponseDtos = variables.SelectMany(x => x.Variabelnavn).FirstOrDefault(y => y.Kode.Id == variable);

            if (variablerResponseDtos == null)
                continue;

            var variableName = new VariableNameViewModel
            {
                Name = variablerResponseDtos.Navn,
                ShortCode = variablerResponseDtos.Kode.Id,
                LongCode = variablerResponseDtos.Kode.Langkode
            };

            var variableStep = appliedVariable.Split("_").Last();
            var variableSteps = variableStep.Split("&");

            foreach (var step in variableSteps)
            {
                var trinnResponseDto = variablerResponseDtos.Variabeltrinn.SelectMany(x => x.Trinn).FirstOrDefault(x => x.Verdi == step);

                if (trinnResponseDto != null)
                {
                    variableName.Steps.Add(new VariableNameStepViewModel { Description = trinnResponseDto.Beskrivelse });
                }
            }

            variableNames.Add(variableName);
        }

        return variableNames;
    }

}