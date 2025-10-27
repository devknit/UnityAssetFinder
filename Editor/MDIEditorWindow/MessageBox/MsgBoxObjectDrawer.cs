
using UnityEngine;

namespace Knit.EditorWindow
{
	internal class MsgBoxObjectDrawer : MsgBoxDrawer
	{
		public MsgBoxObjectDrawer( MsgBoxCustomDrawer drawer)
		{
			m_Drawer = drawer;
			
			if( m_Drawer == null)
			{
				return;
			}
			string id = GetID();
			
			if( EditorPrefsEx.HasKey( id) != false)
			{
				var obj = EditorPrefsEx.GetObject( id, drawer.GetType());
				
				if( obj != null)
				{
					drawer = obj as MsgBoxCustomDrawer;
					drawer.SetContainer( m_Drawer.Container);
					drawer.closeAction = m_Drawer.closeAction;
					m_Drawer = drawer;
				}
			}
		}
		protected override bool OnInit()
		{
			m_Drawer.Init();
			return true;
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			m_Drawer.OnDestroy();
			string id = GetID();
			
			if( EditorPrefsEx.HasKey( id) != false)
			{
				EditorPrefsEx.DeleteKey( id);
			}
		}
		protected override void OnDisable()
		{
			base.OnDisable();
			m_Drawer.OnDisable();
		}
		protected override void OnEnable()
		{
			base.OnEnable();
			m_Drawer.OnEnable();
		}
		protected override void OnDrawMsgBox( Rect rect, object obj)
		{
			m_Drawer.DrawMsgBox( rect, obj);
		}
		protected override void OnSerialize()
		{
			base.OnSerialize();
			
			if( m_Drawer == null)
			{
				return;
			}
			string id = GetID();
			EditorPrefsEx.SetObject( id, m_Drawer);
		}
		string GetID()
		{
			return GetType().FullName + "." + m_Drawer.GetType().FullName + "." + m_Drawer.Container.GetType().FullName;
		}
		protected override Rectangle Rectangle
		{
			get { return m_Drawer.Recttangle; }
		}
        readonly MsgBoxCustomDrawer m_Drawer;
	}
}