using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class LegalForm
    {
        public int Id { get; set; }
        public string? CountryId { get; set; }
        public string? ShortName { get; set; }
        public string? NameUA { get; set; }
        public string? NameEN { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is LegalForm other)
            {
                return ShortName == other.ShortName;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(ShortName?.ToUpperInvariant(), CountryId?.ToUpperInvariant());
        }
    }
}
