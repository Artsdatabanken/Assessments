using Assessments.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessments.Data;

public class AssessmentsDbContext(DbContextOptions<AssessmentsDbContext> options) : DbContext(options)
{
    public DbSet<Feedback> Feedbacks { get; set; }

    public DbSet<FeedbackAttachment> FeedbackAttachments { get; set; }

    public DbSet<EmailValidation> EmailValidations { get; set; }

    public DbSet<Log> Logs { get; set; }
}