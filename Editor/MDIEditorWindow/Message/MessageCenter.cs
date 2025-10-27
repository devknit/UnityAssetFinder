
using System;
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	internal delegate void MessageHandle();
	internal delegate void MessageHandle<T>( T arg);
	internal delegate void MessageHandle<T0, T1>( T0 arg0, T1 arg1);
	internal delegate void MessageHandle<T0, T1, T2>( T0 arg0, T1 arg1, T2 arg2);
	internal delegate void MessageHandle<T0, T1, T2, T3>( T0 arg0, T1 arg1, T2 arg2, T3 arg3);
	
	internal static class MessageCenter
	{
		public static void AddListener( this IMessageDispatcher dispatcher, int messageId, MessageHandle handle)
		{
			if( handle == null)
			{
				return;
			}
			Type containterType = dispatcher.GetContainerType();
			var messages = GetMessagesOfType( containterType);
			
			if( messages == null)
			{
				return;
			}
			if( CombineMethod( messageId, messages, handle) != false)
			{
				messages[ messageId] = (MessageHandle)Delegate.Combine( messages[ messageId], handle);
			}
		}
		public static void AddListener<T>( this IMessageDispatcher dispatcher, int messageId, MessageHandle<T> handle)
		{
			if( handle == null)
			{
				return;
			}
			Type containterType = dispatcher.GetContainerType();
			var messages = GetMessagesOfType( containterType);
			
			if( messages == null)
			{
				return;
			}
			if( CombineMethod( messageId, messages, handle) != false)
			{
				messages[ messageId] = (MessageHandle<T>)Delegate.Combine( messages[ messageId], handle);
			}
		}
		public static void AddListener<T0, T1>( this IMessageDispatcher dispatcher, int messageId, MessageHandle<T0, T1> handle)
		{
			if( handle == null)
			{
				return;
			}
			Type containterType = dispatcher.GetContainerType();
			var messages = GetMessagesOfType( containterType);
			
			if( messages == null)
			{
				return;
			}
			if( CombineMethod( messageId, messages, handle) != false)
			{
				messages[ messageId] = (MessageHandle<T0, T1>)Delegate.Combine( messages[ messageId], handle);
			}
		}
		public static void AddListener<T0, T1, T2>( this IMessageDispatcher dispatcher, int messageId, MessageHandle<T0, T1, T2> handle)
		{
			if( handle == null)
			{
				return;
			}
			Type containterType = dispatcher.GetContainerType();
			var messages = GetMessagesOfType( containterType);
			
			if( messages == null)
			{
				return;
			}
			if( CombineMethod( messageId, messages, handle) != false)
			{
				messages[ messageId] = (MessageHandle<T0, T1, T2>)Delegate.Combine( messages[ messageId], handle);
			}
		}
		public static void AddListener<T0, T1, T2, T3>(this IMessageDispatcher dispatcher, int messageId, MessageHandle<T0, T1, T2, T3> handle)
		{
			if( handle == null)
			{
				return;
			}
			Type containterType = dispatcher.GetContainerType();
			var messages = GetMessagesOfType( containterType);
			
			if( messages == null)
			{
				return;
			}
			if( CombineMethod( messageId, messages, handle) != false)
			{
				messages[ messageId] = (MessageHandle<T0, T1, T2, T3>)Delegate.Combine( messages[ messageId], handle);
			}
		}
		public static void RemoveListener(this IMessageDispatcher dispatcher, int messageId, MessageHandle handle)
		{
			if( handle == null)
			{
				return;
			}
			Type containterType = dispatcher.GetContainerType();
			var messages = GetMessagesOfType( containterType, false);
			
			if( messages != null)
			{
				if( RemoveMethod( messageId, messages, handle) != false)
				{
					messages[ messageId] = (MessageHandle) Delegate.Remove( messages[ messageId], handle);
					RemoveEmptyMessage( messageId, containterType);
				}
			}
		}
		public static void RemoveListener<T>( this IMessageDispatcher dispatcher, int messageId, MessageHandle<T> handle)
		{
			if( handle == null)
			{
				return;
			}
			Type containterType = dispatcher.GetContainerType();
			var messages = GetMessagesOfType( containterType, false);
			
			if( messages != null)
			{
				if( RemoveMethod( messageId, messages, handle) != false)
				{
					messages[ messageId] = (MessageHandle<T>)Delegate.Remove( messages[ messageId], handle);
					RemoveEmptyMessage( messageId, containterType);
				}
			}
		}
		public static void RemoveListener<T0, T1>( this IMessageDispatcher dispatcher, int messageId, MessageHandle<T0, T1> handle)
		{
			if( handle == null)
			{
				return;
			}
			Type containterType = dispatcher.GetContainerType();
			var messages = GetMessagesOfType( containterType, false);
			
			if( messages != null)
			{
				if( RemoveMethod( messageId, messages, handle) != false)
				{
					messages[ messageId] = (MessageHandle<T0, T1>)Delegate.Remove( messages[ messageId], handle);
					RemoveEmptyMessage( messageId, containterType);
				}
			}
		}
		public static void RemoveListener<T0, T1, T2>(this IMessageDispatcher dispatcher, int messageId, MessageHandle<T0, T1, T2> handle)
		{
			if( handle == null)
			{
				return;
			}
			Type containterType = dispatcher.GetContainerType();
			var messages = GetMessagesOfType( containterType, false);
			
			if( messages != null)
			{
				if( RemoveMethod( messageId, messages, handle) != false)
				{
					messages[ messageId] = (MessageHandle<T0, T1, T2>)Delegate.Remove( messages[ messageId], handle);
					RemoveEmptyMessage( messageId, containterType);
				}
			}
		}
		public static void RemoveListener<T0, T1, T2, T3>(this IMessageDispatcher dispatcher, int messageId, MessageHandle<T0, T1, T2, T3> handle)
		{
			if( handle == null)
			{
				return;
			}
			Type containterType = dispatcher.GetContainerType();
			var messages = GetMessagesOfType( containterType, false);
			
			if( messages != null)
			{
				if( RemoveMethod( messageId, messages, handle) != false)
				{
					messages[ messageId] = (MessageHandle<T0, T1, T2, T3>)Delegate.Remove( messages[ messageId], handle);
					RemoveEmptyMessage( messageId, containterType);
				}
			}
		}
		public static void Broadcast(this IMessageDispatcher dispatcher, int messageId)
		{
			Type containterType = dispatcher.GetContainerType();
			
			if( GetMethod( containterType, messageId) is MessageHandle handle)
			{
				handle.Invoke();
			}
		}
		public static void Broadcast<T>( this IMessageDispatcher dispatcher, int messageId, T arg)
		{
			Type containterType = dispatcher.GetContainerType();
			
			if( GetMethod( containterType, messageId) is MessageHandle<T> handle)
			{
				handle.Invoke( arg);
			}
		}
		public static void Broadcast<T0, T1>( this IMessageDispatcher dispatcher, int messageId, T0 arg0, T1 arg1)
		{
			Type containterType = dispatcher.GetContainerType();
			
			if( GetMethod( containterType, messageId) is MessageHandle<T0, T1> handle)
			{
				handle.Invoke( arg0, arg1);
			}
		}
		public static void Broadcast<T0, T1, T2>( this IMessageDispatcher dispatcher, int messageId, T0 arg0, T1 arg1, T2 arg2)
		{
			Type containterType = dispatcher.GetContainerType();
			
			if( GetMethod( containterType, messageId) is MessageHandle<T0, T1, T2> handle)
			{
				handle.Invoke(arg0, arg1, arg2);
			}
		}
		public static void Broadcast<T0, T1, T2, T3>( this IMessageDispatcher dispatcher, int messageId, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			Type containterType = dispatcher.GetContainerType();
			
			if( GetMethod( containterType, messageId) is MessageHandle<T0, T1, T2, T3> handle)
			{
				handle.Invoke( arg0, arg1, arg2, arg3);
			}
		}
		static Dictionary<int, Delegate> GetMessagesOfType( Type containterType, bool createIfNotExists = true)
		{
			if( containterType == null)
			{
				return null;
			}
			string typeName = containterType.FullName;
			
			if( string.IsNullOrEmpty( typeName) != false)
			{
				return null;
			}
			if( m_Messages == null)
			{
				if( createIfNotExists != false)
				{
					m_Messages = new Dictionary<string, Dictionary<int, Delegate>>();
				}
				else
				{
					return null;
				}
			}
			Dictionary<int, Delegate> messages = null;
			
			if( m_Messages.ContainsKey( typeName) != false)
			{
				messages = m_Messages[ typeName];
			}
			else if( createIfNotExists != false)
			{
				messages = new Dictionary<int, Delegate>();
				m_Messages.Add( typeName, messages);
			}
			return messages;
		}
		static bool CombineMethod( int messageId, Dictionary<int, Delegate> messages, Delegate handle)
		{
			if( messages.TryGetValue( messageId, out Delegate message) != false)
			{
				if( message.GetType() == handle.GetType())
				{
					return true;
				}
			}
			else
			{
				messages.Add( messageId, handle);
			}
			return false;
		}
		static bool RemoveMethod( int messageId, Dictionary<int, Delegate> messages, Delegate handle)
		{
			if( messages == null)
			{
				return false;
			}
			if( messages.TryGetValue( messageId, out Delegate message) == false)
			{
				return false;
			}
			if( message.GetType() == handle.GetType())
			{
				return true;
			}
			return false;
		}
		static Delegate GetMethod(Type containterType, int messageId)
		{
			if( containterType == null)
			{
				return null;
			}
			string typeName = containterType.FullName;
			
			if( string.IsNullOrEmpty( typeName) != false)
			{
				return null;
			}
			if( m_Messages.ContainsKey( typeName) != false)
			{
				var messages = m_Messages[ typeName];
				
				if( messages != null && messages.ContainsKey( messageId) != false)
				{
					return messages[ messageId];
				}
			}
			return null;
		}
		static void RemoveEmptyMessage(int messageId, Type containterType)
		{
			if( containterType == null)
			{
				return;
			}
			string typeName = containterType.FullName;
			
			if( string.IsNullOrEmpty( typeName) != false)
			{
				return;
			}
			if( m_Messages.ContainsKey( typeName) != false)
			{
				Dictionary<int, Delegate> messages = m_Messages[typeName];
				
				if( messages.ContainsKey( messageId) && messages[ messageId] == null)
				{
					messages.Remove( messageId);
				}
				if( messages.Count == 0)
				{
					m_Messages.Remove( typeName);
				}
			}
		}
		static Dictionary<string, Dictionary<int, Delegate>> m_Messages;
	}
}