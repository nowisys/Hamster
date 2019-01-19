namespace Hamster.Plugin.Sequence
{
    public class CircleRule : ISequenceRule
    {
        private ISequenceRule parent;
        private int min = 0;
        private int max = int.MaxValue;

        public CircleRule()
        {
            parent = new IncrementRule();
        }

        public ISequenceRule Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public int Min
        {
            get { return min; }
            set { min = value; }
        }

        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        public int GetNext(int currentValue)
        {
            if (currentValue == max)
            {
                return min;
            }
            else
            {
                return parent.GetNext(currentValue);
            }
        }
    }
}
