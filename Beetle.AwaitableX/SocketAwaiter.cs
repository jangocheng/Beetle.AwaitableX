using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Beetle.AwaitableX
{
    public class SocketAwaiter : Awaiter<ArraySegment<byte>, SocketAwaiter>
    {

        private System.Net.Sockets.SocketAsyncEventArgs mSAEA;

        private byte[] mBuffer;

        public SocketAwaiter(int bufferSize)
        {
            mSAEA = new System.Net.Sockets.SocketAsyncEventArgs();
            mBuffer = new byte[bufferSize];
            mSAEA.Completed += OnSocketCompleted;
        }

        public System.Net.Sockets.SocketAsyncEventArgs SocketAsyncEventArg
        {
            get
            {
                return mSAEA;
            }
        }

        public SocketError SocketError
        {
            get;
            set;
        }

        private void OnSocketCompleted(object sender, SocketAsyncEventArgs e)
        {
            SocketError = e.SocketError;
            Completed(new ArraySegment<byte>(mBuffer, e.Offset, e.BytesTransferred), null);
        }

        public int BytesTransferred { get { return this.mSAEA.BytesTransferred; } }

        public SocketAsyncOperation LastOperation { get { return this.mSAEA.LastOperation; } }

        public override void Reset()
        {
            base.Reset();
            SocketError = System.Net.Sockets.SocketError.Success;
        }

        public void SetBuffer(byte[] buffer, int offset, int count)
        {
            Buffer.BlockCopy(buffer, offset, mBuffer, 0, count);
            mSAEA.SetBuffer(0, count);
        }

        public SocketAwaiter Send(System.Net.Sockets.Socket socket)
        {
            return Send(socket, 0, mSAEA.Count);
        }

        public SocketAwaiter Send(System.Net.Sockets.Socket socket, int offset, int count)
        {
            if (!socket.SendAsync(mSAEA))
            {
                OnSocketCompleted(this, mSAEA);
            }
            return this;
        }

        public SocketAwaiter Receive(System.Net.Sockets.Socket socket)
        {
            mSAEA.SetBuffer(0, mBuffer.Length);
            if (!socket.ReceiveAsync(mSAEA))
            {
                OnSocketCompleted(this, mSAEA);
            }
            return this;
        }
    }
}
