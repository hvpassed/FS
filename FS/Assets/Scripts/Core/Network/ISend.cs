using System.Threading.Tasks;

namespace FS.Network
{
    public interface ISend
    {
        ValueTask<bool> Send(byte[] data);
    }
}