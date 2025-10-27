
using UnityEngine;
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	internal static class GUIExpansion
	{
		internal static int Toggle( bool value, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = GUILayoutUtility.GetRect( content, style, options);
			return DoToggle( rect, GUIUtility.GetControlID( kToggleHash, FocusType.Passive, rect), value, content, style);
		}
		static int DoToggle( Rect rect, int id, bool value, GUIContent content, GUIStyle style)
		{
			return DoControl( rect, id, value, rect.Contains( Event.current.mousePosition), content, style);
		}
		static int DoControl( Rect rect, int id, bool on, bool hover, GUIContent content, GUIStyle style)
		{
			Event ev = Event.current;
			
			switch( ev.type)
			{
				case EventType.Repaint:
				{
					if( hover != false && HasMouseControl( id) < 0)
					{
						hover = false;
					}
					style.Draw( rect, content, hover, true, on, false);
					break;
				}
				case EventType.MouseDown:
				{
					if( rect.Contains( ev.mousePosition) != false)
					{
						GrabMouseControl( id, ev.button);
						ev.Use();
					}
					break;
				}
				case EventType.MouseUp:
				{
					int button = HasMouseControl( id);
					if( button >= 0)
					{
						ReleaseMouseControl();
						ev.Use();
						
						if( rect.Contains( ev.mousePosition) != false)
						{
							if( button == ev.button)
							{
								return button;
							}
						}
					}
					break;
				}
				case EventType.MouseDrag:
				{
					if( HasMouseControl( id) >= 0)
					{
						ev.Use();
					}
					break;
				}
			}
			return -1;
		}
		internal static void GrabMouseControl( int id, int button)
		{
			if( s_MouseControl.ContainsKey( id) == false)
			{
				s_MouseControl.Add( id, button);
			}
		}
		internal static int HasMouseControl( int id)
		{
			int button;
			
			if( s_MouseControl.TryGetValue( id, out button) == false)
			{
				button = -1;
			}
			return button;
		}
		internal static void ReleaseMouseControl()
		{
			s_MouseControl.Clear();
		}
		internal static void GrabKeyControl( KeyCode code, int id)
		{
			if( s_KeyControl.ContainsKey( code) == false)
			{
				s_KeyControl.Add( code, id);
			}
		}
		internal static bool HasKeyControl( KeyCode code)
		{
			return s_KeyControl.ContainsKey( code);
		}
		internal static bool HasKeyControl( KeyCode code, int id)
		{
			if( s_KeyControl.TryGetValue( code, out int value) != false)
			{
				return value == id;
			}
			return false;
		}
		internal static void ReleaseKeyControl( KeyCode code)
		{
			if( s_KeyControl.ContainsKey( code) != false)
			{
				s_KeyControl.Remove( code);
			}
		}
		static readonly int kToggleHash = "ExToggle".GetHashCode();
		static readonly Dictionary<int, int> s_MouseControl = new();
		static readonly Dictionary<KeyCode, int> s_KeyControl = new();
	}
}
