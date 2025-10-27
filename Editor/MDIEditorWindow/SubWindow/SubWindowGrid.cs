
using UnityEngine;
using System.Reflection;

namespace Knit.EditorWindow
{
	[SubWindowStyle( SubWindowStyle.Grid)]
	public class SubWindowGrid : SubWindow
	{
		public SubWindowGrid( 
			string title, string icon, bool defaultOpen, MethodInfo method, 
			object target, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpbox) 
			: base(title, icon, defaultOpen, method, target, toolbar, helpbox)
		{
		}
		protected override Rect DrawMainArea( Rect rect)
		{
			GUI.BeginGroup( rect, GUIStyleCache.GetStyle( "GameViewBackground"));
			
			if( m_PanelBackground == null)
			{
				CreatePanelBackground();
			}
			int tileCountX = Mathf.CeilToInt( rect.width / kTileSize);
			int tileCountY = Mathf.CeilToInt( rect.width / kTileSize);
			
			if( m_TileCountX != tileCountX || m_TileCountY != tileCountY)
			{
				CheckBoard( rect, tileCountX, tileCountY);
			}
			m_TileCountX = tileCountX;
			m_TileCountY = tileCountY;
			
			for( int i0 = -tileCountX; i0 < 2 * tileCountX; i0++)
			{
				for( int i1 = -tileCountY; i1 < 2 * tileCountY; i1++)
				{
					GUI.DrawTexture( new Rect(
						rect.x + m_SceneViewPosition.x + i0 * kTileSize, 
						rect.y + m_SceneViewPosition.y + i1 * kTileSize, 
						kTileSize, kTileSize), m_PanelBackground);
				}
			}
			GUI.EndGroup();
			ListenDrawMainPanel( rect);
			
			return new Rect( 
				rect.x + m_SceneViewPosition.x, 
				rect.y + m_SceneViewPosition.y, 
				m_TileCountX * 2 * kTileSize, 
				m_TileCountY * 2 * kTileSize);
		}
		void CreatePanelBackground()
		{
			m_PanelBackground = new Texture2D( kTileSize, kTileSize, TextureFormat.RGBA32, false);
			
			for( int i0 = 0; i0 < kTileSize; i0++)
			{
				for( int i1 = 0; i1 < kTileSize; i1++)
				{
					if( i0 == 0 || i1 == 0)
					{
						m_PanelBackground.SetPixel( i0, i1, new Color( 0.05f, 0.05f, 0.05f));
					}
					else if( i0 % 10 == 0 || i1 % 10 == 0)
					{
						m_PanelBackground.SetPixel( i0, i1, new Color( 0.133f, 0.133f, 0.133f));
					}
					else
					{
						m_PanelBackground.SetPixel( i0, i1, new Color( 0, 0, 0, 0));
					}
				}
			}
			m_PanelBackground.Apply();
		}
		void ListenDrawMainPanel( Rect rect)
		{
			if( Event.current.type == EventType.MouseDown && Event.current.button == 2)
			{
				if( rect.Contains( Event.current.mousePosition) != false)
				{
					m_IsDragging = true;
					Event.current.Use();
				}
			}
			if( m_IsDragging != false && Event.current.type == EventType.MouseUp && Event.current.button == 2)
			{
				Event.current.Use();
				m_IsDragging = false;
			}
			if( m_IsDragging && Event.current.type == EventType.MouseDrag && Event.current.button == 2)
			{
				m_SceneViewPosition += Event.current.delta;
				CheckBoard( rect, m_TileCountX, m_TileCountY);
				Event.current.Use();
			}
		}
		void CheckBoard( Rect rect, int tileCountX, int tileCountY)
		{
			if( m_SceneViewPosition.x < rect.width - tileCountX * 2 * kTileSize)
			{
				m_SceneViewPosition.x = rect.width - tileCountX * 2 * kTileSize;
			}
			if( m_SceneViewPosition.x > tileCountX * kTileSize)
			{
				m_SceneViewPosition.x = tileCountX * kTileSize;
			}
			if( m_SceneViewPosition.y < rect.height - tileCountY * 2 * kTileSize)
			{
				m_SceneViewPosition.y = rect.height - tileCountY * 2 * kTileSize;
			}
			if( m_SceneViewPosition.y > tileCountY * kTileSize)
			{
				m_SceneViewPosition.y = tileCountY * kTileSize;
			}
		}
		Texture2D m_PanelBackground;
		Vector2 m_SceneViewPosition;
		bool m_IsDragging;
		const int kTileSize = 100;
		int m_TileCountX = 0;
		int m_TileCountY = 0;
	}
}