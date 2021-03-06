﻿using System;
using System.Collections.Generic;
using System.Text;

namespace framework.Core.Implementation
{
	//////////////////////////////////////////////////////////////////////////


	class ListenerQueue<T>
	{
		//////////////////////////////////////////////////////////////////////////

		public class View : IDisposable
		{
			public View(ListenerQueue<T> queue)
			{
				m_queue = queue;
				m_listeners = queue.m_listeners;
			}

			public void Dispose() 
			{ 
				m_queue.ungetView(this); 
			}

			public List<T> getListeners()
			{
				return m_listeners;
			}

			ListenerQueue<T> m_queue;
			List<T> m_listeners;
		}

		//////////////////////////////////////////////////////////////////////////

		public ListenerQueue()
		{
			m_viewCount = 0;
			m_listeners = new List<T>();
		}

		//////////////////////////////////////////////////////////////////////////

		public void Add(T listener)
		{
			if (listener == null)
				throw new NullReferenceException();

			lock (m_lock)
			{
				if (!m_listeners.Contains(listener))
				{
					preModify();
					m_listeners.Add(listener);
				}
			}
		}

		//////////////////////////////////////////////////////////////////////////

		public void Remove(T listener)
		{
			lock (m_lock)
			{
				preModify();
				m_listeners.Remove(listener);
			}
		}

		//////////////////////////////////////////////////////////////////////////

		public View getView()
		{
			lock (m_lock)
			{
				++m_viewCount;
				return new View(this);
			}
		}

		//////////////////////////////////////////////////////////////////////////

		public void ungetView(View view)
		{
			lock (m_lock)
			{
				if (Object.ReferenceEquals(m_listeners, view.getListeners()))
					--m_viewCount;
			}
		}

		//////////////////////////////////////////////////////////////////////////

		void preModify()
		{
			if (m_viewCount != 0)
			{
				m_listeners = new List<T>(m_listeners);
				m_viewCount = 0;
			}
		}

		//////////////////////////////////////////////////////////////////////////

		int m_viewCount;
		List<T> m_listeners;
		object m_lock = new object();
	}

	//////////////////////////////////////////////////////////////////////////
}
