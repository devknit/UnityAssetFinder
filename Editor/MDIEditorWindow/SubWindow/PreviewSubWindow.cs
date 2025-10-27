using UnityEngine;
using System.Reflection;

namespace Knit.EditorWindow
{
	[SubWindowStyle( SubWindowStyle.Preview)]
	public class PreviewSubWindow : SubWindow
	{
		public PreviewSubWindow( 
			string title, string icon, bool defaultOpen, MethodInfo method, 
			object target, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpbox)
			: base(title, icon, defaultOpen, method, target, toolbar, helpbox)
		{
		}
		protected override Rect DrawMainArea( Rect rect)
		{
			GUI.Box( rect, string.Empty, GUIStyleCache.GetStyle( "GameViewBackground"));
			return rect;
		}
	}
}