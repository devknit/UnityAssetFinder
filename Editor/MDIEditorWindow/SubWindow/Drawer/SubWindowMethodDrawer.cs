
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Knit.EditorWindow
{
	internal class SubWindowMethodDrawer : SubWindowDrawerBase
	{
		internal static string GetMethodID( MethodInfo method, object target)
		{
			string result;
			
			if( target == null && method == null)
			{
				result = "__METHOD__UnKnownClass.UnKnownMethod";
			}
			else if( target == null)
			{
				result = "__METHOD__UnKnownClass." + method.Name;
			}
			else if( method == null)
			{
				result = "__METHOD__" + target.GetType().FullName + ".UnKnownMethod";
			}
			else
			{
				result = "__METHOD__" + target.GetType().FullName + "." + method.Name;
			}
			return result;
		}
		public SubWindowMethodDrawer( string title, string icon, MethodInfo method, 
			object target, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpbox)
		{
			m_Title = CreateTitle( title, icon);
			m_Method = method;
			m_ToolBar = toolbar;
			m_HelpBox = SubWindowHelpBox.CreateHelpBox( helpbox);
			m_Target = target;
			m_Id = GetMethodID( method, target);
			
			if( m_Method != null)
			{
				ParameterInfo[] p = m_Method.GetParameters();
				m_Params = new object[ p.Length];
			}
		}
		public override string GetID( bool dynamic)
		{
			return m_Id;
		}
		protected override bool OnInit()
		{
			return true;
		}
		public override void DrawWindow( Rect mainRect, Rect toolbarRect, Rect helpboxRect)
		{
			if( m_Method != null)
			{
				if( m_Params.Length > 0)
				{
					m_Params[ 0] = mainRect;
				}
				if( m_Params.Length > 1)
				{
					if( m_ToolBar == SubWindowToolbarType.None)
					{
						m_Params[ 1] = helpboxRect;
					}
					else
					{
						m_Params[ 1] = toolbarRect;
					}
				}
				if( m_Params.Length > 2)
				{
					m_Params[ 2] = helpboxRect;
				}
				m_Method.Invoke( m_Target, m_Params);
			}
		}
		GUIContent CreateTitle( string title, string icon)
		{
			if( string.IsNullOrEmpty( icon) != false)
			{
				return new GUIContent( title);
			}
			Texture2D tex = EditorGUIUtility.FindTexture( icon);
			
			if( tex == null)
			{
				return new GUIContent( title);
			}
			return new GUIContent( title, tex);
		}
		public override GUIContent Title
		{
			get{ return m_Title; }
		}
		protected override SubWindowHelpBox helpBox
		{
			get{ return m_HelpBox; }
		}
		protected override SubWindowToolbarType toolBar
		{
			get{ return m_ToolBar; }
		}
        readonly GUIContent m_Title;
        readonly object[] m_Params;
        readonly MethodInfo m_Method;
        readonly object m_Target;
        readonly SubWindowToolbarType m_ToolBar;
        readonly SubWindowHelpBox m_HelpBox = null;
        readonly string m_Id;
	}
}