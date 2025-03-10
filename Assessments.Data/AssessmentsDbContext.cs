using Assessments.Data.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Assessments.Data;

public class AssessmentsDbContext(DbContextOptions<AssessmentsDbContext> options) : DbContext(options), IDataProtectionKeyContext
{
    public DbSet<Feedback> Feedbacks { get; set; }

    public DbSet<FeedbackAttachment> FeedbackAttachments { get; set; }

    public DbSet<EmailValidation> EmailValidations { get; set; }

    public DbSet<Log> Logs { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}