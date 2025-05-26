using System.Threading.Tasks;

namespace FS.Network
{
    public interface IReceive
    {
        ValueTask<byte[]> Recv();
    }
}