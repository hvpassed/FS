namespace FS.Network
{
    public interface IMessageDispatcher
    {
        void Dispatcher(ISession session, IMessage message);
    }
}