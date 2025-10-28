
using System.Linq;
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	internal sealed class ElementBuilder
	{
		internal ElementBuilder()
		{
			m_RootElements = new List<Element>();
			m_Registered = new SortedDictionary<string, Element>();
		}
		internal List<Element> ToList()
		{
			return Sort( m_RootElements.ToList());
		}
		List<Element> Sort( List<Element> elements)
		{
			elements.Sort( (a, b) =>
			{
				return a.AssetPath.CompareTo( b.AssetPath);
			});
			foreach( var element in elements)
			{
				Sort( element.ChildElements);
			}
			return elements;
		}
		internal bool Append( ElementSource source)
		{
			if( source != null && string.IsNullOrEmpty( source.AssetPath) == false)
			{
				string[] elementNames = source.AssetPath.Split( '/');
				string elementName;
				string path = string.Empty;
				Element element = null;
				Element parent;
				
				for( int i0 = 0; i0 < elementNames.Length; ++i0)
				{
					elementName = elementNames[ i0];
					
					if( string.IsNullOrEmpty( path) == false)
					{
						path += "/";
					}
					path += elementName;
					parent = element;
					
					if( m_Registered.TryGetValue( path, out element) == false)
					{
						try
						{
							if( i0 == elementNames.Length - 1)
							{
								element = Element.Create( source);
							}
							else
							{
								element = Element.Create( path);
							}
						}
						catch( System.Exception e)
						{
							element = null;
							UnityEngine.Debug.LogError( e);
						}
						if( element == null)
						{
							return false;
						}
						else
						{
							if( parent != null)
							{
								parent.Add( element);
							}
							if( i0 == 0)
							{
								m_RootElements.Add( element);
							}
							m_Registered.Add( path, element);
						}
					}
				}
				return true;
			}
			return false;
		}
		internal bool Append( string assetPath, string bundleName, int reference=-1)
		{
			if( string.IsNullOrEmpty( assetPath) == false)
			{
				string[] elementNames = assetPath.Split( '/');
				string elementName;
				string path = string.Empty;
				Element element = null;
				Element parent;
				
				for( int i0 = 0; i0 < elementNames.Length; ++i0)
				{
					elementName = elementNames[ i0];
					
					if( string.IsNullOrEmpty( path) == false)
					{
						path += "/";
					}
					path += elementName;
					parent = element;
					
					if( m_Registered.TryGetValue( path, out element) == false)
					{
						element = Element.Create( path, bundleName, reference);
						if( element == null)
						{
							return false;
						}
						else
						{
							if( parent != null)
							{
								parent.Add( element);
							}
							if( i0 == 0)
							{
								m_RootElements.Add( element);
							}
							m_Registered.Add( path, element);
						}
					}
				}
				return true;
			}
			return false;
		}
		readonly List<Element> m_RootElements;
		readonly SortedDictionary<string, Element> m_Registered;
	}
}
