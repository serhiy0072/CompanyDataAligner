using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class Company
    {
        #region Properties
        public int Id { get; private set; }

        public bool HasMultipleLegalForms = false;
        [JsonProperty("CountryId")]
        public string? CountryId { get; private set; }
        [JsonProperty("FullName")]
        public string? FullName { get; private set; }
        public string? UniqueName { get; set; }
        [JsonProperty("Address")]
        public Address Address { get; private set; }
        public LegalForm? LegalForm { get; set; }
        #endregion
        #region Constructors 
        public Company() {}
        public Company(string name, string fullAddress, string countryId)
        {
            FullName = name;
            Address = new Address(fullAddress);
            CountryId = countryId;
        }
        #endregion
        #region Methods

        public override string ToString()
        {
            return $"{UniqueName}, {LegalForm?.ShortName}, {Address.ToString()}";
        } 
        
        // Перевизначаємо метод Equals для порівняння компаній за UniqueName, LegalForm.ShortName і Address
        public override bool Equals(object obj)
        {
            if (obj is Company other)
            {
                return string.Equals(this.UniqueName, other.UniqueName, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(this.LegalForm?.ShortName, other.LegalForm?.ShortName, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(this.Address?.ToString(), other.Address?.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        // Перевизначаємо GetHashCode для забезпечення коректної роботи з колекціями
        public override int GetHashCode()
        {
            return HashCode.Combine(
                UniqueName?.ToUpperInvariant(),
                LegalForm?.ShortName?.ToUpperInvariant(),
                Address?.ToString()?.ToUpperInvariant());
        }
        #endregion
    }
}
