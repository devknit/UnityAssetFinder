
namespace Knit.EditorWindow
{
	internal class ElementSource
	{
		internal ElementSource( string assetPath, string bundleName, int reference, int missing)
		{
			m_AssetPath = assetPath;
			m_BundleName = bundleName;
			m_Reference = reference;
			m_Missing = missing;
		}
		internal string AssetPath
		{
			get{ return m_AssetPath; }
		}
		internal string BundleName
		{
			get{ return m_BundleName; }
		}
		internal int Reference
		{
			get{ return m_Reference; }
			set{ m_Reference = value; }
		}
		internal int Missing
		{
			get{ return m_Missing; }
			set{ m_Missing = value; }
		}
		readonly string m_AssetPath;
		readonly string m_BundleName;
		int m_Reference;
		int m_Missing;
	}
	internal sealed class ElementComponentSource : ElementSource
	{
		internal ElementComponentSource( string name, System.Type type, 
			string findPath, long localId, string path, int reference, int missing, string bundleName) : base( path, bundleName, reference, missing)
		{
			m_Name = name;
			m_Type = type;
			m_FindPath = findPath;
			m_LocalId = localId;
		}
		internal string Name
		{
			get{ return m_Name; }
		}
		internal string FindPath
		{
			get{ return m_FindPath; }
		}
		internal long LocalId
		{
			get{ return m_LocalId; }
		}
		internal System.Type Type
		{
			get{ return m_Type; }
		}
		readonly string m_Name;
		readonly string m_FindPath;
		readonly long m_LocalId;
		readonly System.Type m_Type;
	}
}
