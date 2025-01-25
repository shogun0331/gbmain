namespace GB
{
    public struct OData<T> : IOData
    {
        private T _value;

        public OData(T value)
        {
            this._value = value;
        }

        public T Get()
        {
            return _value;
        }

        public void Set(T value)
        {
            _value = value;
        }
    }

    public interface IOData { }
}

