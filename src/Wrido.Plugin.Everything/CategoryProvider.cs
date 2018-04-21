using System;
using System.Collections.Generic;
using Everything.Model;

namespace Wrido.Plugin.Everything
{
  public interface ICategoryProvider
  {
    string Get(ResultItem item);
  }

  public class CategoryProvider : ICategoryProvider
  {
    private const string _documents = "Documents";
    private const string _applications = "Applications";
    private const string _pictures = "Pictures";
    private const string _folders = "Folders";
    private const string _compressed = "Compressed Archive";
    private const string _files = "Files";
    private const string _sourceCode = "Source Code";

    private static readonly IDictionary<string, string> FileToCategory = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
      { ".exe", _applications },
      { ".bat", _applications },
      { ".msi", _applications },
      { ".cmd", _applications },

      { ".png", _pictures},
      { ".jpeg", _pictures },
      { ".jpg", _pictures },
      { ".bmp", _pictures },
      { ".gif", _pictures },
      { ".psd", _pictures },

      { ".doc", _documents },
      { ".docx", _documents },
      { ".pdf", _documents },
      { ".ppt", _documents },
      { ".pptx", _documents },
      { ".xlsx", _documents },

      { ".htm", _sourceCode },
      { ".html", _sourceCode },
      { ".cs", _sourceCode },
      { ".js", _sourceCode },
      { ".jsx", _sourceCode },
      { ".ts", _sourceCode },
      { ".ps1", _sourceCode },

      { ".zip", _compressed},
      { ".7z", _compressed},
      { ".tar", _compressed}
    };

    public string Get(ResultItem item)
    {
      if (!(item is FileResultItem file))
      {
        return _folders;
      }

      if (FileToCategory.ContainsKey(file.Extension))
      {
        return FileToCategory[file.Extension];
      }

      return _files;
    }
  }
}
