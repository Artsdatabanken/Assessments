using System.Net;
using Assessments.Data;
using Assessments.Data.Models;
using Assessments.Web.Models;
using Assessments.Shared.Helpers;
using Assessments.Shared.Options;
using Azure.Storage.Blobs;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;

namespace Assessments.Web.Controllers;

public class FeedbackController : BaseController<FeedbackController>
{
    private readonly AssessmentsDbContext _dbContext;
    private readonly BlobContainerClient _blob;
    private readonly string _feedbackSecret;
    private readonly ISendGridClient _sendGridClient;
    private readonly IOptions<ApplicationOptions> _options;
    public const string ValidationCookieName = "adb.req.feedbackvalidation";

    public FeedbackController(IConfiguration configuration, AssessmentsDbContext dbContext, ISendGridClient sendGridClient, IOptions<ApplicationOptions> options)
    {
        _options = options;
        _sendGridClient = sendGridClient;
        _dbContext = dbContext;

        var azureBlobStorageConnectionString = configuration["ConnectionStrings:AzureBlobStorage"];

        if (string.IsNullOrEmpty(azureBlobStorageConnectionString))
            throw new Exception("Missing required config for azure blog storage: ConnectionStrings:AzureBlobStorage");

        _blob = new BlobContainerClient(azureBlobStorageConnectionString, "feedback");

        _feedbackSecret = _options.Value.FeedbackSecret;
    }

    public async Task<IActionResult> Index(string secret)
    {
        if (_feedbackSecret != secret)
            return Unauthorized();

        var feedback = await _dbContext.Feedbacks.AsNoTracking().OrderBy(x => x.CreatedOn).ToListAsync();

        MemoryStream memoryStream;
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Tilbakemeldinger");

            worksheet.Cell(1, 1).InsertTable(feedback);
            workbook.Worksheet(1).Columns().AdjustToContents();

            var worksheet2 = workbook.Worksheets.Add("Vedlegg");

            worksheet2.Cell(1, 1).Value = "FeedbackId";
            worksheet2.Cell(1, 2).Value = "ExpertGroup";
            worksheet2.Cell(1, 3).Value = "FileName";

            var row = 1;

            var attachments = await _dbContext.FeedbackAttachments.Include(x => x.Feedback).AsNoTracking().ToListAsync();

            var baseUrl = $"{Request.Scheme}://{Request.Host.ToUriComponent()}{Request.PathBase.ToUriComponent()}";

            foreach (var attachment in attachments)
            {
                ++row;

                worksheet2.Cell(row, 1).Value = attachment.FeedbackId;
                worksheet2.Cell(row, 2).Value = attachment.Feedback.ExpertGroup;
                worksheet2.Cell(row, 3).Value = attachment.FileName;
                worksheet2.Cell(row, 3).SetHyperlink(new XLHyperlink($"{baseUrl}/feedback/attachment/{attachment.Id}?secret={_feedbackSecret}"));
                worksheet2.Cell(row, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            }

            workbook.Worksheet(2).Columns().AdjustToContents();

            foreach (var workbookWorksheet in workbook.Worksheets)
                workbookWorksheet.SheetView.FreezeRows(1);

            memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"feedback-{DateTime.Now:dd.MM.yyyy}.xlsx"
        };
    }

    public async Task<IActionResult> Attachment(int id, string secret)
    {
        if (_feedbackSecret != secret)
            return Unauthorized();

        var attachment = await _dbContext.FeedbackAttachments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (attachment == null)
            return NotFound();

        var file = _blob.GetBlobClient(attachment.BlobName);

        if (!await file.ExistsAsync())
            return NotFound();

        var stream = await file.OpenReadAsync();

        return File(stream, "application/octet-stream", attachment.FileName);
    }

    public async Task<IActionResult> ValidateEmail(FeedbackViewModel viewModel, string returnUrl)
    {
        ModelState.Remove(nameof(FeedbackViewModel.Comment));

        if (!ModelState.IsValid || !Url.IsLocalUrl(returnUrl))
            return BadRequest("Beklager, en feil oppstod ved innsending av skjema.");

        var email = viewModel.Email.ToLowerInvariant().Trim();
        var emailValidation = await _dbContext.EmailValidations.FirstOrDefaultAsync(x => x.Email == email);

        if (emailValidation == null)
        {
            emailValidation = new EmailValidation
            {
                Email = email,
                FullName = viewModel.FullName.Trim()
            };

            _dbContext.EmailValidations.Add(emailValidation);

            await _dbContext.SaveChangesAsync();
        }

        var message = new SendGridMessage
        {
            From = new EmailAddress("noreply@artsdatabanken.no"),
            Subject = "Bekreftelse av e-postadresse for tilbakemelding"
        };

        message.AddTo(new EmailAddress(emailValidation.Email, emailValidation.FullName));

        returnUrl = returnUrl.Split('?')[0]; // remove querystring

        var validationUrl = new Uri(_options.Value.BaseUrl, $"{returnUrl}?guid={emailValidation.Guid}#Feedback");

        var messageContent = $"<p>Klikk på lenken nedenfor for å bekrefte din e-postadresse. Dette gir deg tilgang til å gi tilbakemelding.</p><p><a href='{validationUrl}'>{validationUrl}</a></p>";

        var sendMail = await SendMail(message, messageContent);

        if (!sendMail)
            return BadRequest("Beklager, en feil oppstod ved sending av e-post.");

        TempData["feedback"] = $"Du vil bli tilsendt en e-post (til {email}) som brukes for å bekrefte e-postadressen og med lenke for tilbakemelding.";

        return Url.IsLocalUrl(returnUrl) ? Redirect($"{returnUrl}#Feedback") : BadRequest();
    }

    public async Task<IActionResult> ForgetValidation(string code, string returnUrl)
    {
        if (!Guid.TryParse(code, out _) || !Url.IsLocalUrl(returnUrl))
            return BadRequest();

        var validation = await _dbContext.EmailValidations.FirstOrDefaultAsync(x => x.Guid == new Guid(code));

        if (validation == null)
            return BadRequest();

        _dbContext.EmailValidations.Remove(validation);

        await _dbContext.SaveChangesAsync();

        HttpContext.Response.Cookies.Delete(ValidationCookieName);

        TempData["feedback"] = "Din e-postadresse er slettet.";

        return Redirect($"{returnUrl.Split('?')[0]}#Feedback");
    }

    [HttpPost]
    public async Task<IActionResult> AddFeedback(FeedbackViewModel viewModel, Guid code, string returnUrl)
    {
        if (!ModelState.IsValid || !Url.IsLocalUrl(returnUrl))
            return BadRequest();

        var validation = await _dbContext.EmailValidations.FirstOrDefaultAsync(x => x.Guid == code);

        if (validation == null || !validation.Email.Equals(viewModel.Email) || !validation.FullName.Equals(viewModel.FullName))
            return BadRequest();
        
        var feedback = new Feedback
        {
            AssessmentId = viewModel.AssessmentId,
            AssessmentName = viewModel.AssessmentName,
            Year = viewModel.Year,
            Type = viewModel.Type,
            ExpertGroup = viewModel.ExpertGroup,
            FullName = viewModel.FullName.Trim().StripHtml(),
            Email = viewModel.Email.ToLowerInvariant(),
            Comment = viewModel.Comment.Trim().StripHtml()
        };

        _dbContext.Feedbacks.Add(feedback);

        await _dbContext.SaveChangesAsync();

        if (viewModel.FormFiles != null)
        {
            await _blob.CreateIfNotExistsAsync();

            string[] permittedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };

            foreach (var formFile in viewModel.FormFiles)
            {
                if (formFile.Length <= 0)
                    continue;

                var extension = Path.GetExtension(formFile.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                    continue;

                var encodedFileName = WebUtility.HtmlEncode(Path.GetFileName(formFile.FileName));
                var blobName = $"{feedback.Type}/{feedback.Year}/{feedback.Id}/{Guid.NewGuid()}_{encodedFileName}";

                await using var stream = formFile.OpenReadStream();
                await _blob.UploadBlobAsync(blobName, stream);

                feedback.Attachments.Add(new FeedbackAttachment
                {
                    FeedbackId = feedback.Id,
                    BlobName = blobName,
                    FileName = formFile.FileName
                });

                Logger.LogDebug("{blobName} uploaded to storage", blobName);
            }

            await _dbContext.SaveChangesAsync();
        }

        var message = new SendGridMessage
        {
            From = new EmailAddress("noreply@artsdatabanken.no"),
            Subject = $"Tilbakemelding på foreløpig vurdering av {feedback.AssessmentName}"
        };

        message.AddTo(new EmailAddress(feedback.Email, feedback.FullName));

        var feedbackAttachments = string.Empty;

        if (feedback.Attachments.Count != 0)
            feedbackAttachments = $"<p>Antall vedlegg: {feedback.Attachments.Count}</p>";

        var messageContent = $"<p>Under innsyn i de foreløpige vurderingene har vi mottatt følgende tilbakemelding på vurderingen av '{feedback.AssessmentName}':</p><p>{feedback.Comment}</p>{feedbackAttachments}";

        var sendMail = await SendMail(message, messageContent);

        if (!sendMail)
            return BadRequest("Beklager, en feil oppstod ved sending av e-post.");

        TempData["feedback"] = "Takk for tilbakemeldingen.";

        return Redirect($"{returnUrl}#Feedback");
    }

    public IActionResult Terms() => View();

    private async Task<bool> SendMail(SendGridMessage message, string messageContent)
    {
        messageContent += "<p>Dette er en automatisk generert e-post som du ikke kan svare på.</p>";

        message.AddContent(MimeType.Html, messageContent);
        message.AddContent(MimeType.Text, messageContent.StripHtml());

        try
        {
            var response = await _sendGridClient.SendEmailAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed sending email with StatusCode: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred: {message}", ex.Message);
            return false;
        }

        return true;
    }
}