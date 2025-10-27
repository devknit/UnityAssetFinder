
using UnityEngine;
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	public class GUIStyleCache
	{
		public static GUIStyle GetStyle( string style)
		{
			if( s_Instance == null)
			{
				s_Instance = new GUIStyleCache();
			}
			if( s_Instance.m_StyleCache == null)
			{
				s_Instance.m_StyleCache = new Dictionary<string, GUIStyle>();
			}
			GUIStyle st = null;
			
			if( s_Instance.m_StyleCache.ContainsKey( style) != false)
			{
				st = s_Instance.m_StyleCache[ style];
			}
			if( st == null)
			{
				st = style;
				s_Instance.m_StyleCache[ style] = st;
			}
			return st;
		}
		static GUIStyleCache s_Instance;
		Dictionary<string, GUIStyle> m_StyleCache;
	}
}