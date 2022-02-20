using System;
using BusinessServices;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Api;

[Route("api/images")]
public class ImageController : Controller
{
    private readonly IStorage _storage;

    /// <inheritdoc />
    public ImageController(IStorage storage) => _storage = storage;

    [HttpGet("{id}")]
    public IActionResult Get(Guid id) => File(_storage.GetImage(id), "image/jpeg");
}