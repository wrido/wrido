using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Wrido.Resources
{
  public class ResourceController : Controller
  {
    private readonly IContentTypeProvider _contentTypeProvider;
    private readonly Dictionary<string, EmbeddedResource> _resources;

    public ResourceController(IEnumerable<EmbeddedResource> resources, IContentTypeProvider contentTypeProvider)
    {
      _contentTypeProvider = contentTypeProvider;
      _resources = resources.ToDictionary(r => r.ResourcePath, r => r, StringComparer.OrdinalIgnoreCase);
    }

    [HttpGet("resources/{*resourcePath}")]
    public async Task<ActionResult> GetResourceAsync(string resourcePath)
    {
      if (!_resources.ContainsKey(resourcePath))
      {
        return NotFound();
      }
      var resource = _resources[resourcePath];
      return File(resource.Data, resource.ContentType);
    }

    [HttpGet("preview/{*filePath}")]
    public async Task<ActionResult> GetFilePreviewAsync(string filePath)
    {
      if (!System.IO.File.Exists(filePath))
      {
        return NotFound();
      }

      var fileBytes = System.IO.File.ReadAllBytes(filePath);
      if (_contentTypeProvider.TryGetContentType(filePath, out var contentType))
      {
        return File(fileBytes, contentType);
      }

      return File(fileBytes, "text/plain");
    }
  }
}
