
using System;
using System.Reflection;

namespace Knit.EditorWindow
{
	internal class ComponentsInitializer
	{
		public static void InitComponents( object container, Type[] types, object[] targets, params ComponentBase[] tools)
		{
			if( targets == null || types == null)
			{
				return;
			}
			if( targets.Length == 0 || types.Length == 0)
			{
				return;
			}
			if( targets.Length != types.Length)
			{
				return;
			}
			for( int i0 = 0; i0 < types.Length; ++i0)
			{
				if( types[ i0] == null)
				{
					continue;
				}
				if( targets[ i0] == null)
				{
					continue;
				}
				RegisterInstanceMethod( container, types[ i0], targets[ i0], tools);
			}
			{
				Type[] globalTypes = typeof( ComponentsInitializer).Assembly.GetTypes();
				
				for( int i0 = 0; i0 < globalTypes.Length; ++i0)
				{
					if( globalTypes[ i0].IsClass == false)
					{
						continue;
					}
					RegisterClass( container, globalTypes[ i0], tools);
				}
			}
			for( int i1 = 0; i1 < tools.Length; ++i1)
			{
				if( tools[ i1].IsInitialized == false)
				{
					tools[ i1].Init();
				}
			}
		}
		static void RegisterInstanceMethod( object container, Type type, object target, ComponentBase[] tools)
		{
			MethodInfo[] methods = type.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			
			for( int i0 = 0; i0 < methods.Length; ++i0)
			{
				for( int i1 = 0; i1 < tools.Length; ++i1)
				{
					if( tools[ i1].IsInitialized == false)
					{
						tools[ i1].RegisterMethod(container, methods[ i0], target);
					}
				}
			}
		}
		static void RegisterClass( object container, Type type, ComponentBase[] tools)
		{
			if( type.IsAbstract)
			{
				return;
			}
			for( int i0 = 0; i0 < tools.Length; ++i0)
			{
				if( tools[ i0].IsInitialized == false)
				{
					tools[ i0].RegisterClass( container, type);
				}
			}
		}
	}
}