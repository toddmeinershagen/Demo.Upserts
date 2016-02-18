namespace Demo.Upserts.Command
{ 
    public class Version
    {
        public Version()
            : this(default(int))
        { }

        public Version(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator int(Version version)
        {
            return version.Value;
        }

        public static explicit operator Version(int value)
        {
            return new Version(value);
        }

        public bool IsNotExisting()
        {
            return Value == 0;
        }

        public bool IsFirstVersion()
        {
            return Value == 1;
        }
    }
}