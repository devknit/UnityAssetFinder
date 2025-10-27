
using System;
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	internal sealed class SearchFilter
	{
		internal bool Valid
		{
			get;
			private set;
		}
		internal void Change( string value)
		{
			Valid = false;
			
			m_Names.Clear();
			m_Paths.Clear();
			m_Types.Clear();
			
			if( value != null)
			{
				value = value.Trim();
				
				if( value.Length > 0)
				{
					string[] argv = value.Split( ' ');
					string arg;
					
					for( int i0 = 0; i0 < argv.Length; ++i0)
					{
						arg = argv[ i0];
						
						if( AssetTypes.kFilters.TryGetValue( arg, out AssetType type) != false)
						{
							if( m_Types.ContainsKey( type) == false)
							{
								m_Types.Add( type, arg.Substring( 2, arg.Length - 2));
							}
						}
						else
						{
							if( arg.IndexOf( "p:~/", StringComparison.OrdinalIgnoreCase) >= 0)
							{
								arg = arg.Substring( 4, arg.Length - 4);
								
								if( arg.Length > 0 && m_Paths.ContainsKey( arg) == false)
								{
									m_Paths.Add( arg, 0);
								}
							}
							else if( arg.IndexOf( "p:", StringComparison.OrdinalIgnoreCase) >= 0)
							{
								arg = arg.Substring( 2, arg.Length - 2);
								
								if( arg.Length > 0 && m_Paths.ContainsKey( arg) == false)
								{
									m_Paths.Add( arg, 1);
								}
							}
							else if( m_Names.Contains( arg) == false)
							{
								m_Names.Add( arg);
							}
						}
					}
					if( m_Names.Count > 0 || m_Paths.Count > 0 || m_Types.Count > 0)
					{
						Valid = true;
					}
				}
			}
			m_OnChangeCallback?.Invoke( Valid);
		}
		internal bool Check( Element element)
		{
			if( Valid != false)
			{
				if( m_Types.Count > 0)
				{
					if( m_Types.ContainsKey( element.AssetType) == false)
					{
						return false;
					}
				}
				if( m_Paths.Count > 0)
				{
					bool check = false;
					
					foreach( var path in m_Paths)
					{
						if( path.Value == 0)
						{
							if( element.Path.IndexOf( path.Key, StringComparison.OrdinalIgnoreCase) == 0)
							{
								check = true;
								break;
							}
						}
						else if( path.Value == 1)
						{
							if( element.Path.IndexOf( path.Key, StringComparison.OrdinalIgnoreCase) >= 0)
							{
								check = true;
								break;
							}
						}
					}
					if( check == false)
					{
						return false;
					}
				}
				foreach( var name in m_Names)
				{
					if( element.name.IndexOf( name, StringComparison.OrdinalIgnoreCase) < 0)
					{
						return false;
					}
				}
			}
			return true;
		}
		internal bool ContainsTypeValue( string value)
		{
			return m_Types.ContainsValue( value);
		}
		internal void ToBuildStringNames( System.Text.StringBuilder builder)
		{
			foreach( var name in m_Names)
			{
				if( builder.Length > 0)
				{
					builder.Append( " ");
				}
				builder.Append( name);
			}
		}
		internal void ToBuildStringPaths( System.Text.StringBuilder builder)
		{
			foreach( var path in m_Paths)
			{
				if( builder.Length > 0)
				{
					builder.Append( " ");
				}
				if( path.Value == 0)
				{
					builder.Append( "p:~/");
				}
				else if( path.Value == 1)
				{
					builder.Append( "p:");
				}
				builder.Append( path.Key);
			}
		}
		internal void ToBuildStringTypes( System.Text.StringBuilder builder)
		{
			foreach( var type in m_Types.Values)
			{
				if( builder.Length > 0)
				{
					builder.Append( " ");
				}
				builder.Append( "t:");
				builder.Append( type);
			}
		}
		internal SearchFilter( Action<bool> onFilterChange=null)
		{
			m_Names = new List<string>();
			m_Paths = new Dictionary<string, int>();
			m_Types = new Dictionary<AssetType, string>();
			m_OnChangeCallback = onFilterChange;
		}
        readonly List<string> m_Names;
		readonly Dictionary<string, int> m_Paths;
		readonly Dictionary<AssetType, string> m_Types;
        readonly Action<bool> m_OnChangeCallback;
	}
}
