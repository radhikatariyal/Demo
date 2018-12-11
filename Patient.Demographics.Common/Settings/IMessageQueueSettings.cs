namespace Patient.Demographics.Common.Settings
{
    public interface IMessageQueueSettings
    {
        string MessageQueueVHost { get; }
        string MessageQueueUsername { get; }
        string MessageQueuePassword { get; }
    }
}