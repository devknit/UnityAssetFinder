using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	internal class SubWindowNode
	{
		public SubWindowNode( bool horizontal, int depth)
		{
			m_IsHorizontal = horizontal;
			m_Depth = depth;
		}
		public virtual void DrawGUI( Rect rect, System.Action repaintAction)
		{
			float offset = 0;
			//Rect resizeRect = default(Rect);
			
			if( m_IsHorizontal != false)
			{
				for( int i0 = 0; i0 < m_Childs.Count; ++i0)
				{
					if( i0 > 0)
					{
						Resize( i0 - 1, i0, new Rect( rect.x + offset - 2, rect.y, 4, rect.height));
					}
					int w = (int)(rect.width * m_Childs[ i0].Weight);
					m_Childs[ i0].DrawGUI( new Rect( rect.x + offset, rect.y, w, rect.height), repaintAction);
					
					if( i0 >= 0 && i0 < m_Childs.Count)
					{
						offset += w;
					}
				}
			}
			else
			{
				for( int i0 = 0; i0 < m_Childs.Count; ++i0)
				{
					if( i0 > 0)
					{
						Resize( i0 - 1, i0, new Rect( rect.x, rect.y + offset - 2, rect.width, 4));
					}
					int h = (int)(rect.height * m_Childs[ i0].Weight);
					m_Childs[ i0].DrawGUI( new Rect( rect.x, rect.y + offset, rect.width, h), repaintAction);
					
					if( i0 >= 0 && i0 < m_Childs.Count)
					{
						offset += h;
					}
				}
			}
			DoResize( repaintAction);
			m_Rect = rect;
		}
		public virtual SubWindow DragWindow( Vector2 position)
		{
			for( int i0 = 0; i0 < m_Childs.Count; ++i0)
			{
				var r = m_Childs[ i0].DragWindow( position);
				
				if( r != null)
				{
					return r;
				}
			}
			return null;
		}
		public void DropWindow( Vector2 position, int depth, SubWindow window, System.Action<SubWindow> preDropAction, System.Action postDropAction)
		{
			TriggerAnchorArea( position, depth, window, preDropAction, postDropAction);
		}
		public void DrawAnchorArea( Vector2 position, int depth, SubWindow window)
		{
			TriggerAnchorArea( position, depth, window, null, null);
		}
		public virtual bool ContainWindow( SubWindow window)
		{
			if( m_Childs.Count == 1)
			{
				return m_Childs[ 0].ContainWindow( window);
			}
			return false;
		}
		public virtual void Insert( SubWindowNode node, int index)
		{
			if( m_Childs.Count == 0)
			{
				node.Weight = 1;
				node.Depth = m_Depth + 1;
				node.IsHorizontal = !m_IsHorizontal;
				m_Childs.Add( node);
				return;
			}
			float w = 1.0f / (m_Childs.Count + 1);
			float ew = w / m_Childs.Count;
			node.Weight = w;
			node.Depth = m_Depth + 1;
			node.IsHorizontal = !m_IsHorizontal;
			
			for( int i0 = 0; i0 < m_Childs.Count; ++i0)
			{
				m_Childs[ i0].Weight -= ew;
			}
			if( index < 0 || index >= m_Childs.Count)
			{
				m_Childs.Add( node);
			}
			else
			{
				m_Childs.Insert( index, node);
			}
		}
		public virtual bool RemoveWindow( SubWindow window)
		{
			for( int i0 = 0; i0 < m_Childs.Count; ++i0)
			{
				bool result = m_Childs[ i0].RemoveWindow( window);
				
				if( result != false)
				{
					return true;
				}
			}
			return false;
		}
		public virtual void ClearEmptyNode()
		{
			for( int i0 = 0; i0 < m_Childs.Count; ++i0)
			{
				m_Childs[ i0].ClearEmptyNode();
				
				if( m_Childs[ i0].Count == 0)
				{
					float w = 1.0f / m_Childs.Count;
					m_Childs.RemoveAt( i0);
					float ew = w / m_Childs.Count;
					
					for( int i1 = 0; i1 < m_Childs.Count; ++i1)
					{
						m_Childs[ i1].Weight += ew;
					}
				}
			}
		}
		public virtual void AddWindow( SubWindow window, int index)
		{
			Insert( new SubWindowLeaf( window, !m_IsHorizontal, Depth + 1), index);
		}
		public virtual void Recalculate( int depth, bool isHorizontal)
		{
			m_Depth = depth;
			m_IsHorizontal = isHorizontal;
			
			if( m_Childs.Count == 0)
			{
				return;
			}
			float weightSum = 0;
			
			for( int i0 = 0; i0 < m_Childs.Count; ++i0)
			{
				if( m_Childs[ i0].Weight < kMinWeight)
				{
					m_Childs[ i0].Weight = kMinWeight;
				}
				else if( m_Childs[ i0].Weight > kMaxWeight)
				{
					m_Childs[ i0].Weight = kMaxWeight;
				}
				weightSum += m_Childs[ i0].Weight;
				m_Childs[ i0].Recalculate( depth + 1, !isHorizontal);
			}
			if( weightSum > 1.0f + Mathf.Epsilon || weightSum < 1.0f - Mathf.Epsilon)
			{
				float m = (1 - weightSum) / m_Childs.Count;
				
				for( int i0 = 0; i0 < m_Childs.Count; i0++)
				{
					m_Childs[ i0].Weight += m;
				}
			}
		}
		public virtual void WriteToLayoutCfg( XmlElement element, XmlDocument document, int index)
		{
			XmlElement currentElement = document.CreateElement( "SubWindowNode");
			currentElement.SetAttribute( "Weight", Weight.ToString());
			currentElement.SetAttribute( "Depth", Depth.ToString());
			currentElement.SetAttribute( "Horizontal", IsHorizontal.ToString());
			currentElement.SetAttribute( "Index", index.ToString());
			element.AppendChild( currentElement);
			
			if( m_Childs != null)
			{
				for( int i0 = 0; i0 < m_Childs.Count; ++i0)
				{
					m_Childs[ i0].WriteToLayoutCfg( currentElement, document, i0);
				}
			}
		}
		public virtual void CreateFromLayoutCfg( XmlElement node, List<SubWindow> windowList, System.Action<SubWindow> onWindowClose)
		{
			string weightStr = node.GetAttribute( "Weight");
			string depthStr = node.GetAttribute( "Depth");
			string horizontalStr = node.GetAttribute( "Horizontal");
			m_Weight = float.Parse( weightStr);
			m_Depth = int.Parse( depthStr);
			m_IsHorizontal = bool.Parse( horizontalStr);
			XmlNodeList nodes = node.ChildNodes;
			
			if( nodes.Count == 0)
			{
				return;
			}
			var sortnode = new XmlElement[ nodes.Count];
			
			foreach( var n in nodes)
			{
				var nd = n as XmlElement;
				string indexstr = nd.GetAttribute( "Index");
				int index = int.Parse( indexstr);
				sortnode[ index] = nd;
			}
			foreach( var n in sortnode)
			{
				if( n.Name == "SubWindowNode")
				{
					var swnode = new SubWindowNode( true, 0);
					swnode.CreateFromLayoutCfg( n, windowList, onWindowClose);
					m_Childs.Add( swnode);
				}
				else if( n.Name == "SubWindowLeaf")
				{
					var swleaf = new SubWindowLeaf( null, true, 0);
					swleaf.CreateFromLayoutCfg( n, windowList, onWindowClose);
					m_Childs.Add( swleaf);
				}
			}
		}
		protected virtual bool TriggerAnchorArea( Vector2 position, int depth, SubWindow window, System.Action<SubWindow> preDropAction, System.Action postDropAction)
		{
			if( depth >= kMaxNodeDepth)
			{
				return false;
			}
			Rect r = default;
			float offset = 0;
			
			for( int i0 = 0; i0 < m_Childs.Count; i0++)
			{
				if( m_Childs[ i0].TriggerAnchorArea( position, depth + 1, window, preDropAction, postDropAction))
				{
					return true;
				}
				if( IsHorizontal != false)
				{
					r = new Rect( m_Rect.x + offset, m_Rect.y, m_Rect.width * m_Childs[ i0].Weight * 0.2f, m_Rect.height);
					
					if( r.Contains( position) != false)
					{
						if( preDropAction == null)
						{
							m_TweenParam = GUIEx.ScaleTweenBox( r, m_TweenParam, string.Empty, GUIStyleCache.GetStyle( "SelectionRect"));
						}
						else
						{
							DropWindow( window, preDropAction, postDropAction, true, i0);
						}
						return true;
					}
					r = new Rect( 
						m_Rect.x + offset + m_Rect.width * m_Childs[ i0].Weight * 0.8f, 
						m_Rect.y, m_Rect.width * m_Childs[ i0].Weight * 0.2f, m_Rect.height);
					
					if( r.Contains( position) != false)
					{
						if( preDropAction == null)
						{
							m_TweenParam = GUIEx.ScaleTweenBox( r, m_TweenParam, string.Empty, GUIStyleCache.GetStyle( "SelectionRect"));
						}
						else
						{
							DropWindow(window, preDropAction, postDropAction, true, i0 + 1);
						}
						return true;
					}
					r = new Rect( m_Rect.x + offset, m_Rect.y + m_Rect.height * 0.8f, m_Rect.width * m_Childs[ i0].Weight, m_Rect.height * 0.2f);
					
					if( r.Contains( position) != false && m_Childs.Count > 1)
					{
						if( preDropAction == null)
						{
							m_TweenParam = GUIEx.ScaleTweenBox( r, m_TweenParam, string.Empty, GUIStyleCache.GetStyle( "SelectionRect"));
						}
						else
						{
							DropWindow( window, preDropAction, postDropAction, false, i0);
						}
						return true;
					}
					offset += m_Childs[ i0].Weight * m_Rect.width;
				}
				else
				{
					r = new Rect( m_Rect.x, m_Rect.y + offset, m_Rect.width, m_Rect.height * m_Childs[ i0].Weight * 0.2f);
					
					if( r.Contains( position) != false)
					{
						if (preDropAction == null)
						{
							m_TweenParam = GUIEx.ScaleTweenBox( r, m_TweenParam, string.Empty, GUIStyleCache.GetStyle( "SelectionRect"));
						}
						else
						{
							DropWindow( window, preDropAction, postDropAction, true, i0);
						}
						return true;
					}
					r = new Rect( 
						m_Rect.x, m_Rect.y + offset + m_Rect.height * m_Childs[ i0].Weight * 0.8f, 
						m_Rect.width, m_Rect.height * m_Childs[ i0].Weight * 0.2f);
					
					if( r.Contains( position) != false)
					{
						if( preDropAction == null)
						{
							m_TweenParam = GUIEx.ScaleTweenBox( r, m_TweenParam, string.Empty, GUIStyleCache.GetStyle( "SelectionRect"));
						}
						else
						{
							DropWindow( window, preDropAction, postDropAction, true, i0 + 1);
						}
						return true;
					}
					r = new Rect( m_Rect.x + m_Rect.width * 0.8f, m_Rect.y + offset, m_Rect.width * 0.2f, m_Rect.height * m_Childs[ i0].Weight);
					
					if( r.Contains( position) != false && m_Childs.Count > 1)
					{
						if( preDropAction == null)
						{
							m_TweenParam = GUIEx.ScaleTweenBox( r, m_TweenParam, string.Empty, GUIStyleCache.GetStyle( "SelectionRect"));
						}
						else
						{
							DropWindow( window, preDropAction, postDropAction, false, i0);
						}
						return true;
					}
					offset += m_Childs[ i0].Weight * m_Rect.height;
				}
			}
			return false;
		}
		void DropWindow( SubWindow window, System.Action<SubWindow> preDropAction, System.Action postDropAction, bool betweenChilds, int dropIndex)
		{
			if( window == null)
			{
				return;
			}
			if( preDropAction == null)
			{
				return;
			}
			if( betweenChilds != false)
			{
				if( preDropAction != null)
				{
					preDropAction(window);
					AddWindow( window, dropIndex);
					postDropAction();
				}
			}
			else
			{
				if( preDropAction != null && dropIndex >= 0 && dropIndex < m_Childs.Count)
				{
					SubWindowNode child = m_Childs[ dropIndex];
					
					if( child.ContainWindow( window))
					{
						return;
					}
					preDropAction( window);
					m_Childs.RemoveAt( dropIndex);
					
                    var node = new SubWindowNode( !m_IsHorizontal, Depth + 1)
                    {
                        Weight = child.Weight
                    };
                    child.IsHorizontal = m_IsHorizontal;
					child.Depth = node.Depth + 1;
					node.Insert( child, 0);
					Insert( node, dropIndex);
					node.AddWindow( window, -1);
					postDropAction();
				}
			}
		}
		void Resize( int first, int second, Rect rect)
		{
			if( m_IsHorizontal != false)
			{
				EditorGUIUtility.AddCursorRect( rect, MouseCursor.ResizeHorizontal);
			}
			else
			{
				EditorGUIUtility.AddCursorRect( rect, MouseCursor.ResizeVertical);
			}
			if( Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				if( rect.Contains( Event.current.mousePosition) != false)
				{
					Event.current.Use();
					m_IsDragging = true;
					m_CurrentResizeFirstId = first;
					m_CurrentResizeSecondId = second;
				}
			}
		}
		void DoResize( System.Action repaintAct)
		{
			if( m_IsDragging != false)
			{
				if( Event.current.type == EventType.MouseUp && Event.current.button == 0)
				{
					m_IsDragging = false;
					Event.current.Use();
				}
				if( Event.current.type == EventType.MouseDrag && Event.current.button == 0)
				{
					float delta = 0;
					
					if( m_IsHorizontal != false)
					{
						delta = Event.current.delta.x / m_Rect.width;
					}
					else
					{
						delta = Event.current.delta.y / m_Rect.height;
					}
					float addW = m_Childs[ m_CurrentResizeFirstId].Weight + delta;
					float musW = m_Childs[ m_CurrentResizeSecondId].Weight - delta;
					
					if( addW >= kMinWeight && addW <= kMaxWeight && musW >= kMinWeight && musW <= kMaxWeight)
					{
						m_Childs[ m_CurrentResizeFirstId].Weight = addW;
						m_Childs[ m_CurrentResizeSecondId].Weight = musW;
					}
					if (repaintAct != null)
					{
						repaintAct();
					}
				}
			}
		}
		public virtual int Count
		{
			get{ return m_Childs.Count; }
		}
		public bool IsHorizontal
		{
			get{ return m_IsHorizontal; }
			protected set{ m_IsHorizontal = value; }
		}
		public float Weight
		{
			get{ return m_Weight;}
			protected set{ m_Weight = value;}
		}
		public int Depth
		{
			get{ return m_Depth; }
			protected set{ m_Depth = value; }
		}
		public Rect Rect
		{
			get{ return m_Rect; }
			protected set{ m_Rect = value; }
		}
		public GUITweenParam TweenParam
		{
			get{ return m_TweenParam; }
			protected set{ m_TweenParam = value; }
		}
		protected const int kMaxNodeDepth = 4;
		const float kMaxWeight = 0.9f;
		const float kMinWeight = 0.1f;
		
        readonly List<SubWindowNode> m_Childs = new();
		float m_Weight = 1;
		int m_Depth;
		bool m_IsHorizontal;
		Rect m_Rect;
		GUITweenParam m_TweenParam;
		//Rect m_OriginRect;
		//float m_RectTweenTime;
		bool m_IsDragging;
		int m_CurrentResizeFirstId;
		int m_CurrentResizeSecondId;
		
	}
}