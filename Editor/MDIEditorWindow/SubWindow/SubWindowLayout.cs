
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	internal class SubWindowLayout
	{
		public List<string> Layouts
		{
			get{ return m_Layouts; }
		}
		public SubWindowLayout(string windowName, string handleName)
		{
			m_WindowName = windowName;
			m_HandleName = handleName;
			//m_LayoutPrefsKey = MDIEditorWindow.GetIndentifier() + "_" + "SubWindowTree_" + m_WindowName;
			
			// if( string.IsNullOrEmpty( m_HandleName) == false)
			// {
			//    m_LayoutPrefsKey = m_LayoutPrefsKey + "_" + m_HandleName;
			// }
			LoadLayoutCfgs();
		}
		public void SaveLayout( string layoutName, string treeId, SubWindowNode node)
		{
			if( string.IsNullOrEmpty( layoutName) != false)
			{
				return;
			}
			if( string.IsNullOrEmpty( treeId) != false)
			{
				return;
			}
			var doc = new XmlDocument();
			XmlElement root = doc.CreateElement( "SubWindowTree");
			root.SetAttribute( "Layout", layoutName);
			root.SetAttribute( "TreeID", treeId.GetHashCode().ToString());
			doc.AppendChild( root);
			
			if( node != null)
			{
				node.WriteToLayoutCfg( root, doc, 0);
			}
			bool isCurrent = layoutName == "Current";
			string path = GetLayoutCfgsPath( isCurrent);
			
			if( Directory.Exists( path) == false)
			{
				Directory.CreateDirectory( path);
			}
			doc.Save( path + "/" + layoutName + ".xml");
			LoadLayoutCfgs();
		}
		public void DeleteLayout( string layoutName)
		{
			if( RemoveLayoutCfg( layoutName) != false)
			{
				LoadLayoutCfgs();
			}
		}
		public XmlElement UseLayout( string layoutName, string treeId)
		{
			if( string.IsNullOrEmpty( layoutName) != false)
			{
				return null;
			}
			if( string.IsNullOrEmpty( treeId) != false)
			{
				return null;
			}
			bool isCurrent = layoutName == "Current";
			string path = Path.Combine( GetLayoutCfgsPath( isCurrent), layoutName + ".xml");
			
			if( File.Exists( path) != false)
			{
				var doc = new XmlDocument();
				doc.Load( path);
				XmlNodeList nodes = doc.ChildNodes;
				
				if( nodes.Count == 1)
				{
					var tree = nodes[ 0] as XmlElement;
					
					if( tree.HasAttribute( "TreeID") == false)
					{
						if( EditorUtility.DisplayDialog( "Error", "The layout is malformed. Did you delete it?", "Yes", "No") != false)
						{
							DeleteLayout( layoutName);
						}
						return null;
					}
					string layoutTreeId = tree.GetAttribute( "TreeID");
					int layoutTreeIDValue = int.Parse( layoutTreeId);
					int treeIdValue = treeId.GetHashCode();
					
					if( layoutTreeIDValue != treeIdValue)
					{
					//	if( EditorUtility.DisplayDialog( "Error", "The layout is different from the current form structure and cannot be used anymore. Is it deleted?", "Yes", "No") != false)
						{
							DeleteLayout( layoutName);
						}
						return null;
					}
					XmlNodeList treenode = tree.ChildNodes;
					
					if( treenode.Count == 1)
					{
						return treenode[ 0] as XmlElement;
					}
				}
			}
			return null;
		}
		public void RevertLayout()
		{
			if( m_Layouts != null && m_Layouts.Count > 0)
			{
				if( EditorUtility.DisplayDialog( "Remove all layouts", "Do you want to delete all layout files?", "Yes", "No") != false)
				{
					foreach( var layout in m_Layouts)
					{
						RemoveLayoutCfg( layout);
					}
					LoadLayoutCfgs();
					//SaveCurrentLayout(null);
				}
			}
		}
		bool RemoveLayoutCfg( string layoutName)
		{
			if( m_Layouts != null && m_Layouts.Contains( layoutName) != false)
			{
				string path = GetLayoutCfgsPath( false) + "/" + layoutName + ".xml";
				
				if( File.Exists( path) != false)
				{
					File.Delete( path);
					return true;
				}
			}
			return false;
		}
		void LoadLayoutCfgs()
		{
			string path = GetLayoutCfgsPath( false);
			
			if( Directory.Exists( path) != false)
			{
				string[] files = Directory.GetFiles( path);
				m_Layouts.Clear();
				
				for( int i0 = 0; i0 < files.Length; ++i0)
				{
					string filename = Path.GetFileName( files[ i0]);
					
					if( string.IsNullOrEmpty( filename))
					{
						continue;
					}
					string ext = Path.GetExtension( files[ i0]);
					
					if( ext != null)
					{
						filename = filename.Replace( ext, string.Empty);
					}
					m_Layouts.Add( filename);
				}
			}
		}
		string GetLayoutCfgsPath( bool isCurrent)
		{
			string rootPath = "SubWindowTree/" + m_WindowName;
			
			if( string.IsNullOrEmpty( m_HandleName) == false)
			{
				rootPath = rootPath + "/" + m_HandleName;
			}
			if( isCurrent != false)
			{
				return Path.Combine( Application.temporaryCachePath, rootPath);
			}
			else
			{
				return rootPath;
			}
		}
        readonly string m_WindowName;
        readonly string m_HandleName;
        //string m_LayoutPrefsKey;
        readonly List<string> m_Layouts = new();
	}
}