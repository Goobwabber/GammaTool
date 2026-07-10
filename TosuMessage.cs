using System.Text.Json.Serialization;

namespace rebellagamma;

public class TosuMessage
{
    public State? state { get; set; }
    public Beatmap? beatmap { get; set; }

    public class State
    {
        public int number { get; set; }
        public string name { get; set; } = string.Empty;
    }

    public class Beatmap
    {
        public Stats? stats { get; set; }

        public class Stats
        {
            public AR? ar { get; set; }

            public class AR
            {
                public double original { get; set; }
                public double converted { get; set; }
            }
        }
    }
}