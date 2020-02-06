using System.Collections.Generic;

namespace PlatoCore.Layout.Models
{

    public class TagAlterations
    {

        private IList<TagAlteration> _alterations;

        public int Count => _alterations?.Count ?? 0;

        public void Add(IEnumerable<TagAlteration> alterations)
        {           
            foreach (var alteration in alterations)
            {
                Add(alteration);
            }            
        }

        public void Add(TagAlteration alteration)
        {
            EnsureList();
            _alterations.Add(alteration);
        }

        public IEnumerable<TagAlteration> First(string id)
        {

            if (_alterations == null)
            {
                return null;
            }

            if (_alterations.Count <= 0)
            {
                return null;
            }

            IList<TagAlteration> output = null;
            foreach (var alteration in _alterations)
            {
                if (alteration.Id.Equals(id, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (output == null)
                    {
                        output = new List<TagAlteration>();
                    }
                    output.Add(alteration);
                }
            }
            return output;
        }

        public IEnumerable<TagAlteration> FirstOrDefault(string id)
        {
            var first = First(id);
            return first ?? default(List<TagAlteration>);
        }

        // -----------

        void EnsureList()
        {
            if (_alterations == null)
            {
                _alterations = new List<TagAlteration>();
            }
        }

    }

}
