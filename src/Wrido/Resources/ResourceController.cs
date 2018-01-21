using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wrido.Core.Resources;

namespace Wrido.Resources
{
  public class ResourceController : Controller
  {
    private readonly Dictionary<string, EmbeddedResource> _resources;

    public ResourceController(IEnumerable<EmbeddedResource> resources)
    {
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
  }
}
