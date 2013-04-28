using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.Xna.Framework;

namespace To_Space.Managers
{
	public class ThreadManager : Manager
	{
		private Dictionary<string, Thread> _threadList;

		public ThreadManager(ToSpaceGame game)
			: base(game)
		{
			_threadList = new Dictionary<string, Thread>();
		}

		public void AddThread(Thread t)
		{
			addThread(t.Name, t);
		}

		private void addThread(string name, Thread t)
		{
			if (!_threadList.ContainsKey(name))
			{
				if (!_threadList.ContainsValue(t))
				{
					_threadList.Add(name, t);
				}
			}
		}

		public Thread GetThread(string name)
		{
			if (_threadList.ContainsKey(name))
			{
				return _threadList[name];
			}

			throw new Exception("Cannot find an entry of the thread with the name: " + name);
		}

		public void RemoveThread(Thread t)
		{
			if (_threadList.ContainsValue(t))
			{
				t.Abort();
				RemoveThread(t.Name);
			}
		}

		public void RemoveThread(string name)
		{
			if (_threadList.ContainsKey(name))
			{
				_threadList.Remove(name);
			}
		}

		public void StopAllThreads()
		{
			foreach (Thread t in _threadList.Values)
			{
				t.Abort();
			}
		}

		public override void Initialize() { }
		public override void LoadContent() { }
		public override void Update(GameTime gameTime) { }
	}
}