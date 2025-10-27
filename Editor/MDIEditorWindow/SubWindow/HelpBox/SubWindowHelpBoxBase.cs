using UnityEngine;

namespace Knit.EditorWindow
{
	public enum SubWindowHelpBoxType
	{
		None,
		Bottom,
		Left,
		Right,
		Top,
		Locker,
	}
	public abstract class SubWindowHelpBox
	{
		public static SubWindowHelpBox CreateHelpBox( SubWindowHelpBoxType helpBoxType)
		{
			switch( helpBoxType)
			{
				case SubWindowHelpBoxType.Bottom:
				{
					return new SubWindowDockHelpBox( SubWindowDockHelpBox.DockPosition.Bottom);
				}
				case SubWindowHelpBoxType.Left:
				{
					return new SubWindowDockHelpBox( SubWindowDockHelpBox.DockPosition.Left);
				}
				case SubWindowHelpBoxType.Right:
				{
					return new SubWindowDockHelpBox( SubWindowDockHelpBox.DockPosition.Right);
				}
				case SubWindowHelpBoxType.Top:
				{
					return new SubWindowDockHelpBox( SubWindowDockHelpBox.DockPosition.Top);
				}
				case SubWindowHelpBoxType.Locker:
				{
					return new SubWindowLockerHelpBox();
				}
				default:
				{
					return null;
				}
			}
		}
		public abstract Rect DrawHelpBox( ref Rect rect);
	}
}