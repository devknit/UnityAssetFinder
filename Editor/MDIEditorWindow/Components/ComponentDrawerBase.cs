
namespace Knit.EditorWindow
{
	internal abstract class ComponentDrawerBase
	{
		public void Init()
		{
			if( m_Initialized != false)
			{
				return;
			}
			if( OnInit() == false)
			{
				return;
			}
			m_Initialized = true;
		}
		public void Enable()
		{
			if( m_Enabled == false)
			{
				OnEnable();
			}
			m_Enabled = true;
		}
		public void Disable()
		{
			if( m_Enabled != false)
			{
				OnDisable();
			}
			m_Enabled = false;
		}
		public void Destroy()
		{
			Disable();
			OnDestroy();
			m_Initialized = false;
		}
		protected abstract bool OnInit();
		protected abstract void OnEnable();
		protected abstract void OnDisable();
		protected abstract void OnDestroy();
		
		public bool IsInitialized
		{
			get{ return m_Initialized; }
		}
		public bool IsEnabled
		{
			get{ return m_Enabled; }
		}
		bool m_Initialized;
		bool m_Enabled;
	}
}