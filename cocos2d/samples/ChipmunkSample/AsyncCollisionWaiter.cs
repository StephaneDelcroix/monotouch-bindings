//
// AsyncCollisionWaiter.cs
//
// Author:
//       Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (c) 2013 S. Delcroix
//
using System;
using System.Threading.Tasks;
using Chipmunk;

namespace CC2DSharp
{
	public class AsyncCollisionWaiter : IDisposable
	{
		Space space;
		uint collisionTypeA, collisionTypeB;
		public AsyncCollisionWaiter (Space space, uint collisionTypeA, uint collisionTypeB)
		{
			this.space = space;
			this.collisionTypeA = collisionTypeA;
			this.collisionTypeB = collisionTypeB;
			space.AddCollisionHandler (collisionTypeA, collisionTypeB, Begin, PreSolve, PostSolve, Separate);
		}

		object syncHandle = new object ();
		TaskCompletionSource<Arbiter> beginTcs, presolveTcs, postsolveTcs, separateTcs;

		public Task<Arbiter> GetBeginAsync ()
		{
			lock (syncHandle) {
				beginTcs = new TaskCompletionSource<Arbiter> ();
			}
			return beginTcs.Task;
		}

		bool Begin (Arbiter arb)
		{
			Console.WriteLine ("began");

			lock (syncHandle) {
				if (beginTcs != null) {
					beginTcs.SetResult (arb);
				}
				beginTcs = null;
			}
			return true;

		}

		public Task<Arbiter> GetPreSolveAsync ()
		{
			lock (syncHandle) {
				presolveTcs = new TaskCompletionSource<Arbiter> ();
			}
			return presolveTcs.Task;
		}

		bool PreSolve (Arbiter arb)
		{
			Console.WriteLine ("presolved");
			lock (syncHandle) {
				if (presolveTcs != null)
					presolveTcs.SetResult (arb);
				presolveTcs = null;
			}
			return true;
		}

		public Task<Arbiter> GetPostSolveAsync ()
		{
			lock (syncHandle) {
				postsolveTcs = new TaskCompletionSource<Arbiter> ();
			}
			return postsolveTcs.Task;
		}

		void PostSolve (Arbiter arb)
		{
			Console.WriteLine ("postsolved");
			lock (syncHandle) {
				if (postsolveTcs != null)
					postsolveTcs.SetResult (arb);
				postsolveTcs = null;
			}
		}

		public Task<Arbiter> GetSeparateAsync ()
		{
			lock (syncHandle) {
				separateTcs = new TaskCompletionSource<Arbiter> ();
			}
			return separateTcs.Task;
		}

		void Separate (Arbiter arb)
		{
			Console.WriteLine ("separated");
			lock (syncHandle) {
				if (separateTcs != null)
					separateTcs.SetResult (arb);
			}
			separateTcs = null;
		}

		public void Dispose ()
		{
			if (space != null)
				space.RemoveCollisionHandler (collisionTypeA:collisionTypeA, collisionTypeB:collisionTypeB);
		}
	}
}

