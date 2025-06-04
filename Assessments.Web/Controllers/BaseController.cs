using Assessments.Web.Infrastructure;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Assessments.Web.Controllers;

public abstract class BaseController<T> : Controller where T : BaseController<T>
{
    private IWebHostEnvironment _environment;

    protected IWebHostEnvironment Environment => _environment ??= HttpContext.RequestServices.GetService<IWebHostEnvironment>();

    private DataRepository _dataRepository;

    protected DataRepository DataRepository => _dataRepository ??= HttpContext.RequestServices.GetService<DataRepository>();

    private IMapper _mapper;

    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();

    private ILogger<T> _logger;

    protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();

    protected const int DefaultPageSize = 25;
}