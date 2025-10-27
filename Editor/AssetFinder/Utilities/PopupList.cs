
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	internal class PopupList : PopupWindowContent
	{
		internal class Element
		{
			internal Element( string text)
			{
				m_Content = new GUIContent( text);
			}
			internal string Label
			{
				get{ return m_Content.text; }
			}
			internal GUIContent Content
			{
				get{ return m_Content; }
			}
			internal bool Selected
			{
				get{ return m_Selected; }
				set{ m_Selected = value; }
			}
            readonly GUIContent m_Content;
			bool m_Selected;
		}
		internal class InputData
		{
			internal InputData()
			{
				elements = new List<Element>();
			}
			internal Element NewOrMatchingElement( string label)
			{
				foreach( var element in elements)
				{
					if( element.Label.Equals( label, StringComparison.OrdinalIgnoreCase) != false)
					{
						return element;
					}
				}
				var newElement = new Element( label);
				elements.Add( newElement);
				
				return newElement;
			}
			internal List<Element> elements;
			internal Action<Element> onSelectCallback;
		}
		internal PopupList( InputData inputData)
		{
			m_InputData = inputData;
		}
		public override Vector2 GetWindowSize()
		{
			return new Vector2( kWindowWidth, m_InputData.elements.Count * kLineHeight + 2 * kMargin);
		}
		public override void OnGUI( Rect windowRect)
		{
			Event ev = Event.current;
			
			if( ev.type == EventType.Layout)
			{
				return;
			}
			if( s_Styles == null)
			{
				s_Styles = new Styles();
			}
			if( ev.type == EventType.KeyDown && ev.keyCode == KeyCode.Escape)
			{
				editorWindow.Close();
				GUIUtility.ExitGUI();
			}
			int i0 = 0;
			
			foreach( Element element in m_InputData.elements)
			{
				var rect = new Rect(
					windowRect.x, 
					windowRect.y + kMargin + i0 * kLineHeight, 
					windowRect.width, 
					kLineHeight);
				
				switch( ev.type)
				{
					case EventType.Repaint:
					{
						bool selected = element.Selected;
						bool hover = i0 == m_SelectedIndex;
						s_Styles.menuItem.Draw( rect, element.Content, hover, selected, selected, false);
						break;
					}
					case EventType.MouseDown:
					{
						if( ev.button == 0 && rect.Contains( ev.mousePosition) != false)
						{
							m_InputData.onSelectCallback?.Invoke( element);
							ev.Use();
						}
						break;
					}
					case EventType.MouseMove:
					{
						if( rect.Contains( ev.mousePosition) != false)
						{
							m_SelectedIndex = i0;
							ev.Use();
						}
						break;
					}
				}
				++i0;
			}
			if( ev.type == EventType.Repaint)
			{
				s_Styles.background.Draw( windowRect, false, false, false, false);
			}
		}
		const float kWindowWidth = 150;
		const float kLineHeight = 16;
		const float kMargin = 10;
		
		class Styles
		{
			internal GUIStyle menuItem = "MenuItem";
			internal GUIStyle background = "grey_border";
		}
		static Styles s_Styles;
		
		readonly InputData m_InputData;
		int m_SelectedIndex;
	}
}
