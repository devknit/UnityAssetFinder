
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	public delegate bool ConditionDelegate( object arg);
}
namespace Knit.EditorWindow
{
	internal class ToolBarTreeNode
	{
		public ToolBarTreeNode( string text, int priority)
		{
			m_NodeList = new List<ToolBarTreeNode>();
			m_Text = text;
			m_Priority = priority;
		}
		public void InsertNode( string nodetext, MethodInfo method, object target, ConditionDelegate condition, object argobj, int priority)
		{
			if( string.IsNullOrEmpty( nodetext) != false)
			{
				return;
			}
			while( nodetext.Length > 0 && nodetext[ 0] == '/')
			{
				nodetext = nodetext[ 1..];
			}
			if( string.IsNullOrEmpty( nodetext) != false)
			{
				return;
			}
			int first = nodetext.IndexOf( '/');
			string lasttext = null;
			bool hasChild = false;
			
			if( first > 0)
			{
				lasttext = nodetext[ (first + 1)..];
				nodetext = nodetext[ ..first];
				
				if( string.IsNullOrEmpty( lasttext) == false)
				{
					hasChild = true;
				}
			}
			AddNode( nodetext, lasttext, priority, method, target, condition, argobj, hasChild);
		}
		void AddNode( 
			string nodeText, string lasttext, int priority, MethodInfo method, 
			object target, ConditionDelegate condition, object argobj, bool hasChild)
		{
			ToolBarTreeNode node = m_NodeList.Find( n => n.m_Text == nodeText);
			
			if( hasChild != false)
			{
				if( node == null)
				{
					node = new ToolBarTreeNode( nodeText, priority);
					node.InsertNode( lasttext, method, target, condition, argobj, priority);
					m_NodeList.Add( node);
				}
				else
				{
					node.InsertNode( lasttext, method, target, condition, argobj, priority);
				}
			}
			else
			{
				if( node == null)
				{
					node = new ToolBarTreeNode( nodeText, priority);
					node.CombineMethod( method, target, condition, argobj);
					m_NodeList.Add( node);
				}
			}
		}
		public void Sort()
		{
			if( m_NodeList != null && m_NodeList.Count > 0)
			{
				m_NodeList.Sort( (a, b) => 
				{
					return a.m_Priority - b.m_Priority;
				});
				for( int i0 = 0; i0 < m_NodeList.Count; ++i0)
				{
					m_NodeList[ i0].Sort();
				}
			}
		}
		public void DrawToolBar()
		{
			for( int i0 = 0; i0 < m_NodeList.Count; ++i0)
			{
				Rect rect = EditorGUILayout.GetControlRect( GUILayout.Width( 70), GUILayout.Height( 17));
				
				if( GUIEx.ToolbarButton( rect, m_NodeList[ i0].m_Text) != false)
				{
					ClickDropDown( rect, m_NodeList[ i0]);
				}
			}
		}
		void CombineMethod( MethodInfo method, object target, ConditionDelegate condition, object argobj)
		{
			m_ArgObj = argobj;
			
			if( m_ArgObj != null)
			{
				m_Condition = condition;
			}
			else
			{
				m_Condition = null;
			}
			if( m_Action == null)
			{
				if( m_ArgObj == null)
				{
					m_Action = Delegate.CreateDelegate( typeof( Action), target, method);
				}
				else
				{
					m_Action = Delegate.CreateDelegate( typeof( Action<object>), target, method);
				}
			}
		}
		void ClickDropDown( Rect rect, ToolBarTreeNode node)
		{
			if( node.m_NodeList == null || node.m_NodeList.Count == 0)
			{
				node.Invoke();
				return;
			}
			var menu = new GenericMenu();
			
			for( int i0 = 0; i0 < node.m_NodeList.Count; ++i0)
			{
				node.m_NodeList[ i0].ShowDropDownMenu( menu, "", false);
			}
			menu.DropDown( rect);
		}
		void ShowDropDownMenu( GenericMenu menu, string text, bool separator)
		{
			string ntext = text + "/" + m_Text;
			
			if( m_NodeList == null || m_NodeList.Count == 0)
			{
				ntext = ntext.TrimStart('/');
				
				if( separator != false)
				{
					menu.AddSeparator((text + "/").TrimStart('/'));
				}
				if( m_Condition != null && m_ArgObj != null)
				{
					menu.AddItem( new GUIContent( ntext), m_Condition( m_ArgObj), Invoke);
				}
				else
				{
					menu.AddItem( new GUIContent( ntext), false, Invoke);
				}
				return;
			}
			int bound = 0;
			
			if( m_NodeList.Count > 0)
			{
				bound = m_NodeList[ 0].m_Priority / 1000;
			}
			for( int i0 = 0; i0 < m_NodeList.Count; ++i0)
			{
				int cBound = m_NodeList[ i0].m_Priority / 1000;
				
				if( cBound > bound)
				{
					bound = cBound;
					m_NodeList[ i0].ShowDropDownMenu( menu, ntext, true);
				}
				else
					m_NodeList[ i0].ShowDropDownMenu( menu, ntext, false);
			}
		}
		void Invoke()
		{
			if( m_Action != null)
			{
				if( m_ArgObj != null)
				{
					m_Action.DynamicInvoke( m_ArgObj);
				}
				else
				{
					m_Action.DynamicInvoke();
				}
			}
		}
		public int Count
		{
			get { return m_NodeList.Count; }
		}
        readonly string m_Text;
		Delegate m_Action;
		ConditionDelegate m_Condition;
		object m_ArgObj;
        readonly List<ToolBarTreeNode> m_NodeList;
        readonly int m_Priority;
	}
}
