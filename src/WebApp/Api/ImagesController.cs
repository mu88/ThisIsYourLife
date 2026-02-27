using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using BusinessServices;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Api;

[Route("api/images")]
public class ImagesController : Controller
{
    private readonly IStorage _storage;

    public ImagesController(IStorage storage) => _storage = storage;

    [HttpGet("{ownerId}/{imageId}")]
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP004:Don\'t ignore created IDisposable", Justification = "Okay due to different lifetime")]
    public IActionResult GetImage(Guid ownerId, Guid imageId) => File(_storage.GetImage(ownerId, imageId), MediaTypeNames.Image.Jpeg);
}
