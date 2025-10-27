
namespace Knit.EditorWindow
{
	internal class ElementSource
	{
		internal ElementSource( string path, int reference, int missing)
		{
			m_Path = path;
			m_Reference = reference;
			m_Missing = missing;
		}
		internal string Path
		{
			get{ return m_Path; }
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
		readonly string m_Path;
		int m_Reference;
		int m_Missing;
	}
	internal sealed class ElementComponentSource : ElementSource
	{
		internal ElementComponentSource( string name, System.Type type, 
			string findPath, long localId, string path, int reference, int missing) : base( path, reference, missing)
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
