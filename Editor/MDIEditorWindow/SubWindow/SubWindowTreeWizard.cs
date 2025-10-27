
using UnityEngine;
using UnityEditor;

namespace Knit.EditorWindow
{
	internal class SubWindowTreeLayoutWizard : ScriptableWizard
	{
		public string layoutname = "";
		SubWindowLayout m_Layout;
		string m_TreeId;
		SubWindowNode m_RootNode;
		
		public static void CreateWizard( SubWindowLayout layout, string treeId, SubWindowNode rootNode)
		{
			SubWindowTreeLayoutWizard wizard =
				DisplayWizard<SubWindowTreeLayoutWizard>( "Save SubWindow Layout", "Save");
			wizard.maxSize = new Vector2( 300, 150);
			wizard.minSize = new Vector2( 300, 150);
			wizard.m_Layout = layout;
			wizard.m_TreeId = treeId;
			wizard.m_RootNode = rootNode;
		}
		void OnWizardCreate()
		{
			if( layoutname == "Default")
			{
				if( EditorUtility.DisplayDialog( "Warning", "Are you sure to override the default layout?", "Yes", "No") == false)
				{
					return;
				}
			}
			if( m_Layout != null)
			{
				m_Layout.SaveLayout( layoutname, m_TreeId, m_RootNode);
			}
			m_Layout = null;
			m_RootNode = null;
			m_TreeId = null;
		}
		void OnWizardUpdate()
		{
			helpString = "Please enter a layout name";
		}
	}
	internal class SubWindowTreeDeleteLayoutWizard : ScriptableWizard
	{
		SubWindowLayout m_Layout;
		//List<string> m_Layouts;
		
		public static void CreateWizard( SubWindowLayout layout)
		{
			SubWindowTreeDeleteLayoutWizard wizard =
				DisplayWizard<SubWindowTreeDeleteLayoutWizard>( "Delete SubWindow Layout");
			wizard.m_Layout = layout;
			//wizard.m_Layouts = layout;
		}
		void OnGUI()
		{
			if( m_Layout != null && m_Layout.Layouts != null)
			{
				for( int i0 = 0; i0 < m_Layout.Layouts.Count; ++i0)
				{
					if( GUILayout.Button( m_Layout.Layouts[ i0]) != false)
					{
						//if (m_Tree != null)
						//{
						m_Layout.DeleteLayout( m_Layout.Layouts[ i0]);
						break;
						//}
					}
				}
			}
		}
	}
}