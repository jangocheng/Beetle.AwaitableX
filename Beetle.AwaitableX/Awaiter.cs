using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Beetle.AwaitableX
{
    public abstract class Awaiter<T, TARGET> : INotifyCompletion, IAwaitResult<TARGET> where TARGET : INotifyCompletion
    {
        private object mResult;

        private Action mContinuation = null;

        private Dictionary<string, object> mProperties = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                object result = null;
                mProperties.TryGetValue(key, out result);
                return result;
            }
            set
            {
                mProperties[key] = value;
            }
        }

        private int mExecuteComplted = 0;

        private void OnExecute(bool useThread = false)
        {
            if (System.Threading.Interlocked.Exchange(ref mExecuteComplted, 1) == 0)
            {
                if (mContinuation != null)
                {
                    if (useThread)
                    {
                        Task.Run(new Action(ExecuteContinuation));
                    }
                    else
                    {
                        ExecuteContinuation();
                    }
                }
            }
        }

        private void ExecuteContinuation()
        {
            try
            {
                mContinuation();
            }
            catch (Exception e_)
            {
                this.Exception = e_;
            }
            finally
            {
                Reset();
            }

        }

        public virtual void Completed(object result, Exception exception, bool useThread = false)
        {
            mResult = (T)result;
            this.Exception = exception;
            IsCompleted = true;
            if (exception == null)
                OnExecute(useThread);
        }

        protected virtual void Reset()
        {
            IsCompleted = false;
            mResult = null;
            mContinuation = null;
            Exception = null;
            mExecuteComplted = 0;
            Tag = null;
            mProperties.Clear();

        }

        public bool IsCompleted
        {
            get;
            set;
        }

        public Exception Exception { get; set; }

        public void OnCompleted(Action continuation)
        {
            mContinuation = continuation;
            if (IsCompleted)
            {
                OnExecute();
            }
        }

        public Awaiter<T, TARGET> GetAwaiter()
        {
            return this;
        }

        public T Result
        {
            get
            {
                if (this.Exception != null)
                    throw this.Exception;
                return (T)mResult;
            }
        }

        public IAwaitResult<TARGET> GetResult()
        {
            return this;
        }

        public object Tag { get; set; }

        public RESULT ResultTo<RESULT>()
        {
            if (this.Exception != null)
                throw this.Exception;
            return (RESULT)mResult;
        }

        public TARGET Target
        {
            get
            {
                return (TARGET)(object)this;
            }
        }
    }
}
