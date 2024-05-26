using System;
using System.Collections.Generic;

namespace DatabaseFirst.Models;

public partial class Country
{
    public int IdCountry { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<CountryTrip> CountryTrips { get; set; }
}
