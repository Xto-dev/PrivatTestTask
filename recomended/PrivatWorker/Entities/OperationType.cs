namespace PrivatWorker.Entities
{
    public class OperationType
    {
        private OperationType(string value) { Value = value; }

        public string Value { get; private set; }

        public static OperationType Online { get { return new OperationType("online"); } }
        public static OperationType Offline { get { return new OperationType("offline"); } }

        public override string ToString()
        {
            return Value;
        }
    }
}
