using System;
using System.Net.Mime;
using BusinessServices;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Api;

[Route("api/images")]
public class ImageController : Controller
{
    private readonly IStorage _storage;

    /// <inheritdoc />
    public ImageController(IStorage storage) => _storage = storage;

    [HttpGet("{ownerId}/{imageId}")]
    public IActionResult GetImage(Guid ownerId, Guid imageId) => File(_storage.GetImage(ownerId, imageId), MediaTypeNames.Image.Jpeg);
}