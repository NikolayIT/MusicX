namespace MusicX.Common.Models
{
    using System.Collections.Generic;

    public class SongAttributes
    {
        private readonly IDictionary<SongAttribute, string> values;

        public SongAttributes()
        {
            this.values = new Dictionary<SongAttribute, string>();
        }

        public string this[SongAttribute key]
        {
            get
            {
                if (this.values.ContainsKey(key))
                {
                    return this.values[key];
                }

                return null;
            }

            set
            {
                if (this.values.ContainsKey(key))
                {
                    this.values[key] = value;
                }

                this.values.Add(key, value);
            }
        }
    }
}
