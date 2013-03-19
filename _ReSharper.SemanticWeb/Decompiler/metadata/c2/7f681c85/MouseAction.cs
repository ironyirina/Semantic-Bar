// Type: System.Windows.Input.MouseAction
// Assembly: PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF\PresentationCore.dll

using System.ComponentModel;
using System.Windows.Markup;

namespace System.Windows.Input
{
  /// <summary>
  /// Specifies constants that define actions performed by the mouse.
  /// </summary>
  [ValueSerializer(typeof (MouseActionValueSerializer))]
  [TypeConverter(typeof (MouseActionConverter))]
  public enum MouseAction : byte
  {
    None,
    LeftClick,
    RightClick,
    MiddleClick,
    WheelClick,
    LeftDoubleClick,
    RightDoubleClick,
    MiddleDoubleClick,
  }
}
