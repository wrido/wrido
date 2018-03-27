using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
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

    [HttpGet("resources")]
    public ActionResult GetResources([FromQuery] string type = null)
    {
      IEnumerable<string> resourcesPaths;
      if (string.IsNullOrEmpty(type))
      {
        resourcesPaths = _resources.Values.Select(v => v.ResourcePath);
      }
      else
      {
        resourcesPaths = _resources.Values
          .Where(v => v.ResourcePath.EndsWith(type, StringComparison.InvariantCultureIgnoreCase))
          .Select(v => v.ResourcePath);
      }

      return Ok(new { _links = resourcesPaths.ToList()});
    }

    [HttpGet("icons/{*filePath}")]
    public ActionResult GetIcon(string filePath)
    {
      if (!System.IO.File.Exists(filePath))
      {
        return NotFound();
      }

      using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath))
      using (var memoryStream = new MemoryStream())
      {
        icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
        return File(memoryStream.GetBuffer(), "image/png");
      }
    }

    [HttpGet("resources/{*resourcePath}")]
    public ActionResult GetResource(string resourcePath)
    {
      if (!_resources.ContainsKey(resourcePath))
      {
        return NotFound();
      }
      var resource = _resources[resourcePath];

      if (_contentTypeProvider.TryGetContentType(resourcePath, out var contentType))
      {
        // temp fix to remove null character
        if (contentType.EndsWith("script"))
        {
          var dataAsString = Encoding.UTF8.GetString(resource.Data).Replace("\0", string.Empty);
          var dataAsBytes = Encoding.UTF8.GetBytes(dataAsString);
          return File(dataAsBytes, contentType);
        }

        return File(resource.Data, contentType);
      }

      return File(resource.Data, "text/plain");
    }

    [HttpGet("preview/{*filePath}")]
    public ActionResult GetFilePreview(string filePath)
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
