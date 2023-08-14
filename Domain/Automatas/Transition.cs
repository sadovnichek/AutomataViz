namespace Domain.Automatas
{
    public class Transition
    {
        public string State { get; private set; }

        public string Value { get; private set; }

        public string Symbol { get; private set; }

        public Transition(string state, string symbol, string value)
        {
            State = state;
            Symbol = symbol;
            Value = value;
        }

        public override string ToString() => $"{State}.{Symbol} = {Value}";

        public override bool Equals(object? obj)
        {
            if (obj is not Transition transition)
                return false;
            return State.Equals(transition.State)
                && Value.Equals(transition.Value)
                && Symbol.Equals(transition.Symbol);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(State, Value, Symbol);
        }
    }
}
