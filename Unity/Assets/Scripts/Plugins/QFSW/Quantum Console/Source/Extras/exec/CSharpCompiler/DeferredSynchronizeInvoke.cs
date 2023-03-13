/*
    Implementation of ISynchronizeInvoke for Unity3D game engine.
    Can be used to invoke anything on main Unity thread.
    ISynchronizeInvoke is used extensively in .NET forms it's is elegant and quite useful in Unity as well.
    I implemented it so i can use it with System.IO.FileSystemWatcher.SynchronizingObject.

    help from: http://www.codeproject.com/Articles/12082/A-DelegateQueue-Class
    example usage: https://gist.github.com/aeroson/90bf21be3fdc4829e631

    license: WTFPL (http://www.wtfpl.net/)
    contact: aeroson (theaeroson @gmail.com)
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

public class DeferredSynchronizeInvoke : ISynchronizeInvoke
{
    Queue<UnityAsyncResult> fifoToExecute = new Queue<UnityAsyncResult>();
    Thread mainThread;
    public bool InvokeRequired { get { return mainThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId; } }

    public DeferredSynchronizeInvoke()
    {
        mainThread = Thread.CurrentThread;
    }
    public IAsyncResult BeginInvoke(Delegate method, object[] args)
    {
        var asyncResult = new UnityAsyncResult()
        {
            method = method,
            args = args,
            IsCompleted = false,
            AsyncWaitHandle = new ManualResetEvent(false),
        };
        lock (fifoToExecute)
        {
            fifoToExecute.Enqueue(asyncResult);
        }
        return asyncResult;
    }
    public object EndInvoke(IAsyncResult result)
    {
        if (!result.IsCompleted)
        {
            result.AsyncWaitHandle.WaitOne();
        }
        return result.AsyncState;
    }
    public object Invoke(Delegate method, object[] args)
    {
        if (InvokeRequired)
        {
            var asyncResult = BeginInvoke(method, args);
            return EndInvoke(asyncResult);
        }
        else
        {
            return method.DynamicInvoke(args);
        }
    }
    public void ProcessQueue()
    {
        if (Thread.CurrentThread != mainThread)
        {
            throw new TargetException(
                this.GetType() + "." + MethodBase.GetCurrentMethod().Name + "() " +
                "must be called from the same thread it was created on " +
                "(created on thread id: " + mainThread.ManagedThreadId + ", called from thread id: " + Thread.CurrentThread.ManagedThreadId
            );
        }
        bool loop = true;
        UnityAsyncResult data = null;
        while (loop)
        {
            lock (fifoToExecute)
            {
                loop = fifoToExecute.Count > 0;
                if (!loop) break;
                data = fifoToExecute.Dequeue();
            }

            data.AsyncState = Invoke(data.method, data.args);
            data.IsCompleted = true;
        }
    }
    class UnityAsyncResult : IAsyncResult
    {
        public Delegate method;
        public object[] args;
        public bool IsCompleted { get; set; }
        public WaitHandle AsyncWaitHandle { get; internal set; }
        public object AsyncState { get; set; }
        public bool CompletedSynchronously { get { return IsCompleted; } }
    }
}