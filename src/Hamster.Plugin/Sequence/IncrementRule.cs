namespace Hamster.Plugin.Sequence
{
    public class IncrementRule : ISequenceRule
    {
        private int stepSize = 1;

        public IncrementRule()
        {
        }

        public int StepSize
        {
            get { return stepSize; }
            set { stepSize = value; }
        }

        public int GetNext(int currentValue)
        {
            return currentValue + stepSize;
        }
    }
}
