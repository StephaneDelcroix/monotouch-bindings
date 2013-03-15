//
// ZeroThreadTaskScheduler.cs
//
// Author:
//       Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (c) 2013 S. Delcroix
//
using System;
using System.Threading.Tasks;

namespace CC2DSharp
{
	public class ZeroThreadTaskScheduler : TaskScheduler
	{
		public ZeroThreadTaskScheduler ()
		{

		}

		//Task currentTask;
		protected override void QueueTask (Task task)
		{
			Console.WriteLine ("queueTask");
			//currentTask = task;
			if (!TryExecuteTaskInline (task, false))
				Console.WriteLine ("failed executing task");
		}

		protected override System.Collections.Generic.IEnumerable<Task> GetScheduledTasks ()
		{
			return null;
		}

		protected override bool TryExecuteTaskInline (Task task, bool taskWasPreviouslyQueued)
		{
			return TryExecuteTask (task);
		}
	}
}

