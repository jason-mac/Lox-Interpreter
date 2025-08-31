namespace LoxInterpreter
{
    class Return : System.Exception
    {
        public readonly object value;

        public Return(object value) :
        base(null, null)
        {
            this.value = value;
        }

    }

}
