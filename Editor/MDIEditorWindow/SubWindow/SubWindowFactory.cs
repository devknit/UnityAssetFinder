using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Knit.EditorWindow
{
	internal class SubWindowFactory
	{
		public static SubWindow CreateSubWindow( 
			SubWindowStyle style, string title, string iconPath, bool defaultOpen,
			MethodInfo method, object target, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpbox)
		{
			if( CheckSubWindowParameters(method, toolbar, helpbox) == false)
			{
				return null;
			}
			if( subWindowClass == null)
			{
				GetSubWinodwStyleClasses();
			}
			else if( subWindowClass.ContainsKey( style) == false)
			{
				GetSubWinodwStyleClasses();
			}
			if( subWindowClass == null || subWindowClass.ContainsKey( style) == false)
			{
				return null;
			}
			return Activator.CreateInstance( subWindowClass[ style], title, iconPath, defaultOpen, method, target, toolbar, helpbox) as SubWindow;
		}
		public static SubWindow CreateSubWindow( object container, bool defaultOpen, SubWindowStyle style, Type customDrawerType)
		{
			if( customDrawerType == null)
			{
				return null;
			}
			if( subWindowClass == null)
			{
				GetSubWinodwStyleClasses();
			}
			else if( subWindowClass.ContainsKey( style) == false)
			{
				GetSubWinodwStyleClasses();
			}
			if( subWindowClass == null || subWindowClass.ContainsKey( style) == false)
			{
				return null;
			}
			Type type = subWindowClass[ style];
			
            if( Activator.CreateInstance( customDrawerType) is not SubWindowCustomDrawer drawer)
            {
                return null;
            }
            drawer.SetContainer( container);
			
			return Activator.CreateInstance( type, defaultOpen, drawer) as SubWindow;
		}
		static bool CheckSubWindowParameters( MethodInfo method, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpbox)
		{
			ParameterInfo[] infos = method.GetParameters();
			
			if( toolbar != SubWindowToolbarType.None && helpbox != SubWindowHelpBoxType.None)
			{
				if( infos.Length != 3)
				{
					return false;
				}
			}
			else if( (toolbar != SubWindowToolbarType.None && helpbox == SubWindowHelpBoxType.None)
			||		(toolbar == SubWindowToolbarType.None && helpbox != SubWindowHelpBoxType.None))
			{
				if( infos.Length != 2)
				{
					return false;
				}
			}
			else if( toolbar == SubWindowToolbarType.None && helpbox == SubWindowHelpBoxType.None)
			{
				if (infos.Length != 1)
				{
					return false;
				}
			}
			for( int i0 = 0; i0 < infos.Length; ++i0)
			{
				if( infos[ i0].ParameterType != typeof( Rect))
				{
					return false;
				}
			}
			return true;
		}
		static void GetSubWinodwStyleClasses()
		{
			if( subWindowClass == null)
			{
				subWindowClass = new Dictionary<SubWindowStyle, Type>();
			}
			subWindowClass.Clear();
			Type sbwindow = typeof( SubWindow);
			Assembly assembly = sbwindow.Assembly;
			Type[] tp = assembly.GetTypes();
			
			for( int i0 = 0; i0 < tp.Length; ++i0)
			{
				if( tp[ i0].IsClass == false)
				{
					continue;
				}
				if( tp[ i0].IsAbstract != false)
				{
					continue;
				}
				if( tp[ i0].IsSubclassOf( sbwindow) || tp[ i0] == sbwindow)
				{
					object[] atts = tp[ i0].GetCustomAttributes( typeof( SubWindowStyleAttribute), false);
					
					for( int i1 = 0; i1 < atts.Length; ++i1)
					{
						var att = atts[ i1] as SubWindowStyleAttribute;
						subWindowClass[ att.subWindowStyle] = tp[ i0];
					}
				}
			}
		}
		static Dictionary<SubWindowStyle, Type> subWindowClass;
	}
}