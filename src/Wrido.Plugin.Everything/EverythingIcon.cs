using System;
using System.Collections.Generic;
using System.Text;
using Wrido.Resources;

namespace Wrido.Plugin.Everything
{
  public class EverythingIcon
  {
    public static readonly Image EverythingLogo = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/everything/resources/everything.png", UriKind.Relative),
      Alt = "Everything",
    };

    public static readonly Image FolderIcon = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/everything/resources/foldericon.png", UriKind.Relative),
      Alt = "Everything",
    };
  }
}
