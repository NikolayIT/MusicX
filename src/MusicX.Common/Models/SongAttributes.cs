namespace MusicX.Common.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SongAttributes : IEnumerable<KeyValuePair<SongMetadataType, IList<string>>>
    {
        private readonly IDictionary<SongMetadataType, IList<string>> values;

        public SongAttributes()
        {
            this.values = new Dictionary<SongMetadataType, IList<string>>();
        }

        public SongAttributes(SongMetadataType type, string value)
            : this()
        {
            this[type] = value;
        }

        public SongAttributes(IEnumerable<Tuple<SongMetadataType, string>> values)
            : this()
        {
            foreach (var value in values)
            {
                this[value.Item1] = value.Item2;
            }
        }

        public string this[SongMetadataType key]
        {
            get
            {
                if (this.values.ContainsKey(key) && this.values[key].Any())
                {
                    return this.values[key].Last();
                }

                return null;
            }

            set
            {
                if (this.values.ContainsKey(key))
                {
                    this.values[key].Add(value);
                }
                else
                {
                    this.values.Add(key, new List<string> { value });
                }
            }
        }

        public IEnumerable<string> All(SongMetadataType attribute)
        {
            if (!this.values.ContainsKey(attribute))
            {
                this.values.Add(attribute, new List<string>());
            }

            return this.values[attribute];
        }

        public IEnumerator<KeyValuePair<SongMetadataType, IList<string>>> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in this.values.Where(x => x.Key != SongMetadataType.Lyrics))
            {
                stringBuilder.Append($"[{item.Key}]=\"{string.Join(",", item.Value)}\"; ");
            }

            return stringBuilder.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
