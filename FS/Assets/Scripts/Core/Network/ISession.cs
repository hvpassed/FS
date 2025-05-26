using System;
using System.Threading.Tasks;

namespace FS.Network
{
    public interface ISession
    {
        // ValueTask<bool> Connect(string address, ushort port);
        //
        // void Disconnect();

        Task StartReceiving();

        Task Send(byte[] data);
    }
}